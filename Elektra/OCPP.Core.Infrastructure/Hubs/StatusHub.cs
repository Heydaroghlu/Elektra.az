using System.Text.Json;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.LocationDTOs;
using OCPP.Core.Application.DTOs.StatusDTOs;
using OCPP.Core.Application.DTOs.TarifDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Infrastructure.Services.StringService;
using Transaction = System.Transactions.Transaction;

namespace OCPP.Core.Infrastructure.Hubs;

public class StatusHub:Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropService _propService=new PropService();
    private readonly OCPPCoreContext _context;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    
    public StatusHub(IUnitOfWork unitOfWork,OCPPCoreContext context,HttpClient httpClient,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _httpClient = httpClient;
        _context = context;
        _mapper = mapper;
    }

    public async Task SendStatus()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("https://ocpp.elektra.pp.az/API/Status");
            List<ChargePointStatus> chargePoints = JsonConvert.DeserializeObject<List<ChargePointStatus>>(response);
            foreach (var point in chargePoints)
            {
                var chargePoint =
                    await _unitOfWork.RepositoryChargePoint.GetAsync(x => x.ChargePointId == point.Id,false,"Tarif","Location");
                if (chargePoint!=null)
                {
                    point.Tarif = _mapper.Map<TarinfReturnDTO>(chargePoint.Tarif);
                    point.Location = _mapper.Map<LocationReturnDTO>(chargePoint.Location);

                }
              
                foreach (var connector in point.OnlineConnectors)
                {
                    var logStatus =  _context.MessageLogs.Where(x =>x.ChargePointId==point.Id && x.ConnectorId==connector.Key && x.Result.Contains("Status"))
                        .OrderByDescending(x => x.LogId).FirstOrDefault().Result;
                    if (connector.Value.Status == 7)
                    {
                        connector.Value.Status =6;
                    }
                    if (connector.Value.Status ==2 && logStatus!=null && logStatus.Contains("Charging"))
                    {
                        connector.Value.Status = 3;
                        OCPP.Core.Database.Transaction?  transaction = await _context.Transactions.Where(x => x.ChargePointId == point.Id && x.ConnectorId == connector.Key && x.StopTime==null).OrderByDescending(x => x.TransactionId).Include(x=>x.ChargePoint).ThenInclude(x=>x.Tarif).Include(x=>x.ChargePoint.Location).Include(x=>x.StartTag).ThenInclude(x=>x.AppUser).FirstOrDefaultAsync();
                        if (transaction != null)
                        {
                            
                            transaction.EndPercent = connector.Value.SoC.ToString();
                            connector.Value.MeterKWH -= transaction.MeterStart;
                            connector.Value.StartTime = transaction.StartTime;
                            connector.Value.Message = transaction.TextMessage;
                            if (transaction.MeterStart > 0)
                            {
                                transaction.EndMessage = connector.Value.MeterKWH.ToString();
                            }
                            else
                            {
                                transaction.EndMessage = 8.ToString();
                            }
                            if (transaction.StartTag!=null)
                            {
                                connector.Value.User = _mapper.Map<UserReturnDTO>(transaction.StartTag.AppUser);
                                connector.Value.UserId = transaction.StartTag.AppUserId;
                            }
                            connector.Value.TransactionId = transaction.TransactionId;
                            await _unitOfWork.CommitAsync();
                        }
                    }
                    else
                    {
                        

                    
                        
                        var status = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x =>
                            x.ChargePointId == point.Id && x.ConnectorId == connector.Key,false,"ChargePoint");
                        if (status.LastStatus == "Reserved")
                        {
                            connector.Value.UserId = status.ReservUser;
                            connector.Value.Status = 6;
                        }
                    }
                }
            }

            await Clients.All.SendAsync("ReceiveMessage", chargePoints);
        }
        catch (HttpRequestException httpEx)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"HTTP request error: {httpEx.Message}");
        }
        catch (TaskCanceledException taskEx)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"Request timeout: {taskEx.Message}");
        }
        catch (Exception ex)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"General error: {ex.Message}");
        }


    }

}