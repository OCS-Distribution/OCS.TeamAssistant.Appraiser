using Telegram.Bot.Types;

namespace OCS.TeamAssistant.Appraiser.Backend.Extensions;

internal static class UserExtensions
{
	public static string GetUserName(this User user)
	{
		if (user is null)
			throw new ArgumentNullException(nameof(user));

		return !string.IsNullOrWhiteSpace(user.LastName)
			? $"{user.FirstName} {user.LastName}"
			: user.FirstName;
	}
}