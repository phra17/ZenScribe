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
    }

    
}
