using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.CStatusDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Infrastructure.Services.SMS;
using OCPP.Core.Persistence.Services.Tag;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SmileService smileService = new SmileService();
        private readonly TagService tagService = new TagService();
        public AccountsController(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper; 
        }
        [HttpGet("createrole")]
        public async Task<IActionResult> CreateRoles()
        {
            //var role1 = new IdentityRole("Member");
            //var role2 = new IdentityRole("Admin");
            var role2 = new IdentityRole("Member");
            //await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);
            return Ok();
        }
        [HttpGet("Users")]
        public async Task<IActionResult> Users(bool IsBlocked=false)
        {
            var data =  _unitOfWork.RepositoryUser.GetAllAsync(x => x.IsBlocked == IsBlocked);
            return Ok(data);
        }
        [HttpPost("Regsiter")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x=>x.UserName==registerDTO.PhoneNumber);
            
            if (user != null)
            {
                if (user.IsBlocked)
                {
                    string otp = await smileService.RandomCode();
                    user.OTP = otp;
                    user.UserName = registerDTO.PhoneNumber;
                    user.FullName = registerDTO.Name + " " + registerDTO.Surname;
                    user.IsBlocked = false;
                    await _unitOfWork.CommitAsync();
                    var result1 = await smileService.Send(registerDTO.PhoneNumber, $"Tesdiq kodunuz: {otp}");
                    return Ok(result1);
                }
                return BadRequest("This User is exist");
            }
           
            string code = await smileService.RandomCode();
            user = _mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.PhoneNumber;
            user.FullName = registerDTO.Name + " " + registerDTO.Surname;
            user.OTP = code;
            var creatUser = await _userManager.CreateAsync(user, "user0910");
            await _userManager.AddToRoleAsync(user, "Member");
            var result = await smileService.Send(registerDTO.PhoneNumber, $"Tesdiq kodunuz: {code}");
            if (result != "ok")
            {
                return BadRequest("Sms error");
            }
            ChargeTag chargeTag = new ChargeTag
            {
                TagId = tagService.GenerateRandomCode(),
                TagName = "Name",
                AppUserId = user.Id,
                ParentTagId = user.Id,

            };
            await _unitOfWork.RepositoryChargeTag.InsertAsync(chargeTag);   
            await _unitOfWork.CommitAsync();
            return Ok(registerDTO);

        }
        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(string PhoneNumber)
        {
            AppUser user = await _userManager.FindByNameAsync(PhoneNumber);
            if (user == null || user.IsBlocked)
            {
                return BadRequest("This User is not exist");
            }
            if(PhoneNumber=="503134473")
            {
                return Ok("ok");
            }
            string code = await smileService.RandomCode();
            user.OTP = code;
            var result = await smileService.Send(PhoneNumber, $"Tesdiq kodunuz: {code}");
            if (result != "ok")
            {
                return BadRequest(result);
            }
            await _unitOfWork.CommitAsync();
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string PhoneNumber, string OTP)
        {
            AppUser user = await _userManager.FindByNameAsync(PhoneNumber);
            if (user == null || user.IsBlocked)
            {
                return BadRequest("This User is not exist");
            }
            if (user.OTP == OTP)
            {
                var result = await _signInManager.PasswordSignInAsync(user, "user0910", false, false);
            }
            else
            {
                return BadRequest("Username or OTP is incorrect");
            }
            return Ok(user.Id);
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return StatusCode(201, "Logout is Success");
        }
        [HttpGet("MyAccount")]
        public async Task<IActionResult> MyAccount(string Id)
        {
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (user == null || user.IsBlocked)
            {
                return NotFound();
            }
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }
        [HttpPost("AccountEdit")]
        public async Task<IActionResult> AccountEdit(string Id,UserEditDTO editDTO)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x=>x.Id==Id);
            if (user == null || user.IsBlocked)
            {
                return NotFound();
            }
            user.Email=editDTO.Email;
            user.PhoneNumber=editDTO.PhoneNumber;
            user.FullName = editDTO.Name + " " + editDTO.Surname;
            await _unitOfWork.CommitAsync();
            return Ok(editDTO);
        }
        [HttpDelete("DeleteAcc")]
        public async Task<IActionResult> DeleteAcc(string Id)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == Id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsBlocked = true;
            await _unitOfWork.CommitAsync();
            return Ok();
        }
        [HttpGet("Reservs")]
        public async Task<IActionResult> Reservs(string Id)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == Id);
            if (user == null)
            {
                return NotFound("User is not exist");
            }
            var data =  _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x => x.ReservUser == user.Id && x.LastStatus=="Reserved",false, "ChargePoint.Location", "ChargePoint.CpImages");
            List<ReservsDTO> reservs=_mapper.Map<List<ReservsDTO>>(data);
            return Ok(reservs);
        }
        [HttpGet("Transactions")]
        public async Task<IActionResult> Transactions(string Id,bool IsActive=true)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == Id);
            if (user == null)
            {
                return NotFound("User is not exist");
            }
            ChargeTag chargeTag = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.AppUserId == user.Id);
            if (chargeTag == null)
            {
                return NotFound("ChargeTag is not exist");
            }
            var data= _unitOfWork.RepositoryTransaction.GetAllAsync(x=>x.StartTag.TagId== chargeTag.TagId,false,"ChargePoint.Location", "ChargePoint.CpImages");
            if(IsActive)
            {
                data = data.Where(x => x.StopTime == null);
            }
            else
            {
                data = data.Where(x => x.StopTime != null);
            }
            List<TransactionDTO> transactions = _mapper.Map<List<TransactionDTO>>(data);
            return Ok(transactions);
        }

    }
}
