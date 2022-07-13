using OCS.TeamAssistant.Appraiser.Domain.Exceptions;

namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class AssessmentSession
{
    public AssessmentSessionId Id { get; private set; } = default!;
    public Appraiser Moderator { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public AssessmentSessionState State { get; private set; } = default!;
    public Story CurrentStory { get; private set; } = default!;
    
    private readonly List<Appraiser> _appraisers;
    public IReadOnlyCollection<Appraiser> Appraisers => _appraisers;

    private AssessmentSession()
    {
        _appraisers = new();
    }
    
    public static AssessmentSession Create()
    {
        return new()
        {
            Id = new AssessmentSessionId(Guid.NewGuid()),
            Moderator = Appraiser.Empty,
            CurrentStory = Story.Empty,
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

    public AssessmentSession MoveToArchive()
    {
        State = AssessmentSessionState.Archived;

        return this;
    }

    public AssessmentSession ConnectModerator(AppraiserId id, string name)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Moderator = Appraiser.Create(id, name, Id);
        return ConnectAppraiser(Moderator);
    }

    public AssessmentSession ConnectAppraiser(AppraiserId id, string name)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        return ConnectAppraiser(Appraiser.Create(id, name, Id));
    }

    public AssessmentSession Next(Story story)
    {
        CurrentStory = story ?? throw new ArgumentNullException(nameof(story));

        return this;
    }

    private AssessmentSession ConnectAppraiser(Appraiser appraiser)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));

        if (_appraisers.Any(a => a.Id == appraiser.Id))
            throw new AppraiserException($"Участник {appraiser.Name} уже подключен.");
        
        _appraisers.Add(appraiser);

        return this;
    }
}