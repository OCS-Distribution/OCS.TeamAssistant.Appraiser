namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class Assessment
{
    public int? Value { get; private set; }
    public DateTimeOffset Created { get; private set; }
    
    public AppraiserId AppraiserId { get; private set; } = default!;
    public Appraiser Appraiser { get; private set; } = default!;

    private Assessment()
    {
    }

    public static Assessment Create(Appraiser appraiser, int? value)
    {
        if (appraiser is null)
            throw new ArgumentNullException(nameof(appraiser));
        
        return new()
        {
            Value = value,
            Created = DateTimeOffset.UtcNow,
            AppraiserId = appraiser.Id,
            Appraiser = appraiser
        };
    }
}