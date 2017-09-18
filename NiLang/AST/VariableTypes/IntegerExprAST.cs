using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class IntegerExprAST : VariableExprAST
    {
        public override string Name { get; protected set; }
        public override ExprType NodeType { get; protected set; }
        public long Value { get; private set; }
        public IntegerExprAST(string name, long value)
        {
            Name = name;
            Value = value;
            NodeType = ExprType.IntExpr;
        }
        public IntegerExprAST Accept(Visitor visitor)
        {
            return visitor.IntegerASTVisit(this);
        }
    }
}
