﻿using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public class AnalyzeResult<T>
{
    public T Node { get; }
    public Scope Scope { get; }

    public AnalyzeResult(T node, Scope scope)
    {
        Node = node;
        Scope = scope;
    }
}

public class SemanticAnalyzer
{
    public AnalyzeResult<SemanticNode> Analyze(Program program)
    {
        return AnalyzeProgram(program, Scope.Empty);
    }
    public AnalyzeResult<SemanticNode> AnalyzeProgram(Program node, Scope scope)
    {
        var functions = new List<FunctionNode>();
        foreach (var statement in node.Functions)
        {
            var functionResult = AnalyzeFunction(statement, scope);
            scope = functionResult.Scope;
            functions.Add(functionResult.Node);
        }
        return new AnalyzeResult<SemanticNode>(new ProgramNode(functions), scope);
    }
    private AnalyzeResult<FunctionNode> AnalyzeFunction(Function function, Scope parentScope)
    {
        var analyzeBlockResult = AnalyzeBlock(function.Block, parentScope);
        return new AnalyzeResult<FunctionNode>(new FunctionNode(function.Name, analyzeBlockResult.Node), analyzeBlockResult.Scope);
    }
    private AnalyzeResult<BlockNode> AnalyzeBlock(Block block, Scope scope)
    {
        var statements = new List<StatementNode>();
        foreach (var statement in block)
        {
            var analyzedStatementResult = AnalyzeStatement(statement, scope);
            statements.Add(analyzedStatementResult.Node);
            scope = analyzedStatementResult.Scope;
        }
        return new AnalyzeResult<BlockNode>(new BlockNode(statements), scope);
    }
    private AnalyzeResult<StatementNode> AnalyzeStatement(Statement statement, Scope scope)
    {
        switch (statement)
        {
            case DeclarationStatement declarationStatement:
                return AnalyzeDeclaration(declarationStatement, scope);
            case ExpressionStatement expressionStatement:
                return AnalyzeExpressionStatement(expressionStatement, scope);
            case IfElseStatement ifElseStatement:
                break;
            case ReturnStatement returnStatement:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }
        throw new InvalidOperationException();
    }
    private AnalyzeResult<StatementNode> AnalyzeExpressionStatement(ExpressionStatement expressionStatement, Scope scope)
    {
        var analyzeExpressionResult = AnalyzeExpression(expressionStatement.Expression, scope);
        return new AnalyzeResult<StatementNode>(new ExpressionStatementNode(analyzeExpressionResult.Node), scope);
    }
    private AnalyzeResult<ExpressionNode> AnalyzeExpression(Expression expression, Scope scope)
    {
        if (expression is AssignmentExpression assignmentExpression)
        {
            return AnalyzeAssignmentExpression(scope, assignmentExpression);
        }
        else if (expression is ConstantExpression c)
        {
            return new AnalyzeResult<ExpressionNode>(new ConstantNode(c.Value), scope);
        }
        if (expression is BinaryExpression binaryExpression)
        {
            return AnalyzeBinaryExpression(binaryExpression, scope);
        }
        if (expression is IdentifierExpression i)
        {
            var symbolNode = GetSymbolNode(scope, i.Identifier);
            return new AnalyzeResult<ExpressionNode>(new SymbolExpressionNode(symbolNode), scope);
        }
        throw new InvalidOperationException("Por aquí no vas a ningún sitio");
    }
    private SymbolNode GetSymbolNode(Scope scope, string name)
    {
        return scope.Get(name).Match(symbol => (SymbolNode)new KnownSymbolNode(symbol), () => new UnknownSymbol(name));
    }
    private AnalyzeResult<ExpressionNode> AnalyzeBinaryExpression(BinaryExpression binaryExpression, Scope scope)
    {
        if (binaryExpression is AddExpression)
        {
            var left = AnalyzeExpression(binaryExpression.Left, scope);
            var right = AnalyzeExpression(binaryExpression.Right, scope);
            return new AnalyzeResult<ExpressionNode>(new AddExpressionNode(left.Node, right.Node), scope);
        }
        throw new InvalidOperationException("Por aquí no vas a ningún sitio tampoco");
    }
    private AnalyzeResult<ExpressionNode> AnalyzeAssignmentExpression(Scope scope, AssignmentExpression assignmentExpression)
    {
        var right = AnalyzeExpression(assignmentExpression.Right, scope);
        var left = scope.Get(assignmentExpression.Left.Identifier);
        return left.Match(
            symbol => new AnalyzeResult<ExpressionNode>(new AssignmentNode(new KnownSymbolNode(symbol), right.Node), scope),
            () => new AnalyzeResult<ExpressionNode>(new AssignmentNode(new UnknownSymbol(assignmentExpression.Left.Identifier), right.Node)
            {
                Errors = [$"Use of undeclared variable '{assignmentExpression.Left.Identifier}'"]
            }, scope));
    }
    private AnalyzeResult<StatementNode> AnalyzeDeclaration(DeclarationStatement declarationStatement, Scope scope)
    {
        var declaration = scope
            .TryDeclare(new Symbol(declarationStatement.Name, IntType.Instance))
            .Match(
                s => new AnalyzeResult<StatementNode>(new DeclarationNode(declarationStatement.Name, s), s),
                _ => new AnalyzeResult<StatementNode>(new DeclarationNode(declarationStatement.Name, scope)
                {
                    Errors = [$"Variable {declarationStatement.Name} is already declared"]
                }, scope));
        return declaration;
    }
}


public class SymbolExpressionNode : ExpressionNode
{
    public SymbolNode SymbolNode { get; }

    public SymbolExpressionNode(SymbolNode symbolNode)
    {
        SymbolNode = symbolNode;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitSymbolExpression(this);
    }

    public override IEnumerable<SemanticNode> Children => [SymbolNode];
}

public class AddExpressionNode : BinaryExpressionNode
{
    public AddExpressionNode(ExpressionNode left, ExpressionNode right) : base(left, right, "+")
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitAddition(this);
    }
}