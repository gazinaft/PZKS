namespace PZKS.Validation;

public class FunctionValidator : ValidatorState
{
    protected override bool Validate(List<Token> tokens)
    {
        if (!tokens.Exists(x => x.TokenType == TokenType.Comma))
        {
            return true;
        }
        var functionScopes = new List<(int, int)>();
        
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType != TokenType.Variable
                || tokens[i + 1].TokenType != TokenType.LeftParent) continue;

            var endPar = Util.GetClosingPar(tokens, i + 1);
            if (i + 2 == tokens.Count || endPar == 0)
            {
                ReportError("Close the function", tokens[i]);
                break;
            }
            
            functionScopes.Add((i + 1, endPar));
        }

        var test1 = CommaInFunctionParentheses(tokens, functionScopes);
        var test2 = ValidateDoubleComma(tokens);
        return test1 && test2;
    }

    private bool ValidateSubexpressions()
    {
        return true;
    }
    
    private bool CommaInFunctionParentheses(List<Token> tokens, List<(int, int)> functionScopes)
    {
        var success = true;
        
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType != TokenType.Comma) continue;
            
            var commaInFunction = functionScopes.Any(x => x.Item1 < i && x.Item2 > i);
            if (!commaInFunction)
            {
                ReportError("Comma out of function scope" + tokens[i]);
                success = false;
            }
            
        }

        return success;
    }

    private bool ValidateDoubleComma(List<Token> tokens)
    {
        var success = true;
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.Comma
                && tokens[i + 1].TokenType == TokenType.Comma)
            {
                ReportError("Double comma:" + tokens[i]);
                success = false;
            }
        }

        return success;
    }
    
    private bool ClosingParentheses(List<Token> tokens)
    {
        var success = true;

        for (int i = 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.RightParent
                && tokens[i - 1].TokenType == TokenType.Comma)
            {
                ReportError("Invalid comma before closing parentheses:", tokens[i]);
                success = false;

            }
        }

        return success;
    }
    
}