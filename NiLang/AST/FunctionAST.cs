using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class FunctionAST : ExprAST
    {
        public PrototypeAST Proto { get; private set; }
        public ExprAST Body { get; private set; }
        public override ExprType NodeType { get; protected set; }
        public FunctionAST(ExprAST proto, ExprAST body)
        {
            this.Proto = (PrototypeAST)proto;
            this.Body = body;
            this.NodeType = ExprType.FunctionExpr;
        }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.FunctionASTVisit(this);
        }
        
    }
}
