# Custom Tag Parser

[![NuGet](https://img.shields.io/nuget/v/CustomTagParser.svg)](https://www.nuget.org/packages/CustomTagParser)

A simple parser of custom tag pairs, for example BBCode.

## Usage

```cs
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
```
```
<b><i>This is bold italics.</i></b> <i>This is just italics.</i> [This is in brackets.]
```

## Purpose

This library is useful for adding custom nestable tags to dialog systems and other dynamic scenarios.

For example:

```cs
CustomTagParser.Parse("Hello [capitalize]Lum[/capitalize]!", [
    new CustomTagPair() {
        OpeningTag = "[capitalize]",
        ClosingTag = "[/capitalize]",
        Replace = (string Contents, string Left, string Right) => {
            return Left + Contents.ToUpper() + Right;
        },
    },
]);
```
```
Hello LUM!
```