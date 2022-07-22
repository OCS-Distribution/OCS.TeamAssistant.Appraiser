using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class AssessmentSession
{
    public AssessmentSessionId Id { get; private set; } = default!;
    public long ChatId { get; private set; }
    public Appraiser Moderator { get; private set; } = default!;
    public LanguageId LanguageId { get; private set; }
    public string Title { get; private set; } = default!;
    public AssessmentSessionState State { get; private set; }
	public Story LastStory { get; private set; } = default!;
    public Story CurrentStory { get; private set; } = default!;

    private readonly List<Appraiser> _appraisers;
    public IReadOnlyCollection<Appraiser> Appraisers => _appraisers;

    private AssessmentSession()
    {
        _appraisers = new();
        LanguageId = LanguageId.Default;
    }

    public static AssessmentSession Create(long chatId)
    {
        return new()
        {
            Id = new(Guid.NewGuid()),
            ChatId = chatId,
            Moderator = Appraiser.Empty,
            CurrentStory = Story.Empty,
			LastStory = Story.Empty,
            State = AssessmentSessionState.Draft,
            Title = AssessmentSessionState.Draft.ToString()
        };
    }

    public AssessmentSession Activate(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

        Title = title;
        State = AssessmentSessionState.Active;

        return this;
    }

    public AssessmentSession ConnectModerator(AppraiserId id, string name)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        Moderator = Appraiser.Create(id, name);
		_appraisers.Add(Moderator);

        return this;
    }

    public AssessmentSession ConnectAppraiser(AppraiserId id, string name)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        var appraiser = Appraiser.Create(id, name);
        _appraisers.Add(appraiser);

        return this;
    }

    public AssessmentSession MoveToNext(string storyTitle)
    {
        if (string.IsNullOrWhiteSpace(storyTitle))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));

		LastStory = CurrentStory;
        CurrentStory = Story.Create(storyTitle, _appraisers);
        State = AssessmentSessionState.Active;

        return this;
    }

    public AssessmentSession StartStorySelection()
	{
		CurrentStory = Story.Empty;
        State = AssessmentSessionState.StorySelection;

        return this;
    }

    public AssessmentSession AsModerator(AppraiserId appraiserId)
    {
        if (appraiserId is null)
            throw new ArgumentNullException(nameof(appraiserId));
        if (!Moderator.Id.Equals(appraiserId))
            throw new AppraiserUserException(MessageId.NoRightsAddTaskToSession, Title);

        return this;
    }

    public AssessmentSession MoveToComplete()
	{
		LastStory = CurrentStory;
        CurrentStory = Story.Empty;

        return this;
    }

    public AssessmentSession DisconnectAppraiser(AppraiserId appraiserId)
    {
        if (appraiserId is null)
            throw new ArgumentNullException(nameof(appraiserId));

        var appraiser = _appraisers.SingleOrDefault(a => a.Id == appraiserId);

        if (appraiser is null)
            throw new AppraiserUserException(MessageId.ShutdownCompletedWithError, Title);

        if (Moderator.Id == appraiser.Id)
            throw new AppraiserUserException(MessageId.ModeratorCannotDisconnectedFromSession, Title);

        _appraisers.Remove(appraiser);

        return this;
    }

	public AssessmentSession Reset()
	{
		CurrentStory = LastStory;

		return this;
	}
}