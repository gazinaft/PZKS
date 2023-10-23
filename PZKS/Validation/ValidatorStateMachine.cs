using PZKS.Validation;

namespace PZKS;

public class ValidatorStateMachine
{
    public ValidatorState? State { get; set; }
    public bool HasErrors { get; set; }
    
    public ValidatorState? StartState { get; set; }

    private void ValidateRecursive(List<Token> tokens)
    {
        var validator = new ValidatorStateMachine
        {
            StartState = StartState
        };
        
        validator.Validate(tokens);
    }
    
    public void Validate(List<Token> tokens)
    {
        if (State != null)
        {
            State.ValidateExpression(tokens);
            return;
        }
        StartState?.ValidateExpression(tokens);
    }

    public void Reset()
    {
        HasErrors = false;
        State = null;
    }
}