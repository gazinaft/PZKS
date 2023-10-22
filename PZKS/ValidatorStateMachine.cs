using PZKS.Validation;

namespace PZKS;

public class ValidatorStateMachine
{
    public ValidatorState? State { get; set; }
    public bool HasErrors { get; set; }
    
    public ValidatorState? StartState { get; set; }
    
    public void Validate(List<Token> tokens)
    {
        if (State != null)
        {
            State.Validate(tokens);
            return;
        }
        StartState?.Validate(tokens);
    }

    public void Reset()
    {
        HasErrors = false;
        State = null;
    }
}