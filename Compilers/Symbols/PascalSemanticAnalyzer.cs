using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;
using Compilers.Interpreter;

namespace Compilers.Symbols
{
    public class PascalSemanticAnalyzer : IPascalNodeVisitor<AnnotatedNode>
    {
        private readonly ILogger _logger;
        private ScopedSymbolTable _currentScope;
        public AnnotatedNode VisitProgram(PascalProgramNode programNode)
        {
            var levelZero = CreateCurrentScope(programNode.ProgramName);


            var global = new ScopedSymbolTable("Global", levelZero, _logger);
            CurrentScope = global;
            programNode.Block.Annotations.Add("Main", null);
            VisitBlock(programNode.Block);
            CurrentScope = global;
            programNode.Block.Annotations.Add("SymbolTable", global);

            return new AnnotatedNode(programNode);
        }

        public ScopedSymbolTable CreateCurrentScope(string name)
        {
            var levelZero = new ScopedSymbolTable(name, null, _logger);
            CurrentScope = levelZero;

            DefineBuiltIns(levelZero);
            return levelZero;
        }

        public static void DefineBuiltIns(ScopedSymbolTable levelZero)
        {
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Int, PascalTerms.Real));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Real));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Pointer));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.String));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Char, PascalTerms.String));
            levelZero.Define(new BuiltInTypeSymbol(PascalTerms.Boolean));
            levelZero.Define(new ProcedureDeclarationSymbol("READ", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char})))
            }));

            levelZero.Define(new FunctionDeclarationSymbol("UpCase", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "look"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char})))
            }, levelZero.LookupSymbol(PascalTerms.Char, true)));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("Halt", new List<ParameterNode>()));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x3"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x4"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));

            
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("WriteLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));

            levelZero.Define(new ProcedureDeclarationSymbol("ReadLn", new List<ParameterNode>()
            {
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("ReadLn", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));

            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x2"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
            levelZero.Define(new ProcedureDeclarationSymbol("Write", new List<ParameterNode>()
            {
                new ParameterNode(new VarDeclarationNode(new VariableOrFunctionCall(new TokenItem() {Value = "x1"}),
                    new TypeNode(new TokenItem() {Value = PascalTerms.Char}))),
            }));
        }

        public ScopedSymbolTable CurrentScope
        {
            get => _currentScope;
            private set
            {
                if (_currentScope != null)
                {
                    _logger.Log($"Closed Scope {_currentScope.ScopeName}");
                }
                _currentScope = value;
                if (value != null)
                {
                    _logger.Log($"Opened Scope {value.ScopeName}");
                }
            }
        }

        public PascalSemanticAnalyzer(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }

        private PascalResult<AnnotatedNode> pascalResult;
        public PascalResult<AnnotatedNode> CheckSyntaxResult(Node rootNode)
        {
            pascalResult = new PascalResult<AnnotatedNode>();
            var result = VisitNode(rootNode);
            result.Annotations.Add("SymbolTable", CurrentScope);
            pascalResult.Result = result;
            return pascalResult;
        }
        public ScopedSymbolTable CheckSyntax(Node rootNode)
        {
            var r = CheckSyntaxResult(rootNode);
            if (r.Errors.Any())
            {
                throw r.Errors[0];
            }
            return r.Result.Annotations["SymbolTable"] as ScopedSymbolTable;
        }



        public AnnotatedNode VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public AnnotatedNode VisitNegationOperator(NegationOperator negate)
        {
            return VisitNode(negate.Right);
        }

        public AnnotatedNode VisitIfStatement(IfStatementNode ifStatement)
        {
            VisitNode(ifStatement.IfCheck);
            VisitNode(ifStatement.IfTrue);
            if (ifStatement.IfFalse != null)
            {
                VisitNode(ifStatement.IfFalse);
            }

            return new AnnotatedNode(ifStatement);
        }

        public AnnotatedNode VisitForLoop(ForLoopNode forLoop)
        {
            VisitAssignment(forLoop.AssignFromNode);
            VisitNode(forLoop.ToNode);
            VisitNode(forLoop.DoStatements);
            return new AnnotatedNode(forLoop);
        }

        public AnnotatedNode VisitFunctionCall(CallNode functionCall)
        {
            return VisitCall(functionCall);
        }

        public AnnotatedNode VisitEqualExpression(EqualExpression equal)
        {
            var left = VisitNode(equal.Left);
            var right = VisitNode(equal.Right);
            return CheckTypeMatch(equal.TokenItem, left, right, equal);
        }

        BuiltInTypeSymbol GetBuiltInType(Symbol sym)
        {
            if (sym is Symbol symbol)
            {
                if (symbol is BuiltInTypeSymbol t)
                {
                    return t;
                }
                if (symbol.Type != null)
                {
                    return GetBuiltInType(symbol.Type);
                }
            }


            return null;
        }
        private AnnotatedNode CheckTypeMatch(TokenItem tokenItem, AnnotatedNode left, AnnotatedNode right, Node nodeForLog, bool twoWayConversion = true)
        {
            if (GetBuiltInType(left?.Symbol) is BuiltInTypeSymbol a && GetBuiltInType(right?.Symbol) is BuiltInTypeSymbol b)
            {
                if (a.Name == b.Name)
                {
                    return new AnnotatedNode(nodeForLog) { Symbol = a }; ;
                }

                if (a.Conversions.Contains(b.Name) && twoWayConversion)
                {
                    return new AnnotatedNode(nodeForLog) { Symbol = b }; ;
                }
                if (b.Conversions.Contains(a.Name))
                {
                    return new AnnotatedNode(nodeForLog) { Symbol = a };
                }


                TypeMismatch(tokenItem, a, b);
            }

            return null;
            //throw new NotImplementedException($"{nodeForLog}");
        }

        private void TypeMismatch(TokenItem tokenItem, BuiltInTypeSymbol a, BuiltInTypeSymbol b)
        {
            pascalResult.Errors.Add(new SemanticException(ErrorCode.TypeMismatch, tokenItem, $"type {a} is not assignable to {b}"));
        }

        public AnnotatedNode VisitWhileLoop(WhileLoopNode whileLoop)
        {
            VisitNode(whileLoop.BoolExpression);
            CurrentScope = new ScopedSymbolTable("_while_", CurrentScope);
            VisitNode(whileLoop.DoStatement);
            whileLoop.Annotations.Add("SymbolTable", CurrentScope);
            CurrentScope = CurrentScope.ParentScope;
            return new AnnotatedNode(whileLoop);
        }

        public AnnotatedNode VisitReal(RealNode real)
        {
            //return real.Value;
            return new AnnotatedNode(real) { Symbol = CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Real, true) };
        }

        public AnnotatedNode VisitInteger(IntegerNode integer)
        {
            return new AnnotatedNode(integer) { Symbol = CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Int, true) }; ;
        }

        public AnnotatedNode VisitBinaryOperator(BinaryOperator binary)
        {
            var left = VisitNode(binary.Left);
            var right = VisitNode(binary.Right);
            return CheckTypeMatch(binary.TokenItem, left, right, binary);
        }

        public AnnotatedNode VisitUnary(UnaryOperator unary)
        {
            var value = VisitNode(unary.Value);
            return value;
        }

        public AnnotatedNode Fail(Node node)
        {
            return this.FailModel(node);
        }

        public AnnotatedNode VisitString(StringNode str)
        {
            if (str.CurrentValue.Length == 1)
            {
                return new AnnotatedNode(str) { Symbol = CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Char, true) }; ;
            }
            else
            {
                return new AnnotatedNode(str) { Symbol = CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true) }; ;
            }
        }

        public AnnotatedNode VisitInOperator(InOperator inOperator)
        {
            var compare = VisitNode(inOperator.CompareNode);

            var list = VisitNode(inOperator.ListExpression);
            if (list.Symbol is CollectionTypeSymbol collection)
            {
                var comType = GetBuiltInType(compare.Symbol);
                if (collection.ItemType.Name != comType.Name && comType.Conversions.Contains(collection.ItemType.Name) != true)
                {
                    TypeMismatch(inOperator.TokenItem, collection, comType);
                }
            }

            return new AnnotatedNode(inOperator) { Symbol = CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.Boolean, true) }; ;
        }

        public AnnotatedNode VisitCaseStatement(CaseStatementNode caseStatement)
        {
            VisitNode(caseStatement.CompareExpression);
            foreach (var caseStatementCaseItemNode in caseStatement.CaseItemNodes)
            {
                VisitCaseItem(caseStatementCaseItemNode);
            }

            if (caseStatement.ElseStatement != null)
            {
                VisitNode(caseStatement.ElseStatement);
            }

            return new AnnotatedNode(caseStatement);
        }

        private void VisitCaseItem(CaseItemNode caseItem)
        {
            foreach (var caseItemCase in caseItem.Cases)
            {
                VisitNode(caseItemCase);
            }
            VisitNode(caseItem.Statement);
        }

        public AnnotatedNode VisitConstantDeclaration(ConstantDeclarationNode decNode)
        {
            var value = decNode.Value;

            var typeName = $"Constant Error: {value}";

            if (value is PointerNode p)
            {
                typeName = p.TokenItem.Token.Name;
            }

            if (value is StringNode)
            {
                typeName = PascalTerms.String;
            }


            //VisitNode(decNode.Value);
            var symbol = this.DefineVariableSymbol(decNode.TokenItem, decNode.ConstantName, typeName);
            return new AnnotatedNode(decNode) { Symbol = symbol };
        }

        public AnnotatedNode VisitPointer(PointerNode pointer)
        {
            return new AnnotatedNode(pointer); ;
        }

        public AnnotatedNode VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            if (CurrentScope.ScopeLevel > 1)
            {
                procedureDeclaration.Annotations.Add("Nested", null);
            }
            DeclareParameters(procedureDeclaration);
            VisitBlock(procedureDeclaration.Block);
            procedureDeclaration.Annotations.Add("SymbolTable", CurrentScope);
            CurrentScope = CurrentScope.ParentScope;
            var procedure = new ProcedureDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters);
            CurrentScope.Define(procedure);
            return new AnnotatedNode(procedureDeclaration);
        }

        public AnnotatedNode VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            return VisitCall(procedureCall);
        }

        public AnnotatedNode VisitFunctionDeclaration(FunctionDeclarationNode procedureDeclaration)
        {
            var procedure = new FunctionDeclarationSymbol(procedureDeclaration.Name, procedureDeclaration.Parameters, CurrentScope.LookupSymbol(procedureDeclaration.ReturnType.TypeValue, true));
            CurrentScope.Define(procedure);
            if (CurrentScope.ScopeLevel > 1)
            {
                procedureDeclaration.Annotations.Add("Nested", null);
            }
            DeclareParameters(procedureDeclaration);
            var symbol = DefineVariableSymbol(procedureDeclaration.Token, procedureDeclaration.FunctionName, procedureDeclaration.ReturnType.TypeValue);

            symbol.Annotations.Add("ReturnVariable", null);
            VisitBlock(procedureDeclaration.Block);

            var returnVariable = CurrentScope.LookupSymbol<VariableSymbol>(procedureDeclaration.FunctionName, false);

            if (returnVariable == null)
            {
                NotFound(procedureDeclaration.Token, procedureDeclaration.ReturnType.TypeValue, procedureDeclaration.FunctionName);
            }

            if (!returnVariable.Initialized)
            {
                pascalResult.Errors.Add(new SemanticException(ErrorCode.DoesNotReturnValue, procedureDeclaration.Token, $"Function {procedureDeclaration.FunctionName} does not return a value"));
            }
            procedureDeclaration.Annotations.Add("SymbolTable", CurrentScope);
            CurrentScope = CurrentScope.ParentScope;

            return new AnnotatedNode(procedureDeclaration) { Symbol = procedure };
        }

        public AnnotatedNode VisitBool(BoolNode boolNode)
        {
            return new AnnotatedNode(boolNode); ;
        }

        public AnnotatedNode VisitRangeExpression(ListRangeExpressionNode listRange)
        {

            CheckTypeMatch(listRange.TokenItem, VisitNode(listRange.FromNode), VisitNode(listRange.ToNode), listRange);

            return new AnnotatedNode(listRange) { Symbol = new CollectionTypeSymbol(CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true)) };
        }

        public AnnotatedNode VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            Node prevNode = null;
            foreach (var stringNode in itemsExpressionNode.Items)
            {
                if (prevNode == null)
                {
                    prevNode = stringNode;
                }

                CheckTypeMatch(itemsExpressionNode.TokenItem, VisitNode(prevNode), VisitNode(stringNode), itemsExpressionNode);
                prevNode = stringNode;
            }
            return new AnnotatedNode(itemsExpressionNode) { Symbol = new CollectionTypeSymbol(CurrentScope.LookupSymbol<BuiltInTypeSymbol>(PascalTerms.String, true)) };
        }

        private void DeclareParameters(DeclarationNode procedureDeclaration)
        {
            var name = procedureDeclaration.Name;
            var scope = new ScopedSymbolTable(name, CurrentScope, _logger);
            CurrentScope = scope;
            var param = procedureDeclaration.Parameters;
            foreach (var varDeclaration in param.Select(p => p.Declaration))
            {
                //VisitVarDeclaration(varDeclaration);
                var typeName = varDeclaration.TypeNode.TypeValue;
                var varName = varDeclaration.VarNode.VariableName;
                var symbol = this.CurrentScope.LookupSymbol(typeName, true);
                var varSymbol = new VariableSymbol(varName, symbol);
                CurrentScope.Define(varSymbol);
            }


        }

        private AnnotatedNode VisitCall(CallNode funCall)
        {
            var symbols = CurrentScope.LookupSymbols<DeclarationSymbol>(funCall.Name, true);
            if (symbols.Any() != true)
            {
                NotFound(funCall.Token, funCall.Type, funCall.Name);
            }

            foreach (var funCallParameter in funCall.Parameters)
            {
                VisitNode(funCallParameter);
            }

            var callCount = funCall.Parameters.Count;



            if (symbols.All(p => p.Parameters.Count != callCount))
            {
                pascalResult.Errors.Add(new SemanticException(ErrorCode.ParameterMismatch,
                    funCall.Token,
                    $"Function {funCall.Name} has {symbols.FirstOrDefault()?.Parameters?.Count} parameters but {callCount} was used"));
            }

            var symbol = symbols.FirstOrDefault(p => p.Parameters.Count == callCount);

            return new AnnotatedNode(funCall) { Symbol = symbol }; ;
        }


        public AnnotatedNode VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            return VisitVariable(call);
        }

        public AnnotatedNode VisitNoOp(NoOp nope)
        {
            return new AnnotatedNode(nope);
        }

        public AnnotatedNode VisitAssignment(AssignmentNode ass)
        {
            var variable = VisitVariable(ass.Left);
            var assignmentType = VisitNode(ass.Right);
            if (variable.Symbol is VariableSymbol v)
            {
                v.Initialized = true;
                if (v.Annotations.ContainsKey("ReturnVariable"))
                {
                    ass.Annotations.Add("Return", null);
                }
            }

            
           
            //CheckType(ass.TokenItem, assignmentType, variable);


            //if (variable.Type.Name == PascalTerms.Int && ass.Right is RealNode r)
            //{
            //    throw new SemanticException(ErrorCode.TypeMismatch, r.TokenItem, $"Cannot assign Real to integer");
            //}

            return CheckTypeMatch(ass.TokenItem, variable, assignmentType, ass, false);
        }

        //private  object CheckType(TokenItem token, object assignmentType, Symbol variable)
        //{
        //    if (GetBuiltInType(assignmentType) is BuiltInTypeSymbol symbol)
        //    {
        //        var varTypeName = variable.Type.Name.ToUpper();
        //        var assTypeName = symbol.Name.ToUpper();
        //        if (varTypeName != assTypeName && symbol.Conversions.Select(p=>p.ToUpper()).Contains(varTypeName.ToUpper()) != true)
        //        {
        //            throw new SemanticException(ErrorCode.TypeMismatch, token,
        //                $"Cannot assign type '{assTypeName}' to '{variable.Name}' with type {varTypeName}");
        //        }

        //        return symbol;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException($"assignment not returning type symbol '{assignmentType}'");
        //    }
        //}

        private AnnotatedNode VisitVariable(VariableOrFunctionCall assLeft)
        {
            var varName = assLeft.VariableName;
            Symbol symbol = CurrentScope.LookupSymbol<VariableSymbol>(varName, true);
            if (symbol == null)
            {
                symbol = CurrentScope.LookupSymbol<FunctionDeclarationSymbol>(varName, true);
            }
            if (symbol == null)
            {
                NotFound(assLeft.TokenItem, "Variable", varName);
            }

            return new AnnotatedNode(assLeft) { Symbol = symbol };
        }

        private void NotFound(TokenItem assLeft, string type, string varName)
        {
            pascalResult.Errors.Add(new SemanticException(ErrorCode.IdNotFound, assLeft, $"{type} '{varName}' was not declared"));
        }


        public AnnotatedNode VisitBlock(BlockNode programBlock)
        {
            foreach (var programBlockDeclaration in programBlock.Declarations)
            {
                VisitNode(programBlockDeclaration);
            }
            VisitCompoundStatement(programBlock.CompoundStatement);
            return new AnnotatedNode(programBlock);
        }

        public AnnotatedNode VisitCompoundStatement(CompoundStatementNode node)
        {
            foreach (var nodeNode in node.Nodes)
            {
                VisitNode(nodeNode);
            }

            return new AnnotatedNode(node);
        }



        public AnnotatedNode VisitVarDeclaration(VarDeclarationNode node)
        {
            if (CurrentScope.ScopeLevel <= 1)
            {
                node.Annotations.Add("Global", null);
            }
            var typeName = node.TypeNode.TypeValue;
            var varName = node.VarNode.VariableName;
            return new AnnotatedNode(node) { Symbol = DefineVariableSymbol(node.VarNode.TokenItem, varName, typeName) };
        }

        private VariableSymbol DefineVariableSymbol(TokenItem node, string varName, string typeName)
        {
            var variable = CurrentScope.LookupSymbol(varName, false);
            if (variable != null)
            {
                pascalResult.Errors.Add(new SemanticException(ErrorCode.DuplicateId, node,
                    $"Variable '{varName}' has already been defined as {variable}"));
            }

            var symbol = this.CurrentScope.LookupSymbol(typeName, true);
            if (symbol == null)
            {
                pascalResult.Errors.Add(new SemanticException(ErrorCode.IdNotFound, node, $"Could not find type {typeName}"));
            }

            var varSymbol = new VariableSymbol(varName, symbol);
            CurrentScope.Define(varSymbol);
            return varSymbol;
        }
    }
}