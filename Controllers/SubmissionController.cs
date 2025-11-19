using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Permissions;
using StudentTeacherManagment.Repositories.SubmissionRepository;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubmissionsController(
            ISubmissionRepository repo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }


        // 
        // CREATE SUBMISSION (Student)
        [HttpPost]
        [HasPermission(AppPermissions.Submissions.Create)]
        public async Task<IActionResult> Create([FromForm] SubmissionCreateDto dto)
        {
            var studentId = _userManager.GetUserId(User);

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("A PDF file is required.");

            // Prevent duplicate submissions
            var existing = await _repo.GetByStudentIdAsync(studentId);
            if (existing.Any(s => s.AssignmentId == dto.AssignmentId))
                return BadRequest("You already submitted this assignment.");

            // ----------------------------------------------------------
            // SAFE ABSOLUTE PATH (fixes crash)
            // ----------------------------------------------------------
            var root = Directory.GetCurrentDirectory();
            var folder = Path.Combine(root, "wwwroot", "submissions");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // This path is what the download endpoint will use
            var relativePath = $"/submissions/{fileName}";

            // Create submission
            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                FilePath = relativePath,       // <<< IMPORTANT: store relative, not physical
                FileName = dto.File.FileName,
                FileMimeType = dto.File.ContentType,
                SubmittedAt = DateTime.UtcNow,
                Status = "Submitted"
            };

            submission = await _repo.CreateAsync(submission);

            return Ok(_mapper.Map<SubmissionResponseDto>(submission));
        }


        // GET SUBMISSION BY ID
        // Student → only their own
        // Teacher/Admin → must own assignment or be admin

        [HttpGet("{id}")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            var submission = await _repo.GetByIdAsync(id);
            if (submission == null)
                return NotFound();

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");
            bool isStudent = roles.Contains("Student");

            if (isStudent && submission.StudentId != userId)
                return Forbid("You can only view your own submissions.");

            if (isTeacher && submission.Assignment.TeacherId != userId)
                return Forbid("You cannot view submissions for assignments you did not create.");

            // Admin bypasses checks

            return Ok(_mapper.Map<SubmissionResponseDto>(submission));
        }


        // GET SUBMISSIONS FOR ASSIGNMENT
        // Teacher/Admin → must own assignment (teacher)
        [HttpGet("assignment/{assignmentId}")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> GetForAssignment(Guid assignmentId)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var submissions = await _repo.GetByAssignmentIdAsync(assignmentId);

            if (!submissions.Any())
                return Ok(new List<SubmissionResponseDto>());

            if (isTeacher && submissions.First().Assignment.TeacherId != userId)
                return Forbid("You cannot view submissions for assignments you did not create.");

            return Ok(_mapper.Map<IEnumerable<SubmissionResponseDto>>(submissions));
        }


        // GRADE SUBMISSION
        // Teacher/Admin → must own assignment (teacher)
        [HttpPut("{id}/grade")]
        [HasPermission(AppPermissions.Submissions.Grade)]
        public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionDto dto)
        {
            var submission = await _repo.GetByIdAsync(id);
            if (submission == null)
                return NotFound("Submission not found.");

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            if (isTeacher && submission.Assignment.TeacherId != userId)
                return Forbid("You cannot grade submissions for assignments you did not create.");

            submission = await _repo.GradeAsync(id, dto.Grade);

            return Ok(_mapper.Map<SubmissionResponseDto>(submission));
        }


        // GET MY SUBMISSIONS (Student)
        [HttpGet("mine")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> GetMySubmissions()
        {
            var studentId = _userManager.GetUserId(User);

            var submissions = await _repo.GetByStudentIdAsync(studentId);

            return Ok(_mapper.Map<IEnumerable<SubmissionResponseDto>>(submissions));
        }


        // DOWNLOAD SUBMISSION FILE
        [HttpGet("{id}/download")]
        [Authorize]
        public async Task<IActionResult> Download(Guid id)
        {
            var submission = await _repo.GetByIdAsync(id);

            if (submission == null)
                return NotFound("Submission not found.");

            var userId = _userManager.GetUserId(User);
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

            bool isStudent = roles.Contains("Student");
            bool isTeacher = roles.Contains("Teacher");

            // STUDENT: only their own file
            if (isStudent && submission.StudentId != userId)
                return Forbid();

            // TEACHER: only their own assignment
            if (isTeacher && submission.Assignment.TeacherId != userId)
                return Forbid("You cannot download submissions for assignments you did not create.");

            // Convert RELATIVE path → ABSOLUTE physical path
            var root = Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(root, "wwwroot", submission.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found on server.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            return File(fileBytes, submission.FileMimeType, submission.FileName);
        }


    }
}
