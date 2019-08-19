using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ILTestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Square(10));
            Console.ReadLine();
        }

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Square(int number);
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Factorial(int x);
    }
}
