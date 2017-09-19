using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class StringAST : ExprAST
    {
        public string Value { get; private set; }
        public override ExprType NodeType { get; protected set; }

        public StringAST(string name, string value)
        {
            this.Value = value;
            this.NodeType = ExprType.StringExpr;
        }
        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.StringVisit(this);
        }
    }
}
