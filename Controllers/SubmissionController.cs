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
        public async Task<IActionResult> CreateSubmission([FromForm] SubmissionCreateDto dto)
        {
            var studentId = _userManager.GetUserId(User);
            if (studentId == null)
                return Unauthorized("Invalid token");

            // Prevent duplicates
            var existing = await _submissionRepo.GetByStudentIdAsync(studentId);
            if (existing.Any(s => s.AssignmentId == dto.AssignmentId))
                return BadRequest("You already submitted this assignment.");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required.");

            // 1. Generate file name
            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var folder = Path.Combine("wwwroot", "submissions");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);

            // 2. Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // 3. Create submission object
            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                FilePath = filePath,
                FileName = dto.File.FileName,
                FileMimeType = dto.File.ContentType,
                SubmittedAt = DateTime.UtcNow,
                Status = "Submitted"
            };

            submission = await _submissionRepo.CreateAsync(submission);

            var response = _mapper.Map<SubmissionResponseDto>(submission);
            return Ok(response);
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetSubmissionById(Guid id)
        {
            var submission = await _submissionRepo.GetByIdAsync(id);
            if (submission == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

            // student only see his own submission
            if (roles.Contains("Student") && submission.StudentId != userId)
                return Forbid();

           

            var response = _mapper.Map<SubmissionResponseDto>(submission);
            return Ok(response);
        }


        [HttpGet("assignment/{assignmentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetSubmissionsForAssignment(Guid assignmentId)
        {
            var teacherId = _userManager.GetUserId(User);

            // Teacher must own the assignment
            var submissions = await _submissionRepo.GetByAssignmentIdAsync(assignmentId);
            if (!submissions.Any())
                return Ok(new List<SubmissionResponseDto>());

            if (submissions.First().Assignment.TeacherId != teacherId)
                return Forbid("You cannot view submissions for assignments you did not create.");

            var response = _mapper.Map<IEnumerable<SubmissionResponseDto>>(submissions);
            return Ok(response);
        }


        [HttpPut("{id}/grade")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GradeSubmission(Guid id, [FromBody] GradeSubmissionDto dto)
        {
            var submission = await _submissionRepo.GetByIdAsync(id);
            if (submission == null)
                return NotFound("Submission not found.");

            // Ensure the teacher owns the assignment
            var teacherId = _userManager.GetUserId(User);
            if (submission.Assignment.TeacherId != teacherId)
                return Forbid("You cannot grade submissions for assignments you did not create.");

            // Grade it
            submission = await _submissionRepo.GradeAsync(id, dto.Grade);

            var response = _mapper.Map<SubmissionResponseDto>(submission);
            return Ok(response);
        }


        [HttpGet("mine")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMySubmissions()
        {
            var studentId = _userManager.GetUserId(User);

            if (studentId == null)
                return Unauthorized("Invalid token");

            var submissions = await _submissionRepo.GetByStudentIdAsync(studentId);

            var response = _mapper.Map<IEnumerable<SubmissionResponseDto>>(submissions);

            return Ok(response);
        }

    }
}
