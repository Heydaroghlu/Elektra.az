using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.CallDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UrlsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(KeyValueDTO keyValue)
        {
            UrlData urlData = _mapper.Map<UrlData>(keyValue);
            await _unitOfWork.RepositoryUrl.InsertAsync(urlData);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(KeyValueDTO keyValueDto)
        {
            var exist = await _unitOfWork.RepositoryUrl.GetAsync(x => x.Key == keyValueDto.Key);
            if (exist == null)
            {
                return NotFound();
            }

            exist.Value = keyValueDto.Value;
            await _unitOfWork.CommitAsync();
            return Ok();
        }
    }
}
