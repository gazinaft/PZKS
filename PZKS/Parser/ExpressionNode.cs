namespace PZKS.Parser;

public class ExpressionNode
{
    public List<ExpressionNode> Children = new List<ExpressionNode>();
    public Token NodeToken;

    public bool IsLeaf()
    {
        return Children.Count == 0;
    }

    public bool IsFunction()
    {
        return NodeToken.TokenType == TokenType.Variable && !IsLeaf();
    }

    public bool IsOperation()
    {
        return NodeToken.IsOperation();
    }
}