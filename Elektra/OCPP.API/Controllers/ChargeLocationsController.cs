using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.LocationDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeLocationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ChargeLocationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(LocationPostDTO postDTO)
        {
            ChargeLocation chargeLocation = _mapper.Map<ChargeLocation>(postDTO);
            await _unitOfWork.RepositoryChargeLocations.InsertAsync(chargeLocation);
            await _unitOfWork.CommitAsync();
            return Ok(chargeLocation);
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(LocationPostDTO postDTO)
        {
            ChargeLocation exist=await _unitOfWork.RepositoryChargeLocations.GetAsync(x=>x.ChargePointId== postDTO.ChargePointId);
            if(exist ==null)
            {
                return NotFound();
            }
            exist.Longitude = postDTO.Longitude;
            exist.Latitude = postDTO.Latitude;
            exist.Address= postDTO.Address;
            await _unitOfWork.CommitAsync();
            return Ok(exist);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string ChargePointId)
        {
            ChargeLocation exist = await _unitOfWork.RepositoryChargeLocations.GetAsync(x => x.ChargePointId == ChargePointId);
            if (exist == null)
            {
                return NotFound();
            }
            return Ok(exist);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data =_unitOfWork.RepositoryChargeLocations.GetAllAsync(x=>x.Deleted==false,false, "ChargePoint");
            List<LocationForMapDTO> locations= _mapper.Map<List<LocationForMapDTO>>(data);
            foreach (LocationForMapDTO location in locations)
            {
                ConnectorStatus connector = await _unitOfWork.RepositoryConnectorStatus.GetAsync(x => x.ChargePointId == location.ChargePoint.ChargePointId);
                if (connector != null)
                {
                    location.ChargePoint.Status = connector.LastStatus;
                }
                else
                {
                    location.ChargePoint.Status = "Available";
                }
            }
            return Ok(locations);
        }
        
    }
}
