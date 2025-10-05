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
    /// <param name="TagUnits">The custom tag units to apply.</param>
    /// <returns>
    /// The result of applying the custom tag pairs.
    /// </returns>
    public static string Parse(string Input, IEnumerable<CustomTagPair>? TagPairs = null, IEnumerable<CustomTagUnit>? TagUnits = null) {
        if (TagPairs is not null) {
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
                    if (Input.AsSpan(Index).StartsWith(TagPair.ClosingTag.Match, TagPair.ClosingTag.ComparisonType)) {
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
                    else if (Input.AsSpan(Index).StartsWith(TagPair.OpeningTag.Match, TagPair.OpeningTag.ComparisonType)) {
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
                string Contents = Input[OpeningTagEndIndex..ClosingTagIndex];

                // Extract text preceding opening tag
                string Left = Input[..OpeningTagIndex];
                // Extract text following closing tag
                string Right = Input[ClosingTagEndIndex..];

                // Check tag condition
                if (TagPair.Condition is not null) {

                    if (!TagPair.Condition(Contents, Left, Right)) {
                        continue;
                    }
                }

                // Fetch new contents
                string NewContents = TagPair.Replace(Contents, Left, Right);

                // Replace tagged contents
                Input = Input[..OpeningTagIndex] + NewContents + Input[ClosingTagEndIndex..];
                goto Retry;
            }
        }

        if (TagUnits is not null) {
            foreach (CustomTagUnit TagUnit in TagUnits) {
            Retry:
                // Find tag
                int TagIndex = Input.IndexOf(TagUnit.Tag.Match, TagUnit.Tag.ComparisonType);
                if (TagIndex < 0) {
                    continue;
                }
                int TagEndIndex = TagIndex + TagUnit.Tag.Match.Length;

                // Extract text preceding opening tag
                string Left = Input[..TagIndex];
                // Extract text following closing tag
                string Right = Input[TagEndIndex..];

                // Check tag condition
                if (TagUnit.Condition is not null) {

                    if (!TagUnit.Condition(Left, Right)) {
                        continue;
                    }
                }

                // Fetch new contents
                string NewContents = TagUnit.Replace(Left, Right);

                // Replace tagged contents
                Input = Input[..TagIndex] + NewContents + Input[TagEndIndex..];
                goto Retry;
            }
        }

        return Input;
    }
}