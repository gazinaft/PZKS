using PZKS.Validation;

namespace PZKS;

public class ValidatorStateMachine
{
    public ValidatorState? State { get; set; }
    public bool HasErrors { get; set; }
    
    public ValidatorState? StartState { get; set; }

    private void ValidateRecursive(List<Token> tokens)
    {
        var validator = new ValidatorStateMachine { StartState = StartState };
        
        validator.Validate(tokens);
    }
    
    public bool Validate(List<Token> tokens)
    {
        var subexpressions = Util.GetSubexpressions(tokens);
        foreach (var subexpression in subexpressions)
        {
            Validate(subexpression);
        }
        if (State != null)
        {
            return State.ValidateExpression(tokens);;
        }
        return StartState?.ValidateExpression(tokens) ?? false;
    }

    public void Reset()
    {
        HasErrors = false;
        State = null;
    }
}