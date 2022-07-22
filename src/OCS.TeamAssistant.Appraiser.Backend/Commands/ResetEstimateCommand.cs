using OCS.TeamAssistant.Appraiser.Application.CommandHandlers.ResetEstimate;

namespace OCS.TeamAssistant.Appraiser.Backend.Commands;

internal sealed record ResetEstimateCommand(long ModeratorId, string ModeratorName) : IResetEstimateCommand;