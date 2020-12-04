namespace Parser
{
    public class Token
    {
        public TokenType TokenType { get; set; }
        public string Argument { get; set; }
        public int Index { get; set; }

        public Token(TokenType tokenType, string argument, int index)
        {
            TokenType = tokenType;
            Argument = argument;
            Index = index;
        }

        public override string ToString() 
            => $"\nToken type: {TokenType}\nArgument: {Argument}\nIndex: {Index}\n";
    }
}
