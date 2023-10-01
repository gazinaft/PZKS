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
        var start = new StartValidator(validator);
        validator.State = start;
        
        var expression = Console.ReadLine();
        if (expression == null) return;

        var tokens = lexer.Scan(expression);
        validator.Validate(tokens);

    }
}