using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.DTOs.TransactionDTOs;
using OCPP.Core.Application.UnitOfWorks;

namespace OCPP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController:ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionsController(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    [HttpGet("Get")]
    public async Task<IActionResult> Get(int Id)
    {
        var data = await _unitOfWork.RepositoryTransaction.GetAsync(x => x.TransactionId == Id);
        if (data == null)
        {
            return NotFound();
        }
        //txt/
        var transactionPostDto = _mapper.Map<TransactionPostDTO>(data);
        return Ok(transactionPostDto);
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(bool? IsActive=null)
    {
        if (IsActive == null)
        {
            var data = await _unitOfWork.RepositoryTransaction.GetAllAsync(x => x.TransactionId!=null,false,"StartTag.AppUser","ChargePoint.CpImages").ToListAsync();
            var transactionPostDtos = _mapper.Map<List<TransactionPostDTO>>(data);
            return Ok(transactionPostDtos);
        }
        if (IsActive!=null && IsActive==false)
        {
           var data = await _unitOfWork.RepositoryTransaction.GetAllAsync(x => x.StopTime != null,false,"StartTag.AppUser","ChargePoint.CpImages").ToListAsync();
           var transactionPostDtos  = _mapper.Map<List<TransactionPostDTO>>(data);
           return Ok(transactionPostDtos);
        }
        else
        {
            var data = await _unitOfWork.RepositoryTransaction.GetAllAsync(x => x.StopTime == null,false,"StartTag.AppUser","ChargePoint.CpImages").ToListAsync();
            var transactionPostDtos = _mapper.Map<List<TransactionPostDTO>>(data);
            return Ok(transactionPostDtos);
        }
    }

   
}