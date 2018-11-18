using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathParserLib.Operations
{
    public class Add : Operation
    {
        public override Result Evaluate(Result left, Result right)
        {
            if (!left.IsNumber())
            {
                return Result.Unexpected("Expected a number on left side and got: " + left.ToString());
            }

            if (!right.IsNumber())
            {
                return Result.Unexpected("Expected a number on right side and got: " + right.ToString());
            }

            return left + right;
        }

        public override int GetPriority()
        {
            return 1;
        }
    }

    public class Subtract : Operation
    {
        public override Result Evaluate(Result left, Result right)
        {
            if (!left.IsNumber())
            {
                return Result.Unexpected("Expected a number on left side and got: " + left.ToString());
            }

            if (!right.IsNumber())
            {
                return Result.Unexpected("Expected a number on right side and got: " + right.ToString());
            }

            return left - right;
        }

        public override int GetPriority()
        {
            return 1;
        }
    }

    public class Multiply : Operation
    {
        public override Result Evaluate(Result left, Result right)
        {
            if (!left.IsNumber())
            {
                return Result.Unexpected("Expected a number on left side and got: " + left.ToString());
            }

            if (!right.IsNumber())
            {
                return Result.Unexpected("Expected a number on right side and got: " + right.ToString());
            }

            return left * right;
        }

        public override int GetPriority()
        {
            return 2;
        }
    }

    public class Divide : Operation
    {

        public override Result Evaluate(Result left, Result right)
        {
            if (!left.IsNumber())
            {
                return Result.Unexpected("Expected a number on left side and got: " + left.ToString());
            }

            if (!right.IsNumber())
            {
                return Result.Unexpected("Expected a number on right side and got: " + right.ToString());
            }

            if (right.IsZero())
            {
                return Result.Unexpected("Division by zero");
            }

            return left / right;
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
