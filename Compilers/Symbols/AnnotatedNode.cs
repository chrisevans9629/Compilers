using System.Collections.Generic;
using Compilers.Ast;

namespace Compilers.Symbols
{
    public class AnnotatedNode
    {
        public Dictionary<string, object> Annotations
        {
            get=> Node.Annotations;
            set => Node.Annotations = value;
        }

        public Symbol Symbol
        {
            get
            {
                if (Annotations.ContainsKey("Symbol"))
                {
                    return Annotations["Symbol"] as Symbol;
                }

                return null;
            }
            set
            {
                if (Annotations.ContainsKey("Symbol"))
                {
                    Annotations["Symbol"] = value;
                }
                else
                {
                    Annotations.Add("Symbol", value);
                }
            }
        }
        public Node Node { get; set; }
        public AnnotatedNode(Node node)
        {
            Node = node;
        }
    }
}