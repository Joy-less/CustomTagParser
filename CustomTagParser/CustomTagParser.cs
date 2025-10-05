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
    public static string Parse(string Input, IEnumerable<CustomTagPair> TagPairs) {
        foreach (CustomTagPair TagPair in TagPairs) {
        Retry:
            // Find tag prefix
            int PrefixIndex = Input.IndexOf(TagPair.OpeningTag.Match, TagPair.OpeningTag.ComparisonType);
            if (PrefixIndex < 0) {
                continue;
            }
            int EndPrefixIndex = PrefixIndex + TagPair.OpeningTag.Match.Length;

            // Find tag suffix
            int SuffixIndex = -1;
            int Depth = 1;
            for (int Index = EndPrefixIndex; Index < Input.Length; Index++) {
                // Suffix
                if (Input.AsSpan(Index).StartsWith(TagPair.ClosingTag.Match, TagPair.ClosingTag.ComparisonType)) {
                    // Pop
                    Depth--;

                    // Found matching suffix
                    if (Depth == 0) {
                        SuffixIndex = Index;
                        break;
                    }

                    // Move past suffix
                    Index += TagPair.ClosingTag.Match.Length;
                    continue;
                }
                // Prefix
                else if (Input.AsSpan(Index).StartsWith(TagPair.OpeningTag.Match, TagPair.OpeningTag.ComparisonType)) {
                    // Push
                    Depth++;

                    // Move past prefix
                    Index += TagPair.OpeningTag.Match.Length;
                    continue;
                }
            }
            if (SuffixIndex < 0) {
                continue;
            }
            int EndSuffixIndex = SuffixIndex + TagPair.ClosingTag.Match.Length;

            // Extract original contents
            string Contents = Input[EndPrefixIndex..SuffixIndex];

            // Check tag condition
            if (TagPair.Condition is not null) {
                string Left = Input[..PrefixIndex];
                string Right = Input[EndSuffixIndex..];

                if (!TagPair.Condition(Contents, Left, Right)) {
                    continue;
                }
            }

            // Fetch new contents
            string NewContents = TagPair.Replace(Contents);

            // Replace tagged contents
            Input = Input[..PrefixIndex] + NewContents + Input[EndSuffixIndex..];
            goto Retry;
        }

        return Input;
    }
}