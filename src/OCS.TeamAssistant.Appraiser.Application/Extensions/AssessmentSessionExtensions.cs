using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Application.Extensions;

internal static class AssessmentSessionExtensions
{
	public static Task<AssessmentSession> EnsureForModerator(
		this Task<AssessmentSession?> assessmentSessionTask,
		string moderatorName)
	{
		if (assessmentSessionTask is null)
			throw new ArgumentNullException(nameof(assessmentSessionTask));
		if (String.IsNullOrWhiteSpace(moderatorName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(moderatorName));

		return Ensure(assessmentSessionTask, MessageId.SessionNotFoundForModerator, moderatorName);
	}

	public static Task<AssessmentSession> EnsureForAppraiser(
		this Task<AssessmentSession?> assessmentSessionTask,
		string moderatorName)
	{
		if (assessmentSessionTask is null)
			throw new ArgumentNullException(nameof(assessmentSessionTask));
		if (String.IsNullOrWhiteSpace(moderatorName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(moderatorName));

		return Ensure(assessmentSessionTask, MessageId.SessionNotFoundForAppraiser, moderatorName);
	}

	private static async Task<AssessmentSession> Ensure(
		this Task<AssessmentSession?> assessmentSessionTask,
		MessageId messageId,
		string userName)
	{
		if (assessmentSessionTask is null)
			throw new ArgumentNullException(nameof(assessmentSessionTask));
		if (messageId is null)
			throw new ArgumentNullException(nameof(messageId));
		if (String.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		var assessmentSession = await assessmentSessionTask;
		if (assessmentSession is null)
			throw new AppraiserUserException(messageId, userName);

		return assessmentSession;
	}
}