using AutoMapper;
using FinancialChatApi.Dtos.Accounts;
using FinancialChatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancialChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<Account> userManager, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(accountLoginDto.Email);

            if (user == default)
                return NotFound();

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, accountLoginDto.Password);

            if (!passwordCheckResult)
                return BadRequest("Incorrect password.");

            var token = await CreateToken(user);
            return Ok(new { token, userName = user.Email, id = user.Id });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(AccountRegisterDto accountRegisterDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(accountRegisterDto.Email);
            if (existingUser != default)
                return BadRequest("Email is already in use.");

            var user = _mapper.Map<Account>(accountRegisterDto);
            var userCreateResult = await _userManager.CreateAsync(user, accountRegisterDto.Password);

            if (!userCreateResult.Succeeded)
                return BadRequest(string.Join(" ", userCreateResult.Errors.Select(e => e.Description)));

            var token = await CreateToken(user);
            return Ok(new { token, userName = user.Email, id = user.Id });
        }

        private async Task<string> CreateToken(Account user)
        {
            var claims = new List<Claim>
             {
                  new Claim(ClaimTypes.Email, user.Email),
                  new Claim(ClaimTypes.Name, user.UserName),
                  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(5),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
