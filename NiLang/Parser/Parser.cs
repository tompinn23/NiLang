﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nilang.Lex;
using Nilang.AST;
using Nilang.AST.VariableTypes;
using LLVMSharp;

namespace Nilang
{
    public class Parser
    {

        private Lexer lexer;
        private Visitor visitor;
        static Dictionary<char, int> BinopPrecedence = new Dictionary<char, int>();
        public Parser(Lexer lexer, Visitor visitor)
        {
            this.lexer = lexer;
            this.visitor = visitor;
            BinopPrecedence['<'] = 10;
            BinopPrecedence['>'] = 10;
            BinopPrecedence['+'] = 20;
            BinopPrecedence['-'] = 20;
            BinopPrecedence['*'] = 30;
            BinopPrecedence['/'] = 30;
        }

        private int GetOpPrecedence()
        {
            char c = lexer.CurrentToken.value[0];
            if (!(c < 128))
                return -1;
            int prec;
            if(!BinopPrecedence.TryGetValue(c, out prec))
                return -1;
            return prec;
           
        }

        public ExprAST ParsePrimary()
        {
            switch(lexer.CurrentToken.type)
            {
                default:
                    Console.WriteLine(lexer.CurrentToken.type);
                    Console.WriteLine(lexer.CurrentToken.value);
                    Console.Read();
                    return LogError("unknown token when expecting an expression");
                case TokenType.Type:
                    return ParseDeclarationExpr();
                case TokenType.Integer:
                    return ParseIntegerExpr();
                case TokenType.Identifier:
                    return ParseIdentifierExpr();
                case TokenType.LeftParenthesis:
                    return ParseParenExpr();
                case TokenType.Keyword:
                    return ParseKeyword();
                case TokenType.EOF:
                    return null;
            }
        }

        private ExprAST ParseKeyword()
        {
            switch (lexer.CurrentToken.value)
            {
                case "def":
                    return ParseFunctionDef();
                    break;
            }
            return null;
        }

        public ExprAST ParseTopLevelExpr()
        {
            var e = ParseExpression();
            if (e == null)
                return LogError("Expression failed");

            var func = new FunctionAST(new PrototypeAST(Type.Void, "", new List<ArgTypePair>()), e);
            visitor.Visit(func);
            return func;
        }


        public ExprAST ParseDeclarationExpr()
        {
            var curTok = lexer.CurrentToken;
            //Consume the type token
            lexer.NextToken();
            if(TypeHelper.ConvertStringToType(curTok.value) == Type.Integer)
            {
                var a = ParseIntegerExpr();
                visitor.Visit(a);
                return a;
            }
            else
            {
                return null;
            }
        }

        public ExprAST LogError(string error)
        {
            Console.WriteLine(error);
            return null;
        }
        public ExprAST ParseIntegerExpr()
        {
            var name = lexer.CurrentToken.value;
            while (lexer.CurrentToken.type != TokenType.Number) lexer.NextToken();
            var Result = new IntegerExprAST(name, Int64.Parse(lexer.CurrentToken.value));
            lexer.NextToken();
            return Result;
        }

        public ExprAST ParseParenExpr()
        {
            // Consume '('
            lexer.NextToken();
            var subExpr = ParseExpression();
            if (subExpr == null) return null;
            if (lexer.CurrentToken.type != TokenType.RightParenthesis) return LogError("Expected ')'");
            lexer.NextToken();
            return subExpr;
        }

        public ExprAST ParseIdentifierExpr()
        {
            string idName = lexer.CurrentToken.value;
            lexer.NextToken();
            //Simple Identifier e.g int b = c;
            if (lexer.CurrentToken.type != TokenType.LeftParenthesis)
                return visitor.Visit(new IdentifierExprAST(idName));
            lexer.NextToken();
            List<ExprAST> Args = new List<ExprAST>();
            if (lexer.CurrentToken.type != TokenType.RightParenthesis)
            {
                while (true)
                {
                    var Arg = ParseExpression();
                    if (Arg != null)
                        Args.Add(Arg);
                    else
                        return null;
                    if (lexer.CurrentToken.type == TokenType.RightParenthesis)
                        break;

                    if (lexer.CurrentToken.type != TokenType.ArgSeperator)
                        return LogError("Expected ')' or ',' in argument list");
                    lexer.NextToken();
                }
            }
            lexer.NextToken();
            return visitor.Visit(new CallExprAST(idName, Args));

        }

