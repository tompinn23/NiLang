using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class IdentifierExprAST : VariableExprAST
    {
        public override string Name { get; protected set; }
        public override ExprType NodeType { get; protected set; }
        public IdentifierExprAST(string name)
        {
            this.Name = name;
            this.NodeType = ExprType.IdentifierExpr;
        }
        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.IdentifierASTVisit(this);
        }
    }
}