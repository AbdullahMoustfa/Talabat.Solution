using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract.AuthService;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController( // Ask CLR To Create Object From Service UserManager + SingInManager
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
           IAuthService authService,
           IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiResponse(401, "Email already existed"));

            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };

            var ResultRegister = await _userManager.CreateAsync(user, model.Password);
            if (!ResultRegister.Succeeded) return BadRequest(new ApiResponse(400));

            var Returned = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            };

            return Ok(Returned);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401, "Invalid Login!!"));

            var resultSingIn = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!resultSingIn.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid Login!!"));

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet] // GET: /api/Account
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email); // Now I got the Email Of user

            var user = await _userManager.FindByEmailAsync(email); // Get the User of this email

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet("address")] // GET: /api/Account/address
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var user = await _userManager.FindByEmailAsync(email);


            var user = await _userManager.FindUserWithAddressAsync(User);

            var MappedAddress = _mapper.Map<Address, AddressDto>(user.Address); 

            return Ok(MappedAddress);
        }

        [Authorize]
        [HttpPut("address")] // PUT: /api/Account/address
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdatedAddress)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var MappedAddress = _mapper.Map<AddressDto, Address>(UpdatedAddress);
            MappedAddress.Id = user.Address.Id;
            user.Address = MappedAddress;
            var Result = await _userManager.UpdateAsync(user);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(UpdatedAddress);
        }

        [HttpGet("emailExists")] // GET: baseUrl/api/Account/emailExists
        public async Task<ActionResult<bool>> CheckEmailExists(string Email)
        {
          // var email = await _userManager.FindByEmailAsync(Email);
          // if (email is null) return false;
          //
          // else return true;

            return await _userManager.FindByEmailAsync(Email) is not null;
        }
    }
}
