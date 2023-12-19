using PZKS.Parser;

namespace PZKS.MatrixSystem;

public class ExecutionBlock
{
    public SysState State;
    public ExpressionNode? Node;

    public ExecutionBlock DeepCopy()
    {
        var result = new ExecutionBlock();
        result.State = State;
        result.Node = Node;
        return result;
    }
}