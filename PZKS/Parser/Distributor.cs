namespace PZKS.Parser;

public class Distributor
{
    private static List<ExpressionNode> _distributiveNodes = new List<ExpressionNode>();
    public void Distribute(ref ExpressionNode tree)
    {
        // DistributeForMultiply(ref tree);

        while (true)
        {
            var mult = DistributeForMultiply(ref tree);
            if (mult)
            {
                _distributiveNodes.Add(tree.DeepCopy());
            }
            var div = DistributeForDivide(ref tree);
            if (div)
            {
                _distributiveNodes.Add(tree.DeepCopy());
            }
            if (!mult && !div) return;
        }
    }

    public bool DistributeForMultiply(ref ExpressionNode tree)
    {
        if (tree.IsLeaf())
        {
            return false;
        }
        if (tree.NodeToken.TokenType == TokenType.Mult)
        {
            var left = tree.Children[0];
            var right = tree.Children[1];
            
            var leftComplex = left.IsLowerOrder();
            var rightComplex = right.IsLowerOrder();
            
            if (rightComplex)
            {
                MultiplySubtree(left, ref right);
                SwapTree(ref tree, ref right);
                tree = right;
                return true;
            }

            if (leftComplex)
            {
                MultiplySubtree(right, ref left);
                SwapTree(ref tree, ref left);
                tree = left;
                return true;
            }
        }

        var worked = false;
        for (var index = 0; index < tree.Children.Count; index++)
        {
            var child = tree.Children[index];
            worked = worked || DistributeForMultiply(ref child);
        }

        return worked;
    }

    public void MultiplySubtree(ExpressionNode multiplier, ref ExpressionNode subtree)
    {
        
        for (var index = 0; index < subtree.Children.Count; index++)
        {
            var child = subtree.Children[index];
            if (child.IsLowerOrder())
            {
                MultiplySubtree(multiplier, ref child);
                continue;
            }

            var mult = new ExpressionNode { NodeToken = new Token(TokenType.Mult, "*") };
            mult.Children.Add(multiplier);
            mult.Children.Add(child);
            if (child.Parent != null)
            {
                subtree.Children[index] = mult;
            }
            mult.Parent = child.Parent;
            child.Parent = mult;
        }
    }

    private bool SwapTree(ref ExpressionNode tree, ref ExpressionNode newTree)
    {
        if (tree.Parent != null)
        {
            var indexOfTree = tree.Parent.Children.IndexOf(tree);
            if (indexOfTree == -1) return false;
            tree.Parent.Children[indexOfTree] = newTree;
        }
        newTree.Parent = tree.Parent;
        return true;
    }
    
    public bool DistributeForDivide(ref ExpressionNode tree)
    {
        if (tree.IsLeaf())
        {
            return false;
        }
        if (tree.NodeToken.TokenType == TokenType.Div)
        {
            var left = tree.Children[0];
            var right = tree.Children[1];
            
            var leftComplex = left.IsLowerOrder();
            if (leftComplex)
            {
                DivideSubtree(ref left, right);
                SwapTree(ref tree, ref left);
                tree = left;
                return true;
            }
        }

        var worked = false;
        for (var index = 0; index < tree.Children.Count; index++)
        {
            var child = tree.Children[index];
            worked = worked || DistributeForDivide(ref child);
        }

        return worked;
    }

    private void DivideSubtree(ref ExpressionNode subtree, ExpressionNode divider)
    {
        for (var index = 0; index < subtree.Children.Count; index++)
        {
            var child = subtree.Children[index];
            if (child.IsLowerOrder())
            {
                DivideSubtree(ref child, divider);
                continue;
            }

            var div = new ExpressionNode { NodeToken = new Token(TokenType.Div, "/") };
            div.Children.Add(child);
            div.Children.Add(divider);
            if (child.Parent != null)
            {
                subtree.Children[index] = div;
            }
            div.Parent = child.Parent;
            child.Parent = div;

        }

    }
}