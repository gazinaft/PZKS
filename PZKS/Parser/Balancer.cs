namespace PZKS.Parser;

public class Balancer
{
    ExpressionNode PlusNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Plus, "+") };
    }

    ExpressionNode MultNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Mult, "*") };
    }
    
    ExpressionNode MinusNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Minus, "-") };
    }

    ExpressionNode DivNode()
    {
        return new ExpressionNode { NodeToken = new Token(TokenType.Div, "/") };
    }

    /// correct
    public ExpressionNode CreateOperationNode(TokenType type)
    {
        return type switch
        {
            TokenType.Mult => MultNode(),
            TokenType.Plus => PlusNode(),
            TokenType.Minus => MinusNode(),
            TokenType.Div => DivNode(),
            _ => throw new Exception("Unexpected operation")
        };
    }

    /// correct
    public static int BalancedTreeHeight(int numberOfNodes)
    {
        return (int)(Math.Log2(numberOfNodes));
    }

    /// correct
    public int GetMonotoneChildrenHeight(ExpressionNode node, TokenType type)
    {
        var heights = new List<int>();
        if (type != node.NodeToken.TokenType)
        {
            return 0;
        }
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild.NodeToken.TokenType == type)
            {
                heights.Add(1 + GetMonotoneChildrenHeight(nodeChild, type));
            }
            else
            {
                heights.Add(0);
            }
        }

        return heights.Count == 0 ? 0 : heights.Max();
    }
    
    /// correct
    public int GetMonotoneChildrenCount(ExpressionNode node, TokenType tokenType)
    {
        var count = new List<int>();
        if (node.NodeToken.TokenType != tokenType)
        {
            return 1;
        }
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild.NodeToken.TokenType == tokenType)
            {
                count.Add(GetMonotoneChildrenCount(nodeChild, tokenType));
            }
            else
            {
                count.Add(0);
            }
        }
        return count.Count == 0 ? 1 : count.Sum() + 1;
    }

    /// correct
    public ExpressionNode BalancedExpressionTreeOfType(TokenType tokenType, int countOfNodes)
    {
        if (countOfNodes == 1)
        {
            return CreateOperationNode(tokenType);
        }
        
        var root = CreateOperationNode(tokenType);
        var leftCount = (countOfNodes - 1) / 2;
        var rightCount = countOfNodes - leftCount - 1;

        if (leftCount > 0)
        {
            var left = BalancedExpressionTreeOfType(tokenType, leftCount);
            left.Parent = root;
            root.Children.Add(left);
        }
        if (rightCount > 0)
        {
            var right = BalancedExpressionTreeOfType(tokenType, rightCount);
            right.Parent = root;
            root.Children.Add(right);
        }

        return root;
    }

    /// correct
    public List<ExpressionNode> GetChildrenOfUnbalancedTree(ExpressionNode node, TokenType type)
    {
        var result = new List<ExpressionNode>();
        if (type != node.NodeToken.TokenType) return result;
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild.NodeToken.TokenType != type)
            {
                result.Add(nodeChild);
            }
            else
            {
                result.AddRange(GetChildrenOfUnbalancedTree(nodeChild, type));
            }
        }

        return result;
    }

    /// seems correct
    public ExpressionNode FillExpressionNode(ref ExpressionNode node, ref List<ExpressionNode> leafChildren, ref int currentIndex)
    {
        // if (leafChildren.Contains(node)) return node;
        for (var index = 0; index < node.Children.Count; index++)
        {
            var nodeChild = node.Children[index];
            var addedCount = 0;
            while (nodeChild.Children.Count < 2 && currentIndex < leafChildren.Count && nodeChild.IsOperation())
            {
                var leaf1 = leafChildren[currentIndex];
                nodeChild.Children.Add(leaf1);
                leaf1.Parent = nodeChild;
                currentIndex++;
                addedCount++;
            }

            if (addedCount < 2)
            {
                FillExpressionNode(ref nodeChild, ref leafChildren, ref currentIndex);
            }
        }
        
        return node;
    }
    
    
    public bool IsStartOfUnbalancedSubtree(ExpressionNode node, TokenType tokenType)
    {
        var height = GetMonotoneChildrenHeight(node, tokenType);
        var count = GetMonotoneChildrenCount(node, tokenType);
        var balancedHeight = BalancedTreeHeight(count); 
        var start = (node.Parent == null || node.Parent.NodeToken.TokenType != tokenType);
        return height > balancedHeight && start;
    }

    public bool FixImbalance(ref ExpressionNode tree, TokenType tokenType)
    {
        // leaf
        if (tree.IsLeaf())
        {
            return false;
        }
        
        // imbalance
        if (IsStartOfUnbalancedSubtree(tree, tokenType))
        {
            var leafs = GetChildrenOfUnbalancedTree(tree, tokenType);
            var childrenCount = GetMonotoneChildrenCount(tree, tokenType);
            var balancedTree = BalancedExpressionTreeOfType(tokenType, childrenCount);
            int counter = 0;
            balancedTree = FillExpressionNode(ref balancedTree, ref leafs, ref counter);
            if (tree.Parent != null)
            {
                var indexOfTree = tree.Parent.Children.IndexOf(tree);
                tree.Parent.Children[indexOfTree] = balancedTree;
                balancedTree.Parent = tree.Parent;
                tree.Parent = null;
            }
            tree = balancedTree;
            return true;
        }

        foreach (var treeChild in tree.Children)
        {
            var expressionNode = treeChild;
            var bFixedTree = FixImbalance(ref expressionNode, tokenType);
            if (bFixedTree) return true;
        }

        return false;
    }

    public bool FixImbalanceNonCommutative(ref ExpressionNode tree, TokenType tokenType)
    {
        var antitokenType = tokenType switch
        {
            TokenType.Minus => TokenType.Plus,
            TokenType.Div => TokenType.Mult, 
            _ => throw new Exception("the type of operation must be - or /")
        };
        // leaf
        if (tree.IsLeaf())
        {
            return false;
        }
        
        // imbalance
        if (IsStartOfUnbalancedSubtree(tree, tokenType))
        {
            var leafs = GetChildrenOfUnbalancedTree(tree, tokenType);
            var NonCommutativePart = leafs[0];
            leafs.RemoveAt(0);
            var newRoot = CreateOperationNode(tokenType);
            newRoot.Children.Add(NonCommutativePart);
            var childrenCount = GetMonotoneChildrenCount(tree, tokenType);
            var balancedTree = BalancedExpressionTreeOfType(antitokenType, childrenCount - 1);
            int counter = 0;
            balancedTree = FillExpressionNode(ref balancedTree, ref leafs, ref counter);
            newRoot.Children.Add(balancedTree);
            balancedTree.Parent = newRoot;
            if (tree.Parent != null)
            {
                var indexOfTree = tree.Parent.Children.IndexOf(tree);
                newRoot.Parent = tree.Parent;
                tree.Parent.Children[indexOfTree] = newRoot;
                tree.Parent = null;
            }
            tree = newRoot;
            return true;
        }

        foreach (var treeChild in tree.Children)
        {
            var expressionNode = treeChild;
            var bFixedTree = FixImbalanceNonCommutative(ref expressionNode, tokenType);
            if (bFixedTree) return true;
        }

        return false;
    }

    public ExpressionNode BalanceTree(ref ExpressionNode tree, TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.Plus or TokenType.Mult => BalancePlusOrMult(ref tree, tokenType),
            TokenType.Div or TokenType.Minus => BalanceMinusOrDiv(ref tree, tokenType),
            _ => tree
        };
    }
    
    public ExpressionNode BalancePlusOrMult(ref ExpressionNode tree, TokenType tokenType)
    {
        while (true)
        {
            var bHasFixedTree = FixImbalance(ref tree, tokenType);
            if (!bHasFixedTree) break;
        }

        return tree;
    }

    public ExpressionNode BalanceMinusOrDiv(ref ExpressionNode tree, TokenType tokenType)
    {
        while (true)
        {
            var bHasFixedTree = FixImbalanceNonCommutative(ref tree, tokenType);
            if (!bHasFixedTree) break;
        }

        return tree;
    }
    
}
