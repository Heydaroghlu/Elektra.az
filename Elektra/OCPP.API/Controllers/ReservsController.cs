using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.DTOs.ReservDTOs;
using OCPP.Core.Application.DTOs.TransactionDTOs;
using OCPP.Core.Application.UnitOfWorks;

namespace OCPP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservsController:ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReservsController(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    [HttpGet("Get")]
    public async Task<IActionResult> Get(int Id)
    {
        var data = await _unitOfWork.RepositoryConnectorLog.GetAsync(x => x.Id==Id,false,"AppUser","ChargePoint.Tarif");
        if (data == null)
        {
            return NotFound();
        }

       var transactionPostDto = _mapper.Map<ReservPostDTO>(data);
        return Ok(transactionPostDto);
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(bool? IsActive=null)
    {
        if (IsActive == null)
        {
            var data =await  _unitOfWork.RepositoryConnectorLog.GetAllAsync(x => x.Id!=0,false,"AppUser","ChargePoint.Tarif").ToListAsync();
        }
        if (IsActive==true)
        {
            var data =await  _unitOfWork.RepositoryConnectorLog.GetAllAsync(x => x.ReservTime>DateTime.UtcNow.AddHours(4),false,"AppUser","ChargePoint.Tarif").ToListAsync();
            var transactionPostDtos = _mapper.Map<List<ReservPostDTO>>(data);
            return Ok(transactionPostDtos);
        }
        else
        {
            var data = await _unitOfWork.RepositoryConnectorLog.GetAllAsync(x => x.ReservTime<DateTime.UtcNow.AddHours(4),false,"AppUser" ,"ChargePoint.Tarif").ToListAsync();
            var transactionPostDtos = _mapper.Map<List<ReservPostDTO>>(data);
            return Ok(transactionPostDtos);
        }
    }
}