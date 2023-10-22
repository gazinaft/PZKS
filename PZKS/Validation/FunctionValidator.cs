namespace PZKS.Validation;

public class FunctionValidator : ValidatorState
{
    public FunctionValidator(ValidatorStateMachine stateMachine) : base(stateMachine) { }

    public override void Validate(List<Token> tokens)
    {
        if (!tokens.Exists(x => x.TokenType == TokenType.Comma))
        {
            ExecuteNextState(tokens);
            return;
        }

        CommaInFunctionParentheses(tokens);
        ValidateDoubleComma(tokens);
        
        ExecuteNextState(tokens);
    }

    private void CommaInFunctionParentheses(List<Token> tokens)
    {
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
        
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType != TokenType.Comma) continue;
            
            var commaInFunction = functionScopes.Any(x => x.Item1 < i && x.Item2 > i);
            if (!commaInFunction) ReportError("Comma out of function scope" + tokens[i]);
        }
    }

    private void ValidateDoubleComma(List<Token> tokens)
    {
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.Comma
                && tokens[i + 1].TokenType == TokenType.Comma)
            {
                ReportError("Double comma:" + tokens[i]);
            }
        }
    }
    
    private void ClosingParentheses(List<Token> tokens)
    {
        for (int i = 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.RightParent
                && tokens[i - 1].TokenType == TokenType.Comma)
            {
                ReportError("Invalid comma before closing parentheses:", tokens[i]);
            }
        }
    }
    
}