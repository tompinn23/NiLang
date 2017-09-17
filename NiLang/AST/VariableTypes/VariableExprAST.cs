using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public abstract class VariableExprAST : ExprAST
    {
        public abstract string Name { get; protected set; }



    }
}
