public record ValidationResult(bool IsValid, IDictionary<string, string[]>? Errors)
{
    public static ValidationResult Success => new(true, null);
}
#endregion