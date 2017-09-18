﻿using LLVMSharp;
using System;
using Nilang.AST;
using Nilang.AST.VariableTypes;
using System.Collections;
using System.Collections.Generic;

namespace Nilang
{
    public class Visitor
    {
        private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);
    
        private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);

        private static Visitor instance; 

        public static Visitor visitor { get { return instance; } }

        public Stack<LLVMValueRef> ResultStack { get => valueStack; }

        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;

        private Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();

        private Stack<LLVMValueRef> blockStack = new Stack<LLVMValueRef>(); 

        private Dictionary<string, LLVMValueRef> namedValues = new Dictionary<string, LLVMValueRef>();

        public Visitor(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.module = module;
            this.builder = builder;
            instance = this;
        }

        public ExprAST Visit(ExprAST node)
        {
            if (node != null)
                return node.Accept(this);
            return null;
        }

        internal BoolExprAST BoolASTVisit(BoolExprAST node)
        {
            valueStack.Push(LLVM.ConstInt(LLVM.Int8Type(), Convert.ToUInt64(node.Value), new LLVMBool(0)));
            return node;
        }

        public IntegerExprAST IntegerASTVisit(IntegerExprAST node)
        {
            valueStack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, new LLVMBool(1)));
            return node;
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
            LLVMValueRef n;
            switch (node.NodeType)
            {
                case ExprType.AddExpr:
                    n = LLVM.BuildFAdd(builder, l, r, "addtmp");
                    break;
                case ExprType.SubtractExpr:
                    n = LLVM.BuildFSub(builder, l, r, "subtmp");
                    break;
                case ExprType.MultExpr:
                    n = LLVM.BuildFMul(builder, l, r, "subtmp");
                    break;
                case ExprType.DivideExpr:
                    n = LLVM.BuildFDiv(builder, l, r, "subtmp");
                    break;
                case ExprType.LessThanExpr:
                    n = LLVM.BuildFCmp(this.builder, LLVMRealPredicate.LLVMRealULT, l, r, "cmptmp");
                    break;
                default:
                    throw new Exception("Invalid Binary Operator");
            }
            this.valueStack.Push(n);
            return node;
        }

        public BlockExprAST BlockASTVisit(BlockExprAST node)
        {
            blockStack.Clear();
            foreach(var el in node.Exprs)
            {
                Visit(el);
                blockStack.Push(valueStack.Pop());
            }
            return node;
        }

        public StringExprAST StringVisit(StringExprAST node)
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
            foreach(var arg in node.Args)
            {
                Visit(arg);
                ArgsV.Add(valueStack.Pop());
                
            }
            valueStack.Push(LLVM.BuildCall(builder, calleeF, ArgsV.ToArray(), "calltmp"));
            return node;
        }

        public FunctionAST FunctionASTVisit(FunctionAST node)
        {
            this.namedValues.Clear();
            this.Visit(node.Proto);
            LLVMValueRef function = valueStack.Pop();
            LLVM.PositionBuilderAtEnd(this.builder, LLVM.AppendBasicBlock(function, "entry"));

            try
            {
                Visit(node.Body);
            }
            catch (Exception)
            {
                LLVM.DeleteFunction(function);
                throw new Exception("Block Parsing Failed");
            }
            var basicBlock = LLVM.GetEntryBasicBlock(function);
            if(node.Proto.ReturnType == Type.Void)
            {
                LLVM.BuildRetVoid(this.builder);
            }
            else
            {
                LLVM.BuildRet(builder, blockStack.Pop());
            }
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

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
                for(int i = 0; i < argumentCount; i++)
                {
                    switch(node.Args[i].Type)
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
                switch(node.ReturnType)
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

                for(int i =0; i < argumentCount; i++)
                {
                    string argumentName = node.Args[i].Name;
                    LLVMValueRef param = LLVM.GetParam(function, (uint)i);
                    LLVM.SetValueName(param, argumentName);
                    this.namedValues[argumentName] = param;
                }
                this.valueStack.Push(function);
                return node;
            }
        }


    }
}