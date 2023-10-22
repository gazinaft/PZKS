namespace PZKS;

public class Token
{
    public static Token InvalidToken = new Token(TokenType.Invalid, "", null, 0);
    
    public readonly TokenType TokenType;
    public readonly String Lexeme;
    public readonly object? Literal;
    public readonly int Index;

    public Token(TokenType tokenType, string lexeme, object? literal, int index)
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
}