using Microsoft.AspNetCore.SignalR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Backend.Hubs;

namespace OCS.TeamAssistant.Appraiser.Backend.Services;

internal sealed class MessagesService : IMessagesService
{
	private readonly IHubContext<MessagesHub> _hubContext;

	public MessagesService(IHubContext<MessagesHub> hubContext)
		=> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

	public Task AssessmentSessionsListChanged() => _hubContext.Clients.All.SendAsync("AssessmentSessions");

	public Task StoryChanged(Guid assessmentSessionId)
		=> _hubContext.Clients.All.SendAsync("Story", assessmentSessionId);
}