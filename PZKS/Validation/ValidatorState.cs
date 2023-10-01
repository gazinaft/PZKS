namespace PZKS.Validation;

public abstract class ValidatorState
{
    protected ValidatorState(ValidatorStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    public abstract void Validate(List<Token> tokens);

    protected void ExecuteNextState(List<Token> tokens)
    {
        if (NextState == null) return;
        StateMachine.State = NextState;
        StateMachine.Validate(tokens);
    }

    public virtual void ReportError(Token token)
    {
        StateMachine.HasErrors = true;
    }
    
    public ValidatorState? NextState { get; set; }
    protected ValidatorStateMachine StateMachine { get; set; }
}