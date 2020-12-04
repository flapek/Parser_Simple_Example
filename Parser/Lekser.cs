using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public class Lekser
    {
        public List<Token> Tokens { get; set; }
        public bool IsSuccess { get; set; } = true;
        public int Index { get; set; } = 0;
        public Token ExceptionToken { get; set; }
        private StringBuilder exceptionString;

        public Lekser()
        {
            Tokens = new List<Token>();
            exceptionString = new StringBuilder();
        }

        public void Analize(string input) 
            => input.All(x => Compare(x.ToString()));

        private bool Compare(string argument)
            => argument switch
            {
                "(" => AddToTokens(argument, TokenType.LeftBracket),
                ")" => AddToTokens(argument, TokenType.RightBracket),
                "+" => AddToTokens(argument, TokenType.Operator),
                "-" => AddToTokens(argument, TokenType.Operator),
                "/" => AddToTokens(argument, TokenType.Operator),
                "*" => AddToTokens(argument, TokenType.Operator),
                " " => AddToTokens(argument, TokenType.WhiteChar),
                string _ when "0123456789".Contains(argument) => BuildNumber(argument),
                _ => InitializeException(argument),
            };

        private bool BuildNumber(string argument)
        {
            if (Tokens.Count == 0)
            {
                AddToTokens(argument, TokenType.Digit);
                return true;
            }

            var t = Tokens.Last();
            if (t.TokenType == TokenType.Digit || t.TokenType == TokenType.Number)
            {
                t.Argument = $"{t.Argument}{argument}";
                t.TokenType = TokenType.Number;
            }
            else
                AddToTokens(argument, TokenType.Digit);

            return true;
        }

        private bool AddToTokens(string x, TokenType tokenType)
        {
            Tokens.Add(new Token(tokenType, x, Index++));
            return true;
        }

        private bool InitializeException(string argument)
        {
            IsSuccess = false;
            if (string.IsNullOrEmpty(exceptionString.ToString()))
                ExceptionToken = new Token(TokenType.Unnown, argument, Index);
            else
                ExceptionToken = new Token(TokenType.Unnown, exceptionString.Append(argument).ToString(), Index);
            return false;
        }

    }
}
