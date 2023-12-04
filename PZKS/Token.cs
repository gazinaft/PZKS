namespace PZKS;

public class Token
{
    public static Token InvalidToken = new Token(TokenType.Invalid, "", null, 0);
    
    public readonly TokenType TokenType;
    public readonly String Lexeme;
    public object? Literal;
    public readonly int Index;

    public Token(TokenType tokenType, string lexeme, object? literal = null, int index = 0)
    {
        TokenType = tokenType;
        Lexeme = lexeme;
        Literal = literal;
        Index = index;
    }

    public override string ToString()
    {
        return TokenType + " " + Lexeme  + " index: " + Index;
    }

    public bool IsLowPrioOperation()
    {
        return TokenType is TokenType.Minus or TokenType.Plus;
    }
    
    public bool IsHighPrioOperation()
    {
        return TokenType is TokenType.Mult or TokenType.Div;
    }

    public bool IsOperation()
    {
        return IsLowPrioOperation() || IsHighPrioOperation();
    }

    public bool IsValue()
    {
        return TokenType is TokenType.Number or TokenType.Variable;
    }
}