using System.Collections.Generic;

namespace Compilers.Interpreter
{
    public class CSharpTypesConverter
    {
        private Dictionary<string, string> types;

        public CSharpTypesConverter()
        {
            types = new Dictionary<string, string>();
            types.Add(PascalTerms.Int, "int");
            types.Add(PascalTerms.Real, "double");
        }
        public string GetTypeName(string pascalType)
        {
            if (types.ContainsKey(pascalType.ToUpper()))
            {
                return types[pascalType.ToUpper()];
            }

            return pascalType;
        }
    }
}