namespace Minesweeper.Test.Tests
{
    public static class PascalTestInputs
    {
        public static class Invalid
        {
            public const string UndefinedProcedureAdd = "program test;begin Add();end.";
            public const string UndefinedVariableCallInProcedureAdd =
                "program test;var t : integer;\r\nprocedure add(a : real);\r\nbegin\r\nend;\r\nbegin\r\nadd(x);\r\nend.";
            public const string TooManyParametersProcedureAdd =
                "program test;var t : integer;\r\nprocedure add();\r\nbegin\r\nend;\r\nbegin\r\nadd(t);\r\nend.";

            public const string FunctionDoesNotHaveReturn =
                "program test;\r\nvar t : integer;\r\nfunction Add() : integer;\r\nbegin\r\nend;\r\nbegin\r\nend.";

            public const string MismatchingType = "program test;\r\nvar asdf : integer;\r\nbegin\r\nasdf := 10.0;\r\nend.";
        }

        public const string ProcedureCallXEquals10 =
            "program test; \n" +
            "var x : integer; \n" +
            "procedure Add(y : integer); \n" +
            "begin \n" +
                "x := x + y; \n" +
            "end; \n" +
            "begin \n" +
            "x := 0; \n" +
            "Add(10); \n" +
            "end.";

        public const string PascalSourceToSource =
            @"
program Main;
var b, x, y : real;
var z : integer;
procedure AlphaA(a : integer);
var b : integer;
    procedure Beta(c : integer);
    var y : integer;
        procedure Gamma(c : integer);
        var x : integer;
        begin { Gamma }
            x := a + b + c + x + y + z;
        end;  { Gamma }
    begin { Beta }
    end;  { Beta }
begin { AlphaA }
end;  { AlphaA }

procedure AlphaB(a : integer);
var c : real;
begin { AlphaB }
    c := a + b;
end;  { AlphaB }
begin { Main }
end.  { Main }";

        public const string PascalSourceToSourceResult = 
            "program Main0;\r\n   var b1 : REAL;\r\n   var x1 : REAL;\r\n   var y1 : REAL;\r\n   var z1 : INTEGER;\r\n   procedure AlphaA1(a2 : INTEGER);\r\n      var b2 : INTEGER;\r\n      procedure Beta2(c3 : INTEGER);\r\n         var y3 : INTEGER;\r\n         procedure Gamma3(c4 : INTEGER);\r\n            var x4 : INTEGER;\r\n         begin\r\n            <x4:INTEGER> := <a2:INTEGER> + <b2:INTEGER> + <c4:INTEGER> + <x4:INTEGER> + <y3:INTEGER> + <z1:INTEGER>;\r\n         end; {END OF Gamma}\r\n      begin\r\n      end; {END OF Beta}\r\n   begin\r\n   end; {END OF AlphaA}\r\n   procedure AlphaB1(a2 : INTEGER);\r\n      var c2 : REAL;\r\n   begin\r\n      <c2:REAL> := <a2:INTEGER> + <b1:REAL>;\r\n   end; {END OF AlphaB}\r\nbegin\r\nend. {END OF Main}";
        public const string PascalProgramWithProceduresWithMultipleParameters =
            "program Main;\r\n   var x, y: real;\r\n\r\n   procedure Foo(a, b : INTEGER; c : REAL);\r\n      var y : integer;\r\n   begin\r\n      x := a + x + y;\r\n   end;\r\n\r\nbegin { Main }\r\n\r\nend.  { Main }";

        public const string PascalProgramWithProceduresWithParameters =
            "program Main;\r\n   var x, y: real;\r\n\r\n   procedure Alpha(a : integer);\r\n      var y : integer;\r\n   begin\r\n      x := a + x + y;\r\n   end;\r\n\r\nbegin { Main }\r\n\r\nend.  { Main }";
        public const string PascalProgramWithProcedures =
            "PROGRAM Part12;\r\nVAR\r\n   a : INTEGER;\r\n\r\nPROCEDURE P1;\r\nVAR\r\n   a : REAL;\r\n   k : INTEGER;\r\n\r\n   PROCEDURE P2;\r\n   VAR\r\n      a, z : INTEGER;\r\n   BEGIN {P2}\r\n      z := 777;\r\n   END;  {P2}\r\n\r\nBEGIN {P1}\r\n\r\nEND;  {P1}\r\n\r\nBEGIN {Part12}\r\n   a := 10;\r\nEND.  {Part12}";

        public const string PascalProgramWithMultipleVarDeclarations = "program main;\n   var b, x, y : real;\n   var z : integer;\n\n \n\nbegin { main }\nend.  { main }";
    }
}