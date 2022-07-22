using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EstimateStory;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record EstimateStoryCommand(long AppraiserId, string AppraiserName, int? Value) : IEstimateStoryCommand;