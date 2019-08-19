using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Symbols
{
    public class Symbol
    {
        public string Name { get; protected set; }
        public Symbol Type { get; protected set; }

        public Symbol(string name, Symbol type = null)
        {
            Name = name;
            Type = type;
        }
    }
    
    public class CollectionTypeSymbol : BuiltInTypeSymbol
    {
        public BuiltInTypeSymbol ItemType { get; set; }
        public CollectionTypeSymbol(BuiltInTypeSymbol itemType) : base(PascalTerms.Collection)
        {
            ItemType = itemType;
        }
    }

    public class BuiltInTypeSymbol : Symbol
    {
        public IList<string> Conversions { get; set; }
        public BuiltInTypeSymbol(string name, params string[] conversions) : base(name)
        {
            Conversions = new List<string>(conversions);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class VariableSymbol : Symbol
    {
        public bool Initialized { get; set; }
        public VariableSymbol(string name, Symbol type) : base(name, type)
        {
        }

        public override string ToString()
        {
            return $"<{Name}:{Type}>";
        }
    }
}