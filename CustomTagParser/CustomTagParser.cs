namespace CustomTags;

/// <summary>
/// A simple parser of custom tag pairs, for example BBCode.
/// </summary>
public static class CustomTagParser {
    /// <summary>
    /// Applies custom tag pairs to the input.
    /// </summary>
    /// <param name="Input">The input string containing custom tag pairs.</param>
    /// <param name="TagPairs">The custom tag pairs to apply.</param>
    /// <returns>
    /// The result of applying the custom tag pairs.
    /// </returns>
    public static string Parse(scoped ReadOnlySpan<char> Input, scoped ReadOnlySpan<CustomTagPair> TagPairs = default) {
        foreach (CustomTagPair TagPair in TagPairs) {
        Retry:
            // Find opening tag
            int OpeningTagIndex = Input.IndexOf(TagPair.OpeningTag.Match, TagPair.OpeningTag.ComparisonType);
            if (OpeningTagIndex < 0) {
                continue;
            }
            int OpeningTagEndIndex = OpeningTagIndex + TagPair.OpeningTag.Match.Length;

            // Find closing tag
            int ClosingTagIndex = -1;
            int Depth = 1;
            for (int Index = OpeningTagEndIndex; Index < Input.Length; Index++) {
                // Closing tag
                if (Input[Index..].StartsWith(TagPair.ClosingTag.Match, TagPair.ClosingTag.ComparisonType)) {
                    // Pop
                    Depth--;

                    // Found matching closing tag
                    if (Depth == 0) {
                        ClosingTagIndex = Index;
                        break;
                    }

                    // Move past closing tag
                    Index += TagPair.ClosingTag.Match.Length;
                    continue;
                }
                // Opening tag
                else if (Input[Index..].StartsWith(TagPair.OpeningTag.Match, TagPair.OpeningTag.ComparisonType)) {
                    // Push
                    Depth++;

                    // Move past opening tag
                    Index += TagPair.OpeningTag.Match.Length;
                    continue;
                }
            }
            if (ClosingTagIndex < 0) {
                continue;
            }
            int ClosingTagEndIndex = ClosingTagIndex + TagPair.ClosingTag.Match.Length;

            // Extract original contents
            string Contents = new(Input[OpeningTagEndIndex..ClosingTagIndex]);

            // Extract text preceding opening tag
            string Left = new(Input[..OpeningTagIndex]);
            // Extract text following closing tag
            string Right = new(Input[ClosingTagEndIndex..]);

            // Check tag condition
            if (TagPair.Condition is not null) {
                if (!TagPair.Condition(Contents, Left, Right)) {
                    continue;
                }
            }

            // Fetch new contents
            string? NewContents = TagPair.Replace(Contents, Left, Right);
            if (NewContents is null) {
                continue;
            }

            // Replace tagged contents
            Input = string.Concat(Input[..OpeningTagIndex], NewContents, Input[ClosingTagEndIndex..]);
            goto Retry;
        }

        return new string(Input);
    }

    /// <summary>
    /// Applies custom tag units to the input.
    /// </summary>
    /// <param name="Input">The input string containing custom tag units.</param>
    /// <param name="TagUnits">The custom tag units to apply.</param>
    /// <returns>
    /// The result of applying the custom tag units.
    /// </returns>
    public static string Parse(scoped ReadOnlySpan<char> Input, scoped ReadOnlySpan<CustomTagUnit> TagUnits = default) {
        foreach (CustomTagUnit TagUnit in TagUnits) {
        Retry:
            // Find tag
            int TagIndex = Input.IndexOf(TagUnit.Tag.Match, TagUnit.Tag.ComparisonType);
            if (TagIndex < 0) {
                continue;
            }
            int TagEndIndex = TagIndex + TagUnit.Tag.Match.Length;

            // Extract text preceding opening tag
            string Left = new(Input[..TagIndex]);
            // Extract text following closing tag
            string Right = new(Input[TagEndIndex..]);

            // Check tag condition
            if (TagUnit.Condition is not null) {
                if (!TagUnit.Condition(Left, Right)) {
                    continue;
                }
            }

            // Fetch new contents
            string? NewContents = TagUnit.Replace(Left, Right);
            if (NewContents is null) {
                continue;
            }

            // Replace tagged contents
            Input = string.Concat(Input[..TagIndex], NewContents, Input[TagEndIndex..]);
            goto Retry;
        }

        return new string(Input);
    }
}