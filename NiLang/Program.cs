using LLVMSharp;
using Nilang;
using Nilang.Lex;
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
            LLVMModuleRef module = LLVM.ModuleCreateWithName(Path.GetFileNameWithoutExtension(filename));
            LLVMBuilderRef builder = LLVM.CreateBuilder();
            string file = File.ReadAllText(filename);
            var lexer = new Lexer(file);
            var parser = new Parser(lexer, new Visitor(module, builder));
            var token = lexer.NextToken();
            while (lexer.CurrentToken.type != TokenType.EOF)
            {
                parser.ParsePrimary();
            }
            LLVM.DumpModule(module);
            Console.Read();
        }
    }
}
