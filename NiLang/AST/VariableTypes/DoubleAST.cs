namespace Nilang.AST.VariableTypes
{
    public sealed class DoubleAST : ExprAST
    {
        public double Value { get; private set; }

        public DoubleAST(double v)
        {
            this.Value = v;
            this.NodeType = ExprType.DoubleExpr;
        }

        public override ExprType NodeType { get; protected set; }

        public override ExprAST Accept(Visitor visitor)
        {
            return visitor.DoubleASTVisit(this);
        }
    }
}