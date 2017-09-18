using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang.AST
{
    public enum ExprType
    {
        //Variable sheeeeeet
        IdentifierExpr,
        VariableExpr,
        StringExpr,
        IntExpr,
        DoubleExpr,
        
        //Stufff for ops
        MultExpr,
        AddExpr,
        SubtractExpr,
        DivideExpr,

        // Function SHIIIIIT
        FunctionExpr,
        CallExpr,
        PrototypeExpr,
        LessThanExpr,
    }
}
