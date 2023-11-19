using System.Text;

namespace PZKS;

public class Util
{
    public static int GetClosingPar(List<Token> tokens, int start = 0)
    {
        var openCount = 0;
        for (int i = start + 1; i < tokens.Count; i++)
        {
            if (tokens[i].TokenType == TokenType.LeftParent)
            {
                openCount += 1;
            }

            if (tokens[i].TokenType != TokenType.RightParent) continue;
            
            if (openCount == 0)
            {
                return i;
            }

            openCount -= 1;
        }

        return 0;
    }

    public static string StringifyTokens(List<Token> tokens)
    {
        var sb = new StringBuilder();
        foreach (var token in tokens)
        {
            sb.Append(token.Lexeme);
        }

        return sb.ToString();
    }

    public static bool IsVariableFunction(List<Token> tokens, int index)
    {
        return tokens.Count > index + 1 && tokens[index + 1].TokenType == TokenType.LeftParent;
    }

    public static List<int> GetCommasInScope(List<Token> tokens, int start, int end)
    {
        var result = new List<int>();
        for (int i = start; i < end; i++)
        {
            if (tokens[i].TokenType == TokenType.Comma)
            {
                result.Add(i);
            }
        }
        return result;
    }

    public static List<(int, int)> GetSubexpressionIndices(List<Token> tokens)
    {
        List<(int, int)> result = new();
        for (var i = 0; i < tokens.Count; i++)
        {
            bool isFunction = false;
            if (tokens[i].TokenType == TokenType.Variable && IsVariableFunction(tokens, i))
            {
                isFunction = true;
                i++;
            }
            
            if (tokens[i].TokenType != TokenType.LeftParent)
            {
                continue;
            }
            var closingParIdx = GetClosingPar(tokens, i);
            if (closingParIdx == 0)
            {
                continue;
            }

            if (!isFunction)
            {
                result.Add((i + 1, closingParIdx - 1));
                continue;
            }

            var comas = GetCommasInScope(tokens, i, closingParIdx);
            if (comas.Count == 0)
            {
                result.Add((i + 1, closingParIdx - 1));
                continue;
            }

            var startIdx = i + 1;
            foreach (var coma in comas)
            {
                result.Add((startIdx, coma - 1));
                startIdx = coma + 1;
            }
        }

        return result;
    }

    public static List<List<Token>> GetSubexpressions(List<Token> tokens)
    {
        List<List<Token>> result = new();
        var indices = GetSubexpressionIndices(tokens);
        foreach (var (start, end) in indices)
        {
            var expr = new List<Token>();
            for (int i = start; i <= end; i++)
            {
                expr.Add(tokens[i]);
            }
            result.Add(expr);
        }
        return result;
    }

    private static HashSet<string> _errors = new HashSet<string>();
    public static void ReportError(string error)
    {
        _errors.Add(error);
    }

    public static bool LogErrors()
    {
        foreach (var error in _errors)
        {
            Console.WriteLine(error);
        }

        var hasErrors = _errors.Count > 0;
        _errors.Clear();
        return hasErrors;
    }

    // public List<Token> TransformSubexpression(List<Token> tokens)
    // {
    //     List<Token> result = new ();
    //     for (var i = 0; i < tokens.Count; i++)
    //     {
    //         bool isFunction = false;
    //         if (tokens[i].TokenType == TokenType.Variable && IsVariableFunction(tokens, i))
    //         {
    //             isFunction = true;
    //             i++;
    //         }
    //             
    //         if (tokens[i].TokenType != TokenType.LeftParent)
    //         {
    //             result.Add(tokens[i]);
    //             continue;
    //         }
    //
    //         var closingParIdx = GetClosingPar(tokens, i);
    //         if (closingParIdx == 0)
    //         {
    //             if (isFunction)
    //             {
    //                 result.Add(tokens[i - 1]);
    //             }
    //             result.Add(tokens[i]);
    //             continue;
    //         }
    //
    //         var nestedTokens = new List<List<Token>>();
    //         var commaIndexes = ;
    //         for (var j = i; j < closingParIdx; j++)
    //         {
    //             nestedTokens.Add(tokens[j]);
    //         }
    //
    //         var transformedTokens = TransformSubexpression(nestedTokens);
    //         result.Add(new Token(TokenType.Variable, StringifyTokens(transformedTokens), transformedTokens, i));
    //         
    //         i = closingParIdx;
    //     }
    //
    //     return result;
    // }
}