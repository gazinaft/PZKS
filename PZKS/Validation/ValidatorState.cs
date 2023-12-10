namespace PZKS.Validation;

public abstract class ValidatorState
{
    protected abstract bool Validate(List<Token> tokens);

    public bool ValidateExpression(List<Token> tokens)
    {
        if (NextState == null)
        {
            return Validate(tokens);
        }
        var thisVal = Validate(tokens);
        var nextVal = NextState.ValidateExpression(tokens);
        
        return thisVal && nextVal;
    }

    protected void ReportError(string message, Token? token = null)
    {
        Util.ReportError(message + " " + token?.ToString());
    }
    
    public ValidatorState? NextState { get; set; }
}