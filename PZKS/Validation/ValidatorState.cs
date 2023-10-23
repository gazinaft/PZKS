namespace PZKS.Validation;

public abstract class ValidatorState
{
    protected abstract bool Validate(List<Token> tokens);

    public bool ValidateExpression(List<Token> tokens)
    {
        if (NextState == null)
        {
            return this.Validate(tokens);
        }
        return this.Validate(tokens) && NextState.ValidateExpression(tokens);
    }

    protected void ExecuteNextState(List<Token> tokens)
    {
        NextState?.Validate(tokens);
    }

    protected virtual void ReportError(string message, Token? token = null)
    {
        Console.WriteLine(message + " " + token?.ToString());
    }
    
    public ValidatorState? NextState { get; set; }
}