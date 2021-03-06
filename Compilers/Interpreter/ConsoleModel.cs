﻿using System;

namespace Compilers.Interpreter
{
    public class ConsoleModel : IConsole
    {
        public Iterator<char> Input { get; set; } = new Iterator<char>("".ToCharArray());

        public void Write(string str)
        {
            Output += str;
        }

        public void Clear()
        {
            Output = "";
        }

        public char Read()
        {
            var v = Input.Current;
            Input.Advance();
            return v;
        }

        public void WriteLine(string str)
        {
            Output += str + Environment.NewLine;
        }

        public string Output { get; private set; }
    }
}