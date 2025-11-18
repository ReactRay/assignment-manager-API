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


    }
}
