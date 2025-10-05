using System.Diagnostics.CodeAnalysis;

namespace CustomTags;

/// <summary>
/// A custom tag in a custom tag pair.
/// </summary>
public readonly record struct CustomTag {
    /// <summary>
    /// The string to match for the tag.
    /// </summary>
    public required string Match { get; init; }
    /// <summary>
    /// The string comparison type to use when searching for the string to match for the tag.
    /// </summary>
    public StringComparison ComparisonType { get; init; } = StringComparison.Ordinal;

    /// <summary>
    /// Constructs a new custom tag.
    /// </summary>
    /// <param name="Match">The string to match for the tag.</param>
    /// <param name="ComparisonType">The string comparison type to use when searching for the string to match for the tag.</param>
    [SetsRequiredMembers]
    public CustomTag(string Match, StringComparison ComparisonType = StringComparison.Ordinal) {
        this.Match = Match;
        this.ComparisonType = ComparisonType;
    }

    /// <summary>
    /// Converts a string to match for the tag to a new custom tag.
    /// </summary>
    /// <param name="Match">The string to match for the tag.</param>
    public static implicit operator CustomTag(string Match) {
        return new CustomTag(Match);
    }
}