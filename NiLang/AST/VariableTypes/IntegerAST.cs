using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class IntegerAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public long Value { get; private set; }

        public IntegerAST(long value)
        {
            Value = value;
            NodeType = ExprType.IntExpr;
        }
        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.IntegerASTVisit(this);
        }
    }
}
