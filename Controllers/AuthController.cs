using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Auth;
using StudentTeacherManagment.Services;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign role
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Invalid role");

            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok("User registered successfully.");
        }

  
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    roles
                }
            });
        }

    
        //stam , bonus for me
        [HttpPost("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            string[] roles = { "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            return Ok("Roles created successfully");
        }
    }
}
