using System.Diagnostics.CodeAnalysis;

namespace CustomTags;

/// <summary>
/// A custom tag pair that can be applied to an input string, for example a BBCode tag.
/// </summary>
public readonly record struct CustomTagPair {
    /// <summary>
    /// The tag that starts the custom tag pair.
    /// </summary>
    public required CustomTag OpeningTag { get; init; }
    /// <summary>
    /// The tag that ends the custom tag pair.
    /// </summary>
    public required CustomTag ClosingTag { get; init; }
    /// <summary>
    /// A function that accepts the contents of the tag pair and returns a string to replace the tagged contents.
    /// </summary>
    public required ReplaceDelegate Replace { get; init; }
    /// <summary>
    /// A function that accepts the contents of the tag pair, the text preceding the opening tag and the text following the closing tag and returns whether the tag pair should be applied.
    /// </summary>
    public ConditionDelegate? Condition { get; init; } = null;

    /// <summary>
    /// A function that accepts the contents of the tag pair and returns a string to replace the tagged contents.
    /// </summary>
    /// <param name="Contents">The original contents of the tag pair.</param>
    /// <returns>
    /// A string to replace the tagged contents.
    /// </returns>
    public delegate string ReplaceDelegate(string Contents);
    /// <summary>
    /// A function that accepts the contents of the tag pair, the text preceding the opening tag and the text following the closing tag and returns whether the tag pair should be applied.
    /// </summary>
    /// <param name="Contents">The original contents of the tag pair.</param>
    /// <param name="Left">The text preceding the opening tag.</param>
    /// <param name="Right">The text following the closing tag.</param>
    /// <returns>
    /// Whether the tag pair should be applied.
    /// </returns>
    public delegate bool ConditionDelegate(string Contents, string Left, string Right);

    /// <summary>
    /// Constructs a new custom tag pair.
    /// </summary>
    /// <param name="OpeningTag">The tag that starts the custom tag pair.</param>
    /// <param name="ClosingTag">The tag that ends the custom tag pair.</param>
    /// <param name="Replace">A function that accepts the contents of the tag pair and returns a string to replace the tagged contents.</param>
    /// <param name="Condition">A function that accepts the contents of the tag pair, the text preceding the opening tag and the text following the closing tag and returns whether the tag pair should be applied.</param>
    [SetsRequiredMembers]
    public CustomTagPair(CustomTag OpeningTag, CustomTag ClosingTag, ReplaceDelegate Replace, ConditionDelegate? Condition = null) {
        this.OpeningTag = OpeningTag;
        this.ClosingTag = ClosingTag;
        this.Replace = Replace;
        this.Condition = Condition;
    }
}