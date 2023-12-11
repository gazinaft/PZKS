namespace PZKS.Parser;

public class ExpressionNode
{
    public List<ExpressionNode> Children = new List<ExpressionNode>();
    public Token NodeToken;
    public ExpressionNode? Parent;
    private bool _isCalculated;
    public int CalculatedSource = 0;
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

    public bool IsHigherOrder()
    {
        return NodeToken.IsHighPrioOperation();
    }
    
    public bool IsLowerOrder()
    {
        return NodeToken.IsLowPrioOperation();
    }

    public override string ToString()
    { 
        return NodeToken.Lexeme;
    }
    
    public bool IsCalculated
    {
        get
        {
            if (IsLeaf())
            {
                _isCalculated = true;
                CalculatedSource = 0b11;
            }

            return _isCalculated;
        }
    }

    public void Calculate(int source)
    {
        CalculatedSource = source;
        _isCalculated = true;
    }

    public bool CanBeCalculated()
    {
        return Children.All(x => x.IsCalculated);
    }
}