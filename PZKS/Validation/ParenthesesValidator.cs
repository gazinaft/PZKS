namespace PZKS.Validation;

// помилки, пов’язані з використанням дужок ( нерівна кількість відкритих та закритих дужок, неправильний порядок дужок, пусті дужки)

public class ParenthesesValidator : ValidatorState
{
    public ParenthesesValidator(ValidatorStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Validate(List<Token> tokens)
    {
        EmptyParenthesesCheck(tokens);
        OrderAndCountParenthesesCheck(tokens);
        
        ExecuteNextState(tokens);
    }

    private void EmptyParenthesesCheck(List<Token> tokens)
    {
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent
                && tokens[i + 1].TokenType == TokenType.RightParent)
            {
                ReportError("Parentheses are empty:", tokens[i]);
            } 
        }
    }

    private void OrderAndCountParenthesesCheck(List<Token> tokens)
    {
        var openBrackets = 0;

        foreach (var t in tokens)
        {
            if (t.TokenType == TokenType.LeftParent)
            {
                openBrackets += 1;
            }
            if (t.TokenType == TokenType.RightParent)
            {
                openBrackets -= 1;
            }
            if (openBrackets < 0)
            {
                ReportError("Closing parentheses before opening parentheses", t);
            }
        }

        if (openBrackets != 0)
        {
            ReportError($"Not all parentheses are closed, you need to close {openBrackets} parentheses");
        }
    }
}