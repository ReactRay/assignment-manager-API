using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentTeacherManagment.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentTeacherManagment.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenService(
            IConfiguration config,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullname", user.FullName),
                new Claim("userid", user.Id)
            };


            // 1️⃣ Add ROLE claims

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Load any CLAIMS attached TO THE ROLE
                var identityRole = _roleManager.FindByNameAsync(role).Result;
                var roleClaims = _roleManager.GetClaimsAsync(identityRole).Result;

                // Add permissions to token
                foreach (var rc in roleClaims)
                {
                    claims.Add(rc); // permission : Permissions.Assignments.Create
                }
            }

            // create JWT
      
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
