namespace OCS.TeamAssistant.Appraiser.Domain.Keys;

public sealed record LanguageId
{
    public string Value { get; }

    public LanguageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value;
    }
    
    public static readonly LanguageId Russian = new("ru");
    public static readonly LanguageId Default = Russian;
}