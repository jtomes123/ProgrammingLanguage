using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathParserLib.Operations
{
    public class Add : Operation
    {
        public Add(Environment e) : base(e) { }

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
        public Subtract(Environment e) : base(e) { }

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
        public Multiply(Environment e) : base(e) { }

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
        public Divide(Environment e) : base(e) { }

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

    public class Print : Operation
    {
        public Print(Environment e) : base(e) { }

        public override Result Evaluate(Result left, Result right)
        {
            if (right.IsNumber())
            {
                environment.Print(right.GetNumber().ToString());
                return Result.Void();
            }
            else if (right.IsText())
            {
                environment.Print(right.GetText());
                return Result.Void();
            }

            return Result.Unexpected("Expected a number or string on right side and got: " + right.ToString());
        }

        public override int GetAmountOfArgumentsOnLeft()
        {
            return 0;
        }
    }

    public class SetVar : Operation
    {
        public SetVar(Environment e) : base(e)
        {
        }

        public override Result Evaluate(Result left, Result right)
        {
            if (left.IsText() && right != null)
            {
                environment.SetVar(left.GetText(), right);
                return Result.Void();
            }

            return Result.Unexpected("Expected a string on the left side and got: " + left);
        }
        public override int GetPriority()
        {
            return 0;
        }
    }

    public class GetVar : Operation
    {
        public GetVar(Environment e) : base(e)
        {
        }

        public override Result Evaluate(Result left, Result right)
        {
            if (left.IsText())
            {
                return environment.GetVar(left.GetText());
            }

            return Result.Unexpected("Expected a string on the left side and got: " + left);
        }

        public override int GetAmountOfArgumentsOnRight()
        {
            return 0;
        }
        public override int GetPriority()
        {
            return 5;
        }
    }
}
