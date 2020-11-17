﻿using System.Collections.Generic;
using System.Text;

namespace Markdown
{
    public class HeaderTokenGetter : ITokenGetter
    {
        public TextToken GetToken(string text, int index, int startPosition)
        {
            var currentText = text.Substring(1);
            var tokenToAdd = new TextToken(startPosition, text.Length,
                TokenType.Header, currentText);

            return tokenToAdd;
        }

        public bool CanCreateToken(string text, int index, int startPosition)
        {
            return index == startPosition && text[startPosition] == '#' 
                                          && index + 1 != text.Length && text[index +1] != '\\';
        }
    }
}