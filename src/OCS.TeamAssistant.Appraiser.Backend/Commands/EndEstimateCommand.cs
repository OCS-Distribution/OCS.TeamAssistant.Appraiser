using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.EndEstimate;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record EndEstimateCommand(long ModeratorId, string ModeratorName) : IEndEstimateCommand;