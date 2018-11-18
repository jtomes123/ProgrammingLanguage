using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathParserLib.Operations;

namespace MathParserLib
{
    public class Parser
    {

    }
    public class TextProcessor
    {
        static char[] numeralChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static char[] letterChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        static char[] symbolChars = new char[] { '(', ')' };
        static char[] operatorChars = new char[] { '+', '-', '*', '/', 'p', '<', '>' };
        public static Queue<Word> ProcessString(string str)
        {
            Queue<Word> temp = new Queue<Word>();
            Queue<char> chars = new Queue<char>();
            foreach (char c in str)
            {
                chars.Enqueue(c);
            }

            while (chars.Count > 0)
            {
                if (chars.Peek() == ' ')
                {
                    chars.Dequeue();
                    continue;
                }

                if (isNumeral(chars.Peek()))
                {
                    string word = String.Empty;
                    while (chars.Count > 0 && isNumeral(chars.Peek()))
                    {
                        word += chars.Dequeue();
                    }
                    temp.Enqueue(new Word(Word.CharacterType.Number, int.Parse(word)));
                    continue;
                }

                if (isSymbol(chars.Peek()))
                {
                    switch (chars.Dequeue())
                    {
                        case '(':
                            temp.Enqueue(new Word(Word.CharacterType.Symbol, null, "", null, Word.SymbolType.ParenthesesOpen));
                            break;
                        case ')':
                            temp.Enqueue(new Word(Word.CharacterType.Symbol, null, "", null, Word.SymbolType.ParenthesesClose));
                            break;
                    }
                    continue;
                }

                if (isOperator(chars.Peek()))
                {
                    switch (chars.Dequeue())
                    {
                        case '+':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.Plus, null));
                            break;
                        case '-':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.Minus, null));
                            break;
                        case '*':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.Multiply, null));
                            break;
                        case '/':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.Divide, null));
                            break;
                        case 'p':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.Print, null));
                            break;
                        case '<':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.SetVar, null));
                            break;
                        case '>':
                            temp.Enqueue(new Word(Word.CharacterType.Operator, null, "", Word.OperatorType.GetVar, null));
                            break;

                    }
                    continue;
                }
                if (isLetter(chars.Peek()))
                {
                    string word = String.Empty;
                    while (chars.Count > 0 && !isOperator(chars.Peek()) && chars.Peek() != ' ')
                    {
                        word += chars.Dequeue();
                    }
                    temp.Enqueue(new Word(Word.CharacterType.Text, null, word));
                    continue;
                }
                temp.Enqueue(new Word(Word.CharacterType.Unexpected, null, chars.Dequeue().ToString()));
            }
            return temp;
        }
        static bool isNumeral(char c)
        {
            return numeralChars.Contains(c);
        }
        static bool isSymbol(char c)
        {
            return symbolChars.Contains(c);
        }
        static bool isOperator(char c)
        {
            return operatorChars.Contains(c);
        }
        static bool isLetter(char c)
        {
            return letterChars.Contains(c);
        }
    }
    public class Chunker
    {
        static Add add;
        static Subtract subtract;
        static Multiply multiply;
        static Divide divide;
        static Print print;
        static GetVar getVar;
        static SetVar setVar;

        public static void SetEnvironment(Environment e)
        {
            add = new Add(e);
            subtract = new Subtract(e);
            multiply = new Multiply(e);
            divide = new Divide(e);
            print = new Print(e);
            getVar = new GetVar(e);
            setVar = new SetVar(e);
        }

        public static Chunk Chunkify(Queue<Word> input)
        {
            Queue<object> temp = new Queue<object>();
            foreach (var t in input)
            {
                temp.Enqueue(t);
            }

            return Chunkify(temp);
        }

        public static Chunk Chunkify(Queue<object> input)
        {
            Chunk c = null;

            if (input.Count < 1)
            {
                return Chunk.Empty;
            }
            /*
            Stack<Chunk> pile = new Stack<Chunk>();
            while (words.Count > 0)
            {
                Word w = words.Dequeue();
                
                switch (w.GetCharacterType())
                {
                    case Word.CharacterType.Number:
                        pile.Push(new Chunk(w));
                        break;
                    case Word.CharacterType.Text:
                        pile.Push(new Chunk(w));
                        break;
                    case Word.CharacterType.Operator:
                        switch (w.GetOperator().GetValueOrDefault())
                        {
                            case Word.OperatorType.Empty:
                                //TODO: Handle error
                                break;
                            case Word.OperatorType.Plus:
                                Stack<Chunk> right = new Stack<Chunk>();
                                for (int i = 0; i < add.GetAmountOfArgumentsOnRight(); i++)
                                {
                                    if(words.Count < 0)
                                    {
                                        break;
                                        //TODO: Handle error
                                    }
                                    right.Push(new Chunk(words.Dequeue()));
                                }

                                c = new Chunk(add, pile.ToArray(), right.ToArray());
                                break;
                            case Word.OperatorType.Minus:
                                break;
                            case Word.OperatorType.Multiply:
                                break;
                            case Word.OperatorType.Divide:
                                break;
                            default:
                                break;
                        }
                        break;
                    case Word.CharacterType.Symbol:
                        break;
                    case Word.CharacterType.Unexpected:
                        break;
                    default:
                        break;
                }               
            }
            */

            //Convert word queue into object array
            object[] expression = input.ToArray();

            //Process expressions in parentheses into single chunks
            while (ContainsOpenParentheses(expression))
            {
                expression = ProcessParentheses(expression);
            }



            // Check if the expression contains any operators
            while (ContainsOperators(expression))
            {
                // Find highest priority of operator in the set
                int? maxPriority = null;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] is Word)
                    {
                        Word word = (Word)expression[i];
                        if (word.GetCharacterType() == Word.CharacterType.Operator)
                        {
                            int? priority = OperatorTypeToOperation(word.GetOperator().GetValueOrDefault())?.GetPriority();
                            if (priority != null)
                            {
                                if (maxPriority == null || priority > maxPriority)
                                    maxPriority = priority;
                            }
                        }
                    }
                }

                if (maxPriority == null)
                    break;

                while (ContainsOperators(expression, maxPriority.Value))
                {
                    // Go from left to right and process operators with highest priority
                    for (int i = 0; i < expression.Length; i++)
                    {
                        if (expression[i] is Word)
                        {
                            Word word = (Word)expression[i];
                            if (word.GetCharacterType() == Word.CharacterType.Operator
                                && OperatorTypeToOperation(word.GetOperator().GetValueOrDefault()).GetPriority() == maxPriority.Value)
                            {
                                expression = ProcessOperator(expression, i);
                                break;
                            }
                        }
                    }
                }
            }
            c = (Chunk)expression.First();

            return c;
        }

        static object[] ProcessOperator(object[] expression, int index)
        {
            Operation operation = OperatorTypeToOperation(((Word)expression[index]).GetOperator().GetValueOrDefault());

            int startIndex = index - operation.GetAmountOfArgumentsOnLeft();
            int endIndex = index + operation.GetAmountOfArgumentsOnRight();
            int length = endIndex - startIndex;

            Chunk[] left = new Chunk[operation.GetAmountOfArgumentsOnLeft()], right = new Chunk[operation.GetAmountOfArgumentsOnRight()];
            string errorMessage = String.Empty;
            if (startIndex < 0)
            {
                errorMessage = "Invalid amount of arguments on left side.";
            }
            else if (endIndex >= expression.Length)
            {
                errorMessage = "Invalid amount of arguments on right side.";
            }
            else
            {
                // Process left side
                for (int i = startIndex; i < index; i++)
                {
                    if (expression[i] is Word)
                    {
                        Word word = (Word)expression[i];
                        if (word.GetCharacterType() == Word.CharacterType.Text || word.GetCharacterType() == Word.CharacterType.Number)
                        {
                            left[i - startIndex] = new Chunk(word);
                        }
                        else
                        {
                            // Thrown an error, invalid syntax, unexpected operator, symbol or unexpected error somewhere else
                            errorMessage = "Invalid syntax: Unexpected " + word.GetCharacterType();
                        }
                    }
                    else if (expression[i] is Chunk)
                    {
                        left[i - startIndex] = (Chunk)expression[i];
                    }

                }

                // Process right side
                for (int i = index + 1; i <= endIndex; i++)
                {
                    if (expression[i] is Word)
                    {
                        Word word = (Word)expression[i];
                        if (word.GetCharacterType() == Word.CharacterType.Text || word.GetCharacterType() == Word.CharacterType.Number)
                        {
                            right[i - 1 - index] = new Chunk(word);
                        }
                        else
                        {
                            // Thrown an error, invalid syntax, unexpected operator, symbol or unexpected error somewhere else
                            errorMessage = "Invalid syntax: Unexpected " + word.GetCharacterType();
                        }
                    }
                    else if (expression[i] is Chunk)
                    {
                        right[i - 1 - index] = (Chunk)expression[i];
                    }
                }
            }
            Chunk c;
            if (errorMessage == String.Empty)
                c = new Chunk(operation, left, right);
            else
                c = new ProgramException(errorMessage);
            object[] processedExpression = new object[expression.Length - length + 1];
            for (int i = 0; i < expression.Length; i++)
            {
                if (i > endIndex)
                {
                    processedExpression[i - endIndex] = expression[i];
                }
                else if (i > startIndex)
                {
                    continue;
                }
                else if (i == startIndex)
                {
                    processedExpression[i] = c;
                }
                else
                {
                    processedExpression[i] = expression[i];
                }
            }
            return processedExpression;
        }

        static object[] ProcessParentheses(object[] expression)
        {
            int? startIndex, endIndex, currentLevel = 0;
            startIndex = endIndex = null;
            string errorMessage = String.Empty;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] is Word)
                {
                    Word w = (Word)expression[i];
                    if (w.GetCharacterType() == Word.CharacterType.Symbol)
                    {
                        if (w.GetSymbol() == Word.SymbolType.ParenthesesOpen)
                        {
                            if (startIndex == null)
                            {
                                startIndex = i;
                                currentLevel++;
                            }
                            else
                            {
                                currentLevel++;
                            }
                        }
                        else if (w.GetSymbol() == Word.SymbolType.ParenthesesClose)
                        {
                            currentLevel--;

                            if (currentLevel == 0)
                            {
                                endIndex = i;
                                break;
                            }
                        }
                    }
                }
                // Handle no open only closed
                if (currentLevel < 0)
                    errorMessage = "Unexpected \")\" are you missing \"(\"?";

            }

            int length = endIndex.Value - startIndex.Value;
            object[] processedExpression = new object[expression.Length - length];
            Chunk c = null;
            if (startIndex != null && endIndex != null)
            {
                
                
                Queue<object> subExpression = new Queue<object>();
                
                for (int i = startIndex.Value + 1; i < endIndex.Value; i++)
                {
                    subExpression.Enqueue(expression[i]);
                }
                c = Chunkify(subExpression);
            }
            // Return an exception if there is an issue
            if(errorMessage != String.Empty)
                c = new ProgramException(errorMessage);

            for (int i = 0; i < expression.Length; i++)
            {
                if (i > endIndex.Value)
                {
                    processedExpression[i - endIndex.Value] = expression[i];
                }
                else if (i > startIndex.Value)
                {
                    continue;
                }
                else if (i == startIndex.Value)
                {
                    processedExpression[i] = c;
                }
                else
                {
                    processedExpression[i] = expression[i];
                }
            }
            return processedExpression;


        }

        static bool ContainsOperators(object[] expression)
        {
            foreach (object o in expression)
            {
                if (o is Word)
                {
                    Word w = (Word)o;
                    if (w.GetCharacterType() == Word.CharacterType.Operator)
                        return true;
                }
            }
            return false;
        }

        static bool ContainsOperators(object[] expression, int priority)
        {
            foreach (object o in expression)
            {
                if (o is Word)
                {
                    Word w = (Word)o;
                    if (w.GetCharacterType() == Word.CharacterType.Operator && OperatorTypeToOperation(w.GetOperator().GetValueOrDefault()).GetPriority() == priority)
                        return true;
                }
            }
            return false;
        }

        static bool ContainsOpenParentheses(object[] expression)
        {
            foreach (object o in expression)
            {
                if (o is Word)
                {
                    Word w = (Word)o;
                    if (w.GetCharacterType() == Word.CharacterType.Symbol && w.GetSymbol() == Word.SymbolType.ParenthesesOpen)
                        return true;
                }
            }
            return false;
        }

        static Operation OperatorTypeToOperation(Word.OperatorType type)
        {
            switch (type)
            {
                case Word.OperatorType.Empty:
                    break;
                case Word.OperatorType.Plus:
                    return add;
                case Word.OperatorType.Minus:
                    return subtract;
                case Word.OperatorType.Multiply:
                    return multiply;
                case Word.OperatorType.Divide:
                    return divide;
                case Word.OperatorType.Print:
                    return print;
                case Word.OperatorType.SetVar:
                    return setVar;
                case Word.OperatorType.GetVar:
                    return getVar;
            }

            return null;
        }
    }
    public class Chunk
    {
        Operation operation;
        Word word;
        Chunk[] left;
        Chunk[] right;

        protected Chunk()
        {

        }

        public Chunk(Word w)
        {
            word = w;
        }

        public Chunk(Operation o, Chunk[] left, Chunk[] right)
        {
            operation = o;
            this.left = left;
            this.right = right;
        }

        public virtual Result Evaluate()
        {
            if (operation == null)
            {
                if (word != null)
                    return (Result)word;

                return Result.Unexpected("Encountered an empty chunk.");
            }

            if (left == null || right == null)
                return Result.Unexpected("Encountered a chunk missing a left or right side.");

            return operation.Evaluate((Result)left.FirstOrDefault(), (Result)right.FirstOrDefault());
        }

        public static Chunk Empty
        {
            get
            {
                return new Chunk();
            }
        }
    }

    public abstract class Environment
    {
        Dictionary<string, Result> memory = new Dictionary<string, Result>();
        public abstract void Print(string text);
        public void SetVar(string varname, Result r)
        {
            if (memory.ContainsKey(varname))
            {
                memory[varname] = r;
            }
            else
            {
                memory.Add(varname, r);
            }
        }

        public Result GetVar(string varname)
        {
            if (memory.ContainsKey(varname))
            {
                return memory[varname];
            }
            else
            {
                return Result.Unexpected("You are trying to access undefined variable \"" + varname + "\".");
            }
        }
    }

    public class ProgramException : Chunk
    {
        string errorMessage;
        public ProgramException(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
        public override Result Evaluate()
        {
            return Result.Unexpected(errorMessage);
        }
    }

    public abstract class Operation
    {
        protected Environment environment;
        public Operation(Environment e)
        {
            environment = e;
        }


        public abstract Result Evaluate(Result left, Result right);
        public virtual int GetAmountOfArgumentsOnLeft()
        {
            return 1;
        }
        public virtual int GetAmountOfArgumentsOnRight()
        {
            return 1;
        }
        public virtual int GetPriority()
        {
            return 0;
        }
    }

    public class Result
    {
        public enum ResultType
        {
            Void, Number, Text, Unexpected
        }
        ResultType resultType;
        int resultNumber;
        string resultText;
        string errorMessage;

        #region Constructors
        private Result()
        {

        }

        public static Result Numeric(int number)
        {
            return new Result()
            {
                resultType = ResultType.Number,
                resultNumber = number
            };
        }

        public static Result Text(string text)
        {
            return new Result()
            {
                resultType = ResultType.Text,
                resultText = text
            };
        }

        public static Result Unexpected(string errorMessage)
        {
            return new Result()
            {
                resultType = ResultType.Unexpected,
                errorMessage = errorMessage
            };
        }

        public static Result Void()
        {
            return new Result()
            {
                resultType = ResultType.Void
            };
        }
        #endregion

        #region Convesion and Operator Overloading
        public static Result operator +(Result a, Result b)
        {
            return Result.Numeric(a.resultNumber + b.resultNumber);
        }
        public static Result operator -(Result a, Result b)
        {
            return Result.Numeric(a.resultNumber - b.resultNumber);
        }
        public static Result operator *(Result a, Result b)
        {
            return Result.Numeric(a.resultNumber * b.resultNumber);
        }
        public static Result operator /(Result a, Result b)
        {
            return Result.Numeric(a.resultNumber / b.resultNumber);
        }
        public static explicit operator Result(Chunk w)
        {
            if (w == null)
                return null;
            return w.Evaluate();
        }

        public static explicit operator Result(Word w)
        {
            Result t = new Result();
            switch (w.GetCharacterType())
            {
                case Word.CharacterType.Number:
                    int? val = w.GetNumber();

                    if (!val.HasValue)
                    {
                        t.resultType = ResultType.Unexpected;
                        t.errorMessage = w.GetErrorMessage();
                        break;
                    }

                    t.resultNumber = val.Value;
                    t.resultType = ResultType.Number;
                    break;
                case Word.CharacterType.Text:
                    string str = w.GetText();
                    if (str == String.Empty)
                    {
                        t.resultType = ResultType.Unexpected;
                        t.errorMessage = w.GetErrorMessage();
                        break;
                    }

                    t.resultText = str;
                    t.resultType = ResultType.Text;
                    break;
                case Word.CharacterType.Operator:
                    t.resultType = ResultType.Unexpected;
                    t.errorMessage = "Expected a number or text but got an operator.";
                    break;
                case Word.CharacterType.Symbol:
                    t.resultType = ResultType.Unexpected;
                    t.errorMessage = "Expected a number or text but got a symbol.";
                    break;
                case Word.CharacterType.Unexpected:
                    t.resultType = ResultType.Unexpected;
                    t.errorMessage = "Unexpected error while processing input.";
                    break;
                default:
                    break;
            }
            return t;
        }
        #endregion

        #region The rest
        public bool IsNumber()
        {
            return resultType == ResultType.Number;
        }
        public bool IsZero()
        {
            return resultType == ResultType.Number && resultNumber == 0;
        }


        public bool IsText()
        {
            return resultType == ResultType.Text;
        }

        public bool IsVoid()
        {
            return resultType == ResultType.Void;
        }

        public bool IsUnexpected()
        {
            return resultType == ResultType.Unexpected;
        }
        #endregion

        public string GetText()
        {
            return resultText;
        }
        public int GetNumber()
        {
            return resultNumber;
        }
        public override string ToString()
        {
            return String.Format("Result ( Type: {0}, Number: {1}, Result Text: {3}, Error Message: {2} )", resultType.ToString(), resultNumber.ToString(), errorMessage, resultText);
        }
    }

    public class Word
    {
        CharacterType type;
        int? number;
        string text, errorMessage;
        OperatorType? operatorType = OperatorType.Empty;
        SymbolType? symbolType = SymbolType.Empty;

        public Word(CharacterType type, int? number = null, string text = "", OperatorType? operatorType = null, SymbolType? symbolType = null, string error = "")
        {
            this.type = type;
            this.number = number;
            this.text = text;
            this.operatorType = operatorType;
            this.symbolType = symbolType;
        }

        public enum CharacterType
        {
            Number, Text, Operator, Symbol, Unexpected
        }
        public enum OperatorType
        {
            Empty, Plus, Minus, Multiply, Divide, SetVar, GetVar, Print
        }
        public enum SymbolType
        {
            Empty, ParenthesesOpen, ParenthesesClose
        }
        public int? GetNumber()
        {
            if (type != CharacterType.Number)
                return null;

            return number;
        }
        public string GetText()
        {
            if (type != CharacterType.Text)
                return String.Empty;

            return text;
        }
        public string GetErrorMessage()
        {
            return errorMessage;
        }
        public OperatorType? GetOperator()
        {
            if (type != CharacterType.Operator)
                return null;

            return operatorType;
        }
        public SymbolType? GetSymbol()
        {
            if (type != CharacterType.Symbol)
                return null;

            return symbolType;
        }
        public CharacterType GetCharacterType()
        {
            return type;
        }
    }
}
