namespace CustomTags.Tests;

public class ParseTests {
    [Fact]
    public void ReadmeBbcodeTest() {
        string Result = CustomTagParser.Parse("[b][i]This is bold italics.[/i][/b] [i]This is just italics.[/i] [lb]This is in brackets.[rb]",
            TagPairs: [
                new CustomTagPair() {
                    OpeningTag = "[b]",
                    ClosingTag = "[/b]",
                    Replace = (string Text, string Left, string Right)
                        => $"<b>" + Text + "</b>",
                },
                new CustomTagPair() {
                    OpeningTag = "[i]",
                    ClosingTag = "[/i]",
                    Replace = (string Text, string Left, string Right)
                        => $"<i>" + Text + "</i>",
                },
            ],
            TagUnits: [
                new CustomTagUnit() {
                    Tag = "[lb]",
                    Replace = (string Left, string Right)
                        => "[",
                },
                new CustomTagUnit() {
                    Tag = "[rb]",
                    Replace = (string Left, string Right)
                        => "]",
                },
            ]
        );

        Result.ShouldBe("<b><i>This is bold italics.</i></b> <i>This is just italics.</i> [This is in brackets.]");
    }
    [Fact]
    public void StandardBbcodeTest() {
        CustomTagParser.Parse("a[a]b[/a]c", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("a(FIRST)b(SECOND)c");
    }
    [Fact]
    public void NestedBbcodeTest() {
        CustomTagParser.Parse("a[a]b[a]c[/a]d[/a]e", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("a(FIRST)b(FIRST)c(SECOND)d(SECOND)e");
    }
    [Fact]
    public void NestedInvalidBbcodeTest() {
        CustomTagParser.Parse("a[a]bc[/a]d[/a]e", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("a(FIRST)bc(SECOND)d[/a]e");
    }
    [Fact]
    public void EscapableBbcodeTest() {
        CustomTagParser.Parse("a[a]b\\[a]c[/a]d[/a]e", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
                Condition = (string Contents, string Left, string Right)
                    => !Left.EndsWith('\\'),
            },
        ]).ShouldBe("a(FIRST)b\\[a]c[/a]d(SECOND)e");
    }
    [Fact]
    public void EmptyContentsBbcodeTest() {
        CustomTagParser.Parse("[a][/a]", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("(FIRST)(SECOND)");
    }
    [Fact]
    public void MultipleBbcodeTest() {
        CustomTagParser.Parse("[a][b][/b][/a][c][/c]", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(a:FIRST)" + Text + "(a:SECOND)",
            },
            new CustomTagPair() {
                OpeningTag = "[b]",
                ClosingTag = "[/b]",
                Replace = (string Text, string Left, string Right)
                    => $"(b:FIRST)" + Text + "(b:SECOND)",
            },
            new CustomTagPair() {
                OpeningTag = "[c]",
                ClosingTag = "[/c]",
                Replace = (string Text, string Left, string Right)
                    => $"(c:FIRST)" + Text + "(c:SECOND)",
            },
        ]).ShouldBe("(a:FIRST)(b:FIRST)(b:SECOND)(a:SECOND)(c:FIRST)(c:SECOND)");
    }
    [Fact]
    public void EmptyInputBbcodeTest() {
        CustomTagParser.Parse("", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("");
    }
    [Fact]
    public void UnitBbcodeTest() {
        CustomTagParser.Parse("a[lb]b[rb]", [], [
            new CustomTagUnit() {
                Tag = "[lb]",
                Replace = (string Left, string Right)
                    => "[",
            },
            new CustomTagUnit() {
                Tag = "[rb]",
                Replace = (string Left, string Right)
                    => "]",
            },
        ]).ShouldBe("a[b]");
    }
}