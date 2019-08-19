using System;
using Compilers.Interpreter;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class RegexLexerTests
    {
        private RegexLexer _regexLexer;

        [SetUp]
        public void Setup()
        {
            _regexLexer = new RegexLexer();
        }

        [Test]
        public void Tokenize_App()
        {
            _regexLexer.Ignore(" ");
            _regexLexer.Add("Name", "[a-zA-Z]+");
            _regexLexer.Add("Equal", "=");
            _regexLexer.Add("Number", @"\d+");


            var tokens = _regexLexer.Tokenize("var test = 10");

            tokens.Should().HaveCount(4);
        }

        [Test]
        public void Tokenize_FullOperations()
        {
            _regexLexer.Ignore(" ");
            _regexLexer.Add("Number", @"\d+");
            _regexLexer.Add("Add", @"\+");
            _regexLexer.Add("Subtract", @"\-");
            _regexLexer.Add("ParLeft", @"\(");
            _regexLexer.Add("ParRight", @"\)");

            var tokens = _regexLexer.Tokenize("10 + (10 - 100)");

            tokens.Should().HaveCount(7);
            tokens[5].Value.Should().Be("100");

        }

        [Test]
        public void Tokenize_Number()
        {
            _regexLexer.Add("Number", @"\d+");

            var tokens = _regexLexer.Tokenize("10");

            tokens.Should().HaveCount(1);
            tokens[0].Token.Name.Should().Be("Number");
        }

        [Test]
        public void Tokenize_Ignored()
        {
            _regexLexer.Ignore(" ");
            var tokens = _regexLexer.Tokenize(" ");
            tokens.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_TokenUnrecognized()
        {
            var ex = Assert.Throws<Exception>(() => _regexLexer.Tokenize(" "));
            ex.Message.Should().Be("Token ' ' unrecognized at position 0 line 0");
        }

        [Test]
        public void Subtraction()
        {
            _regexLexer.Add("Sub", "-");
            var token = _regexLexer.Tokenize("-");
            token.Should().HaveCount(1);
        }
        [Test]
        public void Tokenize_Addition()
        {
            _regexLexer.Add("Number", @"\d+");
            _regexLexer.Add("Add", @"\+");
            var tokens = _regexLexer.Tokenize("10+15");
            tokens.Should().HaveCount(3);

            tokens[1].Token.Name.Should().Be("Add");
            tokens[1].Line.Should().Be(0);
            tokens[1].Index.Should().Be(2);
        }
    }
}