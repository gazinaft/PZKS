namespace PZKS.Validation;


// 	помилки в середині виразу (подвійні операції, відсутність операцій перед або між дужками, операції* або / після відкритої дужки тощо);
public class OperationsValidator : ValidatorState
{
    private static readonly List<TokenType> Operation = new()
        { TokenType.Div, TokenType.Mult, TokenType.Minus, TokenType.Plus };
    
    private static readonly List<TokenType> InParentOperation = new()
        { TokenType.Div, TokenType.Mult };
    
    public OperationsValidator(ValidatorStateMachine stateMachine) : base(stateMachine) { }

    private void ValidateDoubleOperations(List<Token> tokens)
    {
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (Operation.Contains(tokens[i].TokenType)
                && Operation.Contains(tokens[i + 1].TokenType))
            {
                ReportError("Double operations:" + tokens[i]);
            }
        }
    }

    private bool ValidateBeforeParentheses(List<Token> tokens)
    {
        return false;
    }

    private bool ValidateInParentheses(List<Token> tokens)
    {
        return false;
    }

    public override void Validate(List<Token> tokens)
    {
        ValidateDoubleOperations(tokens);
        ValidateBeforeParentheses(tokens);
        ValidateInParentheses(tokens);
        
        ExecuteNextState(tokens);
    }

}