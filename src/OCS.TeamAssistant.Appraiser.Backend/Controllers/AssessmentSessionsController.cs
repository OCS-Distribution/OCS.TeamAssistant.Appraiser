using Microsoft.AspNetCore.Mvc;
using OCS.TeamAssistant.Appraiser.Model;

namespace OCS.TeamAssistant.Appraiser.Backend.Controllers;

[ApiController]
[Route("sessions")]
public sealed class AssessmentSessionsController : ControllerBase
{
	private readonly IAssessmentSessionsService _service;

	public AssessmentSessionsController(IAssessmentSessionsService service)
		=> _service = service ?? throw new ArgumentNullException(nameof(service));

	[HttpGet("list")]
	public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

	[HttpGet("story/{assessmentSessionId}")]
	public async Task<IActionResult> GetStoryDetails(Guid assessmentSessionId)
		=> Ok(await _service.GetStoryDetails(assessmentSessionId));
}