using System.Text;
using TuiCommon;

namespace TuiCli;

public abstract class AnsiColorBase : ScreenBase {
    protected override object BuildAutoWrapString() {
        if (!UsingColors) return ScreenText;
        int nonNullColors = 0;
        int colorChars = 0;
        byte? byted;
        for (int i = 0; i < ScreenText.Length; i++) {
            if ((byted = ForegroundColors[i]) != null) {
                nonNullColors++;
                colorChars += 
                    byted.Value <= 9 ? 1 : 
                    byted.Value <= 99 ? 2 : 3;
            }

            if ((byted = BackgroundColors[i]) != null) {
                nonNullColors++;
                colorChars += 
                    byted.Value <= 9 ? 1 : 
                    byted.Value <= 99 ? 2 : 3;
            }

            if (RefreshCharBuffer[i]) colorChars += 4;
        }

        StringBuilder builder = new(ScreenText.Length + nonNullColors*5 + colorChars + 4);
        for (int i = 0; i < ScreenText.Length; i++) {
            if ((byted = ForegroundColors[i]) != null) {
                builder.Append("\e[38;5;");
                builder.Append(byted.Value);
                builder.Append('m');
            }

            if ((byted = BackgroundColors[i]) != null) {
                builder.Append("\e[48;5;");
                builder.Append(byted.Value);
                builder.Append('m');
            }

            builder.Append(ScreenText[i] == 0 ? ' ' : ScreenText[i]);

            if (RefreshCharBuffer[i])
                builder.Append("\e[0m");
        }

        builder.Append("\e[0m");
        return builder;
    }
}