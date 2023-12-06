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

    private ExpressionNode CreateOperationNode(TokenType type)
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

    private static int BalancedTreeHeight(int numberOfNodes)
    {
        return (int)(Math.Log2(numberOfNodes) + 1);
    }

    private int GetMonotoneChildrenHeight(ExpressionNode node, TokenType type)
    {
        var heights = new List<int>();
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

        return heights.Max();
    }
    
    private int GetMonotoneChildrenCount(ExpressionNode node, TokenType tokenType)
    {
        var count = new List<int>();
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild.NodeToken.TokenType == tokenType)
            {
                count.Add(1 + GetMonotoneChildrenCount(nodeChild, tokenType));
            }
            else
            {
                count.Add(0);
            }
        }

        return count.Sum();
    }

    private ExpressionNode BalancedExpressionTreeOfType(TokenType tokenType, int countOfNodes)
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

    private List<ExpressionNode> GetChildrenOfUnbalancedTree(ExpressionNode node)
    {
        var type = node.NodeToken.TokenType;
        var result = new List<ExpressionNode>();
        foreach (var nodeChild in node.Children)
        {
            if (nodeChild.NodeToken.TokenType != type)
            {
                result.Add(nodeChild);
            }
            else
            {
                result.AddRange(GetChildrenOfUnbalancedTree(nodeChild));
            }
        }

        return result;
    }

    private ExpressionNode FillExpressionNode(ref ExpressionNode node, ref List<ExpressionNode> leafChildren)
    {
        foreach (var nodeChild in node.Children)
        {
            throw new Exception("Bad inserting");
            if (nodeChild.IsLeaf())
            {
                var leaf1 = leafChildren[0];
                nodeChild.Children.Add(leaf1);
                leaf1.Parent = nodeChild;
                leafChildren.RemoveAt(0);
                
                var leaf2 = leafChildren[0];
                nodeChild.Children.Add(leaf2);
                leaf2.Parent = nodeChild;
                leafChildren.RemoveAt(0);
            }
        }

        return node;
    }
    
    private bool IsStartOfUnbalancedSubtree(ExpressionNode node, TokenType tokenType)
    {
        return GetMonotoneChildrenHeight(node, tokenType) > BalancedTreeHeight(GetMonotoneChildrenCount(node, tokenType))
               && (node.Parent == null || node.Parent.NodeToken.TokenType != tokenType);
    }

    private (bool, ExpressionNode?) FixImbalance(ref ExpressionNode tree, TokenType tokenType)
    {
        if (IsStartOfUnbalancedSubtree(tree, tokenType))
        {
            var leafs = GetChildrenOfUnbalancedTree(tree);
            var childrenCount = GetMonotoneChildrenCount(tree, tokenType);
            var balancedTree = BalancedExpressionTreeOfType(tokenType, childrenCount);
            balancedTree = FillExpressionNode(ref balancedTree, ref leafs);
            if (tree.Parent != null)
            {
                var indexOfTree = tree.Parent.Children.IndexOf(tree);
                tree.Parent.Children[indexOfTree] = balancedTree;
                balancedTree.Parent = tree.Parent;
                tree.Parent = null;
                return (true, null);
            }
            
            return (true, balancedTree);
        }

        if (tree.IsLeaf())
        {
            return (false, null);
        }

        foreach (var treeChild in tree.Children)
        {
            var expressionNode = treeChild;
            var (bFixedTree, fixedTree) = FixImbalance(ref expressionNode, tokenType);
            if (bFixedTree) return (true, fixedTree);
        }

        return (false, null);
    }
    
    ExpressionNode BalancePlusOrMult(ref ExpressionNode tree, TokenType tokenType)
    {
        while (true)
        {
            var (bHasFixedTree, fixedTree) = FixImbalance(ref tree, tokenType);
            if (bHasFixedTree && fixedTree != null)
            {
                tree = fixedTree;
            }
            if (!bHasFixedTree) break;
        }

        return tree;
    }

    
    
}