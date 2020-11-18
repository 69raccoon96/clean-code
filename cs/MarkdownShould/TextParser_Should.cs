﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown.Tests
{
    internal class TextParser_Should
    {
        private TextParser textParser;

        [SetUp]
        public void SetUp()
        {
            var tokenGetters = new ITokenReader[]
            {
                new HeaderTokenReader(),
                new StrongTokenReader(),
                new EmphasizedTokenReader(),
                new TextTokenReader()
            };
            textParser = new TextParser(tokenGetters);
        }

        [Test]
        public void GetTextTokens_ThrowArgumentException_NullText()
        {
            Action act = () => textParser.GetTextTokens(null);

            act.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void GetTextTokens_ReturnEmptyList_EmptyText()
        {
            var textTokens = textParser.GetTextTokens("");

            textTokens.Should().BeEmpty();
        }

        [Test]
        public void GetTextTokens_ReturnListWithEmphasizedToken_OneUnderliningElement()
        {
            var subTokens = new List<TextToken> {new TextToken(0, 2, TokenType.Text, "ab")};
            var expectedList = new List<TextToken> {new TextToken(0, 4, TokenType.Emphasized, "ab", subTokens)};
            var text = "_ab_";

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithTextToken_NoClosingUnderlining()
        {
            var text = "_ab";
            var expectedList = new List<TextToken> {new TextToken(0, 3, TokenType.Text, "_ab")};

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithThreeEmphasizedToken_TwoUnderliningElements()
        {
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 4, TokenType.Emphasized, "ab",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "ab")}),
                new TextToken(4, 1, TokenType.Text, " "),
                new TextToken(5, 4, TokenType.Emphasized, "ba",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "ba")})
            };
            var text = "_ab_ _ba_";

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithTextToken_TextWithoutAnySpecialSymbols()
        {
            var text = "ab";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 2, TokenType.Text, "ab")
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void
            GetTextTokens_ReturnListWithCorrectTokens_TextWithTwoUnderliningElementsAndTwoElementsWithoutAnySpecialSymbols()
        {
            var text = "aaa _bb_ aaa _aa_";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 4, TokenType.Text, "aaa "),
                new TextToken(4, 4, TokenType.Emphasized, "bb",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "bb")}),
                new TextToken(8, 5, TokenType.Text, " aaa "),
                new TextToken(13, 4, TokenType.Emphasized, "aa",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "aa")})
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithStrongToken_TextWithOneDoubleUnderliningElement()
        {
            var text = "__aa__";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 6, TokenType.Strong, "aa",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "aa")})
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithStrongToken_TextWithTWoDoubleUnderliningElement()
        {
            var text = "__aa__ __bb__";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 6, TokenType.Strong, "aa",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "aa")}),
                new TextToken(6, 1, TokenType.Text, " "),
                new TextToken(7, 6, TokenType.Strong, "bb",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "bb")})
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithStrongEmphasizedAndNotClosedElements()
        {
            var text = "__aa__ _bb_ ac_";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 6, TokenType.Strong, "aa",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "aa")}),
                new TextToken(6, 1, TokenType.Text, " "),
                new TextToken(7, 4, TokenType.Emphasized, "bb",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "bb")}),
                new TextToken(11, 4, TokenType.Text, " ac_")
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithShieldSymbol()
        {
            var text = "\\_ab\\_";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 4, TokenType.Text, "_ab_")
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithShieldInsideStrongTag()
        {
            var text = "__\\_ab__";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 8, TokenType.Strong, "\\_ab",
                    new List<TextToken> {new TextToken(0, 3, TokenType.Text, "_ab")})
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_NoTextBetweenUnderlinings()
        {
            var text = "____";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 4, TokenType.Text, "____", null)
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithSpaceBetweenWords()
        {
            var text = "a_a b_b";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 7, TokenType.Text, "a_a b_b", null)
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithUnderliningsInsideWords()
        {
            var text = "a_bc_de";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 1, TokenType.Text, "a", null),
                new TextToken(1, 4, TokenType.Emphasized, "bc",
                    new List<TextToken> {new TextToken(0, 2, TokenType.Text, "bc")}),
                new TextToken(5, 2, TokenType.Text, "de", null)
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithNumbersBetweenUnderlinings()
        {
            var text = "ab1_2_3";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 7, TokenType.Text, "ab1_2_3", null)
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public void GetTextTokens_ReturnListWithCorrectTokens_TextWithHeader()
        {
            var text = "#ab";
            var expectedList = new List<TextToken>
            {
                new TextToken(0, 3, TokenType.Header, "ab",
                    new List<TextToken>
                    {
                        new TextToken(0, 2, TokenType.Text, "ab")
                    })
            };

            var actualList = textParser.GetTextTokens(text);

            actualList.Should().BeEquivalentTo(expectedList);
        }
    }
}