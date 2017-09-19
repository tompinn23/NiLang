using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    class BoolExprAST : ExprAST
    { 
        public bool Value { get; private set; }
        public override ExprType NodeType { get; protected set; }
        
        public BoolExprAST(string name, bool value)
        {
            this.Value = value;
        }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.BoolASTVisit(this);
        }
    }
}
