using PZKS.Parser;

namespace PZKS.MatrixSystem;

public class ExecutionBlock
{
    public SysState State;
    public string? NodeString;

    public ExecutionBlock DeepCopy()
    {
        var result = new ExecutionBlock();
        result.State = State;
        result.NodeString = NodeString;
        return result;
    }

    public override string ToString()
    {
        if ((State & SysState.Write) != 0 && (State & SysState.Execute) != 0)
        {
            return "E " + NodeString?.ToString() + " S";
        }

        return State switch
        {
            SysState.Execute => "E " + NodeString?.ToString(),
            SysState.Read => "R",
            SysState.Write => "S",
            SysState.Empty => "0",
            _ => "err"
        };
    }
}