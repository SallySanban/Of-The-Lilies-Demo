using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Dialogue.LogicalLines
{
    public static class LogicalLineUtils
    {
        public static class Encapsulation
        {
            public struct EncapsulatedData
            {
                public bool isNull => lines == null;
                public List<string> lines;
                public int startingIndex;
                public int endingIndex;
                public int fileStartIndex;
            }

            private const char encapsulationStart = '{';
            private const char encapsulationEnd = '}';

            public static EncapsulatedData RipEncapsulationData(Conversation conversation, int startingIndex, bool ripHeaderAndEncapsulators = false, int parentStartingIndex = 0)
            {
                int encapsulationDepth = 0;

                EncapsulatedData data = new EncapsulatedData { lines = new List<string>(), startingIndex = (startingIndex + parentStartingIndex), endingIndex = 0 };

                for (int i = startingIndex; i < conversation.countLines; i++)
                {
                    string line = conversation.GetLines()[i];

                    if(ripHeaderAndEncapsulators || (encapsulationDepth > 0 && !IsEncapsulationEnd(line)))
                    {
                        data.lines.Add(line);
                    }

                    if (IsEncapsulationStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if (IsEncapsulationEnd(line))
                    {
                        encapsulationDepth--;

                        if (encapsulationDepth == 0)
                        {
                            data.endingIndex = (i + parentStartingIndex);
                            break;
                        }
                    }
                }

                return data;
            }

            public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(encapsulationStart);

            public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(encapsulationEnd);
        }

        public static class Expressions
        {
            public static HashSet<string> operators = new HashSet<string>()
            {
                "=",
                "-",
                "+",
                "*",
                "/",
                "-=",
                "+=",
                "*=",
                "/="
            };

            public static readonly string regexArithmetic = @"([-+*/=]=?)";
            public static readonly string regexOperatorLine = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";

            public static object CalculateValue(string[] expressionParts)
            {
                List<string> operandStrings = new List<string>();
                List<string> operatorStrings = new List<string>();
                List<object> operands = new List<object>();

                for(int i = 0; i < expressionParts.Length; i++)
                {
                    string part = expressionParts[i].Trim();

                    if(part == string.Empty)
                    {
                        continue;
                    }

                    if (operators.Contains(part))
                    {
                        operatorStrings.Add(part);
                    }
                    else
                    {
                        operandStrings.Add(part);
                    }

                    foreach(string operandString in operandStrings)
                    {
                        operands.Add(ExtractValue(operandString));
                    }

                    CalculateValueDivMul(operatorStrings, operands);
                    CalculateValueAddSub(operatorStrings, operands);
                }

                return operands[0];
            }

            private static void CalculateValueDivMul(List<string> operatorStrings, List<object> operands)
            {
                for(int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];

                    if(operatorString == "*" || operatorString == "/")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if(operatorString == "*")
                        {
                            operands[i] = leftOperand * rightOperand;
                        }
                        else
                        {
                            if(rightOperand == 0)
                            {
                                Debug.LogError("Cannot divide by 0");
                                return;
                            }

                            operands[i] = leftOperand / rightOperand;
                        }

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);

                        i--;
                    }
                }
            }

            private static void CalculateValueAddSub(List<string> operatorStrings, List<object> operands)
            {
                for(int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];

                    if(operatorString == "+" || operatorString == "-")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if(operatorString == "+")
                        {
                            operands[i] = leftOperand + rightOperand;
                        }
                        else
                        {
                            operands[i] = leftOperand - rightOperand;
                        }

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);

                        i--;
                    }
                }
            }

            private static object ExtractValue(string value)
            {
                bool negate = false;

                if (value.StartsWith("!"))
                {
                    negate = true;
                    value = value.Substring(1);
                }

                if (value.StartsWith(VariableStore.variableId))
                {
                    string variableName = value.TrimStart(VariableStore.variableId);

                    if (!VariableStore.HasVariable(variableName))
                    {
                        Debug.LogError($"Variable {variableName} does not exist!");
                        return null;
                    }

                    VariableStore.TryGetValue(variableName, out object val);

                    if(val is bool boolValue && negate)
                    {
                        return !boolValue;
                    }

                    return val;
                }
                else if (value.StartsWith('\"') && value.EndsWith('\"'))
                {
                    value = TagManager.Inject(value);
                    return value.Trim('"');
                }
                else
                {
                    if(int.TryParse(value, out int intValue))
                    {
                        return intValue;
                    }
                    else if(float.TryParse(value, out float floatValue))
                    {
                        return floatValue;
                    }
                    else if(bool.TryParse(value, out bool boolValue))
                    {
                        return negate ? !boolValue : boolValue;
                    }
                    else
                    {
                        value = TagManager.Inject(value);
                        return value;
                    }
                }
            }
        }

        public static class Conditions
        {
            public static readonly string regexConditionalOperators = @"(==|!=|<=|>=|<|>|&&|\|\|)";

            private delegate bool OperatorFunc<T>(T left, T right);

            private static Dictionary<string, OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>()
            {
                { "&&", (left, right) => left && right },
                { "||", (left, right) => left || right },
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right }
            };

            private static Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
            {
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right },
                { ">", (left, right) => left > right },
                { ">=", (left, right) => left >= right },
                { "<", (left, right) => left < right },
                { "<=", (left, right) => left <= right },
            };

            private static Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>()
            {
                { "==", (left, right) => left == right },
                { "!=", (left, right) => left != right },
                { ">", (left, right) => left > right },
                { ">=", (left, right) => left >= right },
                { "<", (left, right) => left < right },
                { "<=", (left, right) => left <= right },
            };

            public static bool EvaluateCondition(string condition)
            {
                condition = TagManager.Inject(condition);

                string[] parts = Regex.Split(condition, regexConditionalOperators).Select(p => p.Trim()).ToArray();

                for(int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\"") && parts[i].EndsWith("\""))
                    {
                        parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                    }
                }

                if(parts.Length == 1)
                {
                    if (bool.TryParse(parts[0], out bool result))
                    {
                        return result;
                    }
                    else
                    {
                        Debug.LogError($"No condition found: {condition}");
                        return false;
                    }
                }
                else if(parts.Length == 3)
                {
                    return EvaluateExpression(parts[0], parts[1], parts[2]);
                }
                else
                {
                    Debug.LogError($"Unsupported condition format: {condition}");
                    return false;
                }
            }

            private static bool EvaluateExpression(string left, string op, string right)
            {
                if(bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                {
                    if (boolOperators.ContainsKey(op))
                    {
                        return boolOperators[op](leftBool, rightBool);
                    }
                }

                if(float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                {
                    if (floatOperators.ContainsKey(op))
                    {
                        return floatOperators[op](leftFloat, rightFloat);
                    }
                }

                if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                {
                    if (intOperators.ContainsKey(op))
                    {
                        return intOperators[op](leftInt, rightInt);
                    }
                }

                switch (op)
                {
                    case "==":
                        return left == right;
                    case "!=":
                        return left != right;
                    default:
                        throw new InvalidOperationException($"Unsupported operation: {op}");
                }
            }
        }
    }
}

