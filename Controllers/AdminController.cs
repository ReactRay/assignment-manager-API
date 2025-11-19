using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.DTOs.Admin;
using StudentTeacherManagment.Services.AdminHelpers;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService AdminService)
        {
            _adminService = AdminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _adminService.GetAllUsersAsync());
        }

        [HttpPost("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            await _adminService.AssignRoleAsync(userId, roleName);
            return Ok($"Role '{roleName}' assigned.");
        }

        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            await _adminService.RemoveRoleAsync(userId, roleName);
            return Ok($"Role '{roleName}' removed.");
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserRequestDto dto)
        {
            return Ok(await _adminService.CreateUserWithRoleAsync(dto, "Admin"));
        }

        [HttpPost("create-teacher")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateUserRequestDto dto)
        {
            return Ok(await _adminService.CreateUserWithRoleAsync(dto, "Teacher"));
        }

        [HttpPost("create-student")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateUserRequestDto dto)
        {
            return Ok(await _adminService.CreateUserWithRoleAsync(dto, "Student"));
        }


    }
}
