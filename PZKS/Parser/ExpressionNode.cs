namespace PZKS.Parser;

public class ExpressionNode
{
    public List<ExpressionNode> Children = new List<ExpressionNode>();
    public Token NodeToken;
    public ExpressionNode? Parent;
    private bool _isCalculated;
    public int CalculatedSource = 0;
    private static int _currentNumber = 0;
    public string Name;

    public ExpressionNode()
    {
        Name = "P[" + ++_currentNumber + "]";
    }
    
    public bool IsLeaf()
    {
        return Children.Count == 0;
    }

    public ExpressionNode DeepCopy()
    {
        var result = new ExpressionNode();
        result.NodeToken = NodeToken;
        result._isCalculated = _isCalculated;
        result.CalculatedSource = CalculatedSource;
        result.Children = Children.Select(x => x.DeepCopy()).ToList();
        foreach (var resultChild in result.Children)
        {
            resultChild.Parent = result;
        }
        
        return result;
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
        return Name + " " + NodeToken.Lexeme;
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