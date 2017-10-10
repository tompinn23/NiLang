using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    sealed class ReturnAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }

        public Type type { get; private set; }
        public string value { get; private set; }

        public ReturnAST(Type type, string value)
        {
            this.type = type;
            this.value = value;
            this.NodeType = ExprType.ReturnExpr;
        }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.ReturnVisit(this);
        }
    }
}
