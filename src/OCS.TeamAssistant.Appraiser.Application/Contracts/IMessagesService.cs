namespace OCS.TeamAssistant.Appraiser.Application.Contracts;

public interface IMessagesService
{
	Task AssessmentSessionsListChanged();

	Task StoryChanged(Guid assessmentSessionId);
}