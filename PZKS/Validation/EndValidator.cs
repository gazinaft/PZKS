namespace PZKS.Validation;

// 	помилки у кінці виразу (наприклад, вираз не може закінчуватись будь-якою алгебраїчною операцією);
public class EndValidator : ValidatorState
{
    private static readonly List<TokenType> ForbiddenSymbols = new()
        { TokenType.Div, TokenType.Minus, TokenType.Mult, TokenType.Plus };
    

    protected override bool Validate(List<Token> tokens)
    {
        bool success = true;
        if (tokens.Count == 0) return false;
        var lastToken = tokens[^1];
        if (ForbiddenSymbols.Contains(lastToken.TokenType))
        {
            ReportError("Algebraic operation at the end of the expression", lastToken);
            success = false;
        }

        return success;
    }

}