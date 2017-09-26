using Nilang.AST.VariableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class VariableExprAST : ExprAST
    {
        public string Name { get; private set; }
        private ExprAST v;
        public ExprAST Value { get
            {
                switch(type)
                {
                    case Type.Double:
                        return (DoubleAST)v;
                    case Type.Integer:
                        return (IntegerAST)v;
                    case Type.Bool:
                        return (BoolAST)v;
                    default:
                        return v;
                }
            }
            private set { v = value; } } 

        public override ExprType NodeType { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        private Type type;

        public VariableExprAST(string name, DoubleAST v)
        {
            this.Name = name;
            this.Value = v;
            this.type = Type.Double;
        }
        public VariableExprAST(string name, IntegerAST v)
        {
            this.Name = name;
            this.Value = v;
            this.type = Type.Integer;
        }

        public VariableExprAST(string name, BoolAST v)
        {
            this.Name = name;
            this.Value = v;
            this.type = Type.Bool;
        }


        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.VariableExprVisit(this);
        }
    }
}
