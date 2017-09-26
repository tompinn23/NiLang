using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class ExternAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public string fnName { get; private set; }
        public Type ret { get; private set; }
        public List<ArgTypePair> args { get; private set; }

        public ExternAST(string name, Type ret, List<ArgTypePair> args)
        {
            this.fnName = name;
            this.ret = ret;
            this.args = args;
            this.NodeType = ExprType.ExternExpr;
        }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.ExternASTVisit(this);
        }
    }
}
