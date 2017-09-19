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
        static LLVMModuleRef module;
        static LLVMBuilderRef builder;
        static void Main(string[] args)
        {
            string filename = @"test.ni";
            string outname = Path.GetFileNameWithoutExtension(filename) + ".o";
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

            var targetTriple = LLVM.GetDefaultTargetTriple();

            //LLVM.InitializeAllTargetInfos();
            //LLVM.InitializeAllTargets();
            //LLVM.InitializeAllTargetMCs();
            //LLVM.InitializeAllAsmParsers();
            //LLVM.InitializeAllAsmPrinters();
            //LLVMTargetRef target;
            //string error;
            //LLVM.GetTargetFromTriple(targetTriple.ToString(), out target, out error);
            //if (error.Length > 0)
            //    throw new Exception(error);
            //var cpu = "generic";
            //var feature = "";
            //var machine = LLVM.CreateTargetMachine(target, targetTriple.ToString(), cpu, feature, LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault, LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel.LLVMCodeModelDefault);
            

            //LLVM.SetDataLayout(module, LLVM.CopyStringRepOfTargetData(LLVM.CreateTargetDataLayout(machine)).ToString());
            //LLVM.SetTarget(module, targetTriple.ToString());


            string file = File.ReadAllText(filename);
            var lexer = new Lexer(file);
            var main = createMain();
            var block = LLVM.AppendBasicBlock(main, "entry");
            LLVM.PositionBuilder(builder, block, block.GetFirstInstruction());

            declareCFunctions();

            var parser = new Parser(lexer, new Visitor(module, builder, main, block, FPM));
            var token = lexer.NextToken();
            LLVM.DumpModule(module);

            while (lexer.CurrentToken.type != TokenType.EOF)
            {
                parser.ParsePrimary();
                Console.WriteLine("\n##################################################################################################################\n");
                LLVM.DumpModule(module);
            }

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
