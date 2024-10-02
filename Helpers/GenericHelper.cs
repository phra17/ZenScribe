using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenScribe.Helpers
{
    public static class GenericHelper
    {
        public static int CountWords(string text)
        {
            int _wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                _wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }
            return _wordCount;
        }

        public static int CalculateMaxDescriptionLineLength(RichTextBox MainText)
        {
            Graphics g = MainText.CreateGraphics();
            float twoCharW = g.MeasureString("aa", MainText.Font).Width;
            float oneCharW = g.MeasureString("a", MainText.Font).Width;
            return (int)((float)MainText.Width / (twoCharW - oneCharW));
        }

        public static void LoadColorSchemes(Color[] backgrounds, Color[] foregrounds) {
            Color black = System.Drawing.ColorTranslator.FromHtml("#000000");
            Color black1 = System.Drawing.ColorTranslator.FromHtml("#222323");
            Color black2 = System.Drawing.ColorTranslator.FromHtml("#382b26");
            Color black3 = System.Drawing.ColorTranslator.FromHtml("#3e232c");
            Color white = System.Drawing.ColorTranslator.FromHtml("#ffffff");
            Color white1 = System.Drawing.ColorTranslator.FromHtml("#f0f6f0");
            Color white2 = System.Drawing.ColorTranslator.FromHtml("#b8c2b9");
            Color white3 = System.Drawing.ColorTranslator.FromHtml("#edf6d6");
            backgrounds[0] = black;
            backgrounds[1] = black2;
            backgrounds[2] = black3;
            foregrounds[0] = white;
            foregrounds[1] = white2;
            foregrounds[2] = white3;
        }

        public static void LoadCommandList(List<string> commandList) {
            commandList.Add("Ctrl + S = SAVE");
            commandList.Add("Ctrl + O = OPEN FILE");
            commandList.Add("Ctrl + F = SEARCH");
            commandList.Add("Ctrl + N = NEW FILE");
            commandList.Add("Alt + = LARGER FONT");
            commandList.Add("Alt - = SMALLER FONT");
            commandList.Add("Alt + C = CHANGE COLOR");
            commandList.Add("Alt + F = CHANGE FONT");
        }
    }

    
}
