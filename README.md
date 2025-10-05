# Custom Tag Parser

A simple parser of custom tag pairs, for example BBCode.

## Usage

```cs
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
```
```
<b><i>This is bold italics.</i></b> <i>This is just italics.</i> [This is in brackets.]
```

## Purpose

This library is useful for adding custom nestable tags to dialog systems and other dynamic scenarios.

For example:

```cs
CustomTagParser.Parse("Hello [name]Arthur[/name]!", [
    new CustomTagPair() {
        OpeningTag = "[name]",
        ClosingTag = "[/name]",
        Replace = (string Name, string Left, string Right) => {
            switch (Name) {
                case "Arthur":
                    return ArthurTitle switch {
                        Title.Knight => "Sir Arthur",
                        Title.King => "King Arthur",
                        _ => "Arthur",
                    };
                default:
                    return Name;
            }
        },
    },
]);
```