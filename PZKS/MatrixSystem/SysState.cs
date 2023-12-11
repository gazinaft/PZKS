namespace PZKS.MatrixSystem;

[Flags]
public enum SysState
{
    Empty = 0b000,
    Execute = 0b001,
    Write = 0b010,
    Read = 0b100,
}
