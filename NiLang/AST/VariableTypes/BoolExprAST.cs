using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    class BoolExprAST : VariableExprAST
    {
        public override string Name { get; protected set; }
        public bool Value { get; private set; }
        public override ExprType NodeType { get; protected set; }
        
        public BoolExprAST(string name, bool value)
        {
            this.Name = name;
            this.Value = value;
        }

        public BoolExprAST Accept(Visitor visitor)
        {
            return visitor.BoolASTVisit(this);
        }
    }
}
