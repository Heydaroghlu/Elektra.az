using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Database;
using OCPP.Core.Server.Messages_OCPP16;
using OCPP.Core.Server.Messages_OCPP20;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OCPP.Core.Server
{
    public partial class OCPPMiddleware
    {
        /// <summary>
        /// Waits for new OCPP V1.6 messages on the open websocket connection and delegates processing to a controller
        /// </summary>
        private async Task Receive16(ChargePointStatus chargePointStatus, HttpContext context, OCPPCoreContext dbContext)
        {
            ILogger logger = _logFactory.CreateLogger("OCPPMiddleware.OCPP16");
            ControllerOCPP16 controller16 = new ControllerOCPP16(_configuration, _logFactory, chargePointStatus, dbContext);

            int maxMessageSizeBytes = _configuration.GetValue<int>("MaxMessageSize", 0);
            byte[] buffer = new byte[1024 * 50]; // Adjust buffer size as needed
            MemoryStream memStream = new MemoryStream();

            while (chargePointStatus.WebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await chargePointStatus.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    memStream.Write(buffer, 0, result.Count);

                    if (memStream.Length > maxMessageSizeBytes && maxMessageSizeBytes > 0)
                    {
                        logger.LogInformation("OCPPMiddleware.Receive16 => Allowed message size exceeded - closing connection");
                        await chargePointStatus.WebSocket.CloseOutputAsync(WebSocketCloseStatus.MessageTooBig, string.Empty, CancellationToken.None);
                        return;
                    }
                }
                while (!result.EndOfMessage);

                string ocppMessage = UTF8Encoding.UTF8.GetString(memStream.ToArray());
                memStream.SetLength(0); // Reset the memory stream for the next message

                logger.LogDebug("OCPPMiddleware.Receive16 => Raw OCPP Message: {0}", ocppMessage);

                Match match = Regex.Match(ocppMessage, MessageRegExp);
                if (match.Success)
                {
                    string messageTypeId = match.Groups[1].Value;
                    string uniqueId = match.Groups[2].Value;
                    string action = match.Groups[3].Value;
                    string jsonPayload = match.Groups[4].Value;

                    logger.LogInformation("OCPPMiddleware.Receive16 => OCPP-Message: Type={0} / ID={1} / Action={2}", messageTypeId, uniqueId, action);

                    OCPPMessage msgIn = new OCPPMessage(messageTypeId, uniqueId, action, jsonPayload);
                    if (msgIn.MessageType == "2")
                    {
                        // Request from chargepoint to OCPP server
                        OCPPMessage msgOut = controller16.ProcessRequest(msgIn);


                        // Send OCPP message with optional logging/dump
                        await SendOcpp16Message(msgOut, logger, chargePointStatus.WebSocket);
                    }
                    else if (msgIn.MessageType == "3" || msgIn.MessageType == "4")
                    {
                        // Process answer from chargepoint
                        if (_requestQueue.ContainsKey(msgIn.UniqueId))
                        {
                            controller16.ProcessAnswer(msgIn, _requestQueue[msgIn.UniqueId]);
                            _requestQueue.Remove(msgIn.UniqueId);
                        }
                        else
                        {
                            logger.LogError("OCPPMiddleware.Receive16 => HttpContext from caller not found / Msg: {0}", ocppMessage);
                        }
                    }
                    else
                    {
                        // Unknown message type
                        logger.LogError("OCPPMiddleware.Receive16 => Unknown message type: {0} / Msg: {1}", msgIn.MessageType, ocppMessage);
                    }
                }
                else
                {
                    logger.LogWarning("OCPPMiddleware.Receive16 => Error in RegEx-Matching: Msg={0}", ocppMessage);
                }
            }

            logger.LogInformation("OCPPMiddleware.Receive16 => WebSocket closed: State={0} / CloseStatus={1}", chargePointStatus.WebSocket.State, chargePointStatus.WebSocket.CloseStatus);
            _chargePointStatusDict.Remove(chargePointStatus.Id, out _);
        }

        /// <summary>
        /// Waits for new OCPP V1.6 messages on the open websocket connection and delegates processing to a controller
        /// </summary>
        private async Task Reset16(ChargePointStatus chargePointStatus, HttpContext apiCallerContext, OCPPCoreContext dbContext)
        {
            ILogger logger = _logFactory.CreateLogger("OCPPMiddleware.OCPP16");
            ControllerOCPP16 controller16 = new ControllerOCPP16(_configuration, _logFactory, chargePointStatus, dbContext);

            Messages_OCPP16.ResetRequest resetRequest = new Messages_OCPP16.ResetRequest();
            resetRequest.Type = Messages_OCPP16.ResetRequestType.Soft;
            string jsonResetRequest = JsonConvert.SerializeObject(resetRequest);

            OCPPMessage msgOut = new OCPPMessage();
            msgOut.MessageType = "2";
            msgOut.Action = "Reset";
            msgOut.UniqueId = Guid.NewGuid().ToString("N");
            msgOut.JsonPayload = jsonResetRequest;
            msgOut.TaskCompletionSource = new TaskCompletionSource<string>();

            _requestQueue.Add(msgOut.UniqueId, msgOut);

            
            await SendOcpp16Message(msgOut, logger, chargePointStatus.WebSocket);

            // Cihazdan cavab
            string apiResult = await msgOut.TaskCompletionSource.Task;

            // 
            apiCallerContext.Response.StatusCode = 200;
            apiCallerContext.Response.ContentType = "application/json";
            await apiCallerContext.Response.WriteAsync(apiResult);
        }
        private async Task<HttpResponse> RemoteStartTransaction16(ChargeStartDTO chargeStart, ChargePointStatus chargePointStatus, HttpContext apiCallerContext, OCPPCoreContext dbContext)
        {
            ILogger logger = _logFactory.CreateLogger("OCPPMiddleware.OCPP16");

            // StartTransactionRequest
            Messages_OCPP16.RemoteStartTransactionRequest startTransactionRequest = new Messages_OCPP16.RemoteStartTransactionRequest();
            startTransactionRequest.ConnectorId = chargeStart.ConnectorId;
            startTransactionRequest.IdTag = chargeStart.UserId;
            
            startTransactionRequest.ChargingProfile= new ChargingProfile();
            startTransactionRequest.ChargingProfile.ChargingProfileId = chargeStart.ConnectorId;
            startTransactionRequest.ChargingProfile.StackLevel = 0;
            startTransactionRequest.ChargingProfile.StackLevel = 0;
            //startTransactionRequest.ChargingProfile.ChargingSchedule.ChargingRateUnit = chargeStart.PreAuth.CardUid;
            startTransactionRequest.ChargingProfile.ChargingProfilePurpose = "TxProfile"; // Örnek amaç
            startTransactionRequest.ChargingProfile.ChargingProfileKind = "Absolute"; // Örnek tür

            // ChargingSchedule 
            ChargingSchedule chargingSchedule = new ChargingSchedule();
            chargingSchedule.ChargingRateUnit = "W";
            ChargingSchedulePeriod schedulePeriod = new ChargingSchedulePeriod();
            schedulePeriod.StartPeriod = 0;
            schedulePeriod.Limit = chargeStart.EndKw ?? 0; // 50 kW limit
            chargingSchedule.ChargingSchedulePeriod = new ChargingSchedulePeriod[] { schedulePeriod };

            startTransactionRequest.ChargingProfile.ChargingSchedule = chargingSchedule;

            string jsonResetRequest = JsonConvert.SerializeObject(startTransactionRequest);

            OCPPMessage msgOut = new OCPPMessage();
            msgOut.MessageType = "2";
            msgOut.Action = "RemoteStartTransaction";
            msgOut.UniqueId = Guid.NewGuid().ToString("N");
            msgOut.JsonPayload = jsonResetRequest;
            msgOut.TaskCompletionSource = new TaskCompletionSource<string>();

            _requestQueue.Add(msgOut.UniqueId, msgOut);

            // Send OCPP message with optional logging/dump
            await SendOcpp16Message(msgOut, logger, chargePointStatus.WebSocket);

            // Wait for asynchronous chargepoint response and processing
            string apiResult = await msgOut.TaskCompletionSource.Task;

           
            apiCallerContext.Response.StatusCode = 200;
            apiCallerContext.Response.ContentType = "application/json";
            await apiCallerContext.Response.WriteAsync(apiResult);
            return apiCallerContext.Response;
        }
        private async Task<HttpResponse> RemoteStopTransaction16(ChargeStopDTO chargeStop,  ChargePointStatus chargePointStatus, HttpContext apiCallerContext, OCPPCoreContext dbContext)
        {
            ILogger logger = _logFactory.CreateLogger("OCPPMiddleware.OCPP16");

            // RemoteStopTransactionRequest nesnesi oluşturma
            Messages_OCPP16.RemoteStopTransactionRequest stopTransactionRequest = new Messages_OCPP16.RemoteStopTransactionRequest
            {
                TransactionId = chargeStop.TransactionId,
                IdTag=chargeStop.IdTag,
                Reason= "EVDisconnected",
            };
            logger.LogInformation($"StopTransaction isa-TransactionId-: {chargeStop.TransactionId}");
            var transaction = dbContext.Transactions.FirstOrDefault(x => x.TransactionId == chargeStop.TransactionId);
            if (transaction != null)
            {
                var log = dbContext.MessageLogs.Where(x =>
                    x.ChargePointId == transaction.ChargePointId &&
                    x.ConnectorId == transaction.ConnectorId && x.Message == "MeterValues").OrderByDescending(x=>x.LogId).FirstOrDefault();
                transaction.EndMessage = log.Result;
            }
            
            // JSON formatına serileştirme
            string jsonStopRequest = JsonConvert.SerializeObject(stopTransactionRequest);

            // OCPP Mesajını oluşturma
            OCPPMessage msgOut = new OCPPMessage
            {
                MessageType = "2",
                Action = "RemoteStopTransaction",
                UniqueId = Guid.NewGuid().ToString("N"),
                JsonPayload = jsonStopRequest,
                TaskCompletionSource = new TaskCompletionSource<string>()
            };

            _requestQueue.Add(msgOut.UniqueId, msgOut);

            // OCPP mesajını gönderme
            await SendOcpp16Message(msgOut, logger, chargePointStatus.WebSocket);

            // ChargePoint'ten gelen yanıtı bekleme
            string apiResult = await msgOut.TaskCompletionSource.Task;

            // İsteğe bağlı: Yanıtı loglama
            logger.LogInformation($"StopTransaction response: {apiResult}");

            // Optional: HTTP yanıtı oluşturma
            if (apiCallerContext != null)
            {
                apiCallerContext.Response.StatusCode = 200;
                apiCallerContext.Response.ContentType = "application/json";
                await apiCallerContext.Response.WriteAsync(apiResult);
                return apiCallerContext.Response;
            }
            return apiCallerContext.Response;
        }
        /// <summary>
        /// Sends a Unlock-Request to the chargepoint
        /// </summary>
        /// 
        private async Task UnlockConnector16(ChargePointStatus chargePointStatus, HttpContext apiCallerContext, OCPPCoreContext dbContext)
        {
            ILogger logger = _logFactory.CreateLogger("OCPPMiddleware.OCPP16");
            ControllerOCPP16 controller16 = new ControllerOCPP16(_configuration, _logFactory, chargePointStatus, dbContext);

            Messages_OCPP16.UnlockConnectorRequest unlockConnectorRequest = new Messages_OCPP16.UnlockConnectorRequest();
            unlockConnectorRequest.ConnectorId = 0;

            string jsonResetRequest = JsonConvert.SerializeObject(unlockConnectorRequest);

            OCPPMessage msgOut = new OCPPMessage();
            msgOut.MessageType = "2";
            msgOut.Action = "UnlockConnector";
            msgOut.UniqueId = Guid.NewGuid().ToString("N");
            msgOut.JsonPayload = jsonResetRequest;
            msgOut.TaskCompletionSource = new TaskCompletionSource<string>();

            // store HttpContext with MsgId for later answer processing (=> send anwer to API caller)
            _requestQueue.Add(msgOut.UniqueId, msgOut);

            // Send OCPP message with optional logging/dump
            await SendOcpp16Message(msgOut, logger, chargePointStatus.WebSocket);

            // Wait for asynchronous chargepoint response and processing
            string apiResult = await msgOut.TaskCompletionSource.Task;

            // 
            apiCallerContext.Response.StatusCode = 200;
            apiCallerContext.Response.ContentType = "application/json";
            await apiCallerContext.Response.WriteAsync(apiResult);
        }

        private async Task SendOcpp16Message(OCPPMessage msg, ILogger logger, WebSocket webSocket)
        {
            string ocppTextMessage = null;

            if (string.IsNullOrEmpty(msg.ErrorCode))
            {
                if (msg.MessageType == "2")
                {
                    // OCPP-Request
                    ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",{3}]", msg.MessageType, msg.UniqueId, msg.Action, msg.JsonPayload);
                }
                else
                {
                    // OCPP-Response
                    ocppTextMessage = string.Format("[{0},\"{1}\",{2}]", msg.MessageType, msg.UniqueId, msg.JsonPayload);
                }
            }
            else
            {
                ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]", msg.MessageType, msg.UniqueId, msg.ErrorCode, msg.ErrorDescription, "{}");
            }
            logger.LogTrace("OCPPMiddleware.OCPP16 => SendOcppMessage: {0}", ocppTextMessage);

            if (string.IsNullOrEmpty(ocppTextMessage))
            {
                // invalid message
                ocppTextMessage = string.Format("[{0},\"{1}\",\"{2}\",\"{3}\",{4}]", "4", string.Empty, Messages_OCPP16.ErrorCodes.ProtocolError, string.Empty, "{}");
            }

            /*string dumpDir = _configuration.GetValue<string>("MessageDumpDir");
            if (!string.IsNullOrWhiteSpace(dumpDir))
            {
                // Write outgoing message into dump directory
                string path = Path.Combine(dumpDir, string.Format("{0}_ocpp16-out.txt", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ffff")));
                try
                {
                    File.WriteAllText(path, ocppTextMessage);
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "OCPPMiddleware.SendOcpp16Message=> Error dumping message to path: '{0}'", path);
                }
            }*/

            byte[] binaryMessage = UTF8Encoding.UTF8.GetBytes(ocppTextMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(binaryMessage, 0, binaryMessage.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
