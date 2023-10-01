using PZKS.Validation;

namespace PZKS;

public class ValidatorStateMachine
{
    public ValidatorState State { get; set; }
    public bool HasErrors { get; set; }
    
    public void Validate(List<Token> tokens)
    {
        State.Validate(tokens);
    }
}