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

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargePointsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ChargePointsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ChargepointPostDTO postDTO)
        {
            ChargePoint chargePoint = _mapper.Map<ChargePoint>(postDTO);
            await _unitOfWork.RepositoryChargePoint.InsertAsync(chargePoint);
            foreach (var item in postDTO.ConnectorId)
            {
                ChargeConnector chargeConnector = new ChargeConnector
                {
                    ConnectorId = item,
                    ChargePoint = chargePoint,
                    ChargePointId = chargePoint.ChargePointId
                };
                await _unitOfWork.RepositoryChargeConnector.InsertAsync(chargeConnector);
            }
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
            exist.Name = postDTO.Name;
            exist.Username = postDTO.Username;
            exist.Comment = postDTO.Comment;
            exist.PowerKw = postDTO.PowerKw;
            exist.PriceForReserv=postDTO.PriceForReserv;
            exist.PriceForHour=postDTO.PriceForHour;
            exist.PriceForKw=postDTO.PriceForKw;    
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

        [HttpGet("All")]
        public async Task<IActionResult> GetAll(string? search)
        {
            var data = _unitOfWork.RepositoryChargePoint.GetAllAsync(x => x.IsDeleted==false,false, "CpImages","Location", "ConnectorStatus");
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
            ChargePoint exist = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == ChargePointId && x.IsDeleted==false,false, "CpImages", "Location", "ConnectorStatus");
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

            return Ok(connector);
        }
        [HttpPost("Reserv")]
        public async Task<IActionResult> Reserv(CpReservDTO cpReserv)
        {
            ConnectorStatus connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == cpReserv.ChargePointId && x.ConnectorId == cpReserv.ConnectorId);
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == cpReserv.AppUserId);
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
            if(connector.LastStatus!= "Available")
            {
                if(connector.LastStatus== "Reserved" && connector.ReservUser!=chargeStart.UserId)
                {
                    return BadRequest("Point is not Available");
                }
                
            }
            UrlData url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "RemoteStartTransaction");
            if (url == null)
            {
                return NotFound("Url is not exist");
            }
            ChargeTag chargeTag = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.AppUserId == chargeStart.UserId);
            chargeStart.UserId=chargeTag.TagId;
            url.Value=url.Value+chargeStart.ChargePointId;
            CallService<CpReservDTO> call = new CallService<CpReservDTO>();
            call.Send(url.Value, chargeStart);
            return Ok();
        }
        [HttpPost("Stop")]
        public async Task<IActionResult> Stop(string ChargePointId, ChargeStopDTO chargeStart)
        {

            var url = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == "RemoteStopTransaction");
            if (url == null)
            {
                return NotFound("Url is not exist");
            }
           
            CallService<CpReservDTO> call = new CallService<CpReservDTO>();
            Transaction transaction = await _unitOfWork.RepositoryTransaction.GetAsync(x => x.TransactionId == chargeStart.TransactionId);
            if (transaction == null)
            {
                return NotFound("Transaction is null");
            }
            string urlEnd = url.Value + ChargePointId;
            call.Send(urlEnd, chargeStart);
            ConnectorStatus connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == ChargePointId && x.ConnectorId == transaction.ConnectorId);
            connector.LastStatus = "Available";
            connector.ReservMinute = 0;
            connector.ReservUser = null;
            await _unitOfWork.CommitAsync();
            var result = await PaymentFromBalance(transaction.TransactionId);
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





        private async Task<bool> PaymentFromBalance(int TransactionId)
        {
            Transaction transaction = await _unitOfWork.RepositoryTransaction.GetAsync(x => x.TransactionId == TransactionId,false, "StartTag");
            ChargeTag chargeTag = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.TagId == transaction.StartTag.TagId);
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == chargeTag.AppUserId);
            ChargePoint chargePoint = await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == transaction.ChargePointId);
            if (transaction.MeterStop !=null)
            {
                double stopKw = transaction.MeterStop ?? 0.0;
                double Kw = stopKw - transaction.MeterStart;
                double total = Kw * chargePoint.PriceForKw;
                user.Balance = user.Balance - total;
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;


        }


    }
}
