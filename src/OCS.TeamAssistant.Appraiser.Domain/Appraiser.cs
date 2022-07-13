namespace OCS.TeamAssistant.Appraiser.Domain;

public sealed class Appraiser
{
    public static readonly Appraiser Empty = new()
    {
        Id = new AppraiserId(0),
        Name = nameof(Appraiser)
    };

    public AppraiserId Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    private Appraiser()
    {
    }
    
    public static Appraiser Create(AppraiserId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        return new()
        {
            Id = id ?? throw new ArgumentNullException(nameof(id)),
            Name = name
        };
    }
}