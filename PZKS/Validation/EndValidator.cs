namespace PZKS.Validation;

// 	помилки у кінці виразу (наприклад, вираз не може закінчуватись будь-якою алгебраїчною операцією);
public class EndValidator : ValidatorState
{
    private static readonly List<TokenType> ForbiddenSymbols = new()
        { TokenType.Div, TokenType.Minus, TokenType.Mult, TokenType.Plus };
    
    public EndValidator(ValidatorStateMachine stateMachine) : base(stateMachine) { }

    public override void Validate(List<Token> tokens)
    {
        var lastToken = tokens[^1];
        if (ForbiddenSymbols.Contains(lastToken.TokenType))
        {
            ReportError("Algebraic operation at the end of the expression", lastToken);
        }
        ExecuteNextState(tokens);
    }

}