// See https://aka.ms/new-console-template for more information

using PZKS.MatrixSystem;
using PZKS.Parser;
using PZKS.Validation;

namespace PZKS;

// 	помилки на початку арифметичного виразу ( наприклад, вираз не може починатись із закритої дужки, алгебраїчних операцій * та /);
// 	помилки, пов’язані з неправильним написанням імен змінних,  констант та при необхідності функцій;
// 	помилки у кінці виразу (наприклад, вираз не може закінчуватись будь-якою алгебраїчною операцією);
// 	помилки в середині виразу (подвійні операції, відсутність операцій перед або між дужками, операції* або / після відкритої дужки тощо);
// 	помилки, пов’язані з використанням дужок ( нерівна кількість відкритих та закритих дужок, неправильний порядок дужок, пусті дужки).



public static class Program
{
    public static void Main()
    {
        var lexer = new Lexer();
        var validator = new ValidatorStateMachine();
        var start = new StartValidator();
        var parentheses = new ParenthesesValidator();
        var operations = new OperationsValidator();
        var commas = new FunctionValidator();
        var end = new EndValidator();
        
        validator.StartState = start;
        start.NextState = parentheses;
        parentheses.NextState = commas;
        commas.NextState = operations;
        operations.NextState = end;

        // parse as expressions func arguments
        // parentheses between variables
        var expression = "-cos(-&t))/(*(*f)(127.0.0.1, /dev/null/, (t==0)?4more_errors:b^2) - .5";
        // var expression = "-cos(-t)/f(127,001, dev/null, (t0)+4*more_errorsb2) - 5";
        // var expression = "-cos(-t)/f(127001, dev/null, (t0)*4*more_errorsb2) - 5";
        // var expression = "a*b*c + m*a*b + a*c - a*smnj*k/m + m";

        // var isParsed = double.TryParse("12,3", out double res);
        // Console.WriteLine(isParsed + " " + res);
        // var expression = "a*b*c/d + e*f*g/h + t*(a-q) - 5.0*i - 4*j + k + L + m*n*k*(p-1) + sin*(pi*R)*log*(q)/sin*(3*pi/4 + x*pi/2)";
        Console.WriteLine(expression);
        // var squashed = Util.TransformPreTree(tokens1);
        // var subExpressions = (List<List<Token>>)squashed[0].Literal;
        // Console.WriteLine(subExpressions);
        //
        // var parser = new Parser.Parser();
        // var balancer = new Balancer();
        // var unbalanced = parser.CreateTree(squashed);
        // var tree = balancer.BalancePlusOrMult(ref unbalanced, TokenType.Plus);
        // // var tree = balancer.BalancedExpressionTreeOfType(TokenType.Plus, 7);
        // // var matrix = new MatrixSystem.MatrixSystem(tree);
        // var sysEval = new SystemEvaluator(unbalanced);
        // var optimal = sysEval.GetOptimalSystem();
        // var bestTree = optimal.GetTree();
        // Console.WriteLine(sysEval.GetAllStats());
        // Console.WriteLine(optimal.ToString());
        // // Console.WriteLine(tree.Children[1].Children[0].ToString());
        // Console.WriteLine(bestTree);
        while (expression != null)
        {
            var tokens = lexer.Scan(expression);
            validator.Validate(tokens);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
            validator.Reset();
            var hasErrors = Util.LogErrors();
            foreach (var error in hasErrors)
            {
                Console.WriteLine(error);
            }
            Console.WriteLine("The expression has errors:" + (hasErrors.Count > 0));
            // expression = Console.ReadLine();
            expression = null;
        }
    }
}