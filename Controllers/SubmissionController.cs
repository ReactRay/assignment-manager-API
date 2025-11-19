using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Services;
using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Permissions;
using AutoMapper;

namespace StudentTeacherManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _service;
        private readonly IMapper _mapper;

        public SubmissionsController(ISubmissionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [HasPermission(AppPermissions.Submissions.Create)]
        public async Task<IActionResult> Create([FromForm] SubmissionCreateDto dto)
        {
            var studentId = User.FindFirst("id")?.Value;

            var sub = await _service.CreateSubmissionAsync(studentId, dto);
            return Ok(_mapper.Map<SubmissionResponseDto>(sub));
        }

        [HttpGet("{id}")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> Get(Guid id)
        {
            var userId = User.FindFirst("id")?.Value;
            var sub = await _service.GetByIdAsync(id, userId);
            return Ok(_mapper.Map<SubmissionResponseDto>(sub));
        }

        [HttpGet("assignment/{assignmentId}")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> GetForAssignment(Guid assignmentId)
        {
            var userId = User.FindFirst("id")?.Value;
            var subs = await _service.GetForAssignmentAsync(assignmentId, userId);
            return Ok(_mapper.Map<IEnumerable<SubmissionResponseDto>>(subs));
        }

        [HttpPut("{id}/grade")]
        [HasPermission(AppPermissions.Submissions.Grade)]
        public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionDto dto)
        {
            var userId = User.FindFirst("id")?.Value;
            var sub = await _service.GradeSubmissionAsync(id, dto.Grade, userId);
            return Ok(_mapper.Map<SubmissionResponseDto>(sub));
        }

        [HttpGet("mine")]
        [HasPermission(AppPermissions.Submissions.Read)]
        public async Task<IActionResult> Mine()
        {
            var studentId = User.FindFirst("id")?.Value;
            var subs = await _service.GetMySubmissionsAsync(studentId);
            return Ok(_mapper.Map<IEnumerable<SubmissionResponseDto>>(subs));
        }

        [HttpGet("{id}/download")]
        [Authorize]
        public async Task<IActionResult> Download(Guid id)
        {
            var userId = User.FindFirst("id")?.Value;

            var (bytes, mime, fileName) = await _service.DownloadFileAsync(id, userId);

            return File(bytes, mime, fileName);
        }
    }
}
