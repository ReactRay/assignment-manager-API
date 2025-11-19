using Microsoft.AspNetCore.Mvc;
using StudentTeacherManagment.Models.DTOs.Assignments;
using StudentTeacherManagment.Permissions;
using StudentTeacherManagment.Services.AssignmentHelpers;


[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost]
    [HasPermission(AppPermissions.Assignments.Create)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentDto dto)
    {
        var result = await _assignmentService.CreateAsync(User, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [HasPermission(AppPermissions.Assignments.Read)]
    public async Task<IActionResult> GetAll()
        => Ok(await _assignmentService.GetAllAsync(User));

    [HttpGet("{id}")]
    [HasPermission(AppPermissions.Assignments.Read)]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _assignmentService.GetByIdAsync(User, id));

    [HttpPut("{id}")]
    [HasPermission(AppPermissions.Assignments.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentDto dto)
        => Ok(await _assignmentService.UpdateAsync(User, id, dto));

    [HttpDelete("{id}")]
    [HasPermission(AppPermissions.Assignments.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _assignmentService.DeleteAsync(User, id);
        return NoContent();
    }
}
