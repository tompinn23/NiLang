using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class BinaryOpExprAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public ExprAST Lhs { get; private set; }
        public ExprAST Rhs { get; private set; }

        public BinaryOpExprAST(char op, ExprAST lhs, ExprAST rhs)
        {
            switch(op)
            {
                case '+':
                    this.NodeType = ExprType.AddExpr;
                    break;
                case '-':
                    this.NodeType = ExprType.SubtractExpr;
                    break;
                case '*':
                    this.NodeType = ExprType.MultExpr;
                    break;
                case '/':
                    this.NodeType = ExprType.DivideExpr;
                    break;
                case '<':
                    this.NodeType = ExprType.LessThanExpr;
                    break;
                default:
                    throw new ArgumentException("op " + op + " is not a valid operator");
            }
            this.Lhs = lhs;
            this.Rhs = rhs;
        }
        public BinaryOpExprAST Accept(Visitor visitor)
        {
            return visitor.BinaryOpASTVisit(this);
        }
    }
}
