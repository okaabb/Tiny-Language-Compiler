using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        bool ma = false;
        bool exit = false;
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.exit = false;
            this.ma = false;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            if (!ma)
            {
                Errors.Error_List.Add("Parsing Error: Expected Main function" + "\r\n");

            }
            return root;
        }

        //hanna

        Node Program()
        {
            Node program = new Node("Program");
            // bool ma = false;
            if (InputPointer < TokenStream.Count)
            {
                if (IsDataType())
                {
                    if (Token_Class.String == TokenStream[InputPointer].token_type)
                    {
                        program.Children.Add(match(Token_Class.String));
                    }
                    else if (Token_Class.Integer == TokenStream[InputPointer].token_type)
                    {
                        program.Children.Add(match(Token_Class.Integer));
                    }
                    else if (Token_Class.Float == TokenStream[InputPointer].token_type)
                    {
                        program.Children.Add(match(Token_Class.Float));
                    }
                    if (Check(Token_Class.main))
                    {
                        program.Children.Add(Main());
                        ma = true;
                    }
                    else
                    {
                        InputPointer--;
                        program.Children.Clear();
                        program.Children.Add(FunctionStat());
                        program.Children.Add(Program());
                    }
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected DataType Or Function Statement" + "\r\n");
                InputPointer++;
                return null;
            }

            MessageBox.Show("Success");
            return program;
        }



        Node declaration()
        {
            Node dec = new Node("Declaration");

            if (Check(Token_Class.String))
            {
                dec.Children.Add(match(Token_Class.String));
            }
            else if (Check(Token_Class.Integer))
            {
                dec.Children.Add(match(Token_Class.Integer));
            }
            else if (Check(Token_Class.Float))
            {
                dec.Children.Add(match(Token_Class.Float));
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected DataType" + "\r\n");
                InputPointer++;
                return null;
            }
            dec.Children.Add(Ids());
            dec.Children.Add(match(Token_Class.Semicolon));

            return dec;
        }

        Node Ids()
        {
            Node ids = new Node("Identifiers");
            ids.Children.Add(match(Token_Class.Identifier));
            ids.Children.Add(Assign());
            ids.Children.Add(NextId());
            return ids;
        }

        Node Assign()
        {
            Node assign = new Node("Assign");
            if (Check(Token_Class.AssignOp))
            {
                assign.Children.Add(match(Token_Class.AssignOp));
                assign.Children.Add(Expression());
            }
            else
            {
                return null;
            }
            return assign;
        }

        Node NextId()
        {
            Node nx = new Node("Next Identifier");
            if (Check(Token_Class.Comma))
            {

                nx.Children.Add(match(Token_Class.Comma));
                nx.Children.Add(Ids());

            }
            else
            {
                return null;
            }
            return nx;
        }

        //nour
        Node Parameter()
        {

            Node parameter = new Node("Parameter");
            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
        Node Expression()
        {
            // write your code here to match statements


            Node expression = new Node("Expression");
            if (Check(Token_Class.String))
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else
            {
                expression.Children.Add(Equation());
            }
            return expression;
        }



        // osama
        bool IsDataType()
        {
            if (Check(Token_Class.Integer))
            {
                return true;
            }
            else if (Check(Token_Class.Float))
            {
                return true;
            }
            else if (Check(Token_Class.String))
            {
                return true;
            }
            return false;
        }
        Node DataType()
        {
            Node DataType = new Node("DataType");
            if (Check(Token_Class.Integer))
            {
                DataType.Children.Add(match(Token_Class.Integer));
            }
            else if (Check(Token_Class.Float))
            {
                DataType.Children.Add(match(Token_Class.Float));
            }
            else
            {
                DataType.Children.Add(match(Token_Class.String));
            }

            return DataType;
        }
        Node FunctionDec()
        {
            Node FunDec = new Node("FunctionDeclaration");
            if (InputPointer < TokenStream.Count)
            {
                FunDec.Children.Add(DataType());
                FunDec.Children.Add(match(Token_Class.Identifier));
                FunDec.Children.Add(match(Token_Class.LParanthesis));
                if (IsDataType())
                {
                    FunDec.Children.Add(Parameter());
                }
                while (!exit)
                {
                    if (Check(Token_Class.Comma))
                    {
                        FunDec.Children.Add(match(Token_Class.Comma));
                        FunDec.Children.Add(Parameter());
                    }
                    else
                    {
                        break;
                    }
                }
                FunDec.Children.Add(match(Token_Class.RParanthesis));
                return FunDec;
            }
            else return null;
        }
        Node FunctionStat()
        {
            Node FunStat = new Node("FunctionStatement");

            FunStat.Children.Add(FunctionDec());
            FunStat.Children.Add(function_body());
            return FunStat;
        }
        Node Repeat()
        {
            Node RepeatNode = new Node("Repeat");
            if (Check(Token_Class.Repeat))
            {
                RepeatNode.Children.Add(match(Token_Class.Repeat));
                int x = InputPointer;

                while (!exit)
                {
                    if (Check(Token_Class.Until))
                    {
                        RepeatNode.Children.Add(match(Token_Class.Until));
                        RepeatNode.Children.Add(ConditionStatement());
                        break;
                    }
                    else
                    {
                        RepeatNode.Children.Add(Statements());
                    }
                }
                if (exit)
                {
                    //InputPointer = x + 1;
                    Errors.Error_List.Add("Parsing Error: Expected until Statement" + "\r\n");
                    //return null;
                }
                return RepeatNode;
            }
            return null;
        }

        // Implement your logic here
        Node Statements()
        {
            Node statements = new Node("statements");

            // write your code here to match statements
            Node s1 = Statement();
            if (s1 == null)
            {
                InputPointer++;
                return null;
            }
            statements.Children.Add(s1);
            statements.Children.Add(State());

            return statements;
        }


        Node Statement()
        {
            Node statement = new Node("statement");

            // write your code here to match statements

            if (Check(Token_Class.Read))
            {
                statement.Children.Add(match(Token_Class.Read));
                statement.Children.Add(match(Token_Class.Identifier));
                statement.Children.Add(match(Token_Class.Semicolon));

            }
            else if (Check(Token_Class.If))
            {
                statement.Children.Add(IfStatement());
            }
            else if (Check(Token_Class.ElseIf))
            {
                statement.Children.Add(match(Token_Class.ElseIf));
                statement.Children.Add(ConditionStatement());
                statement.Children.Add(match(Token_Class.Then));
                statement.Children.Add(Statements());

            }
            else if (Check(Token_Class.Else))
            {
                statement.Children.Add(match(Token_Class.Else));
                statement.Children.Add(Statements());
            }

            else if (Check(Token_Class.Set))
            {
                statement.Children.Add(match(Token_Class.Set));
                statement.Children.Add(match(Token_Class.Identifier));
                statement.Children.Add(match(Token_Class.EqualOp));
                statement.Children.Add(Expression());
            }
            else if (Check(Token_Class.Repeat))
            {
                statement.Children.Add(Repeat());
            }
            else if (Check(Token_Class.Write))
            {
                statement.Children.Add(match(Token_Class.Write));
                if ((InputPointer < TokenStream.Count) && Token_Class.Endl == TokenStream[InputPointer].token_type)
                    statement.Children.Add(match(Token_Class.Endl));
                else
                    statement.Children.Add(Expression());

                statement.Children.Add(match(Token_Class.Semicolon));

            }
            else if (IsDataType())
            {
                InputPointer++;
                if (Check(Token_Class.main))
                {
                    InputPointer--;
                    statement.Children.Add(Main());

                }
                else
                {
                    InputPointer--;
                    statement.Children.Add(declaration());
                }


            }
            else if (Check(Token_Class.Identifier))
            {

                InputPointer++;
                if (Check(Token_Class.LParanthesis))
                {
                    InputPointer--;
                    statement.Children.Add(FunctionCall());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else
                {
                    InputPointer--;
                    statement.Children.Add(Assignment_Statement());

                }
            }
            else if (Check(Token_Class.Return))
            {
                statement.Children.Add(Return_stat());
            }


            else return null;
            return statement;
        }


        //haidy
        Node Assignment_Statement()
        {
            Node AssignmentStatement = new Node("AssignmentStatement");

            // write your code here to match statements

            AssignmentStatement.Children.Add(match(Token_Class.Identifier));
            AssignmentStatement.Children.Add(match(Token_Class.AssignOp));
            AssignmentStatement.Children.Add(Expression());
            AssignmentStatement.Children.Add(match(Token_Class.Semicolon));

            return AssignmentStatement;
        }
        Node Return_stat()
        {
            Node Returnstat = new Node("ReturnStatement");

            // write your code here to match statements
            Returnstat.Children.Add(match(Token_Class.Return));
            Returnstat.Children.Add(Expression());
            Returnstat.Children.Add(match(Token_Class.Semicolon));

            return Returnstat;
        }
        Node Main()
        {
            Node MAIN = new Node("Main");

            // write your code here to match statements

            MAIN.Children.Add(match(Token_Class.main));
            MAIN.Children.Add(match(Token_Class.LParanthesis));
            MAIN.Children.Add(match(Token_Class.RParanthesis));

            MAIN.Children.Add(function_body());

            return MAIN;
        }
        Node function_body()
        {
            Node functionbody = new Node("function_body");

            // write your code here to match statements
            functionbody.Children.Add(match(Token_Class.LCurlyBrackets));
            // write your code here to match statements        
            while (!exit)
            {
                if (InputPointer > TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Return Statement" + "\r\n");
                    InputPointer++;
                    return null;
                }
                if (Check(Token_Class.Return))
                {
                    functionbody.Children.Add(Return_stat());
                    break;
                }
                else
                {
                    functionbody.Children.Add(Statements());
                }
            }
            if (exit)
            {
                Errors.Error_List.Add("Parsing Error: Expected Return Statement" + "\r\n");
                // InputPointer++;
                return functionbody;
            }
            functionbody.Children.Add(match(Token_Class.RCurlyBrackets));

            return functionbody;
        }

        //mariam
        Node Term()
        {
            Node TermNode = new Node("Term");

            if (Check(Token_Class.Identifier))
            {
                TermNode.Children.Add(match(Token_Class.Identifier));
                if (Check(Token_Class.LParanthesis))
                {
                    InputPointer--;
                    TermNode.Children.Clear();
                    TermNode.Children.Add(FunctionCall());
                }
            }
            else if (Check(Token_Class.Number))
            {
                TermNode.Children.Add(match(Token_Class.Number));
            }


            return TermNode;
        }

        Node Equation()
        {

            Node equation = new Node("Equation");

            if (Check(Token_Class.Number) || Check(Token_Class.Identifier))
            {
                equation.Children.Add(Term());
            }
            else if (Check(Token_Class.LParanthesis))
            {
                equation.Children.Add(match(Token_Class.LParanthesis));
                equation.Children.Add(Equation());
                equation.Children.Add(match(Token_Class.RParanthesis));
            }
            equation.Children.Add(Eq());

            return equation;

        }
        Node Eq()
        {
            Node eq = new Node("Eq");

            if (Check(Token_Class.MultiplyOp) || Check(Token_Class.DivideOp) || Check(Token_Class.MinusOp) || Check(Token_Class.PlusOp))
            {
                eq.Children.Add(ArOp());
                eq.Children.Add(Equation());
                return eq;
            }
            else
            {
                return null;
            }
        }
        Node ArOp()
        {
            Node arop = new Node("ArOp");
            if (InputPointer < TokenStream.Count)
            {
                if (Check(Token_Class.PlusOp))
                {
                    arop.Children.Add(match(Token_Class.PlusOp));
                }
                else if (Check(Token_Class.MinusOp))
                {
                    arop.Children.Add(match(Token_Class.MinusOp));
                }
                else if (Check(Token_Class.MultiplyOp))
                {
                    arop.Children.Add(match(Token_Class.MultiplyOp));
                }
                else if (Check(Token_Class.DivideOp))
                {
                    arop.Children.Add(match(Token_Class.DivideOp));
                }

            }
            return arop;
        }

        Node IfStatement()
        {
            Node ifStatement = new Node("IfStatement");

            if (InputPointer < TokenStream.Count)
            {
                ifStatement.Children.Add(match(Token_Class.If));
                ifStatement.Children.Add(ConditionStatement());
                ifStatement.Children.Add(match(Token_Class.Then));


                bool Stop = false;
                while (!exit)
                {
                    if (Check(Token_Class.End))
                    {
                        ifStatement.Children.Add(match(Token_Class.End));
                        break;
                    }

                    else if (Stop == false && Check(Token_Class.ElseIf))
                    {
                        ifStatement.Children.Add(match(Token_Class.ElseIf));
                        ifStatement.Children.Add(ConditionStatement());
                        ifStatement.Children.Add(match(Token_Class.Then));
                    }
                    else if (Stop == false && Check(Token_Class.Else))
                    {
                        ifStatement.Children.Add(match(Token_Class.Else));
                        Stop = true;
                    }

                    else
                    {
                        ifStatement.Children.Add(Statements());

                    }
                }
            }
            return ifStatement;
        }


        Node FunctionCall()
        {
            Node FunctionCallNode = new Node("FunctionCall");

            FunctionCallNode.Children.Add(match(Token_Class.Identifier));
            FunctionCallNode.Children.Add(match(Token_Class.LParanthesis));

            int cont = 0;
            while (!exit)
            {
                if (Check(Token_Class.RParanthesis))
                {
                    FunctionCallNode.Children.Add(match(Token_Class.RParanthesis));

                    break;
                }
                else
                {
                    if (cont > 0)
                    {
                        FunctionCallNode.Children.Add(match(Token_Class.Comma));
                    }
                    FunctionCallNode.Children.Add(Expression());
                    ///

                }
                cont++;
            }
            return FunctionCallNode;
        }


        Node ConditionOperator()
        {
            Node ConditionOP = new Node("ConditionOperator");
            if (Check(Token_Class.LessThanOp))
            {
                ConditionOP.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (Check(Token_Class.GreaterThanOp))
            {
                ConditionOP.Children.Add(match(Token_Class.GreaterThanOp));

            }
            else if (Check(Token_Class.EqualOp))
            {
                ConditionOP.Children.Add(match(Token_Class.EqualOp));

            }
            else if (Check(Token_Class.NotEqualOp))
            {
                ConditionOP.Children.Add(match(Token_Class.NotEqualOp));
            }
            else
            {
                return null;
            }
            return ConditionOP;
        }

        Node Condition()
        {
            Node ConditionNode = new Node("Condition");
            ConditionNode.Children.Add(match(Token_Class.Identifier));
            ConditionNode.Children.Add(ConditionOperator());
            ConditionNode.Children.Add(Term());
            return ConditionNode;
        }


        Node ConditionStatement()
        {
            Node ConditionStatNode = new Node("ConditionStatement");
            ConditionStatNode.Children.Add(Condition());
            while (!exit)
            {
                if (Check(Token_Class.AndOp) || Check(Token_Class.OrOp))
                {
                    ConditionStatNode.Children.Add(BooleanOperator());
                    ConditionStatNode.Children.Add(Condition());
                }
                else
                {
                    break;
                }
            }

            return ConditionStatNode;
        }
        Node BooleanOperator()
        {
            Node BooleanOperator = new Node("BooleanOperator");
            if (Check(Token_Class.AndOp))
            {
                BooleanOperator.Children.Add(match(Token_Class.AndOp));


            }
            else if (Check(Token_Class.OrOp))
            {
                BooleanOperator.Children.Add(match(Token_Class.OrOp));


            }
            return BooleanOperator;
        }


        Node State()
        {
            Node state = new Node("state");

            // write your code here to match statements
            if (Check(Token_Class.Semicolon))
            {
                state.Children.Add(match(Token_Class.Semicolon));
                state.Children.Add(Statement());
                state.Children.Add(State());

            }
            else
            {
                return null;
            };
            return state;
        }

        public bool Check(Token_Class CheckToken)
        {
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == CheckToken)
            {
                return true;
            }
            return false;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    exit = true;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                exit = true;
                return null;
            }
        }


        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }

}
