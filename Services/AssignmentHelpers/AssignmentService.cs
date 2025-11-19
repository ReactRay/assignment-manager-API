using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Assignments;
using StudentTeacherManagment.Repositories.AssignmentRepository;
using System.Security.Claims;

namespace StudentTeacherManagment.Services.AssignmentHelpers

{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssignmentService(IAssignmentRepository repo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<AssignmentResponseDto> CreateAsync(ClaimsPrincipal principal, CreateAssignmentDto dto)
        {
            var teacherId = _userManager.GetUserId(principal);

            var assignment = _mapper.Map<Assignment>(dto);
            assignment.TeacherId = teacherId;

            assignment = await _repo.CreateAsync(assignment);

            return _mapper.Map<AssignmentResponseDto>(assignment);
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetAllAsync(ClaimsPrincipal principal)
        {
            var userId = _userManager.GetUserId(principal);
            var roles = await GetRolesAsync(principal);

            bool isTeacher = roles.Contains("Teacher");

            var data = isTeacher
                ? await _repo.GetByTeacherIdAsync(userId)
                : await _repo.GetAllAsync();

            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(data);
        }

        public async Task<object> GetByIdAsync(ClaimsPrincipal principal, Guid id)
        {
            var roles = await GetRolesAsync(principal);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var assignment = (isTeacher || isAdmin)
                ? await _repo.GetFullByIdAsync(id)
                : await _repo.GetByIdAsync(id);

            if (assignment == null)
                throw new Exception("Assignment not found");

            return (isTeacher || isAdmin)
                ? _mapper.Map<AssignmentDetailsDto>(assignment)
                : _mapper.Map<AssignmentResponseDto>(assignment);
        }

        public async Task<AssignmentResponseDto> UpdateAsync(ClaimsPrincipal principal, Guid id, UpdateAssignmentDto dto)
        {
            var userId = _userManager.GetUserId(principal);
            var roles = await GetRolesAsync(principal);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var assignment = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Not found");

            if (isTeacher && assignment.TeacherId != userId)
                throw new UnauthorizedAccessException("Not allowed");

            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;

            var updated = await _repo.UpdateAsync(assignment);

            return _mapper.Map<AssignmentResponseDto>(updated);
        }

        public async Task DeleteAsync(ClaimsPrincipal principal, Guid id)
        {
            var userId = _userManager.GetUserId(principal);
            var roles = await GetRolesAsync(principal);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var assignment = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Not found");

            if (isTeacher && assignment.TeacherId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Not allowed");

            await _repo.DeleteAsync(id);
        }

        private async Task<IList<string>> GetRolesAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            return await _userManager.GetRolesAsync(user);
        }
    }
}
