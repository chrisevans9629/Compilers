using System.Collections.Generic;

namespace Compilers.Interpreter
{
    public enum Tokens
    {
        Procedure = ' '
    }
    public class PascalTerms
    {
        public const string While = "WHILE";
        public const string NotEqual = "<>";
        public const string Not = "NOT";
        public const string In = "IN";
        public const string Pointer = "^Pointer";
        public const string EndOfFile = "EndOfFile";
        public const string For = "FOR";
        public const string Do = "DO";
        public const string To = "TO";
        public const string If = "IF";
        public const string Then = "THEN";
        public const string Else = "ELSE";
        public const string Function = "FUNCTION";
        public const string Procedure = "PROCEDURE";
        public const string Program = "PROGRAM";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string Var = "VAR";
        public const string Const = "CONST";
        public const string Real = "REAL";
        public const string Int = "INTEGER";
        public const string Boolean = "BOOLEAN";
        public const string Char = "CHAR";
        public const string String = "STRING";
        public static IEnumerable<string> BuiltInTypes = new List<string>()
        {
            Real,
            Int,
            Boolean,
            Char,
            String
        };
        public const string StringConst = "STRING_Const";
        public const string BoolConst = "Bool_Const";
        public const string RealConst = "Real_Const";
        public const string IntegerConst = "Integer_Const";
        public const string Id = "ID";
        public const string Assign = "ASSIGN";
        public const string IntDiv = "DIV";
        public const string Case = "CASE";
        public const string Of = "OF";
        public const string Equal = "=";
        public const string Dot      = ".";
        public const string Colon    = ":";
        public const string LeftBracket = "[";
        public const string RightBracket = "]";
        public const string Semi     = ";";
        public const string Comma    = ",";
      
        public const string LParinth = "(";
        public const string RParinth = ")";
        public const string Multi    = "*";
        public const string FloatDiv = "/";
        public const string Add      = "+";
        public const string Sub      = "-";


        public static IEnumerable<string> SingleTokens = new List<string>()
        {
            Dot,
            Colon,
            Semi,
            Comma,
            LParinth,
            RParinth,
            Multi,
            FloatDiv,
            Add,
            Sub,
            Equal,
            LeftBracket,
            RightBracket
        };

        public static string Collection = "COLLECTION";


        public static Token CreateToken(string name)
        {
            return new Token(){Name = name};
        }

        public static Dictionary<string,Token> Reservations = new Dictionary<string,Token>()
        {
            { IntDiv,CreateToken(IntDiv)} ,
            { Begin, CreateToken(Begin)},
            {End, CreateToken(End)},
            {Var, CreateToken(Var)},
            {Int, CreateToken(Int)},
            {Real, CreateToken(Real)},
            {Program, CreateToken(Program)},
            {Procedure, CreateToken(Procedure)},
            {Function, CreateToken(Function)},
            {If, CreateToken(If)},
            {Then, CreateToken(Then)},
            {Else, CreateToken(Else)},
            {"TRUE", CreateToken(BoolConst) },
            {"FALSE", CreateToken(BoolConst) },
            {For, CreateToken(For) },
            {To, CreateToken(To) },
            {Do, CreateToken(Do) },
            {Const, CreateToken(Const) },
            {In, CreateToken(In) },
            {Not, CreateToken(Not) },
            {Case, CreateToken(Case) },
            {Of, CreateToken(Of) },
            {While, CreateToken(While) }
            
        };
    }
}