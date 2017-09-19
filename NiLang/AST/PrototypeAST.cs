using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class PrototypeAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public Type ReturnType { get; private set; }
        public string Name { get; private set; }
        public List<ArgTypePair> Args { get; private set; }

        public PrototypeAST(Type retType, string name, List<ArgTypePair> args)
        {
            this.ReturnType = retType;
            this.Name = name;
            this.Args = args;
            this.NodeType = ExprType.PrototypeExpr;
        }
        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.PrototypeASTVisit(this);
        }
    }
}
