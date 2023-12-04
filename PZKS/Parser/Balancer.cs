namespace PZKS.Parser;

public class Balancer
{
    ExpressionNode PlusNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Plus, "+") };
    }

    ExpressionNode MultNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Mult, "*") };
    }
    
    ExpressionNode MinusNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Minus, "-") };
    }

    ExpressionNode DivNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Div, "/") };
    }

    private ExpressionNode CreateOperationNode(TokenType type)
    {
        return type switch
        {
            TokenType.Mult => MultNode(),
            TokenType.Plus => PlusNode(),
            TokenType.Minus => MinusNode(),
            TokenType.Div => DivNode(),
            _ => throw new Exception("Unexpected operation")
        };
    }

    private static int BalancedTreeHeight(int numberOfNodes)
    {
        return (int)(Math.Log2(numberOfNodes) + 1);
    }
    
    // ExpressionNode BalancePlus(ExpressionNode tree)
    // {
    //     
    // }
}