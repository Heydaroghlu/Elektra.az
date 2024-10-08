using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.StatusDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Persistence.Services.OCPPCall;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Application.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OCPP.Core.Application.DTOs.CStatusDTOs;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.DTOs.PaymentDTOs.Response;
using OCPP.Core.Persistence.Services.Payment;
using OCPP.Core.Persistence.Services.Requestor;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargePointsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RequestorService _requestorService;
        private readonly IMapper _mapper;
        private readonly OCPPCoreContext _context;
        public ChargePointsController(IUnitOfWork unitOfWork, IMapper mapper,OCPPCoreContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _requestorService = new RequestorService(_unitOfWork);
        }

        private async Task<List<DeleteCustomerSavedCards>> GetCards(GetCustomerCards getCustomerCards)
        {
            string data = await _requestorService.Requestor(getCustomerCards, "GetCustomerCards", MethodType.Post);
            var cards = JsonSerializer.Deserialize<List<DeleteCustomerSavedCards>>(data);
            return cards;

        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ChargepointPostDTO postDTO)
        {
            ChargePoint chargePoint = _mapper.Map<ChargePoint>(postDTO);
            await _unitOfWork.RepositoryChargePoint.InsertAsync(chargePoint);
            await _unitOfWork.CommitAsync();
            return Ok(chargePoint);
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(ChargepointPostDTO postDTO)
        {
            ChargePoint exist = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == postDTO.ChargePointId);
            if (exist == null)
            {
                return NotFound();
            }

            if (postDTO.CpImages != null && postDTO.CpImages.Count > 0)
            {
                var images = await _unitOfWork.RepositoryCpImage.GetAllAsync(x => x.ChargePointId == exist.ChargePointId).ToListAsync();
                _unitOfWork.RepositoryCpImage.RemoveRange(images);
                foreach (var img in postDTO.CpImages)
                {
                    CpImage cpImagE = _mapper.Map<CpImage>(img);
                    cpImagE.ChargePointId = exist.ChargePointId;
                    _unitOfWork.RepositoryCpImage.InsertAsync(cpImagE);
                }
            }
            exist.Name = postDTO.Name;
            exist.Username = postDTO.Username;
            exist.Comment = postDTO.Comment;
            exist.TarifId = postDTO.TarifId;
            exist.ClientCertThumb = postDTO.ClientCertThumb;
            await _unitOfWork.CommitAsync();
            return Ok(exist);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string ChargePointId)
        {
            ChargePoint exist = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == ChargePointId);
            if (exist == null)
            {
                return NotFound();
            }
            exist.IsDeleted = true;
            await _unitOfWork.CommitAsync();
            return Ok(exist);
        }

        [HttpPut("Power")]
        public async Task<IActionResult> Power(string ChargePointId, int ConnectorId,string Type)
        {
            var connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x =>
                x.ChargePointId == ChargePointId && x.ConnectorId == ConnectorId);
            if (connector == null)
            {
                return NotFound();
            }

            connector.Type = Type;
            await _unitOfWork.CommitAsync();
            return Ok();

        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(string? search)
        {
            var data = _unitOfWork.RepositoryChargePoint.GetAllAsync(x => x.IsDeleted==false,false, "Tarif","CpImages","Location", "ConnectorStatus");
            if(search!=null)
            {
                data = data.Where(x => x.Name.Contains(search));
            }
            List<CpReturnDTO> cpReturnDTO = _mapper.Map<List<CpReturnDTO>>(data);
            return Ok(cpReturnDTO.ToList());
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string ChargePointId)
        {
            ChargePoint exist = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == ChargePointId && x.IsDeleted==false,false, "CpImages", "Location", "ConnectorStatus","Tarif");
            if (exist == null)
            {
                return NotFound();
            }
            CpReturnDTO cpReturnDTO = _mapper.Map<CpReturnDTO>(exist);
            return Ok(cpReturnDTO);
        }
        [HttpGet("Status")]
        public async Task<IActionResult> GetStatus(string ChargePointId)
        {
            List<ConnectorStatus> connector=await _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x=>x.ChargePointId==ChargePointId).ToListAsync();
            if (connector == null)
            {
                return NotFound();
            }

            List<ReservsDTO> connReservsDtos = _mapper.Map < List<ReservsDTO>>(connector);
            return Ok(connReservsDtos);
        }
        [HttpPost("Reserv")]
        public async Task<IActionResult> Reserv(CpReservDTO cpReserv)
        {
            ConnectorStatus connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == cpReserv.ChargePointId && x.ConnectorId == cpReserv.ConnectorId);
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == cpReserv.AppUserId);
            ChargePoint chargePoint =
                await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == cpReserv.ChargePointId,false,"Tarif");
            var total = cpReserv.Minute * chargePoint.Tarif.PriceForKw;
            if (user.Balance < total)
            {
                return BadRequest("Balance must be minimum " + total);
            }
            user.Balance -= total;
            ConnectorLog connectorLog = new ConnectorLog
            {
                LastStatus = connector.LastStatus,
                NewStatus = "Reserved",
                ConnectorId=cpReserv.ConnectorId,
                AppUserId = cpReserv.AppUserId,
                ChargePointId = cpReserv.ChargePointId,
                StartReserv = DateTime.UtcNow.AddHours(4),
                ReservTime = DateTime.UtcNow.AddHours(4).AddMinutes(cpReserv.Minute)
            };
            if (user==null || user.IsBlocked)
            {
                return NotFound("User is not exist");
            }
            if (connector == null)
            {
                return NotFound("ChargePoint is not exist");
            }
            if(connector.LastStatus != "Available")
            {
                return BadRequest("ChargePoint is not Available");
            }
            connector.LastStatus="Reserved";
            connector.ReservMinute = cpReserv.Minute;
            connector.ReservUser = cpReserv.AppUserId;
            connector.ReservStart = DateTime.UtcNow.AddHours(4);
            connector.ReservTime = DateTime.UtcNow.AddHours(4).AddMinutes(cpReserv.Minute);
            MessageLog messageLog = new MessageLog
            {
                LogTime = DateTime.UtcNow.AddHours(4),
                ChargePointId=cpReserv.ChargePointId,
                ConnectorId=cpReserv.ConnectorId,
                Message= "StatusNotification",
                Result= "Info= / Status=Reserved / ",
                ErrorCode= "NoError"

            };
            await _unitOfWork.RepositoryMessageLog.InsertAsync(messageLog);
            await _unitOfWork.RepositoryConnectorLog.InsertAsync(connectorLog);
            await _unitOfWork.CommitAsync();
            return Ok();
        }
        [HttpPost("RejectReserv")]
        public async Task<IActionResult> Reject(RejectReservDTO rejectReserv)
        {
            ConnectorLog connector=await _unitOfWork.RepositoryConnectorLog.GetAllAsync(x=>x.ChargePointId==rejectReserv.ChargePointId).OrderByDescending(x=>x.StartReserv).FirstOrDefaultAsync();
            if (connector == null)
            {
                return BadRequest("User or Charge is not same");
            }
            if (connector!=null && connector.AppUserId != rejectReserv.AppUserId)
            {
                return BadRequest("User is not same");
            }
            connector.LastStatus = "Reserved";
            connector.NewStatus = "Available";
            ConnectorStatus status = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == rejectReserv.ChargePointId && x.ConnectorId == rejectReserv.ConnectorId);
            status.LastStatus = "Available";
            status.ReservMinute = 0;
            status.ConnectorId=rejectReserv.ConnectorId;
            status.ReservUser = " ";
            status.LastStatusTime= DateTime.Now.AddHours(4);
            status.LastStatusTime = DateTime.Now.AddHours(4);
            status.ReservTime = null;
            await _unitOfWork.CommitAsync();
            return Ok();
        }
        [HttpPost("Start")]
        public async Task<IActionResult> Start(ChargeStartDTO chargeStart)
        {
            ChargeTag chargeTag = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.AppUserId == chargeStart.UserId);
            chargeTag.WithBalance = chargeStart.IsBalance;
            await _unitOfWork.CommitAsync();
            var existTransaction =await _unitOfWork.RepositoryTransaction.GetAsync(x => x.StartTag.TagId == chargeTag.TagId && x.StopTime==null,false,"StartTag");
            if (existTransaction!=null)
            {
                return BadRequest("You have an outstanding transaction");
            }
            
            ChargePoint charge = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId==chargeStart.ChargePointId);
            if(charge==null)
            {
                return NotFound("Point Notfound");
            }
            ConnectorStatus connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == charge.ChargePointId && x.ConnectorId == chargeStart.ConnectorId);
            if(connector==null)
            {
                return NotFound("Connector Notfound");
            }
            if(connector.LastStatus== "Reserved" && connector.ReservUser!=chargeStart.UserId)
            {
                return BadRequest("Point is not Available");
            }
            if (!chargeStart.IsBalance)
            {
                var preauth =await PreAth(chargeStart.PreAuth);
                if (preauth==null)
                {
                    //return BadRequest("Payment error");
                }
            }
            UrlData url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "RemoteStartTransaction");
            if (url == null)
            {
                return NotFound("Url is not exist");
            }
            chargeStart.UserId=chargeTag.TagId;
            string regUrl=url.Value+chargeStart.ChargePointId;
            CallService<CpReservDTO> call = new CallService<CpReservDTO>();
            string jsonString = JsonSerializer.Serialize(chargeStart);
            var resp =await PostDataWithTimeoutAsync(regUrl, jsonString);
            if (resp != "timeout")
            {
                return BadRequest(resp);
            }
            
            return Ok();
        }
        [HttpPost("Stop")]
        public async Task<IActionResult> Stop(string ChargePointId, ChargeStopDTO chargeStart)
        {
            var url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "RemoteStopTransaction",false);
            if (url == null)
            {
                return NotFound("Url is not exist");
            }
            CallService<CpReservDTO> call = new CallService<CpReservDTO>();
            Transaction transaction = await _unitOfWork.RepositoryTransaction.GetAsync(x => x.TransactionId == chargeStart.TransactionId,true,"StartTag.AppUser","ChargePoint.Tarif","ChargePoint.ConnectorStatus");
            if (transaction == null)
            {
                return NotFound("Transaction is null");
            }

            ChargeTag chargeTag =
                await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.TagId == transaction.StartTag.TagId);
            if (transaction.IsPayment == true)
            {
                return BadRequest("Has been paid");
            }
            var charge = transaction.ChargePoint;
            decimal total = Convert.ToDecimal(transaction.EndMessage) * Convert.ToDecimal(charge.Tarif.PriceForKw);
            transaction.TotalAmount =Convert.ToDouble(total) ;
            transaction.StopTime=DateTime.UtcNow.AddHours(4);
            string urlEnd = url.Value + ChargePointId;
            transaction.IsPayment = true;
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (!chargeTag.WithBalance)
            {
                var preauthcomp =await PreAthCompletion(transaction.StartTag.AppUserId,transaction, transaction.TotalAmount);
                if (preauthcomp == null)
                {
                    return BadRequest("Payment error"+preauthcomp.Status);
                }
            }
            else
            {
                var user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == chargeTag.AppUserId);
                user.Balance -= transaction.TotalAmount;
                await _unitOfWork.CommitAsync();
            }
            
            var connector =
                transaction.ChargePoint.ConnectorStatus.Where(x => x.ConnectorId == transaction.ConnectorId).FirstOrDefault();
            connector.LastStatus = "Available";
            connector.ReservMinute = 0;
            connector.ReservUser = null; 
            string jsonString = JsonSerializer.Serialize(chargeStart);
            var resp =await PostDataWithTimeoutAsync(url.Value+ChargePointId, jsonString);
            if (resp != "timeout")
            {
                return BadRequest(resp);
            }
            
            return Ok();
        }

        [HttpPost("Restart")]
        public async Task<IActionResult> Restart(string ChargePointId)
        {
            ChargePoint charge = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == ChargePointId);
            if (charge==null)
            {
                return NotFound();
            }

            var url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "Restart");
            if (url == null)
            {
                return NotFound("Url is null");
            }

            var result =await ResetService(url.Value+ChargePointId);
            return Ok(result);
        }
        [HttpGet("ActiveStatus")]
        public async Task<IActionResult> ActiveStatus(string? ChargePointId)
        {
            UrlData url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "Status");
            if (url == null)
            {
                return NotFound("Url is not exist");
            }
            
            CallService<CpReservDTO> call = new CallService<CpReservDTO>();
            var result =await call.Get(url.Value);
            string data = result.Content.ReadAsStringAsync().Result;
            if(data !=null)
            {
                List<ChargePointStatus> chargePoints = JsonConvert.DeserializeObject<List<ChargePointStatus>>(data);
                if(ChargePointId!=null)
                {
                    var end = chargePoints.Where(x => x.Id == ChargePointId);
                    if(end==null)
                    {
                        return NotFound();
                    }
                    return Ok(end);
                }
                return Ok(chargePoints);
            }
            return Ok();
        }
        [HttpPost("CustomerMessage")]
        public async Task<IActionResult> SendMessage(int TransacTionId,string text)
        {
            Transaction connectorStatus =
                await _unitOfWork.RepositoryTransaction.GetAsync(x=>x.TransactionId==TransacTionId);
            if (connectorStatus == null)
            {
                return NotFound();
            }
            connectorStatus.TextMessage = text;
            await _unitOfWork.CommitAsync();
            return Ok();
        }




        private async Task<string> ResetService(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(new Uri(url));
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    return e.Message;
                }
            }
        }

        private async Task<PreAuthResponse> PreAth(PreAuthSavedCard savedCard)
        {
            var result = await _requestorService.Requestor(savedCard, "PreauthWithSavedCardRecurring", MethodType.Post);
            PreAuthResponse response=  JsonConvert.DeserializeObject<PreAuthResponse>(result);
            PaymentLog log = new PaymentLog()
            {
                AppUserId = savedCard.MemberId,
                TransactionId = response.TransactionId.ToString(),
                TransactionType = response.Status,
                Status = response.Status,
                ApiType = "PreAuth",
                Amount = Convert.ToDouble(savedCard.Amount),
                CreatedAt = DateTime.UtcNow.AddHours(4)
            };
            _unitOfWork.RepositoryPaymentLog.InsertAsync(log);
            await _unitOfWork.CommitAsync();
            if (response.Status != "PREAUTH-APPROVED")
            {
                return null;
            }

            return response;
        }
        private async Task<PreauthCompResponse> PreAthCompletion(string UserId,Transaction transaction ,double amount)
        {
            PreauthCompletion completion = new PreauthCompletion()
            {
                TransactionId = transaction.CardUID,
                Amount = amount,
                MemberId = UserId,
                Language = "En",
                PartnerId = "string"
            };
            var  request =await _requestorService.Requestor(completion, "PreauthCompletion", MethodType.Post);
            if (request == null || request.Contains("Hata710"))
            {
                return null;
            }
            PreauthCompResponse response = JsonSerializer.Deserialize<PreauthCompResponse>(request);
            PaymentLog log = new PaymentLog()
            {
                AppUserId = completion.MemberId,
                TransactionId = transaction.CardUID.ToString(),
                TransactionType = "PreAuthComp",
                Status = response.Status,
                ApiType = "PreAthCompletion",
                Amount = completion.Amount,
                CreatedAt = DateTime.UtcNow.AddHours(4)
            };
            _unitOfWork.RepositoryPaymentLog.InsertAsync(log);
            await _unitOfWork.CommitAsync();
            if (response.Status != "Successful")
            {
                return null;
            }

            return response;
        }
        private async Task<string> PostDataWithTimeoutAsync(string url, string jsonData)
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            if (url.Contains("Stop"))
            {
                client.Timeout = TimeSpan.FromSeconds(15);
            }

            try
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
        
                response.EnsureSuccessStatusCode(); 

                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                return "timeout";
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while posting data.", ex);
            }
        }
        private async Task<bool> PaymentFromBalance(int TransactionId)
        {
            Transaction transaction = await _unitOfWork.RepositoryTransaction.GetAsync(x => x.TransactionId == TransactionId,false, "StartTag");
            ChargeTag chargeTag = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.TagId == transaction.StartTag.TagId);
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == chargeTag.AppUserId);
            ChargePoint chargePoint = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == transaction.ChargePointId,false,"Tarif");
            if (user.Balance > 30)
            {
                
            }
            else
            {
                /*var data = _requestorService.Requestor(dto, "PreauthWithSavedCardRecurring", MethodType.Post);
                if (data.Result.Contains("Hata710"))
                {
                    return false;
                }*/
   
            }
            if (transaction.MeterStop !=null)
            {
                double stopKw = transaction.MeterStop ?? 0.0;
                double Kw = stopKw - transaction.MeterStart;
                double total = Kw * chargePoint.Tarif.PriceForKw;
                user.Balance = user.Balance - total;
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;


        }

      


    }
}
