using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Only Admin can use these endpoints
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ------------------------------------------------------
        // 1. GET ALL USERS + THEIR ROLES
        // ------------------------------------------------------
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // ------------------------------------------------------
        // 2. ASSIGN ROLE
        // ------------------------------------------------------
        [HttpPost("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> GiveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role does not exist.");

            await _userManager.AddToRoleAsync(user, roleName);
            return Ok($"Role '{roleName}' added to {user.Email}");
        }

        // ------------------------------------------------------
        // 3. REMOVE ROLE
        // ------------------------------------------------------
        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            await _userManager.RemoveFromRoleAsync(user, roleName);
            return Ok($"Role '{roleName}' removed from {user.Email}");
        }

        // ------------------------------------------------------
        // 4. CREATE ADMIN USER
        // ------------------------------------------------------
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserRequest dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok("Admin created.");
        }

        // ------------------------------------------------------
        // 5. CREATE TEACHER USER
        // ------------------------------------------------------
        [HttpPost("create-teacher")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateUserRequest dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Teacher");

            return Ok("Teacher created.");
        }

        // ------------------------------------------------------
        // OPTIONAL: CREATE STUDENT
        // ------------------------------------------------------
        [HttpPost("create-student")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateUserRequest dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Student");

            return Ok("Student created.");
        }
    }

    // SIMPLE DTO FOR USER CREATION
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
