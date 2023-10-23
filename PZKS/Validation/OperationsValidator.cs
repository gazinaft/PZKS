namespace PZKS.Validation;


// 	помилки в середині виразу (подвійні операції, відсутність операцій перед або між дужками, операції* або / після відкритої дужки тощо);
public class OperationsValidator : ValidatorState
{
    private static readonly List<TokenType> Operation = new()
        { TokenType.Div, TokenType.Mult, TokenType.Minus, TokenType.Plus };
    
    private static readonly List<TokenType> InParentOperation = new()
        { TokenType.Div, TokenType.Mult };
    

    protected override bool Validate(List<Token> tokens)
    {
        return ValidateDoubleOperations(tokens) &&
               ValidateBeforeParentheses(tokens) &&
               ValidateInParentheses(tokens) &&
               ClosingParentheses(tokens);
    }
    
    private bool ValidateDoubleOperations(List<Token> tokens)
    {
        var success = true;
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (Operation.Contains(tokens[i].TokenType)
                && Operation.Contains(tokens[i + 1].TokenType))
            {
                ReportError("Double operations:" + tokens[i]);
                success = false;
            }
        }

        return success;
    }

    private bool ValidateBeforeParentheses(List<Token> tokens)
    {
        var success = true;

        for (int i = 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent
                && (!Operation.Contains(tokens[i - 1].TokenType)
                    && tokens[i - 1].TokenType != TokenType.Variable
                    && tokens[i - 1].TokenType != TokenType.LeftParent))
            {
                ReportError("Missing an operation symbol before parentheses:", tokens[i]);
                success = false;

            }
        }

        return success;
    }

    private bool ValidateInParentheses(List<Token> tokens)
    {
        var success = true;

        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent
                && InParentOperation.Contains(tokens[i + 1].TokenType))
            {
                ReportError("Invalid operation after opening parentheses:", tokens[i]);
                success = false;
            }
        }

        return success;
    }

    private bool ClosingParentheses(List<Token> tokens)
    {
        bool success = true;
        for (int i = 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.RightParent
                && InParentOperation.Contains(tokens[i - 1].TokenType))
            {
                ReportError("Invalid operation before closing parentheses:", tokens[i]);
                success = false;
            }
        }

        return success;
    }

}