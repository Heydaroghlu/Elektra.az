using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.Core.Infrastructure.Hubs;

public class StatusHub:Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public StatusHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SendStatus()
    {
            var connector = await _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x => x.ChargePointId == "Mr12").ToListAsync();
            if (connector != null && connector.Count > 0)
            {
                await Clients.All.SendAsync("ReceiveMessage", connector);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", connector);
            }
        
        
    }
}