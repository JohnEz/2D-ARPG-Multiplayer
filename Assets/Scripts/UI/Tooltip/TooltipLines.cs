using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineStyle {
    Default,
    Title,
    SubTitle,
    Description,
    Custom
}

public class TooltipLines {

    public class TooltipLine {
        public string LeftText;
        public string RightText;
        public bool IsComplete = false; // if it has a left and right column
        public RectOffset Padding;
        public LineStyle Style;

        public TooltipLine(string leftText, string rightText, bool isComplete, RectOffset padding, LineStyle style) {
            LeftText = leftText;
            RightText = rightText;
            IsComplete = isComplete || (leftText != string.Empty && rightText != string.Empty); // Feels hacky
            Padding = padding;
            Style = style;
        }
    }

    public List<TooltipLine> Lines = new List<TooltipLine>();

    private void AddLine(TooltipLine line) {
        Lines.Add(line);
    }

    public void AddLine(string text, LineStyle style) {
        AddLine(new TooltipLine(text, string.Empty, false, new RectOffset(), style));
    }

    public void AddLine(string text, RectOffset padding) {
        AddLine(new TooltipLine(text, string.Empty, false, padding, LineStyle.Default));
    }

    public void AddColumn(string text, LineStyle style) {
        if (Lines.Count == 0 || Lines[Lines.Count - 1].IsComplete) {
            AddLine(new TooltipLine(text, string.Empty, false, new RectOffset(), style));
            return;
        }

        TooltipLine currentLine = Lines[Lines.Count - 1];

        currentLine.RightText = text;
        currentLine.IsComplete = true;
    }
}