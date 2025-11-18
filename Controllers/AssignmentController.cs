using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Assignments;
using StudentTeacherManagment.Permissions;
using StudentTeacherManagment.Repositories.AssignmentRepository;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssignmentsController(
            IAssignmentRepository repo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // CREATE ASSIGNMENT (Teacher/Admin)
 
        [HttpPost]
        [HasPermission(AppPermissions.Assignments.Create)]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentDto dto)
        {
            var teacherId = _userManager.GetUserId(User);

            var assignment = _mapper.Map<Assignment>(dto);
            assignment.TeacherId = teacherId;

            assignment = await _repo.CreateAsync(assignment);

            return CreatedAtAction(nameof(GetById),
                new { id = assignment.Id },
                _mapper.Map<AssignmentResponseDto>(assignment));
        }

        // GET ALL ASSIGNMENTS (Teacher = own, Student/Admin = all)
        [HttpGet]
        [HasPermission(AppPermissions.Assignments.Read)]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");

            var assignments = isTeacher
                ? await _repo.GetByTeacherIdAsync(userId)
                : await _repo.GetAllAsync();

            return Ok(_mapper.Map<IEnumerable<AssignmentResponseDto>>(assignments));
        }


        // GET ASSIGNMENT BY ID
        // Teacher/Admin -> full details (with submissions)
        // Student --> basic assignment
  
        [HttpGet("{id}")]
        [HasPermission(AppPermissions.Assignments.Read)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            // Teacher/Admin → get full assignment with submissions
            var assignment = (isTeacher || isAdmin)
                ? await _repo.GetFullByIdAsync(id)
                : await _repo.GetByIdAsync(id);

            if (assignment == null)
                return NotFound();

            // Teacher/Admin → detailed DTO
            if (isTeacher || isAdmin)
                return Ok(_mapper.Map<AssignmentDetailsDto>(assignment));

            // Student → basic DTO
            return Ok(_mapper.Map<AssignmentResponseDto>(assignment));
        }


        // UPDATE ASSIGNMENT
        // Teacher → can update ONLY their own assignments
        // Admin → can update any assignment
     
        [HttpPut("{id}")]
        [HasPermission(AppPermissions.Assignments.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentDto dto)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var assignment = await _repo.GetByIdAsync(id);
            if (assignment == null)
                return NotFound("Assignment not found.");

            // Teachers can only update their own work
            if (isTeacher && assignment.TeacherId != userId)
                return Forbid("You cannot update another teacher's assignment.");

            // Admin skips the check
            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;

            var updated = await _repo.UpdateAsync(assignment);

            return Ok(_mapper.Map<AssignmentResponseDto>(updated));
        }

        // DELETE ASSIGNMENT
        // Teacher → delete ONLY their own assignments
        // Admin → delete any assignment
     
        [HttpDelete("{id}")]
        [HasPermission(AppPermissions.Assignments.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var assignment = await _repo.GetByIdAsync(id);
            if (assignment == null)
                return NotFound("Assignment not found.");

            // Teachers can only delete their own assignments
            if (isTeacher && assignment.TeacherId != userId)
                return Forbid("You cannot delete another teacher's assignment.");

            // Admin can delete anything
            var deleted = await _repo.DeleteAsync(id);

            if (!deleted)
                return StatusCode(500, "Failed to delete assignment.");

            return NoContent();
        }

    }
}
