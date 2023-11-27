using BooksApi.Context;
using BooksApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BooksApi.Token;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenGenerator _token;

        public RegisterController(UserManager<AppUser> userManager, TokenGenerator token)
        {
            _userManager = userManager;
            _token = token;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new AppUser { Id = Guid.NewGuid().ToString(), Ad = model.Name, Soyad=model.Surname, Email = model.Email, Password = model.Password, UserName = model.UserName};

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("kayıt başarılı");
            }

            return BadRequest("Kayıt başarısız" );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = _userManager.Users.Any(x => x.Email == model.Email && x.Password == model.Password);

            if (result)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = _token.GenerateToken(user);

                return Ok(new { Token = token });
            }

            return Unauthorized("login işlemi başarısız.");
            
        }

    }

}
