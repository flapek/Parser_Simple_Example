using System;
using System.Collections.Generic;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var lekser = new Lekser();
            var parser = new Parser(lekser);

            string expresion = "(4/2)*((2 - 1)*(2*2*2*(2+1)+5)*1)";
            parser.Lekser.Analize(expresion);

            if (!parser.Lekser.IsSuccess)
                DisplayError("Lexical analysis error.", expresion, lekser);
            else
            {
                parser.Analize();
                if (parser.IsSuccess)
                {
                    parser.Solve();
                    Console.WriteLine("Recognize expression.");
                    Console.WriteLine($"Result {parser.Result}");
                }
                else
                    DisplayError("Parsing error.", expresion, lekser);
            }
            Console.ReadLine();
        }


        private static void DisplayError(string msg, string expresion, Lekser lekser)
        {
            Console.WriteLine(msg);
            Console.WriteLine(expresion);
            Console.WriteLine($"Undefined token: {lekser.ExceptionToken}\n\n");
            Display(lekser.Tokens);
        }

        private static void Display(List<Token> tokens)
        {
            foreach (var token in tokens)
                Console.WriteLine($"<{token.TokenType},{token.Argument}>");
        }
    }
}
