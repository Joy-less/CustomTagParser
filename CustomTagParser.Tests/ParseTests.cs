namespace CustomTags.Tests;

public class ParseTests {
    [Fact]
    public void ReadmeBbcodeTest() {
        string Result = CustomTagParser.Parse("[b][i]This is bold italics.[/i][/b] [i]This is just italics.[/i] [lb]This is in brackets.[rb]", [
            new CustomTagPair() {
                OpeningTag = "[b]",
                ClosingTag = "[/b]",
                Replace = (string Contents, string Left, string Right)
                    => Left + $"<b>" + Contents + "</b>" + Right,
            },
            new CustomTagPair() {
                OpeningTag = "[i]",
                ClosingTag = "[/i]",
                Replace = (string Contents, string Left, string Right)
                    => Left + $"<i>" + Contents + "</i>" + Right,
            },
        ]);
        Result = CustomTagParser.Parse(Result, [
            new CustomTagUnit() {
                Tag = "[lb]",
                Replace = (string Left, string Right)
                    => Left + "[" + Right,
            },
            new CustomTagUnit() {
                Tag = "[rb]",
                Replace = (string Left, string Right)
                    => Left + "]" + Right,
            },
        ]);

        Result.ShouldBe("<b><i>This is bold italics.</i></b> <i>This is just italics.</i> [This is in brackets.]");

        CustomTagParser.Parse("Hello [capitalize]Lum[/capitalize]!", [
            new CustomTagPair() {
                OpeningTag = "[capitalize]",
                ClosingTag = "[/capitalize]",
                Replace = (string Contents, string Left, string Right) => {
                    return Left + Contents.ToUpper() + Right;
                },
            },
        ]).ShouldBe("Hello LUM!");
    }
    [Fact]
    public void StandardBbcodeTest() {
        CustomTagParser.Parse("a[a]b[/a]c", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text, string Left, string Right)
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
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
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
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
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
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
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
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
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
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
                    => Left + $"(a:FIRST)" + Text + "(a:SECOND)" + Right,
            },
            new CustomTagPair() {
                OpeningTag = "[b]",
                ClosingTag = "[/b]",
                Replace = (string Text, string Left, string Right)
                    => Left + $"(b:FIRST)" + Text + "(b:SECOND)" + Right,
            },
            new CustomTagPair() {
                OpeningTag = "[c]",
                ClosingTag = "[/c]",
                Replace = (string Text, string Left, string Right)
                    => Left + $"(c:FIRST)" + Text + "(c:SECOND)" + Right,
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
                    => Left + $"(FIRST)" + Text + "(SECOND)" + Right,
            },
        ]).ShouldBe("");
    }
    [Fact]
    public void UnitBbcodeTest() {
        CustomTagParser.Parse("a[lb]b[rb]", [
            new CustomTagUnit() {
                Tag = "[lb]",
                Replace = (string Left, string Right)
                    => Left + "[" + Right,
            },
            new CustomTagUnit() {
                Tag = "[rb]",
                Replace = (string Left, string Right)
                    => Left + "]" + Right,
            },
        ]).ShouldBe("a[b]");
    }
    [Fact]
    public void FindClosingTagTest() {
        CustomTagParser.FindClosingTag("[[][]] ] []]", "[", "]").ShouldBe(7);
        CustomTagParser.FindClosingTag("[[][]] ] []]", '[', ']').ShouldBe(7);
        CustomTagParser.FindClosingTag("[[][] ] ][]]", 1, "[", "]").ShouldBe(6);
        CustomTagParser.FindClosingTag("[[][] ] ][]]", 1, '[', ']').ShouldBe(6);
    }
}