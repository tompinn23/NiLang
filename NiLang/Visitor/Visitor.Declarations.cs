using LLVMSharp;
using Nilang.AST;
using Nilang.AST.VariableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang
{
    public partial class Visitor
    {

        internal BoolAST BoolASTVisit(BoolAST node)
        {
            valueStack.Push(LLVM.ConstInt(LLVM.Int8Type(), Convert.ToUInt64(node.Value), new LLVMBool(0)));
            return node;
        }

        internal ExprAST DoubleASTVisit(DoubleAST doubleAST)
        {
            throw new NotImplementedException();
        }

        public IntegerAST IntegerASTVisit(IntegerAST node)
        {
            valueStack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, new LLVMBool(1)));
            return node;
        }
        //TODO: Implement Strings +feature
        public StringAST StringVisit(StringAST node)
        {
            throw new NotImplementedException();
        }

        public ExprAST VariableExprVisit(VariableExprAST node)
        {
            LLVMValueRef initValue;
            this.Visit(node.Value);
            initValue = valueStack.Pop();
            LLVMValueRef alloca = default(LLVMValueRef);
            switch (LLVM.GetTypeKind(LLVM.TypeOf(initValue)))
            {
                case LLVMTypeKind.LLVMIntegerTypeKind:
                    alloca = CreateEntryBlockAlloca(mainFunction, LLVMTypeRef.Int64Type(), node.Name);
                    break;
                case LLVMTypeKind.LLVMDoubleTypeKind:
                    alloca = CreateEntryBlockAlloca(mainFunction, LLVMTypeRef.DoubleType(), node.Name);
                    break;
            }
            LLVM.BuildStore(builder, initValue, alloca);
            return node;
        }
    }
}
