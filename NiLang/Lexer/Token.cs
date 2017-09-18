using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.Lex
{
    public struct Token
    {
        public TokenType type;
        public string value;
        public int line;
        public int column;
        public Token(TokenType type, string value, int line, int column)
        {
            this.type = type;
            this.value = value;
            this.line = line;
            this.column = column;
        }
    }
    public enum TokenType
    {
        String = 0,
        Integer = 1,
        Double = 36,
        Identifier = 2,
        While = 3,
        If = 4,
        For = 5,

        Keyword = 6,
        Type = 7,
        Access = 8,


        Plus = 9,
        Minus = 10,
        Times = 11,
        Divide = 12,
        PlusEquals = 13,
        MinusEquals = 14,
        TimesEquals = 15,
        DivideEquals = 16,


        Equal = 17,
        GreaterThan = 18,
        GreaterThanOrEqual = 19,
        LessThan = 20,
        LessThanOrEqual = 21,
        NotEqual = 22,
        And = 23,
        Or = 24,

        Reference = 25,




        Assign = 26,



        LeftCurlyBrace = 27,
        RightCurlyBrace = 28,
        LeftParenthesis = 29,
        RightParenthesis = 30,

        EOL = 31,
        EOF = 32,
        EmptySpace = 33,
        NewLine = 34,
        Tab = 35,
        // 36 used
        Unkown = 37,
        ArgSeperator = 38,
        Number = 39
    }
}
