using System;

namespace Minesweeper.Test
{
    public enum ErrorCode
    {
        UnexpectedToken,
        IdNotFound,
        DuplicateId,
        Runtime,
        ParameterMismatch,
        DoesNotReturnValue,
        TypeMismatch
    }

    public class PascalException : Exception
    {
        public ErrorCode Error { get; }
        public TokenItem Token { get; }


        public PascalException(ErrorCode error, TokenItem token, string message) : this(error, token, message, null)
        {
        }

        public PascalException(ErrorCode error, TokenItem token, string message, Exception ex) : base(message + Location(token), ex)
        {
            Error = error;
            Token = token;
        }

        private static string Location(TokenItem current)
        {
            if (current == null) return "";
            return $" at line {current.Line} column {current.Column} index {current.Index}";
        }
    }

    public class LexerException : PascalException
    {
        public LexerException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class ParserException : PascalException
    {
        public ParserException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class SemanticException : PascalException
    {
        public SemanticException(ErrorCode error, TokenItem token, string message) : base(error, token, message)
        {
        }
    }

    public class RuntimeException : PascalException
    {
        public RuntimeException(ErrorCode error, TokenItem token, string message, Exception ex = null) : base(error, token, message, ex)
        {
        }
    }
}