using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public sealed class BlockExprAST : ExprAST
    {
        public override ExprType NodeType { get; protected set; }
        public List<ExprAST> Exprs { get; private set; }

        public BlockExprAST(List<ExprAST> exprs)
        {
            Exprs = exprs;
        }
        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.BlockASTVisit(this);
        }
    }
}
