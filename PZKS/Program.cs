// See https://aka.ms/new-console-template for more information

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
        // /a*b**c + m)*a*b + a*c - a*smn(j*k/m + m

        // var isParsed = double.TryParse("12,3", out double res);
        // Console.WriteLine(isParsed + " " + res);
        // var expression = "-cos(-t)/f(127,001, /dev/null, +(t0)more_errorsb2) - 5";
        Console.WriteLine(expression);
        while (expression != null)
        {
            var tokens = lexer.Scan(expression);
            validator.Validate(tokens);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
            validator.Reset();
            expression = Console.ReadLine();
            
        }
    }
}