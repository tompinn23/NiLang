using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class CallExprAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public string Callee { get; private set; }
        public List<ExprAST> Args { get; private set; }
        public CallExprAST(string name, List<ExprAST> args)
        {
            this.Callee = name;
            this.Args = args;
            this.NodeType = ExprType.CallExpr;
        }
        public CallExprAST Accept(Visitor visitor)
        {
            return visitor.CallASTVisit(this);
        }
    }
}
