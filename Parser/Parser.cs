using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public class Parser
    {
        public Lekser Lekser { get; set; }
        public int Index { get; private set; } = 0;
        public double Result { get; private set; } = 0;
        public int ValidBracketsCount { get; private set; } = 0;
        public bool IsSuccess { get; private set; } = true;

        public Parser(Lekser lekser)
        {
            Index = 0;
            ValidBracketsCount = 0;
            Lekser = lekser;
        }

        public void Analize()
        {
            ClearWhiteChar();

            foreach (var item in Lekser.Tokens)
            {
                switch (item.TokenType)
                {
                    case TokenType.LeftBracket:
                        ValidBracketsCount++;
                        break;
                    case TokenType.RightBracket:
                        if (Index == 0)
                            return;
                        ValidBracketsCount--;
                        break;
                    case TokenType.Unnown:
                        return;
                    default:
                        CheckLast(item);
                        break;
                }
                Index++;
            }
            if (ValidBracketsCount != 0)
            {
                InitializeException(true);
                return;
            }
        }

        public void Solve()
        {
            while (Lekser.Tokens.Count > 1)
            {
                var tokens = new List<Token>();
                var leftBracketIndex = -1;
                var rightBracketIndex = -1;
                if (Lekser.Tokens.Any(x => x.TokenType == TokenType.LeftBracket || x.TokenType == TokenType.RightBracket))
                {
                    leftBracketIndex = SearchLeftBracket();
                    rightBracketIndex = SearchRightBracket(leftBracketIndex);
                }
                if (leftBracketIndex >= 0 && rightBracketIndex >= 0)
                {
                    tokens = Lekser.Tokens.GetRange(leftBracketIndex, rightBracketIndex - leftBracketIndex);
                    Lekser.Tokens.RemoveRange(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                }
                else
                {
                    tokens = Lekser.Tokens.ToList();
                    Lekser.Tokens.RemoveRange(1, Lekser.Tokens.Count - 1);
                }

                var token = Calculate(tokens);

                Lekser.Tokens[leftBracketIndex > -1 ? leftBracketIndex : 0] = token;

            }
            Result = double.Parse(Lekser.Tokens.FirstOrDefault().Argument);
        }


        private Token Calculate(List<Token> tokens)
        {
            var tokenResult = new Token(TokenType.Unnown, "", 0);
            while (tokens.Count > 1)
            {
                var @operator = tokens.FirstOrDefault(x => x.TokenType == TokenType.Operator && (x.Argument == "/" || x.Argument == "*"));
                var operatorIndex = tokens.IndexOf(@operator);
                if (operatorIndex < 0 || @operator is null)
                {
                    @operator = tokens.FirstOrDefault(x => x.TokenType == TokenType.Operator && (x.Argument == "+" || x.Argument == "-"));
                    operatorIndex = tokens.IndexOf(@operator);
                }

                var result = RecognizeOperation(double.Parse(tokens[operatorIndex - 1].Argument), @operator, double.Parse(tokens[operatorIndex + 1].Argument));
                tokens.RemoveAt(operatorIndex + 1);
                tokens.RemoveAt(operatorIndex - 1);
                @operator.Argument = result.ToString();
                if (result.ToString().Contains("-") && result.ToString().Length == 1)
                    @operator.TokenType = TokenType.Digit;
                else
                    @operator.TokenType = TokenType.Number;

                tokenResult = @operator;
            }
            return tokenResult;

        }

        private double RecognizeOperation(double firstNumber, Token @operator, double secondNumber)
        {
            double result = 0;
            switch (@operator.Argument)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;
                case "-":
                    result = firstNumber - secondNumber;
                    break;
                case "/":
                    result = firstNumber / secondNumber;
                    break;
                case "*":
                    result = firstNumber * secondNumber;
                    break;
                default:
                    break;
            }
            return result;
        }

        private int SearchLeftBracket()
        {
            var token = Lekser.Tokens.FindLast(x => x.TokenType == TokenType.LeftBracket);
            int index = Lekser.Tokens.IndexOf(token);
            Lekser.Tokens.Remove(token);
            return index;
        }

        private int SearchRightBracket(int leftBracketIndex)
        {
            int index = 0;
            for (int i = leftBracketIndex; i < Lekser.Tokens.Count(); i++)
            {
                if (Lekser.Tokens[i].TokenType == TokenType.RightBracket)
                {
                    index = i;
                    break;
                }
            }
            Lekser.Tokens.RemoveAt(index);
            return index;
        }

        private void ClearWhiteChar() => Lekser.Tokens.RemoveAll(t => t.TokenType == TokenType.WhiteChar);

        private void CheckLast(Token item)
        {
            if (Index == 0)
                return;

            switch (item.TokenType)
            {
                case TokenType.Operator:
                    if (Lekser.Tokens[Index - 1].TokenType == TokenType.Operator)
                        InitializeException();
                    break;
                case TokenType.Number:
                case TokenType.Digit:
                    if (Lekser.Tokens[Index - 1].TokenType == TokenType.Digit
                        || Lekser.Tokens[Index - 1].TokenType == TokenType.Number)
                        InitializeException();
                    break;
                default:
                    break;
            }
        }

        private void InitializeException(bool bracketsError = false)
        {
            IsSuccess = false;
            Lekser.ExceptionToken = !bracketsError ? Lekser.Tokens[Index] : new Token(TokenType.LeftBracket, "( or )", 0);
        }
    }
}