        public ExprAST ParseExpression()
        {
            var LHS = ParsePrimary();
            if (LHS == null)
                return null;
            return ParseBinOpsRHS(0, LHS);
        }

        public ExprAST ParseBinOpsRHS(int ExprPrec, ExprAST LHS)
        {
            while(true)
            {
                int opPrec = GetOpPrecedence();
                if (opPrec < ExprPrec)
                    return LHS;
                char BinOp = lexer.CurrentToken.value[0];
                lexer.NextToken();
                var RHS = ParsePrimary();
                if (RHS == null)
                    return null;
                int NextPrec = GetOpPrecedence();
                if(opPrec < NextPrec)
                {
                    RHS = ParseBinOpsRHS(opPrec + 1, RHS);
                    if (RHS == null)
                        return null;
                }
                LHS = new BinaryOpExprAST(BinOp, LHS, RHS);
            }
        }

        public ExprAST ParseFunctionDef()
        {
            lexer.NextToken();
            var proto = ParsePrototype();
            var block = ParseBlock();
            if (block == null)
                return LogError("Function Definition Failed");
            return visitor.Visit(new FunctionAST(proto, block));

        }

        public ExprAST ParseBlock()
        {
            if (lexer.CurrentToken.type != TokenType.LeftCurlyBrace)
                return LogError("Expected '{'");
            ///Consume '{'
            lexer.NextToken();
            List<ExprAST> exprs = new List<ExprAST>();
            while(lexer.NextToken().type != TokenType.RightCurlyBrace)
            {
               var line = ParsePrimary();
               if (line == null)
                    return LogError("Failed to parse block");
                exprs.Add(line);
            }
            lexer.NextToken();
            return new BlockExprAST(exprs);
        }

        public ExprAST ParsePrototype()
        {
            if (lexer.CurrentToken.type != TokenType.Type)
                return LogError("Expected return type in function definition");
            var type = TypeHelper.ConvertStringToType(lexer.CurrentToken.value);
            lexer.NextToken();

            if (lexer.CurrentToken.type != TokenType.Identifier)
                return LogError("Expected function name in prototype");

            string FnName = lexer.CurrentToken.value;
            lexer.NextToken();
            if (lexer.CurrentToken.type != TokenType.LeftParenthesis)
                return LogError("Expected '(' in function prototype");

            List<ArgTypePair> Args = new List<ArgTypePair>();
            while(lexer.NextToken().type != TokenType.RightParenthesis)
            {
                if (lexer.CurrentToken.type != TokenType.Type)
                    return LogError("Expected a type for function parameter");
                var argType = TypeHelper.ConvertStringToType(lexer.CurrentToken.value);
                ///Consume type token
                lexer.NextToken();
                if (lexer.CurrentToken.type != TokenType.Identifier)
                    return LogError("Expected an parameter name.");
                var name = lexer.CurrentToken.value;
                Args.Add(new ArgTypePair(argType, name));
                //Consume the , or )
                lexer.NextToken();
                if (lexer.CurrentToken.type == TokenType.RightParenthesis)
                    break;
                if (lexer.CurrentToken.type != TokenType.ArgSeperator)
                    return LogError("Expected a ',' to separate the args.");
                }
            if (lexer.CurrentToken.type != TokenType.RightParenthesis)
                LogError("Expected a ')' ");
            //Consume ')'
            lexer.NextToken();
            return new PrototypeAST(type, FnName, Args);
        }

        //public ExprAST ReturnTypedVariable(Type type, string name, string value="")
        //{
        //    if
        //}

    }
}