
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        public DataContext _context { get; }
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto RegisterDto)
        {

            if (await UserExist(RegisterDto.UserName))
            {
                return BadRequest("UserName is taken");
            }

            using var hmac = new HMACSHA512();
            AppUser user = new AppUser
            {
                UserName = RegisterDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(RegisterDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

            
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            AppUser user;
            user = await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
            {
                return BadRequest("UserName or Password are invalid");
            }

            //validacion para ver si el password coincide
            using var hmac = new HMACSHA512(user.PasswordSalt);
            byte[] ComputedHashRequest = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < ComputedHashRequest.Length; i++)
            {
                if (ComputedHashRequest[i] != user.PasswordHash[i])
                {
                    return BadRequest("UserName or Password are invalid");
                }
            }

            //validacion para ver si el password coincide/
             return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExist(string UserName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == UserName.ToLower());

        }
    }
}