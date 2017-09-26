using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class BoolAST : ExprAST
    { 
        public bool Value { get; private set; }
        public override ExprType NodeType { get; protected set; }
        
        public BoolAST(string name, bool value)
        {
            this.Value = value;
        }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.BoolASTVisit(this);
        }
    }
}
