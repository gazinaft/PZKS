using System.Text;
using PZKS.Parser;

namespace PZKS.MatrixSystem;

public class SystemEvaluator
{
    public List<MatrixSystem> Systems { get; set; }
    private Balancer _balancer;

    public SystemEvaluator(ExpressionNode tree)
    {
        Systems = new List<MatrixSystem>();
        _balancer = new Balancer();
        var defaultSystem = new MatrixSystem(tree.DeepCopy()) {Label = "Non-optimized"};
        defaultSystem.Calculate();
        Systems.Add(defaultSystem);

        var operations = new List<TokenType>
        {
            TokenType.Minus,
            TokenType.Div,
            TokenType.Plus,
            TokenType.Mult
        };
        var accumulativeCopy = tree.DeepCopy();
        foreach (var tokenType in operations)
        {
            _balancer.BalanceTree(ref accumulativeCopy, tokenType);
            var system = new MatrixSystem(accumulativeCopy.DeepCopy()){ Label = tokenType.ToString() };
            system.Calculate();
            Systems.Add(system);
        }
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