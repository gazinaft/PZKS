using System.Text;
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

    private ExpressionNode _tree;
    private Dictionary<int, List<ExecutionBlock>> MatrixHosts;
    private int _currentPointer;
    public string Label { get; set; }

    public bool IsTerminal() => _tree.IsCalculated;

    public ExpressionNode GetTree() => _tree;
    
    public void Reset()
    {
        var list1 = new List<ExecutionBlock>();
        var list2 = new List<ExecutionBlock>();
        for (int i = 0; i < 1000; i++)
        {
            list1.Add(new ExecutionBlock{State = SysState.Empty});
            list2.Add(new ExecutionBlock{State = SysState.Empty});
        }
        MatrixHosts = new Dictionary<int, List<ExecutionBlock>>
        {
            { 1, list1 },
            { 2, list2 }
        };
        _currentPointer = 0;
    }

    public MatrixSystem(ExpressionNode tree)
    {
        _tree = tree;
        Reset();
    }
    
    private static int OtherHost(int host) => host == 1 ? 2 : 1;

    private int FirstEmptyPositionAtHost(int host)
    {
        for (int i = MatrixHosts[host].Count - 1; i >= 0; i--)
        {
            if (MatrixHosts[host][i].State != SysState.Empty)
            {
                return i + 1;
            }
        }

        return 0;
    }
    
    private int UpdatePointer()
    {
        _currentPointer = Math.Max(FirstEmptyPositionAtHost(1), FirstEmptyPositionAtHost(2));
        return _currentPointer;
    }
    
    public float GetEffectivityCoef(int host)
    {
        if (!MatrixHosts.ContainsKey(host)) return 0;
        
        float total = MatrixHosts[host].Count;
        float working = MatrixHosts[host].Count(x => (x.State & SysState.Execute) > 0);
        return working / total;
    }

    private int GetHostWorkingTime(int index)
    {
        return MatrixHosts[index].Count(x => (x.State & SysState.Execute) > 0);
    }

    public float GetAcceleration()
    {
        float sumLinearTime = MatrixHosts.Keys.Sum(GetHostWorkingTime);
        float parallelTime = FirstEmptyPositionAtHost(1);
        return sumLinearTime / parallelTime;
    }

    public void Calculate()
    {
        while (!IsTerminal())
        {
            var nodes = GetNodesCanBeCalculated(ref _tree);
            var largest = CalcBestExpressionNodes(nodes);
        }
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

    private List<ExpressionNode> CalcBestExpressionNodes(List<ExpressionNode> expressionNodes) // for one does all the job
    {
        var result = new List<ExpressionNode>();
        if (expressionNodes.Count == 0) return result;
        var grouped = expressionNodes.GroupBy(x => x.NodeToken.TokenType);
        var largestGroup = grouped
            .OrderByDescending(g => g.Count())
            .First().ToList();
        if (largestGroup.Count == 1)
        {
            var node = largestGroup[0];
            var minHost = MinHostByOperationCost(node);
            int cost = CostOfOperation(node, minHost);
            FillSendReceive(minHost, cost);
            _currentPointer += cost;
            FillExecutionForOne(node, minHost);
            var executionCost = OperationLength[node.NodeToken.TokenType];
            _currentPointer += executionCost;

            return largestGroup;
        }

        var bestI = 0;
        var bestJ = 1;
        float bestCost = 100000;
        for (int i = 0; i < largestGroup.Count; i++)
        {
            for (int j = 0; j < largestGroup.Count; j++)
            {
                if (i == j) continue;
                var node1 = largestGroup[i];
                var node2 = largestGroup[j];
                var newCost = CostOfOperations(node1, node2);
                if (newCost < bestCost)
                {
                    bestCost = newCost;
                    bestI = i;
                    bestJ = j;
                }
            }
        }
        FillExecutionForMany(largestGroup[bestI], largestGroup[bestJ]);
        
        return largestGroup;
    }
    // private void GraphSearch(List<ExpressionNode> expressionNodes)
    // {
    //     if (expressionNodes.Count == 0) return;
    //     var grouped = expressionNodes.GroupBy(x => x.NodeToken.TokenType);
    //     foreach (var tokenGroup in grouped)
    //     {
    //         var nodesInGroup = tokenGroup.ToList();
    //         if (nodesInGroup.Count == 1)
    //         {
    //             var node = nodesInGroup[0];
    //
    //             for (int i = 1; i <= 2; i++)
    //             {
    //                 var system1 = DeepCopy();
    //                 int cost1 = CostOfOperation(node, i);
    //                 system1.FillSendReceive(i, cost1);
    //                 system1._currentPointer += cost1;
    //                 system1.FillExecutionForOne(node, i);
    //                 system1._currentPointer += OperationLength[node.NodeToken.TokenType];
    //                 system1.GraphSearch(system1.GetNodesCanBeCalculated(ref system1._tree));
    //             }
    //         }
    //     }
    //     _searchOptimalMatrixSystems.Remove(this);
    //
    // }
    private float CostOfOperations(ExpressionNode node1, ExpressionNode node2)
    {
        return CostOfOperation(node1, 1) + CostOfOperation(node2, 2) - (node1.Parent == node2.Parent ? 0.5f : 0f);
    }

    private static int CostOfOperation(ExpressionNode node, int host)
    {
        return node.Children.Count(x => (x.CalculatedSource & host) == 0);
    }

    private static int MinHostByOperationCost(ExpressionNode node)
    {
        return CostOfOperation(node, 1) > CostOfOperation(node, 2) ? 2 : 1;
    }

    // doesn't move ptr
    private void FillSendReceive(int hostToReceive, int costOfOperation)
    {
        for (int i = 0; i < costOfOperation; i++)
        {
            MatrixHosts[hostToReceive][_currentPointer + i].State = SysState.Read;
            MatrixHosts[OtherHost(hostToReceive)][_currentPointer + i].State |= SysState.Write;
        }
    }

    // doesn't move ptr
    private void FillExecutionForOne(ExpressionNode node, int host)
    {
        var counter = OperationLength[node.NodeToken.TokenType];
        for (int i = 0; i < counter; i++)
        {
            MatrixHosts[host][_currentPointer + i].State |= SysState.Execute;
            MatrixHosts[host][_currentPointer + i].NodeString = node.ToString();
        }
        node.Calculate(host);
    }

    private void FillExecutionForMany(ExpressionNode node1, ExpressionNode node2)
    {
        var cost1 = CostOfOperation(node1, 1);
        var cost2 = CostOfOperation(node2, 2);
        var executionCost = OperationLength[node1.NodeToken.TokenType];

        if (cost1 >= cost2) // send from 1 to 2 first
        {
            FillSendReceive(1, cost1); // send from 2 to 1. 1 can start after this
            _currentPointer += cost1;
            FillExecutionForOne(node1, 1); // execute 1
            FillSendReceive(2, cost2); // send from 1 to 2
            _currentPointer += cost2;
            FillExecutionForOne(node2, 2); // execute 2
            _currentPointer += executionCost;
        }
        else // send from 2 to 1 first
        {
            FillSendReceive(2, cost2); // send from 1 to 2. 2 can start after this. if no delay, start immediately
            _currentPointer += cost2;
            FillExecutionForOne(node2, 2); // execute 2
            FillSendReceive(1, cost1); // send from 2 to 1
            _currentPointer += cost1;
            FillExecutionForOne(node1, 1); // execute 1
            _currentPointer += executionCost;
        }
    }

    private MatrixSystem DeepCopy()
    {
        var result = new MatrixSystem(_tree.DeepCopy());
        result.MatrixHosts[1] = MatrixHosts[1].Select(x => x.DeepCopy()).ToList();
        result.MatrixHosts[2] = MatrixHosts[2].Select(x => x.DeepCopy()).ToList();
        
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _currentPointer; i++)
        {
            sb.Append(MatrixHosts[1][i].ToString()).Append("|").Append(MatrixHosts[2][i].ToString()).Append('\n');
        }
        return sb.ToString();
    }

    public int GetExecutionTime()
    {
        return _currentPointer;
    }
}