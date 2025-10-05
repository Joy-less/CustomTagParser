namespace CustomTags.Tests;

public class ParseTests {
    [Fact]
    public void StandardBbcodeTest() {
        CustomTagParser.Parse("a[a]b[/a]c", [
            new CustomTagPair() {
                OpeningTag = "[a]",
                ClosingTag = "[/a]",
                Replace = (string Text)
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
                Replace = (string Text)
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
                Replace = (string Text)
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
                Replace = (string Text)
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
                Replace = (string Text)
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
                Replace = (string Text)
                    => $"(a:FIRST)" + Text + "(a:SECOND)",
            },
            new CustomTagPair() {
                OpeningTag = "[b]",
                ClosingTag = "[/b]",
                Replace = (string Text)
                    => $"(b:FIRST)" + Text + "(b:SECOND)",
            },
            new CustomTagPair() {
                OpeningTag = "[c]",
                ClosingTag = "[/c]",
                Replace = (string Text)
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
                Replace = (string Text)
                    => $"(FIRST)" + Text + "(SECOND)",
            },
        ]).ShouldBe("");
    }
}