using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilang
{
    public struct ArgTypePair
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }

        public ArgTypePair(Type type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }

    public enum Type
    {
        Integer,
        Int32,
        String,
        Double,
        Function,
        Void,
        Var,
        Bool,
    }

    public static class TypeHelper
    {
        public static Type ConvertStringToType(string type)
        {
            switch(type)
            {
                case "int":
                    return Type.Integer;
                case "int32":
                    return Type.Int32;
                case "string":
                    return Type.String;
                case "double":
                    return Type.Double;
                case "void":
                    return Type.Void;
                case "var":
                    return Type.Var;
            }
            return Type.Var;
        }
    }
}
