using PZKS.Parser;

namespace PZKS.MatrixSystem;

public class MatrixSystem
{
    // get optimal operation nodes
    // execute nodes
    // repeat until root is executed
    
    private static readonly Dictionary<TokenType, int> OperationLength = new()
    {
        { TokenType.Plus, 2 },
        { TokenType.Minus, 3 },
        { TokenType.Mult, 4 },
        { TokenType.Div, 8 },
        { TokenType.Variable, 10 }
    };

    private Dictionary<int, List<ExecutionBlock>> MatrixHosts;
    private int _currentPointer;

    public void Reset()
    {
        MatrixHosts = new Dictionary<int, List<ExecutionBlock>>
        {
            { 1, Enumerable.Repeat(new ExecutionBlock{ State = SysState.Empty }, 1000).ToList() },
            { 2, Enumerable.Repeat(new ExecutionBlock{ State = SysState.Empty }, 1000).ToList() }
        };
        _currentPointer = 0;
    }

    public MatrixSystem()
    {
        Reset();
    }

    public void CostToCalculate(ExpressionNode node)
    {
        
    }
    
    // Calculate
    public void ExecuteExpressionNode(ExpressionNode node1, int host)
    {
        node1.Calculate(host);
    }

    public void WriteOperationsToSystem(ExpressionNode? node1, ExpressionNode? node2)
    {
        if (node1 != null)
        {
            var cyclesToTransmit1 = CostOfOperation(node1, 1);
            
        }
    }

    private void WriteOneOperationToSystem(ExpressionNode node, int host)
    {
        UpdatePointer();
        var cyclesToTransmit = CostOfOperation(node, host);
        
    }

    private static int OtherHost(int host) => host == 1 ? 2 : 1;

    private void ReadDataFromHost(int sourceHost)
    {
        
    }

    private int LastValidPositionAtHost(int host)
    {
        var counter = 0;
        for (int i = MatrixHosts[host].Count - 1; i >= 0; i--)
        {
            if (MatrixHosts[host][i].State != SysState.Empty)
            {
                counter = i;
                break;
            }
        }

        return counter;
    }
    
    private int UpdatePointer()
    {
        var len1 = MatrixHosts[1].Count;
        var len2 = MatrixHosts[2].Count;
        if (len1 == len2)
        {
            _currentPointer = len1;
            return _currentPointer;
        }

        int maxLen = len1 > len2 ? len1 : len2;
        int delta = Math.Abs(len1 - len2);
        int minHost = len1 > len2 ? 2 : 1;

        _currentPointer = maxLen;
        return _currentPointer;
    }
    
    public float GetEffectivityCoef(int index)
    {
        if (!MatrixHosts.ContainsKey(index)) return 0;
        
        float total = MatrixHosts[index].Count;
        float working = MatrixHosts[index].Count(x => (x.State & SysState.Execute) > 0);
        return working / total;
    }

    private int GetHostWorkingTime(int index)
    {
        return MatrixHosts[index].Count(x => (x.State & SysState.Execute) > 0);
    }

    public float GetAcceleration()
    {
        float sumLinearTime = MatrixHosts.Keys.Sum(GetHostWorkingTime);
        float parallelTime = MatrixHosts[1].Count;
        return sumLinearTime / parallelTime;
    }
    
    
    public List<ExpressionNode> GetNodesCanBeCalculated(ref ExpressionNode node)
    {
        var result = new List<ExpressionNode>();
        if (node.CanBeCalculated())
        {
            if (node.IsCalculated)
            {
                return result;
            }
            result.Add(node);
            return result;
        }

        for (var index = 0; index < node.Children.Count; index++)
        {
            var expressionNode = node.Children[index];
            if (expressionNode.CanBeCalculated() && !expressionNode.IsCalculated)
            {
                result.Add(expressionNode);
            }
            else
            {
                result.AddRange(GetNodesCanBeCalculated(ref expressionNode));
            }
        }

        return result;
    }

    private List<ExpressionNode> GetBestExpressionNodes(List<ExpressionNode> expressionNodes)
    {
        var result = new List<ExpressionNode>();
        if (expressionNodes.Count == 0) return result;
        var grouped = expressionNodes.GroupBy(x => x.NodeToken.TokenType);
        var largestGroup = grouped.OrderByDescending(g => g.Count()).First().ToList();
        if (largestGroup.Count == 1)
        {
            
        }
    }
    
    private float CostOfOperations(ExpressionNode node1, ExpressionNode node2)
    {
        return CostOfOperation(node1, 1) + CostOfOperation(node2, 2) - (node1.Parent == node2.Parent ? 0.5f : 0f);
    }

    private float CostOfOperation(ExpressionNode node, int host)
    {
        var NonCompatibleDependencies = node.Children.Count(x => (x.CalculatedSource & host) == 0);
        return NonCompatibleDependencies;
    }
}