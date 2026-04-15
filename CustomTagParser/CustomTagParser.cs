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
            int ClosingTagIndex = FindClosingTag(Input, OpeningTagEndIndex, TagPair.OpeningTag, TagPair.ClosingTag);
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

            // Fetch new input
            string? NewInput = TagPair.Replace(Contents, Left, Right);
            if (NewInput is null) {
                continue;
            }

            // Replace input
            Input = NewInput;
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

            // Fetch new input
            string? NewInput = TagUnit.Replace(Left, Right);
            if (NewInput is null) {
                continue;
            }

            // Replace input
            Input = NewInput;
            goto Retry;
        }

        return new string(Input);
    }

    /// <summary>
    /// Finds the start index of the closing custom tag, ignoring nested custom tags.
    /// </summary>
    /// <param name="Input">The input string containing custom tags.</param>
    /// <param name="OpeningTag">The custom tag that starts a custom tag pair.</param>
    /// <param name="ClosingTag">The custom tag that ends a custom tag pair.</param>
    /// <returns>
    /// The start index of the closing custom tag, ignoring nested custom tags, otherwise -1 if not found.
    /// </returns>
    public static int FindClosingTag(scoped ReadOnlySpan<char> Input, CustomTag OpeningTag, CustomTag ClosingTag) {
        return FindClosingTag(Input, 0, OpeningTag, ClosingTag);
    }

    /// <summary>
    /// Finds the start index of the closing custom tag, ignoring nested custom tags.
    /// </summary>
    /// <param name="Input">The input string containing custom tags.</param>
    /// <param name="StartIndex">The starting index to check in <paramref name="Input"/>.</param>
    /// <param name="OpeningTag">The custom tag that starts a custom tag pair.</param>
    /// <param name="ClosingTag">The custom tag that ends a custom tag pair.</param>
    /// <returns>
    /// The start index of the closing custom tag, ignoring nested custom tags, otherwise -1 if not found.
    /// </returns>
    public static int FindClosingTag(scoped ReadOnlySpan<char> Input, int StartIndex, CustomTag OpeningTag, CustomTag ClosingTag) {
        int Depth = 0;
        for (int Index = StartIndex; Index < Input.Length; Index++) {
            // Opening tag
            if (Input[Index..].StartsWith(OpeningTag.Match, OpeningTag.ComparisonType)) {
                // Push
                Depth++;

                // Move past opening tag
                Index += OpeningTag.Match.Length;
                // Negate index increment
                Index--;
            }
            // Closing tag
            else if (Input[Index..].StartsWith(ClosingTag.Match, ClosingTag.ComparisonType)) {
                // Found matching closing tag
                if (Depth == 0) {
                    return Index;
                }

                // Pop
                Depth--;

                // Move past closing tag
                Index += ClosingTag.Match.Length;
                // Negate index increment
                Index--;
            }
        }
        return -1;
    }

    /// <summary>
    /// Finds the index of the closing custom char, ignoring nested custom chars.
    /// </summary>
    /// <param name="Input">The input string containing custom chars.</param>
    /// <param name="OpeningChar">The custom char that starts a custom char pair.</param>
    /// <param name="ClosingChar">The custom char that ends a custom char pair.</param>
    /// <returns>
    /// The index of the closing custom char, ignoring nested custom chars, otherwise -1 if not found.
    /// </returns>
    public static int FindClosingTag(scoped ReadOnlySpan<char> Input, char OpeningChar, char ClosingChar) {
        return FindClosingTag(Input, 0, OpeningChar, ClosingChar);
    }

    /// <summary>
    /// Finds the index of the closing custom char, ignoring nested custom chars.
    /// </summary>
    /// <param name="Input">The input string containing custom chars.</param>
    /// <param name="StartIndex">The starting index to check in <paramref name="Input"/>.</param>
    /// <param name="OpeningChar">The custom char that starts a custom char pair.</param>
    /// <param name="ClosingChar">The custom char that ends a custom char pair.</param>
    /// <returns>
    /// The index of the closing custom char, ignoring nested custom chars, otherwise -1 if not found.
    /// </returns>
    public static int FindClosingTag(scoped ReadOnlySpan<char> Input, int StartIndex, char OpeningChar, char ClosingChar) {
        int Depth = 0;
        for (int Index = StartIndex; Index < Input.Length; Index++) {
            // Opening tag
            if (Input[Index] == OpeningChar) {
                // Push
                Depth++;
            }
            // Closing tag
            else if (Input[Index] == ClosingChar) {
                // Found matching closing char
                if (Depth == 0) {
                    return Index;
                }

                // Pop
                Depth--;
            }
        }
        return -1;
    }
}