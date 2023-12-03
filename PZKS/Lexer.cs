namespace PZKS;

public class Lexer
{
    private int current = 0;
    private int start = 0;
    private List<Token> _tokens = new List<Token>();
    private int _stringLen = 0;
    
    public List<Token> Scan(string expression)
    {
        current = 0;
        _tokens = new List<Token>();
        _stringLen = expression.Length;
        start = 0;
        while (!IsAtEnd())
        {
            ScanToken(expression);
            start = current;
        }
        
        return _tokens;
    }

    private void ScanToken(string expression)
    {
        var c = Advance(expression);

        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParent, "(");
                break;
            case ')':
                AddToken(TokenType.RightParent, ")");
                break;
            case '/':
                AddToken(TokenType.Div, "/");
                break;
            case '-':
                AddToken(TokenType.Minus, "-");
                break;
            case '+':
                AddToken(TokenType.Plus, "+");
                break;
            case '*':
                AddToken(TokenType.Mult, "*");
                break;
            case ',':
                AddToken(TokenType.Comma, ",");
                break;
            case ' ':
                break;
            default:
                if (IsDigit(c))
                {
                    ReadNumber(expression);
                    break;
                }

                if (IsAlpha(c))
                {
                    ReadVariable(expression);
                    break;
                }
                Util.ReportError("Invalid symbol at " + (current - 1) + expression[current -1]);
                break;
        };
        
        
    }
    
    private void AddToken(TokenType tokenType, string text, object? literal = null)
    {
        _tokens.Add(new Token(tokenType, text, literal, current));
    }
    
    private char Advance(string source)
    {
        current++;
        return source[current - 1];
    }

    private char Peek(string source)
    {
        return IsAtEnd() ? '\0' : source[current];
    }
    
    private char PeekNext(string source)
    {
        return current + 1 >= source.Length ? '\0' : source[current + 1];
    } 
    
    private bool Match(char c, string source)
    {
        if (IsAtEnd()) return false;
        if (c != source[current]) return false;

        current++;
        return true;
    }
    
    private bool IsAtEnd()
    {
        return current >= _stringLen;
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsDigit(c) || IsAlpha(c);
    }

    private void ReadNumber(string source)
    {
        while (IsDigit(Peek(source))) Advance(source);

        // Look for a fractional part.
        if (Peek(source) == '.' && IsDigit(PeekNext(source))) {
            // Consume the "."
            Advance(source);

            while (IsDigit(Peek(source))) Advance(source);
        }

        var text = source.Substring(start, current - start);
        // if (!double.TryParse(text, out var res))
        // {
        //     Console.WriteLine("Wrong format number at "  + current + " " + source[current - 1]);
        //     return;
        // }
        AddToken(TokenType.Number, text);
    }

    private void ReadVariable(string source)
    {
        while (IsAlphaNumeric(Peek(source))) Advance(source);
        
        var text = source.Substring(start, current - start);
        AddToken(TokenType.Variable, text);
    }
}