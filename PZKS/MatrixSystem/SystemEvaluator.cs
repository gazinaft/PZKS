using System.Reflection.Emit;
using System.Text;
using PZKS.Parser;

namespace PZKS.MatrixSystem;

public class SystemEvaluator
{
    public List<MatrixSystem> Systems { get; set; }
    private Balancer _balancer;
    private Distributor _distributor;
    public SystemEvaluator(ExpressionNode tree)
    {
        Systems = new List<MatrixSystem>();
        _balancer = new Balancer();
        _distributor = new Distributor();
        var defaultSystem = new MatrixSystem(tree.DeepCopy()) {Label = "Non-optimized"};
        defaultSystem.Calculate();
        Systems.Add(defaultSystem);

        var numberOfMethods = 5;
        for (int a = 0; a < numberOfMethods; a++)
        {
            for (int b = 0; b < numberOfMethods; b++)
            {
                if (a == b) continue;
                for (int c = 0; c < numberOfMethods; c++)
                {
                    if (c == a || c == b) continue;
                    for (int d = 0; d < numberOfMethods; d++)
                    {
                        if (d == a || d == b || d == c) continue;
                        for (int e = 0; e < numberOfMethods; e++)
                        {
                            if (e == a || e == b || e == c || e == d) continue;
                            var accumulativeCopy = tree.DeepCopy();
                            var label = "";
                            ChangeTree(a, ref accumulativeCopy, ref label);
                            ChangeTree(b, ref accumulativeCopy, ref label);
                            ChangeTree(c, ref accumulativeCopy, ref label);
                            ChangeTree(d, ref accumulativeCopy, ref label);
                            ChangeTree(e, ref accumulativeCopy, ref label);
                        }
                    }
                }
            }
        }

    }

    public void ChangeTree(int formOfChange, ref ExpressionNode tree, ref string treeLabel)
    {
        switch (formOfChange)
        {
            case 0:
                _balancer.BalancePlusOrMult(ref tree, TokenType.Plus);
                treeLabel += "Plus|";
                break;
            case 1:
                _balancer.BalancePlusOrMult(ref tree, TokenType.Mult);
                treeLabel += "Mult|";
                break;
            case 2:
                _balancer.BalanceMinusOrDiv(ref tree, TokenType.Minus);
                treeLabel += "Minus|";
                break;
            case 3:
                _balancer.BalanceMinusOrDiv(ref tree, TokenType.Div);
                treeLabel += "Div|";
                break;
            case 4:
                _distributor.Distribute(ref tree);
                break;
            default:
                break;
        }

        if (formOfChange != 4)
        {
            var system = new MatrixSystem(tree.DeepCopy()) { Label = treeLabel };
            system.Calculate();
            Systems.Add(system);
            return;
        }

        var i = 0;
        foreach (var parallelForm in Distributor.GetAllForms())
        {
            var system = new MatrixSystem(parallelForm.DeepCopy());
            system.Label = treeLabel + $"Distribute{++i}";
            system.Calculate();
            Systems.Add(system);
        }
        treeLabel += "Distribute|";

    }
    
    public string GetSystemStats(int index)
    {
        if (index >= Systems.Count) return "";
        var sb = new StringBuilder();

        sb.Append($"Label: {Systems[index].Label} \n");
        sb.Append($"Execution time: {Systems[index].GetExecutionTime()} \n");
        sb.Append($"Acceleration: {Systems[index].GetAcceleration()} \n");
        sb.Append($"Effectivity coef for 1 host: {Systems[index].GetEffectivityCoef(1)} \n");
        sb.Append($"Effectivity coef for 2 host: {Systems[index].GetEffectivityCoef(2)} \n");

        return sb.ToString();
    }

    public string GetAllStats()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Systems.Count; i++)
        {
            sb.Append(GetSystemStats(i));
            sb.Append('\n');
        }

        return sb.ToString();
    }

    public MatrixSystem GetOptimalSystem()
    {
        return Systems.MinBy(x => x.GetExecutionTime()) ?? throw new InvalidOperationException();
    }
}