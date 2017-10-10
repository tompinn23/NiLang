using LLVMSharp;
using System;
using Nilang.AST;
using Nilang.AST.VariableTypes;
using System.Collections;
using System.Collections.Generic;

namespace Nilang
{
    public partial class Visitor
    {
        private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);
    
        private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);     

        private static Visitor instance; 

        public static Visitor visitor { get { return instance; } }

        public Stack<LLVMValueRef> ResultStack { get => valueStack; }

        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;
        private LLVMValueRef mainFunction;

        private LLVMValueRef entryBlock;
        private LLVMPassManagerRef FPM;
        private Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();

        private Stack<LLVMValueRef> blockStack = new Stack<LLVMValueRef>(); 

        private Dictionary<string, LLVMValueRef> namedValues = new Dictionary<string, LLVMValueRef>();

        private Dictionary<string, Dictionary<string, LLVMValueRef>> contexts = new Dictionary<string, Dictionary<string, LLVMValueRef>>();

        public Visitor(LLVMModuleRef module, LLVMBuilderRef builder, LLVMPassManagerRef FPM)
        {
            this.module = module;
            this.builder = builder;
            this.FPM = FPM;
            instance = this;
        }

        public ExprAST Visit(ExprAST node)
        {
            if (node != null)
                return node.Accept(this);
            return null;
        }

        public static LLVMValueRef CreateEntryBlockAlloca(LLVMValueRef Function, LLVMTypeRef type, string name)
        {
            var build = new IRBuilder();
            build.PositionBuilder(Function.GetEntryBasicBlock(), Function.GetEntryBasicBlock().GetFirstInstruction());
            return build.CreateAlloca(type, name);
        }

        public IdentifierExprAST IdentifierASTVisit(IdentifierExprAST node)
        {
            LLVMValueRef value;
            if (!namedValues.TryGetValue(node.Name, out value))
                throw new Exception($"Unknown Variable Name: {node.Name}");
            valueStack.Push(value);
            return node;
        }

        public BinaryOpExprAST BinaryOpASTVisit(BinaryOpExprAST node)
        {
            Visit(node.Lhs);
            Visit(node.Rhs);
            LLVMValueRef r = valueStack.Pop();
            LLVMValueRef l = valueStack.Pop();
            r = LLVM.BuildLoad(builder, r, "r");
            l = LLVM.BuildLoad(builder, l, "l");
            LLVMValueRef n;
            var i = false;
            if (LLVM.GetTypeKind(LLVM.TypeOf(r)) == LLVMTypeKind.LLVMIntegerTypeKind || LLVM.GetTypeKind(LLVM.TypeOf(r)) == LLVMTypeKind.LLVMIntegerTypeKind)
               i = true;
            switch (node.NodeType)
            {
                case ExprType.AddExpr:
                    if(i) n = LLVM.BuildAdd(builder, l, r, "addtmp");
                    else n = LLVM.BuildFAdd(builder, l, r, "addtmp");
                    break;
                case ExprType.SubtractExpr:
                    if(i) n = LLVM.BuildSub(builder, l, r, "subtmp");
                    else n = LLVM.BuildFSub(builder, l, r, "subtmp");
                    break;
                case ExprType.MultExpr:
                    if(i) n = LLVM.BuildMul(builder, l, r, "subtmp");
                    else n = LLVM.BuildFMul(builder, l, r, "subtmp");
                    break;
                case ExprType.DivideExpr:
                    if(i) n = LLVM.BuildFDiv(builder, l, r, "subtmp");
                    else n = LLVM.BuildFDiv(builder, l, r, "subtmp");
                    break;
                case ExprType.LessThanExpr:
                    if(i) n = LLVM.BuildICmp(this.builder, LLVMIntPredicate.LLVMIntULT, l, r, "cmptmp");
                    n = LLVM.BuildFCmp(this.builder, LLVMRealPredicate.LLVMRealULT, l, r, "cmptmp");
                    break;
                default:
                    throw new Exception("Invalid Binary Operator");
            }
            this.valueStack.Push(n);
            return node;
        }
    }
}