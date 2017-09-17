using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public abstract class ExprAST
    {
        public abstract ExprType NodeType { get; protected set; }
    }
}
