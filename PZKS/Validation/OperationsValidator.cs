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
        var test1 = ValidateDoubleOperations(tokens);
        // var test2 = ValidateBeforeParentheses(tokens);
        var test3 = ValidateInParentheses(tokens); 
        var test4 = ClosingParentheses(tokens);
        var test5 = ValidateOperationsBetweenVariables(tokens);
        return test1 && test3 && test4 && test5;
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

    private bool ValidateOperationsBetweenVariables(List<Token> tokens)
    {
        var leftProhibited = new List<TokenType> { TokenType.Variable , TokenType.RightParent, TokenType.Number };
        var rightProhibited = new List<TokenType> { TokenType.Variable , TokenType.Number };
        var res = true;
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            var lToken = tokens[i].TokenType;
            var rToken = tokens[i + 1].TokenType;
            if (leftProhibited.Contains(lToken) && rightProhibited.Contains(rToken)
                || (rToken == TokenType.LeftParent && lToken is TokenType.Number or TokenType.RightParent))
            {
                ReportError("No operation between " + tokens[i] + " " + tokens[i+1]);
                res = false;
            }
        }

        return res;
    }
    
}