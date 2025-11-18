using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Repositories.SubmissionRepository;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubmissionsController(
            ISubmissionRepository submissionRepo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _submissionRepo = submissionRepo;
            _mapper = mapper;
            _userManager = userManager;
        }



        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateSubmission([FromBody] SubmissionCreateDto dto)
        {
            var studentId = _userManager.GetUserId(User);

            if (studentId == null)
                return Unauthorized("Invalid token");

            // Optional: Prevent duplicate submissions per assignment
            var existing = await _submissionRepo.GetByStudentIdAsync(studentId);
            if (existing.Any(s => s.AssignmentId == dto.AssignmentId))
                return BadRequest("You already submitted this assignment.");

            // Map DTO -> domain
            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                Content = dto.Content,
                SubmittedAt = DateTime.UtcNow,
                Status = "Submitted"
            };

            submission = await _submissionRepo.CreateAsync(submission);

            var response = _mapper.Map<SubmissionResponseDto>(submission);
            return Ok(response);
        }
    }
}
