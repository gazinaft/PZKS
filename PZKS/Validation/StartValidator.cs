namespace PZKS.Validation;

// 	помилки на початку арифметичного виразу ( наприклад, вираз не може починатись із закритої дужки, алгебраїчних операцій * та /);
public class StartValidator : ValidatorState
{
    private static readonly List<TokenType> ForbiddenSymbols = new() { TokenType.Div , TokenType.Mult, TokenType.RightParent};
    
    protected override bool Validate(List<Token> tokens)
    {
        if (!ForbiddenSymbols.Contains(tokens[0].TokenType)) return true;
        
        ReportError("Invalid token at the start of the expression", tokens[0]);
        return false;
    }
}