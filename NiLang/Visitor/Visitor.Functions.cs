using LLVMSharp;
using Nilang.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang
{
    public partial class Visitor
    {
        public Type ReturnType { get; private set; }

        internal ExprAST ReturnVisit(ReturnAST node)
        {
            if (node.type != ReturnType)
                return null;
            switch (node.type)
            {
                case Type.Integer:
                    LLVM.BuildRet(this.builder, LLVM.ConstInt(LLVM.Int64Type(), (ulong)Int64.Parse(node.value), new LLVMBool(1)));
                    break;
                case Type.Double:
                    LLVM.BuildRet(this.builder, LLVM.ConstReal(LLVM.DoubleType(), double.Parse(node.value)));
                    break;
                case Type.Bool:
                    LLVM.BuildRet(this.builder, LLVM.ConstInt(LLVM.Int8Type(), Convert.ToUInt64(bool.Parse(node.value)), new LLVMBool(0)));
                    break;
                case Type.String:
                    //TODO: Implement Strings as a return value. +feature +important id:1 gh:3
                    throw new NotImplementedException();
                case Type.Void:
                    LLVM.BuildRetVoid(this.builder);
                    break;
            }
            return node
        }
        // TODO: Decide on Externals? +feature id:2 gh:4
        public ExprAST ExternASTVisit(ExternAST externAST)
        {
            throw new NotImplementedException();
        }

        public CallExprAST CallASTVisit(CallExprAST node)
        {
            var calleeF = LLVM.GetNamedFunction(this.module, node.Callee);
            if (calleeF.Pointer == IntPtr.Zero)
                throw new Exception($"Function: {node.Callee} Not Found");
            if (LLVM.CountParams(calleeF) != node.Args.Count)
                throw new Exception("Incorrect Number of arguments passed.");
            List<LLVMValueRef> ArgsV = new List<LLVMValueRef>();
            int i = 0;
            foreach (var arg in node.Args)
            {
                Visit(arg);
                ArgsV.Add(valueStack.Pop());

            }
            valueStack.Push(LLVM.BuildCall(builder, calleeF, ArgsV.ToArray(), "calltmp"));
            return node;
        }

        public BlockExprAST BlockASTVisit(BlockExprAST node)
        {
            blockStack.Clear();
            foreach (var el in node.Exprs)
            {
                Visit(el);
                blockStack.Push(valueStack.Pop());
            }
            return node;
        }

        public FunctionAST FunctionASTVisit(FunctionAST node)
        {
            this.namedValues.Clear();
            this.Visit(node.Proto);
            LLVMValueRef function = valueStack.Pop();
            this.ReturnType = node.Proto.ReturnType;
            try
            {
                Visit(node.Body);
            }
            catch (Exception)
            {
                LLVM.DeleteFunction(function);
                throw new Exception("Block Parsing Failed");
            }
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
            LLVM.RunFunctionPassManager(FPM, function);
            this.valueStack.Push(function);

            return node;

        }

        public PrototypeAST PrototypeASTVisit(PrototypeAST node)
        {
            var argumentCount = (uint)node.Args.Count;
            var arguments = new LLVMTypeRef[Math.Max(argumentCount, 1)];

            var function = LLVM.GetNamedFunction(this.module, node.Name);

            if (function.Pointer != IntPtr.Zero)
            {
                if (LLVM.CountBasicBlocks(function) != 0)
                    throw new Exception("Redefinition Of Function");

                if (LLVM.CountParams(function) != argumentCount)
                    throw new Exception("Redefinition of function with diff args num.");
                return node;
            }
            else
            {
                for (int i = 0; i < argumentCount; i++)
                {
                    switch (node.Args[i].Type)
                    {
                        case Type.Bool:
                            arguments[i] = LLVM.Int8Type();
                            break;
                        case Type.Double:
                            arguments[i] = LLVM.DoubleType();
                            break;
                        //Function Type Goes Here!!
                        case Type.Integer:
                            arguments[i] = LLVM.Int64Type();
                            break;
                        case Type.Void:
                            arguments[i] = LLVM.VoidType();
                            break;
                            //String type as well
                            //And the others. :)
                    }
                }
                LLVMTypeRef ret = LLVM.VoidType();
                switch (node.ReturnType)
                {
                    case Type.Bool:
                        ret = LLVM.Int8Type();
                        break;
                    case Type.Double:
                        ret = LLVM.DoubleType();
                        break;
                    //Function Type Goes Here!!
                    case Type.Integer:
                        ret = LLVM.Int64Type();
                        break;
                    case Type.Void:
                        ret = LLVM.VoidType();
                        break;
                }
                function = LLVM.AddFunction(module, node.Name, LLVM.FunctionType(ret, arguments, LLVMBoolFalse));
                LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
                var funcEntry = LLVM.AppendBasicBlock(function, "entry");
                LLVM.PositionBuilderAtEnd(this.builder, funcEntry);

                for (int i = 0; i < argumentCount; i++)
                {
                    string argumentName = node.Args[i].Name;
                    LLVMValueRef param = LLVM.GetParam(function, (uint)i);
                    LLVM.SetValueName(param, argumentName);
                    LLVMValueRef Alloca = CreateEntryBlockAlloca(function, arguments[i], argumentName);
                    LLVM.BuildStore(builder, param, Alloca);
                    this.namedValues[argumentName] = Alloca;
                }
                this.valueStack.Push(function);
                return node;
            }
        }

    }
}
