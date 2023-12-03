namespace PZKS.Parser;

public class Parser
{
    private static Dictionary<string, int> _operationOrder = new Dictionary<string, int>
    {
        {"*", 1},
        {"/", 1},
        {"+", 2},
        {"-", 2}
    };

    private ExpressionNode CreateExpressionNode(Token token)
    {
        if (token.Literal == null)
        {
            return new ExpressionNode { NodeToken = token };
        }
        var subExpressions = (List<List<Token>>)token.Literal;
        if (subExpressions.Count == 1)
        {
            return CreateTree(subExpressions[0]);
        }

        var functionName = token.Lexeme.TakeWhile(x => x != '(').ToString();
        var rootToken = new Token(TokenType.Variable, functionName ?? "function", null, 0);
        var subtrees = subExpressions.Select(CreateTree).ToList();
        
        return new ExpressionNode { NodeToken = rootToken, Children = subtrees };
    }

    public int GetHigherPrioEnd(List<Token> tokens, int start)
    {
        for (int i = start; i < tokens.Count; i++)
        {
            if (tokens[i].IsLowPrioOperation())
            {
                return i - 1;
            }
        }

        return tokens.Count - 1;
    }

    public ExpressionNode AppendSameOrder(ref ExpressionNode tree, List<Token> tokens, int indexOfNextVar)
    {
        var root = new ExpressionNode();
        return root;
    }

    public ExpressionNode AppendHigherOrder(ref ExpressionNode tree, List<Token> tokens, int startHigherChain)
    {
        var higherTokens =
            tokens.GetRange(startHigherChain, GetHigherPrioEnd(tokens, startHigherChain) - startHigherChain);
        tree.Children.Add(CreateTree(higherTokens));
        return tree;
    }
    
    public ExpressionNode CreateTree(List<Token> tokens)
    {
        ExpressionNode root = null;

        if (tokens.Count < 2)
        {
            return CreateExpressionNode(tokens[0]);
        }
        
        switch (tokens[0].TokenType)
        {
            case TokenType.Minus:
            {
                root = CreateExpressionNode(tokens[0]);
                var firstVar = CreateExpressionNode(tokens[1]);
                root.Children.Add(firstVar);
                break;
            }
            case TokenType.Plus:
            {
                root = CreateExpressionNode(tokens[1]);
                break;
            }
            default:
            {
                root = CreateExpressionNode(tokens[1]);
                var firstVar = CreateExpressionNode(tokens[0]);
                root.Children.Add(firstVar);
                break;
            }
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            // if ()
        }

        return root;
    }
}