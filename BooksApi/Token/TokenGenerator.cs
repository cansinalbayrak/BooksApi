using BooksApi.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BooksApi.Token
{
    public class TokenGenerator
    {
        private readonly UserManager<AppUser> _manager;
        private readonly RoleManager<AppRole> _manager2;

        public TokenGenerator(UserManager<AppUser> manager, RoleManager<AppRole> manager2)
        {
            _manager = manager;
            _manager2 = manager2;
        }

        public string GenerateToken(AppUser user)
        {
            //var user = await _manager.FindByIdAsync(id);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Ad),
                new Claim(ClaimTypes.Surname, user.Soyad),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var roles = _manager.GetRolesAsync(user).Result;
            foreach (var name in roles)
            {
                var role = _manager2.FindByNameAsync(name).Result;
                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("11111Cansin11111cansin"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost",
                audience: "http://localhost",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
