using Nilang.Lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiLang
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"test.ni";
            if(args.Length == 1)
            {
                filename = args[0];
            }
            string file = File.ReadAllText(filename);
            var lexer = new Lexer(file);
            var token = lexer.NextToken();
            var tokens = new List<Token> { token };
            while(token.type != TokenType.EOF)
            {
                token = lexer.NextToken();
                tokens.Add(token);
            }
            foreach (var t in tokens) Console.Write(t.value);
            Console.Read();
        }
    }
}
