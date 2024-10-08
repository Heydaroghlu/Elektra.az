using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.DTOs.DashboardDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Infrastructure.Services.SMS;

namespace OCPP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashBoardController:ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SmileService _smileService = new SmileService();

    public DashBoardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("UserBlock")]
    public async Task<IActionResult> UserBlocked(string UserId)
    {
        AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == UserId);
        if (user == null)
        {
            return NotFound();
        }
        user.IsBlocked = true;
        await _unitOfWork.CommitAsync();
        return Ok();
    }
    [HttpPost("UserUnBlock")]
    public async Task<IActionResult> UserUnBlocked(string UserId)
    {
        AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == UserId);
        if (user == null)
        {
            return NotFound();
        }
        user.IsBlocked = false;
        await _unitOfWork.CommitAsync();
        return Ok();
    }
    [HttpPost("BalanceEdit")]
    public async Task<IActionResult> BalanceEdit(BalanceEditDTO balanceEditDto)
    {
        AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == balanceEditDto.UserId);
        if (user == null)
        {
            return NotFound();
        }
        user.Balance = balanceEditDto.Amount;
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost("SendSms")]
    public async Task<IActionResult> SendSms(string? PhoneNumber,string Message)
    {
        if (PhoneNumber != null)
        {
            _smileService.Send(PhoneNumber, Message);
        }
        else
        {
            var users = await _unitOfWork.RepositoryUser.GetAllAsync(x => x.IsBlocked == false && x.Type == 0).ToListAsync();
            foreach (var user in users)
            {
                if (user.PhoneNumber != null)
                {
                    await _smileService.Send(user.PhoneNumber, Message);
                }
               
            }
        }

        return Ok();
    }
    [HttpGet("Stations")]
    public async Task<IActionResult> Stations()
    {
        var ports = await _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x => x.ConnectorId!=0).ToListAsync();
        var charges = await _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x => x.ConnectorId != 0).GroupBy(x=>x.ChargePointId).ToListAsync();
        var active = ports.Where(x=>x.LastStatus=="Aviable").ToList();
        var deactive = ports.Where(x => x.LastStatus != "Aviable").ToList();
        DashboardDTO dashboardDto = new DashboardDTO()
        {
            Station = 12,
            Active = 2,
            DeActive = 10,
            Seans = 23,
            ActiveSeans = 0,
            UsedEnergy = 34789,
            Cost = 345.89
        };
        return Ok(dashboardDto);
    }

    [HttpGet("PaymentLogs")]
    public async Task<IActionResult> PaymentLogs(string UserId)
    {
        var data = await _unitOfWork.RepositoryPaymentLog.GetAllAsync(x => x.AppUserId == UserId).ToListAsync();
        return Ok(data);
    }
    
    
}