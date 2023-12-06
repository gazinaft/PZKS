namespace PZKS.Parser;

public class Parser
{

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
        var rootNode = new ExpressionNode { NodeToken = rootToken };
        var subtrees = subExpressions.Select(CreateTree).ToList();
        foreach (var subtree in subtrees)
        {
            subtree.Parent = rootNode;
        }
        rootNode.Children = subtrees;
        
        return rootNode;
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

    public ExpressionNode AppendHigherOrder(ref ExpressionNode tree, List<Token> tokens, int startHigherChain)
    {
        var higherTokens =
            tokens.GetRange(startHigherChain, GetHigherPrioEnd(tokens, startHigherChain) - startHigherChain + 1);
        var higherExpression = CreateTree(higherTokens);
        tree.Children.Add(higherExpression);
        higherExpression.Parent = tree;
        
        return tree;
    }
    
    public ExpressionNode CreateTree(List<Token> tokens)
    {
        ExpressionNode root = null;

        if (tokens.Count < 2)
        {
            return CreateExpressionNode(tokens[0]);
        }

        var counter = 0;
        switch (tokens[0].TokenType)
        {
            case TokenType.Minus:
            {
                root = CreateExpressionNode(tokens[0]);
                var firstVar = CreateExpressionNode(tokens[1]);
                root.Children.Add(firstVar);
                firstVar.Parent = root;
                counter = 2;
                break;
            }
            case TokenType.Plus:
            {
                root = CreateExpressionNode(tokens[1]);
                counter = 2;
                break;
            }
            default:
            {
                root = CreateExpressionNode(tokens[0]);
                counter = 1;
                break;
            }
        }
        
        while (counter < tokens.Count)
        {
            var currentValue = CreateExpressionNode(tokens[counter]);
            if (counter == tokens.Count - 1) // last token, guaranteed variable
            {
                root.Children.Add(currentValue);
                currentValue.Parent = root;
                break;
            }

            var isOperation = currentValue.IsOperation();
            
            if (isOperation) // means operation is low Or Same Prio
            {
                currentValue.Children.Add(root);
                root.Parent = currentValue;
                root = currentValue;
                counter++;
                continue;
            }

            var nextOperation = CreateExpressionNode(tokens[counter + 1]);

            if (nextOperation.IsHigherOrder() && root.IsLowerOrder())
            {
                root = AppendHigherOrder(ref root, tokens, counter);
                counter = GetHigherPrioEnd(tokens, counter);
            }
            else
            {
                root.Children.Add(currentValue);
                currentValue.Parent = root;
            }
            
            counter++;
        }

        return root;
    }
}