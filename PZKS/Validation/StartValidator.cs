namespace PZKS.Validation;

// 	помилки на початку арифметичного виразу ( наприклад, вираз не може починатись із закритої дужки, алгебраїчних операцій * та /);
public class StartValidator : ValidatorState
{
    private static List<TokenType> _forbiddenSymbols = new List<TokenType>() { TokenType.Div , TokenType.Mult, TokenType.RightParent};


    public StartValidator(ValidatorStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Validate(List<Token> tokens)
    {
        if (_forbiddenSymbols.Contains(tokens[0].TokenType))
        {
            ReportError(tokens[0]);
        }
        ExecuteNextState(tokens);
    }
    
    public override void ReportError(Token token)
    {
        base.ReportError(token);
        Console.WriteLine("Invalid token at the start of the expression " + token.ToString());
    }
}