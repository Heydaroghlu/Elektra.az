using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.WishListDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WishListsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string UserId)
        {
            var data=_unitOfWork.RepositoryWishlist.GetAllAsync(x=>x.AppUserId== UserId,false,"ChargePoint.CpImages","ChargePoint.Location");  
            List<WishListDTO> wishList=_mapper.Map<List<WishListDTO>>(data);
            return Ok(wishList);
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(string UserId,string ChargeId)
        {
            Wishlist wishlist = new Wishlist
            {
                AppUserId = UserId,
                ChargePointId = ChargeId
            };
            await _unitOfWork.RepositoryWishlist.InsertAsync(wishlist);
            await _unitOfWork.CommitAsync();
            return Ok(wishlist);
        }
        [HttpPost("Remove")]
        public async Task<IActionResult> Remove(int Id)
        {
            var data = await _unitOfWork.RepositoryWishlist.GetAsync(x => x.Id == Id);
            if(data == null)
            {
                return NotFound();
            }
             _unitOfWork.RepositoryWishlist.Remove(data);
            await _unitOfWork.CommitAsync();
            return Ok();
        }
    }
}
