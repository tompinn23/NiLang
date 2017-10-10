using LLVMSharp;
using Nilang;
using Nilang.Lex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NiLang
{
    class Program
    {
        static LLVMModuleRef module;
        static LLVMBuilderRef builder;
        static void Main(string[] args)
        {
            string filename = @"test.ni";
            string outname = Path.GetFileNameWithoutExtension(filename) + ".bc";
            if(args.Length == 1)
            {
                filename = args[0];
            }
            module = LLVM.ModuleCreateWithName(Path.GetFileNameWithoutExtension(filename));
            builder = LLVM.CreateBuilder();

            var FPM = LLVM.CreateFunctionPassManagerForModule(module);
            LLVM.AddPromoteMemoryToRegisterPass(FPM);
            LLVM.AddInstructionCombiningPass(FPM);
            LLVM.AddReassociatePass(FPM);
            LLVM.AddNewGVNPass(FPM);
            LLVM.AddCFGSimplificationPass(FPM);
            LLVM.FinalizeFunctionPassManager(FPM);

            string file = File.ReadAllText(filename);
            var lexer = new Lexer(file);
            var parser = new Parser(lexer, new Visitor(module, builder, FPM));
            var token = lexer.NextToken();
            LLVM.DumpModule(module);

            while (lexer.CurrentToken.type != TokenType.EOF)
            {
                parser.ParsePrimary();
                Console.WriteLine("\n##################################################################################################################\n");
                LLVM.DumpModule(module);
            }

            string error = "";

            Console.WriteLine("\n##################################################################################################################\n");
            LLVM.DumpModule(module);
            LLVM.VerifyModule(module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out error);
            LLVM.WriteBitcodeToFile(module, outname);
            var clang = Process.Start("clang++", "test.bc -o test.exe");
            clang.WaitForExit();
            Console.Read();
        }

        private static void declareCFunctions()
        {
            LLVMTypeRef PutCharType = LLVMTypeRef.FunctionType(LLVM.Int32Type(), new LLVMTypeRef[] { LLVM.Int32Type() }, false);
            LLVM.AddFunction(module, "putchar", PutCharType);
        }

        static LLVMValueRef createMain()
        {
            var context = LLVM.GetGlobalContext();
            LLVMTypeRef funcType = LLVMTypeRef.FunctionType(LLVMTypeRef.Int64Type(), new LLVMTypeRef[] {  }, false);
            LLVMValueRef function = LLVM.AddFunction(module, "main", funcType);
            return function;
        }
    }
}
