using System.Diagnostics.CodeAnalysis;

namespace CustomTags;

/// <summary>
/// A custom tag unit that can be applied to an input string, for example a BBCode tag.
/// </summary>
public readonly record struct CustomTagUnit {
    /// <summary>
    /// The custom tag unit.
    /// </summary>
    public required CustomTag Tag { get; init; }
    /// <summary>
    /// A function that accepts the text preceding the tag and the text following the tag and returns a string to replace the tag.
    /// </summary>
    public required ReplaceDelegate Replace { get; init; }
    /// <summary>
    /// A function that accepts the text preceding the tag and the text following the tag and returns whether the tag pair should be applied.
    /// </summary>
    public ConditionDelegate? Condition { get; init; } = null;

    /// <summary>
    /// A function that accepts the text preceding the tag and the text following the tag and returns a string to replace the tag.
    /// </summary>
    /// <param name="Left">The text preceding the tag.</param>
    /// <param name="Right">The text following the tag.</param>
    /// <returns>
    /// A string to replace the tag.
    /// </returns>
    public delegate string ReplaceDelegate(string Left, string Right);
    /// <summary>
    /// A function that accepts the text preceding the tag and the text following the tag and returns whether the tag pair should be applied.
    /// </summary>
    /// <param name="Left">The text preceding the tag.</param>
    /// <param name="Right">The text following the tag.</param>
    /// <returns>
    /// Whether the tag should be applied.
    /// </returns>
    public delegate bool ConditionDelegate(string Left, string Right);

    /// <summary>
    /// Constructs a new custom tag pair.
    /// </summary>
    /// <param name="Tag">The tag unit.</param>
    /// <param name="Replace">A function that accepts the text preceding the tag and the text following the tag and returns a string to replace the tag.</param>
    /// <param name="Condition">A function that accepts the text preceding the tag and the text following the tag and returns whether the tag pair should be applied.</param>
    [SetsRequiredMembers]
    public CustomTagUnit(CustomTag Tag, ReplaceDelegate Replace, ConditionDelegate? Condition = null) {
        this.Tag = Tag;
        this.Replace = Replace;
        this.Condition = Condition;
    }
}