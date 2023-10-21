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

    protected virtual void ReportError(string message, Token? token = null)
    {
        StateMachine.HasErrors = true;
        Console.WriteLine(message + " " + token?.ToString());
    }
    
    public ValidatorState? NextState { get; set; }
    protected ValidatorStateMachine StateMachine { get; set; }
}