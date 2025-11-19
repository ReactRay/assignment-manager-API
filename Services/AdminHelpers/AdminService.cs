using Microsoft.AspNetCore.Identity;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Admin;
using StudentTeacherManagment.Services.AdminHelpers;
using System.Security.Claims;

namespace StudentTeacherManagment.Services.AdminHelpers
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserWithRolesDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserWithRolesDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return result;
        }

        public async Task<string> CreateUserWithRoleAsync(CreateUserRequestDto dto, string role)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);

            return $"{role} created.";
        }

        public async Task AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("Role does not exist.");

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task RemoveRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found.");

            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

     

    }
}
