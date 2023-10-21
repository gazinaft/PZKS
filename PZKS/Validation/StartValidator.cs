namespace PZKS.Validation;

// 	помилки на початку арифметичного виразу ( наприклад, вираз не може починатись із закритої дужки, алгебраїчних операцій * та /);
public class StartValidator : ValidatorState
{
    private static readonly List<TokenType> ForbiddenSymbols = new() { TokenType.Div , TokenType.Mult, TokenType.RightParent};


    public StartValidator(ValidatorStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Validate(List<Token> tokens)
    {
        if (ForbiddenSymbols.Contains(tokens[0].TokenType))
        {
            ReportError("Invalid token at the start of the expression", tokens[0]);
        }
        ExecuteNextState(tokens);
    }
}