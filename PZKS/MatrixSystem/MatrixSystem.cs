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
            { 1, new List<ExecutionBlock>() },
            { 2, new List<ExecutionBlock>() }
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
    public void ExecuteExpressionNodes(ExpressionNode? node1, ExpressionNode? node2)
    {
        if (node1 != null)
        {
            node1.Calculate(1);
        }
        if (node2 != null)
        {
            node2.Calculate(2);
        }
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
    
    
}