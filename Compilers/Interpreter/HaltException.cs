using System;

namespace Minesweeper.Test
{
    public class HaltException : RuntimeException
    {
        public HaltException(ErrorCode error, TokenItem token, string message, Exception ex = null) : base(error, token, message, ex)
        {
        }
    }
}