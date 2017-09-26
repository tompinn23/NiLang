using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nilang.Lex
{
    public class Lexer
    {
        public static string[] keywords = new string[] { "def", "return", "class", "if", "else", "elif", "switch", "case", "this", "override", "extends", "implements", "interface", "default", "break", "continue", "import" };
        public static string[] access = new string[] { "private", "public" };
        public static string[] types = new string[] { "void", "int", "double", "string", "float", "var", "bool", "char" };

        string input;
        int pos = 0;
        int line = 0;
        int column = 0;

        public Token CurrentToken { get; private set; }

        public Lexer(string input)
        {
            input = Regex.Replace(input, @"\r\n?|\n", "\n");
            this.input = input;
            
        }
        public Token NextToken()
        {
            var token = nextToken();
            while (token.type == TokenType.EmptySpace || token.type == TokenType.EOL || token.type == TokenType.Tab || token.type == TokenType.NewLine)
                token = nextToken();
            return CurrentToken = token;
        }

        private Token nextToken()
        {
            if (this.pos >= input.Length) return new Token(TokenType.EOF, "EOF", this.line, this.column);
            var chr = input[this.pos];
            var pos = this.pos;
            var line = this.line;
            var column = this.column;
            var nextChar = ' ';
            if(this.pos + 1 < input.Length) nextChar = input[pos + 1];
            if (chr.ToString() + nextChar.ToString() == "//")
            {
                while (chr != '\n')
                {
                    this.pos += 1;
                    chr = input[this.pos];
                }
                //this.pos += 1;
                //chr = input[this.pos];
            }
            if (chr == '"') return recognizeStringLiteral();
            else if (char.IsLetter(chr)) return recognizeIdentifier();
            else if (char.IsDigit(chr)) return recognizeNumber();
            else if (IsOperator(chr)) return recognizeOperator();
            else if (IsDelimeter(chr)) return recognizeDelimeter();
            else if (chr == ' ')
            {
                this.pos += 1;
                this.column += 1;
                return new Token(TokenType.EmptySpace, " ", line, column);
            }
            else if (chr == '\n')
            {
                this.pos += 1;
                this.line += 0;
                this.column = 0;
                return new Token(TokenType.NewLine, "\n", line, column);
            }
            else if (chr == '\t')
            {
                this.pos += 1;
                this.column += 1;
                return new Token(TokenType.Tab, "\t", line, column);
            }
            else if (chr == ',')
            {
                this.pos += 1;
                this.column += 1;
                return new Token(TokenType.ArgSeperator, ",", line, column);
            }
            this.pos += 1;
            this.column += 1;
            return new Token(TokenType.Unkown, chr.ToString(), line, column);
        }

        private Token recognizeStringLiteral()
        {
            var str = "\"";
            var line = this.line;
            var column = this.column;
            this.pos += 1;
            var pos = this.pos;
            while (pos < input.Length)
            {
                var chr = input[pos];
                if ((input[pos - 1] == '"')) break;
                str += chr;
                pos += 1;
            }
            this.pos += str.Length;
            this.column += str.Length;
            return new Token(TokenType.String, str, line, column);
        }

        private bool IsOperator(char chr)
        {
            var operators = new char[] { '=', '&', '<', '>', '+', '-', '/', '*', '!', '|' };
            return operators.Contains(chr);
        }

        private bool IsDelimeter(char chr)
        {
            var delimeters = new char[] { '{', '}', '(', ')', ';' };
            return delimeters.Contains(chr);
        }

        private Token recognizeDelimeter()
        {
            var line = this.line;
            var column = this.column;
            var chr = input[pos];
            pos += 1;
            this.column += 1;
            if(chr == '(') return new Token(TokenType.LeftParenthesis, "(", line, column);
            if (chr == ')') return new Token(TokenType.RightParenthesis, ")", line, column);
            if (chr == '{') return new Token(TokenType.LeftCurlyBrace, "{", line, column);
            if (chr == '}') return new Token(TokenType.RightCurlyBrace, "}", line, column);
            if (chr == ';') return new Token(TokenType.EOL, ";", line, column);
            return new Token(TokenType.Unkown, chr.ToString(), line, column);
        }

        private Token recognizeOperator()
        {
            char nextChar = ' ';
            var chr = input[pos];
            if (pos + 1 < input.Length) nextChar = input[pos + 1];
            var comparisonOps = new string[] { "==", "<", ">", "&&", "!=", "||" };
            var arithmeticOps = new string[] { "+", "+=", "-", "-=", "/", "/=", "*", "*=" };
            string op = chr.ToString() + nextChar.ToString();
            if (comparisonOps.Contains(chr.ToString()) || comparisonOps.Contains(op)) return recognizeComparisonOperator();
            if (arithmeticOps.Contains(chr.ToString()) || arithmeticOps.Contains(op)) return recognizeArithmeticOperator();
            if(chr == '=' && nextChar != '=')
            {
                var line = this.line;
                var column = this.column;
                pos += 1;
                this.column += 1;
                return new Token(TokenType.Assign, "=", line, column);
            }
            if (chr == '&' && nextChar != '&')
            {
                var line = this.line;
                var column = this.column;
                pos += 1;
                this.column += 1;
                return new Token(TokenType.Reference, "&", line, column);
            }
            return new Token(TokenType.Unkown, (chr + nextChar).ToString(), line, column);
        }

        private Token recognizeArithmeticOperator()
        {
            var nextChar = ' ';
            var line = this.line;
            var column = this.column;
            var chr = input[pos];
            if(pos + 1 < input.Length) nextChar = input[pos + 1];
            pos += 1;
            this.column += 1;

            if(nextChar == '=')
            {
                pos += 1;
                this.column += 1;
            }
            switch (chr)
            {
                case '+':
                    if (nextChar == '=') return new Token(TokenType.PlusEquals, "+=", line, column);
                    else return new Token(TokenType.Plus, "+", line, column);
                case '-':
                    if (nextChar == '=') return new Token(TokenType.MinusEquals, "-=", line, column);
                    else return new Token(TokenType.Minus, "-", line, column);
                case '/':
                    if (nextChar == '=') return new Token(TokenType.DivideEquals, "/=", line, column);
                    else return new Token(TokenType.Divide, "/", line, column);
                case '*':
                    if (nextChar == '=') return new Token(TokenType.TimesEquals, "+=", line, column);
                    else return new Token(TokenType.Times, "+", line, column);
            }
            return new Token(TokenType.Unkown, chr.ToString() + nextChar.ToString(), line, column);
        }

        private Token recognizeComparisonOperator()
        {
            var nextChar = ' ';
            var line = this.line;
            var column = this.column;
            var chr = input[pos];
            if (pos + 1 < input.Length) nextChar = input[pos + 1];
            pos += 1;
            this.column += 1;

            if (nextChar == '=')
            {
                pos += 1;
                this.column += 1;
            }
            if (chr == '>')
            {
                if (nextChar == '=') return new Token(TokenType.GreaterThanOrEqual, ">=", line, column);
                else return new Token(TokenType.GreaterThan, ">", line, column);
            }
            else if (chr == '<')
            {
                if (nextChar == '=') return new Token(TokenType.LessThanOrEqual, "<=", line, column);
                else return new Token(TokenType.LessThan, "<", line, column);
            }
            else if ((chr + nextChar).ToString() == "==") return new Token(TokenType.Equal, "==", line, column);
            else if ((chr + nextChar).ToString() == "!=") return new Token(TokenType.NotEqual, "!=", line, column);
            else if ((chr + nextChar).ToString() == "&&") return new Token(TokenType.And, "&&", line, column);
            else if ((chr + nextChar).ToString() == "||") return new Token(TokenType.Or, "||", line, column);
            return new Token(TokenType.Unkown, (chr + nextChar).ToString(), line, column);
        }

        private Token recognizeNumber()
        {
            var pos = this.pos;
            var line = this.line;
            var column = this.column;
            var numInfo = new NumberFSM().Run(input.Substring(pos));
            var number = input.Substring(pos,numInfo);
            this.pos += numInfo;
            this.column += numInfo;
            return new Token(TokenType.Number, number, line, column);
                
        }

        private Token recognizeIdentifier()
        {
            var identifier = "";
            var line = this.line;
            var column = this.column;
            var pos = this.pos;
            while(pos < input.Length)
            {
                var chr = input[pos];
                if (!(char.IsLetterOrDigit(chr) || chr == '_')) break;
                identifier += chr;
                pos += 1;
            }
            this.pos += identifier.Length;
            this.column += identifier.Length;

            if (keywords.Contains(identifier)) return new Token(TokenType.Keyword, identifier, line, column);
            else if (types.Contains(identifier)) return new Token(TokenType.Type, identifier, line, column);
            else if (access.Contains(identifier)) return new Token(TokenType.Access, identifier, line, column);
            else return new Token(TokenType.Identifier, identifier, line, column);
        }

        class NumberFSM
        {
            State InitialState = State.Initial;
            enum State
            {
                Initial = 1,
                Integer = 2,
                BeginNumberWithFractionalPart = 3,
                NumberWithFractionalPart = 4,
                BeginNumberWithExponent = 5,
                BeginNumberWithSignedExponent = 6,
                NumberWithExponent = 7,
                NoNextState = -1,
            }

            public int Run(string input)
            {
                var currState = InitialState;
                var prevState = InitialState;
                int i;
                for(i = 0; i < input.Length; i++)
                {
                    char character = input[i];
                    prevState = currState;
                    currState = nextState(currState, character);
                    if (currState == State.NoNextState)
                    {
                        return i;
                    }
                }
                return i;
            }

            private State nextState(State currState,char chr)
            {
                switch(currState)
                {
                    case State.Initial:
                        if (char.IsDigit(chr)) return State.Integer;
                        break;
                    case State.Integer:
                        if (char.IsDigit(chr)) return State.Integer;
                        if (chr == '.') return State.BeginNumberWithFractionalPart;
                        if (char.ToLower(chr) == 'e') return State.BeginNumberWithExponent;
                        break;
                    case State.BeginNumberWithFractionalPart:
                        if (char.IsDigit(chr)) return State.NumberWithFractionalPart;
                        break;
                    case State.NumberWithFractionalPart:
                        if (char.IsDigit(chr)) return State.NumberWithFractionalPart;
                        if (char.ToLower(chr) == 'e') return State.BeginNumberWithExponent;
                        break;
                    case State.BeginNumberWithExponent:
                        if (chr == '+' || chr == '-') return State.BeginNumberWithSignedExponent;
                        if (char.IsDigit(chr)) return State.NumberWithExponent;
                        break;
                    case State.BeginNumberWithSignedExponent:
                        if (char.IsDigit(chr)) return State.BeginNumberWithExponent;
                        break;
                    case State.NumberWithExponent:
                        if (char.IsDigit(chr)) return State.NumberWithExponent;
                        break;
                }
                return State.NoNextState;
            }
        }

    }
}
