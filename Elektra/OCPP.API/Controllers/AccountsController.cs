using Application.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OCPP.Core.Application.Abstractions;
using OCPP.Core.Application.DTOs.CStatusDTOs;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.DTOs.TokenDTO;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Infrastructure.Services.SMS;
using OCPP.Core.Persistence.Services.Requestor;
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
        private readonly ITokenHandler _tokenHandler;
        RequestorService _requestorService;

        public AccountsController(UserManager<AppUser> userManager,ITokenHandler tokenHandler ,IMapper mapper, IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _tokenHandler = tokenHandler;
            _mapper = mapper; 
            _requestorService = new RequestorService(_unitOfWork);
        }
        [HttpGet("createrole")]
        public async Task<IActionResult> CreateRoles()
        {
            var role1 = new IdentityRole("SuperAdmin");
            var role2 = new IdentityRole("Admin");
            var role3 = new IdentityRole("UpUser");

            //var role2 = new IdentityRole("Member");
            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);
            await _roleManager.CreateAsync(role3);

            return Ok();
        }

        [HttpPost("UserCreate")]
        public async Task<IActionResult> UserCreate()
        {
            AppUser user = new AppUser();
            user.UserName = "502300402";
            user.FullName = "Elektra-Admin";
            user.OTP = "9999";
            user.Type = 1;
            await _userManager.CreateAsync(user, "Elektra2024");
            _userManager.AddToRoleAsync(user, "Admin");
            await _unitOfWork.CommitAsync();
            return Ok();
        }
        [HttpGet("Users")]
        public async Task<IActionResult> Users(bool IsBlocked=false)
        {
            var data = await _unitOfWork.RepositoryUser.GetAllAsync(x => x.IsBlocked == IsBlocked && x.Type==0,false,"ChargeTag").ToListAsync();
            List<UserDTO> userDtos = _mapper.Map<List<UserDTO>>(data);
            return Ok(userDtos);
        }
        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin(LoginDTO loginDto)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.UserName == loginDto.UserName);
            if (user == null || user.IsBlocked)
            {
                return BadRequest("This User is not exist");
            }
            if (user.Type == 0 || user.Type == 3)
            {
                return BadRequest("You are not admin!");
            }
            var result =  _signInManager.PasswordSignInAsync(user, loginDto.Password,false,true);
            if(!result.Result.Succeeded)
            {
                return BadRequest("This User is not exist");
            }
            TokenDTO token = _tokenHandler.CreateAccessToken(user,12);
            return Ok(token);
        }

        [HttpPost("AdminEdit")]
        public async Task<IActionResult> AdminEdit(AdminEditDTO adminEditDto)
        {
            AppUser user = await _userManager.FindByIdAsync(adminEditDto.Id);
            if (user == null || user.IsBlocked)
            {
                return BadRequest("This User is not exist");
            }
            user.UserName = adminEditDto.UserName;
            if (adminEditDto.NewPassword != null)
            {
                _userManager.ChangePasswordAsync(user, adminEditDto.Password, adminEditDto.NewPassword);
            }

            return Ok();
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
                    user.RegisteredTime = DateTime.UtcNow.AddHours(4);
                    user.Type = 0;
                    await _unitOfWork.CommitAsync();
                    var result1 = await smileService.Send(registerDTO.PhoneNumber, $"Tesdiq kodunuz: {otp}");
                    return Ok(result1);
                }
                return BadRequest("This User is exist");
            }
           
            string code = await smileService.RandomCode();
            user = _mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.PhoneNumber;
            user.RegisteredTime = DateTime.UtcNow.AddHours(4);
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

            GetCustomerCards getCustomerCards = new GetCustomerCards()
            {
                MemberId = user.Id,
                PartnerId = "string"
            };
            
            
            TokenDTO token = _tokenHandler.CreateAccessToken(user,720);
            return Ok(token);

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
            var user = await _unitOfWork.RepositoryUser.GetAsync(x => x.IsBlocked == false && x.Id==Id,false,"ChargeTag");
            if (user == null || user.IsBlocked)
            {
                return NotFound();
            }
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        [HttpGet("UserPayments")]
        public async Task<IActionResult> UserPayments(string UserId)
        {
            AppUser user = await _userManager.FindByIdAsync(UserId);
            if (user == null || user.IsBlocked)
            {
                return NotFound();
            }

            var data = await _unitOfWork.RepositoryPaymentLog.GetAllAsync(x => x.AppUserId == user.Id).ToListAsync();
            List<PaymentLogReturnDTO> paymentLogReturnDtos = _mapper.Map<List<PaymentLogReturnDTO>>(data);
            return Ok(paymentLogReturnDtos);
        }

        [HttpGet("UserTag")]
        public async Task<IActionResult> UserData(string TagId)
        {
            var userId = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.TagId == TagId);
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == userId.AppUserId);
            if (user == null)
            {
                return NotFound();
            }
            UserReturnDTO userReturnDto = _mapper.Map<UserReturnDTO>(user);
            return Ok(userReturnDto);
        }
        [HttpGet("TagForUserId")]
        public async Task<IActionResult> TagForUserId(string UserId)
        {
            var user = await _unitOfWork.RepositoryChargeTag.GetAsync(x => x.AppUserId == UserId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.TagId);
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
            user.ImgUrl = editDTO.ImgUrl;
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
            var data =  _unitOfWork.RepositoryConnectorStatus.GetAllAsync(x => x.ReservUser == user.Id && x.LastStatus=="Reserved",false, "ChargePoint.Location", "ChargePoint.CpImages","ChargePoint.Tarif");
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
            var data=await _unitOfWork.RepositoryTransaction.GetAllAsync(x=>x.TransactionId!=null,false,"StartTag.AppUser","ChargePoint").ToListAsync();
            if (data!=null && data.Count>0)
            {
                if(IsActive)
                {
                    data = data.Where(x => x.StartTag.TagId==chargeTag.TagId && x.StopTime == null).ToList();
                }
                else
                {
                    data =data.Where(x =>x.StartTag.TagId==chargeTag.TagId &&  x.StopTime != null).ToList();
                }
                var transactions = _mapper.Map<List<TransactionDTO>>(data.ToList());
                return Ok(transactions);
            }

            return Ok();
        }

    }
}
