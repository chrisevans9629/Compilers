using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class PascalResult<T>
    {
        public IList<PascalException> Errors { get; set; } = new List<PascalException>();

        public T Result { get; set; }
    }
    public class PascalLexer
    {
        private readonly ILogger _logger;
        private string _str;

        private LexerIterator iterator;
        public PascalLexer(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }



        public char Current => iterator.Current;
        private int Index => iterator.index;
        private int Column => iterator.Column;
        private int Line => iterator.Line;
        void Advance()
        {
            iterator.Advance();
        }



        TokenItem Id()
        {
            var result = "";


            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                result += Current;
                Advance();
            }
            //uwu


            var token = CreateToken(PascalTerms.Id, result);


            if (PascalTerms.Reservations.ContainsKey(result.ToUpper()))
            {
                token.Token = PascalTerms.Reservations[result.ToUpper()];
            }

            return token;
        }

        TokenItem CreateToken(string name, string value, int? index = null)
        {
            var token = new TokenItem()
            {
                Index = index ?? Index - value.Length,
                Column = Column - value.Length,
                Line = Line,
                Value = value,
                Token = new Token()
                {
                    Name = name
                }
            };
            return token;
        }

        public char Peek()
        {
            return iterator.Peek();
        }

        public PascalResult<IList<TokenItem>> TokenizeResult(string str)
        {
            var result = new PascalResult<IList<TokenItem>>();
            _str = str;
            _logger.Log($"Tokenizing String:\n'{_str}'");
            iterator = new LexerIterator(str.ToCharArray());
            //rawr
            var items = new List<TokenItem>();
            while (Current != default(char))
            {
                if (char.IsWhiteSpace(Current))
                {
                    Advance();
                }
                else if (char.IsLetter(Current) || Current == '_')
                {
                    items.Add(Id());
                }
                else if (Current == '^' && char.IsLetter(Peek()))
                {
                    Advance();
                    items.Add(CreateToken(PascalTerms.Pointer, Current.ToString()));
                    Advance();
                }
                else if (Current == '<' && Peek() == '>')
                {
                    Advance();
                    Advance();
                    items.Add(CreateToken(PascalTerms.NotEqual, "<>"));
                }
                else if (Current == ':' && Peek() == '=')
                {
                    Advance();
                    Advance();
                    items.Add(CreateToken(PascalTerms.Assign, ":="));
                }
                else if (char.IsNumber(Current) || (Current == '.' && char.IsNumber(Peek())))
                {
                    //10.10 valid
                    //10.10.10 invalid
                    //10. 10 invalid
                    var num = "";
                    bool hasFloat = false;
                    while (char.IsNumber(Current) || Current == '.')
                    {
                        if (Current == '.' && hasFloat)
                        {
                            result.Errors.Add(
                             new LexerException(ErrorCode.UnexpectedToken, CreateToken("Error", num) ,$"Number {num} cannot have two periods"));
                            Advance();
                        }
                        if (Current == '.')
                        {
                            hasFloat = true;
                        }
                        num += Current;
                        Advance();
                    }

                    if (hasFloat)
                    {
                        items.Add(CreateToken(PascalTerms.RealConst, num));
                    }
                    else
                    {
                        items.Add(CreateToken(PascalTerms.IntegerConst, num));
                    }
                }
                else if (Current == '\'')
                {
                    Advance();
                    var index = Index;
                    var quote = "";
                    while (Current != '\'' && Current != default(char))
                    {
                        quote += Current;
                        Advance();
                    }
                    Advance();
                    items.Add(CreateToken(PascalTerms.StringConst, quote, index));
                }
                else if (Current == '{')
                {
                    Advance();
                    while (Current != '}' && Current != default(char))
                    {
                        Advance();
                    }
                    Advance();
                }
                else if (PascalTerms.SingleTokens.Contains(Current.ToString()))
                {
                    var item = PascalTerms.SingleTokens.First(p => p == Current.ToString());

                    Advance();

                    items.Add(CreateToken(item, item));
                }
                else
                {
                    var current = Current;
                    Advance();
                    var token = CreateToken("Error", current.ToString());
                    result.Errors.Add(
                     new LexerException(ErrorCode.UnexpectedToken, token,
                        $"Unexpected token '{current}'"));
                    Advance();
                }
            }

            result.Result = items;
            return result;
        }

        public IList<TokenItem> Tokenize(string str)
        {
            var data = TokenizeResult(str);
            if (data.Errors.Any())
            {
                throw data.Errors[0];
            }
            return data.Result;

        }
      

    }
}