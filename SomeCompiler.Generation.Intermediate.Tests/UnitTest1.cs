using SomeCompiler.Core;
using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var gen = new IntermediateCodeGenerator();
            var symbolExpressionNode = new SymbolExpressionNode(new KnownSymbolNode(new Symbol("a", IntType.Instance)));
            var codes = gen.GenerateFunction(new ProgramNode(new List<FunctionNode>()
            {
                new("Hola", new BlockNode(new List<StatementNode>()
                {
                    new ExpressionStatementNode(new BinaryExpressionNode(symbolExpressionNode, new ConstantNode(1), Operator.Addition))
                }))
            }));
            
            
        }
    }
}