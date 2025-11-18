using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs;
using StudentTeacherManagment.Models.DTOs.Assignments;
using StudentTeacherManagment.Repositories.AssignmentRepository;
using System.Security.Claims;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssignmentsController(
            IAssignmentRepository assignmentRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
            _userManager = userManager;
        }


        // POST: api/assignments
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto assignment)
        {
            // Get current teacher id from JWT
            var teacherId = _userManager.GetUserId(User); // reads NameIdentifier claim

            if (teacherId == null)
                return Unauthorized("Invalid token");

            // Map DTO -> domain model
            var domainAssignment = _mapper.Map<Assignment>(assignment);
            domainAssignment.TeacherId = teacherId;

            // Save via repository
            domainAssignment = await _assignmentRepository.CreateAsync(domainAssignment);

            var response = _mapper.Map<AssignmentResponseDto>(assignment);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = domainAssignment.Id }, response);
        }







    }
}
