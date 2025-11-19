using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Repositories.SubmissionRepository;

namespace StudentTeacherManagment.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SubmissionService(
            ISubmissionRepository repo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _repo = repo;
            _userManager = userManager;
            _env = env;
        }


        public async Task<Submission> CreateSubmissionAsync(string studentId, SubmissionCreateDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("A file is required.");

            // Allowed MIME types
            var allowedTypes = new[]
            {
        "application/pdf",
        "application/msword", // .doc
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
        "text/plain" // .txt
    };

            if (!allowedTypes.Contains(dto.File.ContentType))
                throw new Exception("Only PDF, DOC, DOCX, or TXT files are allowed.");

            // Allowed extensions (extra validation)
            var ext = Path.GetExtension(dto.File.FileName).ToLower();
            var allowedExt = new[] { ".pdf", ".doc", ".docx", ".txt" };

            if (!allowedExt.Contains(ext))
                throw new Exception("Invalid file type. Allowed: PDF, DOC, DOCX, TXT.");

            // Check duplicate submission
            var existing = await _repo.GetByStudentIdAsync(studentId);
            if (existing.Any(s => s.AssignmentId == dto.AssignmentId))
                throw new Exception("You already submitted this assignment.");

            // Ensure folder exists
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "submissions");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            // Save file
            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.File.CopyToAsync(stream);

            var relativePath = $"/uploads/submissions/{fileName}";

            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                FilePath = relativePath,
                FileName = dto.File.FileName,
                FileMimeType = dto.File.ContentType,
                SubmittedAt = DateTime.UtcNow,
                Status = "Submitted"
            };

            return await _repo.CreateAsync(submission);
        }

        public async Task<Submission> GetByIdAsync(Guid id, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            var sub = await _repo.GetByIdAsync(id);
            if (sub == null) throw new Exception("Submission not found.");

            bool isStudent = roles.Contains("Student");
            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            if (isStudent && sub.StudentId != userId)
                throw new UnauthorizedAccessException();

            if (isTeacher && sub.Assignment.TeacherId != userId)
                throw new UnauthorizedAccessException();

            return sub;
        }


        public async Task<IEnumerable<Submission>> GetForAssignmentAsync(Guid assignmentId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            var subs = await _repo.GetByAssignmentIdAsync(assignmentId);

            if (!subs.Any()) return subs;

            if (isTeacher && subs.First().Assignment.TeacherId != userId)
                throw new UnauthorizedAccessException();

            return subs;
        }

        public async Task<Submission> GradeSubmissionAsync(Guid id, int grade, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            var sub = await _repo.GetByIdAsync(id);
            if (sub == null) throw new Exception("Submission not found.");

            bool isTeacher = roles.Contains("Teacher");
            bool isAdmin = roles.Contains("Admin");

            if (isTeacher && sub.Assignment.TeacherId != userId)
                throw new UnauthorizedAccessException();

            return await _repo.GradeAsync(id, grade);
        }

        public async Task<IEnumerable<Submission>> GetMySubmissionsAsync(string studentId)
        {
            return await _repo.GetByStudentIdAsync(studentId);
        }


        public async Task<(byte[] fileBytes, string mime, string fileName)> DownloadFileAsync(Guid id, string userId)
        {
            var sub = await GetByIdAsync(id, userId);

            var fullPath = Path.Combine(_env.WebRootPath, sub.FilePath.TrimStart('/'));

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found on server.");

            var bytes = await File.ReadAllBytesAsync(fullPath);

            return (bytes, sub.FileMimeType, sub.FileName);
        }
    }
}
