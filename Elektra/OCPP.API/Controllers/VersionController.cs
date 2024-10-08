using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.VersionDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VersionController:ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VersionController(IUnitOfWork  unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(string version)
    {
        var data = await _unitOfWork.RepositoryVersionHistory.GetAsync(x=>x.IsDeleted==false);
        if (data!=null && data.Version != version && data.IsCritic)
        {
            return BadRequest(data.Version);
        }
        return Ok(data.Version);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(VersionPostDTO versionPostDto)
    {
        var exist = await _unitOfWork.RepositoryVersionHistory.GetAsync(x =>x.IsDeleted == false);
        if (exist != null)
        {
            return BadRequest("Current version is exist.Please delete and create new version");
        }
        VersionHistory versionHistory = _mapper.Map<VersionHistory>(versionPostDto);
        await _unitOfWork.RepositoryVersionHistory.InsertAsync(versionHistory);
        await _unitOfWork.CommitAsync();
        return Ok(versionPostDto);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(int Id)
    {
        var exist = await _unitOfWork.RepositoryVersionHistory.GetAsync(x => x.Id == Id && x.IsDeleted == false);
        if (exist == null)
        {
            return NotFound();
        }
        exist.IsDeleted = true;
        await _unitOfWork.CommitAsync();
        return Ok();
    }
}