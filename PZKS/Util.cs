using System.Text;

namespace PZKS;

public static class Util
{
    private static HashSet<string> _errors = new HashSet<string>();

    
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

    public static int GetOpeningPar(List<Token> tokens, int end = 0)
    {
        var openCount = 0;
        for (int i = end - 1; i > 0; i--)
        {
            if (tokens[i].TokenType == TokenType.RightParent)
            {
                openCount += 1;
            }

            if (tokens[i].TokenType != TokenType.LeftParent) continue;
            
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

    public static List<(int, int)> GetCommaSeparatedSubexpressionIndices(List<Token> tokens, int start, int end)
    {
        var result = new List<(int, int)>();
        if (end - start == 1)
        {
            return result;
        }
        var startSubexpression = start;
        for (int i = start; i < end; i++)
        {
            if (IsFunction(tokens, i) && tokens[i].TokenType == TokenType.Variable)
            {
                i = GetClosingPar(tokens, i + 1);
            }
            if (tokens[i].TokenType == TokenType.Comma)
            {
                result.Add((startSubexpression, i - 1));
                startSubexpression = i + 1;
            }
        }
        result.Add((startSubexpression, end));

        return result;
    }
    
    public static List<(int, int)> GetSubexpressionIndices(List<Token> tokens)
    {
        List<(int, int)> result = new();
        for (var i = 0; i < tokens.Count; i++)
        {
            bool isFunction = false;
            if (IsFunction(tokens, i) && tokens[i].TokenType == TokenType.Variable)
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

    public static void ReportError(string error)
    {
        _errors.Add(error);
    }

    public static List<string> LogErrors()
    {
        var result = _errors.ToList();
        _errors.Clear();
        return result;
    }

    public static int GetTokenWeight(List<Token> tokens, int tokenIndex)
    {
        return 0;
    }
    
    private static List<List<Token>> _distributiveForms = new();

    private static bool IsFunction(List<Token> tokens, int index)
    {
        if (tokens[index].TokenType == TokenType.RightParent)
        {
            var opening = GetOpeningPar(tokens, index);
            return opening != 0 && tokens[opening - 1].TokenType == TokenType.Variable;
        }

        if (tokens[index].TokenType == TokenType.Variable)
        {
            return index < tokens.Count - 1
                   && tokens[index + 1].TokenType == TokenType.LeftParent
                   && GetClosingPar(tokens, index + 1) != 0;
        }

        return false;
    }
    
    // return all multiply symbols, which can be expanded
    public static List<Token> OpenParentheses(List<Token> tokens)
    {
        List<Token> result = new();
        for (int i = 1; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType == TokenType.Mult)
            {
                var functionStartLeft = GetOpeningPar(tokens, i - 1) - 1;
                var functionEndRight = GetClosingPar(tokens, i + 2);
                // left variable is simple
                var isSimpleLeft = tokens[i - 1].TokenType == TokenType.Variable
                                 || tokens[i - 1].TokenType == TokenType.Number;
                var isSimpleRight = tokens[i + 1].TokenType == TokenType.Variable || tokens[i + 1].TokenType == TokenType.Number;

                if (isSimpleLeft && isSimpleRight) continue;
                
                
            }
        }

        return result;
    }

    public static List<Token> SquashFunctions(List<Token> tokens)
    {
        var result = new List<Token>();
        for (var i = 0; i < tokens.Count; i++)
        {
            // start of a function
            if (tokens[i].TokenType == TokenType.Variable
                && IsFunction(tokens, i))
            {
                List<List<Token>> subexpressions = new();
                var endParent = GetClosingPar(tokens, i + 1);
                var commaSubexpressions = GetCommaSeparatedSubexpressionIndices(tokens, i + 2, endParent - 1);
                foreach (var (startSub, endSub) in commaSubexpressions)
                {
                    subexpressions.Add(SquashFunctions(tokens.GetRange(startSub, endSub - startSub + 1)));
                }
                
                var squashedTokensToStringify = tokens.GetRange(i, endParent - i + 1); 
                var squashed = new Token(TokenType.Variable, StringifyTokens(squashedTokensToStringify), subexpressions, i);
                result.Add(squashed);
                i = endParent;
            }
            else
            {
                result.Add(tokens[i]);
                if (tokens[i].Literal != null)
                {
                    var subexpressions = (List<List<Token>>)tokens[i].Literal!;
                    var newSubexpressions = subexpressions.Select(SquashFunctions).ToList();
                    tokens[i].Literal = newSubexpressions;
                }
            }
        }

        return result;
    }

    public static List<Token> SquashParentheses(List<Token> tokens)
    {
        var result = new List<Token>();
        for (var i = 0; i < tokens.Count; i++)
        {
            // start of parentheses
            if (tokens[i].TokenType == TokenType.LeftParent)
            {
                List<List<Token>> subexpressions = new();
                var endParent = GetClosingPar(tokens, i);
                var subexpression = tokens.GetRange(i + 1, endParent - i - 1);
                subexpressions.Add(SquashParentheses(subexpression));

                var squashed = new Token(TokenType.Variable, StringifyTokens(subexpression), subexpressions, i);
                result.Add(squashed);
                i = endParent;
            }
            else
            {
                result.Add(tokens[i]);
                if (tokens[i].Literal != null)
                {
                    var subexpressions = (List<List<Token>>)tokens[i].Literal!;
                    var newSubexpressions = subexpressions.Select(SquashParentheses).ToList();
                    tokens[i].Literal = newSubexpressions;
                }
            }
        }

        return result;
    }

    public static List<Token> UnSquashFunctions(List<Token> tokens)
    {
        var result = new List<Token>();
        foreach (var token in tokens)
        {
            if (token.Literal == null)
            {
                result.Add(token);
            }
            else
            {
                result.AddRange((List<Token>)token.Literal);
            }
        }

        return result;
    }

    private static List<Token> GetFullMultiplierChain(List<Token> tokens, int indexOfMultiply)
    {
        var result = new List<Token>();
        var chainBreakingTokens = new List<TokenType>
        {
            TokenType.Minus,
            TokenType.Plus,
            TokenType.LeftParent,
        };
        // find start without sign
        var indexOfChainStart = indexOfMultiply - 1;
        for (int i = indexOfMultiply; i >= 0; i--)
        {
            if (chainBreakingTokens.Contains(tokens[i].TokenType))
            {
                break;
            }

            indexOfChainStart = i;
        }

        var isPositive = false;
        // plus
        if (indexOfChainStart == 0
            || tokens[indexOfChainStart - 1].TokenType == TokenType.Plus
            || tokens[indexOfChainStart - 1].TokenType == TokenType.LeftParent)
        {
            isPositive = true;
            // result.Add(new Token(TokenType.Plus, "+", null, indexOfChainStart));
        }
        else
        {
            isPositive = false;
            // result.Add(new Token(TokenType.Minus, "-", null, indexOfChainStart));
        }
        // go from start and find all consecutive multiplications
        
        for (int i = indexOfChainStart; i < tokens.Count; i++)
        {
            
        }

        throw new Exception();
    }
    
    private static List<(int, int)> GetMidOperationIndices(List<Token> tokens)
    {
        List<(int, int)> result = new();
        int start = 0;
        for (var i = 1; i < tokens.Count - 1; i++)
        {
            if (tokens[i].TokenType != TokenType.Minus && tokens[i].TokenType != TokenType.Plus)
            {
                continue;
            }
            result.Add((start, i - 1));
            start = i + 1;
        }
        result.Add((start, tokens.Count - 1));

        return result;
    }
    
    // multiplier takes with minus
    static List<Token> MultiplyParentheses(List<Token> multiplier, List<Token> expression)
    {
        var breaks = new List<int>();
        for (int i = 0; i < expression.Count; i++)
        {
            if (expression[i].TokenType == TokenType.Plus
                || expression[i].TokenType == TokenType.Minus)
            {
                breaks.Add(i);
            }
        }

        var result = new List<Token>();
        if (breaks.Count == 0)
        {
            result.AddRange(multiplier);
            result.Add(new Token(TokenType.Mult, "*", null, multiplier[^1].Index + 1));
            result.AddRange(expression);
            return result;
        }

        var midOperations = GetMidOperationIndices(expression);
        var invertSymbols = multiplier[0].TokenType == TokenType.Minus;
        var unsignedMultiplier = multiplier.Skip(1).ToList();
        for (int i = 0; i < breaks.Count + 1; ++i)
        {
            foreach (var (segStart, segEnd) in midOperations)
            {
                if (segStart == 0 || expression[segStart - 1].TokenType == TokenType.Plus)
                {
                    result.AddRange(multiplier);
                    result.Add(new Token(TokenType.Mult, "*", null, multiplier[^1].Index + 1));
                    result.AddRange(expression.GetRange(segStart, segEnd - segStart + 1));
                }

                if (expression[segStart - 1].TokenType == TokenType.Minus)
                {
                    result.Add(invertSymbols ?
                        new Token(TokenType.Plus, "+", null, multiplier[0].Index - 1) :
                        new Token(TokenType.Minus, "-", null, multiplier[0].Index - 1));
                    result.AddRange(unsignedMultiplier);
                    result.Add(new Token(TokenType.Mult, "*", null, multiplier[^1].Index + 1));
                    result.AddRange(expression.GetRange(segStart, segEnd - segStart + 1));
                }
            }
        }

        return result;
    }

    private static List<Token> MultiplyParentParent(List<Token> multiplier, List<Token> expression)
    {
        List<List<Token>> multipliers = new();
        var midOperations = GetMidOperationIndices(multiplier);
        foreach (var (segStart, segEnd) in midOperations)
        {
            var separatedMultiplier = new List<Token>();

            if (segStart == 0 || expression[segStart - 1].TokenType == TokenType.Plus)
            {
                separatedMultiplier.Add(new Token(TokenType.Plus, "+", null, multiplier[0].Index - 1));
            }

            if (expression[segStart - 1].TokenType == TokenType.Minus)
            {
                
            }

            var a = multipliers[0].Concat(separatedMultiplier);
            separatedMultiplier.AddRange(multiplier.GetRange(segStart, segEnd - segStart + 1));
            multipliers.Add(separatedMultiplier);
        }

        return multipliers.Aggregate((agg, x) => agg.Concat(MultiplyParentheses(x, expression)).ToList());
    }
    
    
    
    public static List<Token> TransformPreTree(List<Token> tokens)
    {
        return SquashParentheses(SquashFunctions(tokens));
    }
    
    
    
}