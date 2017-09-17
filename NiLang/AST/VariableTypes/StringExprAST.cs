using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST.VariableTypes
{
    public sealed class StringExprAST : VariableExprAST
    {
        public string Value { get; private set; }
        public override string Name { get; protected set; }
        public override ExprType NodeType { get; protected set; }

        public StringExprAST(string name, string value)
        {
            this.Name = name;
            this.Value = value;
            this.NodeType = ExprType.StringExpr;
        }
    }
}
