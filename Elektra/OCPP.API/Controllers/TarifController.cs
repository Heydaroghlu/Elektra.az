using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.DTOs.TarifDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TarifController:ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TarifController(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(TarifPostDTO tarifPostDto)
    {
        Tarif tarif = _mapper.Map<Tarif>(tarifPostDto);
        _unitOfWork.RepositoryTarif.InsertAsync(tarif);
        await _unitOfWork.CommitAsync();
        return Ok(tarifPostDto);
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> Edit(int Id, TarifPostDTO tarifPostDto)
    {
        var tarif = await _unitOfWork.RepositoryTarif.GetAsync(x => x.Id == Id);
        if (tarif == null)
        {
            return NotFound();
        }
    
        tarif.PowerKw = tarifPostDto.PowerKw;
        tarif.PriceForReserv = tarifPostDto.PriceForReserv;
        tarif.PriceForKw = tarifPostDto.PriceForKw;
        tarif.PriceForHour = tarifPostDto.PriceForHour;
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int Id)
    {
        var tarif = await _unitOfWork.RepositoryTarif.GetAsync(x => x.Id == Id);
        if (tarif == null)
        {
            return NotFound();
        }
        tarif.Deleted = true;
        await _unitOfWork.CommitAsync();
        return Ok();
    }
    [HttpGet("Get")]
    public async Task<IActionResult> Get(int Id)
    {
        var data = await _unitOfWork.RepositoryTarif.GetAsync(x => x.Id==Id);
        if (data == null)
        {
            return NotFound();
        }
        var tarifPostDtos = _mapper.Map<List<TarinfReturnDTO>>(data);
        return Ok(tarifPostDtos);
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.RepositoryTarif.GetAllAsync(x => !x.Deleted).ToListAsync();
        var tarifPostDtos = _mapper.Map<List<TarinfReturnDTO>>(data);
        return Ok(tarifPostDtos);
    }
}