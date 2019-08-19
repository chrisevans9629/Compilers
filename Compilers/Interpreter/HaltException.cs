using System;

namespace Compilers.Interpreter
{
    public class HaltException : RuntimeException
    {
        public HaltException(ErrorCode error, TokenItem token, string message, Exception ex = null) : base(error, token, message, ex)
        {
        }
    }
}