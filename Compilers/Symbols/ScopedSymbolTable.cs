using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;

namespace Compilers.Symbols
{

    
    public class ScopedSymbolTable
    {
        public ScopedSymbolTable ParentScope { get; }
        private readonly ILogger _logger;
        public string ScopeName { get; }
        public int ScopeLevel { get; }
        IList<Symbol> symbols = new List<Symbol>();

        public ScopedSymbolTable(string scopeName,  ScopedSymbolTable parentScope = null, ILogger logger = null)
        {
            ParentScope = parentScope;
            _logger = logger ?? new Logger();
            ScopeName = scopeName;
            ScopeLevel = ParentScope?.ScopeLevel + 1 ?? 0;
            
            if (parentScope?.ScopeLevel >= ScopeLevel)
            {
                throw new InvalidOperationException($"Parent scope must be greater:\nParent: {parentScope}, Current: {this}");
            }
        }

        public int Count => symbols.Count + (ParentScope?.Count ?? 0);

        public void Define(Symbol symbol)
        {
            _logger.Log($"Define: {symbol}");
            symbols.Add(symbol);
        }

        public override string ToString()
        {
            var str = "Symbols: ";
            foreach (var symbol in symbols)
            {
                str += symbol.Name + ", ";
            }

            return str;
        }

        public int LookupSymbolScope(string name)
        {
            _logger.Log($"Lookup Scope: {name}");
            var current = symbols.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (current == null && ParentScope != null)
            {
                return ParentScope.LookupSymbolScope(name);
            }
            return ScopeLevel;
        }

        public Symbol LookupSymbol(string name, bool searchParent)
        {
            _logger.Log($"Lookup: {name}");
            var current = symbols.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (current == null && ParentScope != null && searchParent)
            {
                return ParentScope.LookupSymbol(name, true);
            }
            return current;
        }
        public T LookupSymbol<T>(string name, bool searchParent) where T: Symbol
        {
            _logger.Log($"Lookup: {name}");
            var current = symbols.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (current == null && ParentScope != null && searchParent)
            {
                return ParentScope.LookupSymbol<T>(name, true);
            }
            if (current is T c)
            {
                return c;
            }

            return default(T);
        }

        public IList<T> LookupSymbols<T>(string name, bool searchParent) where T : Symbol
        {
            var syms = new List<T>();
            _logger.Log($"Lookup: {name}");
            var current = symbols.OfType<T>().Where(p => p.Name.ToLower() == name.ToLower());
            syms.AddRange(current);
            if ( ParentScope != null && searchParent)
            {
                var parentsyms = ParentScope.LookupSymbols<T>(name, true);
                syms.AddRange(parentsyms);
            }

            return syms;
        }
    }
}