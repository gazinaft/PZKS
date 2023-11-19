namespace PZKS.Validation;

// помилки, пов’язані з використанням дужок ( нерівна кількість відкритих та закритих дужок, неправильний порядок дужок, пусті дужки)

public class ParenthesesValidator : ValidatorState
{

    protected override bool Validate(List<Token> tokens)
    {
        var test1 = EmptyParenthesesCheck(tokens);
        var test2 = OrderAndCountParenthesesCheck(tokens);
        return test1 && test2;
    }

    private bool EmptyParenthesesCheck(List<Token> tokens)
    {
        var success = true;

        for (var i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent
                && tokens[i + 1].TokenType == TokenType.RightParent)
            {
                ReportError("Parentheses are empty:", tokens[i]);
                success = false;
            } 
        }

        return success;
    }

    private bool OrderAndCountParenthesesCheck(List<Token> tokens)
    {
        var success = true;
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
                success = false;
            }
        }

        if (openBrackets > 0)
        {
            ReportError($"Not all parentheses are closed, you need to close {openBrackets} parentheses");
            success = false;
        }

        if (openBrackets < 0)
        {
            ReportError($"Not all parentheses have opening, there are {-openBrackets} closing parentheses without opening");
            success = false;
        }

        return success;
    }
}