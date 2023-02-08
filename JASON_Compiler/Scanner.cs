using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    If, Integer, Float, String, Read, Write, Repeat, Until, ElseIf,
    Else, Then, Return, Endl, End, Call, Set, Program, main,

    Dot, Semicolon, Comma, LParanthesis, RParanthesis,
    PlusOp, MinusOp, MultiplyOp, DivideOp, AssignOp,

    GreaterThanOp, LessThanOp, NotEqualOp, EqualOp,
    RSquareBrackects, RCurlyBrackets, LSquareBrackects, LCurlyBrackets,
    AndOp, OrOp, Identifier, Number, Comment
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("Program", Token_Class.Program);
            ReservedWords.Add("main", Token_Class.main);
            ReservedWords.Add("Call", Token_Class.Call);
            ReservedWords.Add("Repeat", Token_Class.Repeat);
            ReservedWords.Add("Set", Token_Class.Set);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);


            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyBrackets);
            Operators.Add("}", Token_Class.RCurlyBrackets);
            Operators.Add("[", Token_Class.LSquareBrackects);
            Operators.Add("]", Token_Class.RSquareBrackects);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("–", Token_Class.MinusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add(":=", Token_Class.AssignOp);
        }




        public void StartScanning(string SourceCode)
        {
            Tokens.Clear();
            Errors.Error_List.Clear();

            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i + 1;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();


                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;
                // identifier

                if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z'))
                {
                    while (j < SourceCode.Length && ((SourceCode[j] >= 'A' && SourceCode[j] <= 'Z') || (SourceCode[j] >= 'a' && SourceCode[j] <= 'z') || (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    j--;
                    i = j;

                }
                //number

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while (j < SourceCode.Length && (SourceCode[j] == '.' || (SourceCode[j] >= 'A' && SourceCode[j] <= 'Z') || (SourceCode[j] >= 'a' && SourceCode[j] <= 'z') || (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    j--;
                    i = j;

                }
                else if (CurrentChar == '"')
                {
                    while (j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                        if (SourceCode[j] == '"')
                        {
                            break;
                        }
                        j++;
                    }

                    i = j;
                }

                else
                {
                    if (j < SourceCode.Length)
                    {
                        string CurrentLexeme2 = CurrentChar.ToString() + SourceCode[j];

                        if (CurrentLexeme2 == "/*")
                        {
                            j++;
                            while (j < SourceCode.Length)
                            {
                                CurrentLexeme2 += SourceCode[j];
                                if (SourceCode[j] == '/' && SourceCode[j - 1] == '*')
                                {
                                    break;
                                }
                                j++;
                            }
                            CurrentLexeme = CurrentLexeme2;
                            i = j;
                        }
                        else if (Operators.ContainsKey(CurrentLexeme2))
                        {
                            CurrentLexeme = CurrentLexeme2;
                            i = j;
                        }
                        else if (CurrentLexeme2 == ">=" || CurrentLexeme2 == "<=")
                        {
                            CurrentLexeme = CurrentLexeme2;
                            i = j;
                        }
                        else if (CurrentLexeme2[0] == '.' && (CurrentLexeme2[1] >= '0' && CurrentLexeme2[1] <= '9'))
                        {
                            CurrentLexeme = CurrentLexeme2;
                            i = j;
                        }
                    }
                }

                FindTokenClass(CurrentLexeme);
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);

            }

            //Is it a Constant?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it an operator?

            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            // It is string
            else if (isStringLiteral(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }

            // it is comment
            else if (IsComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;

            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }

        bool isIdentifier(string lex)
        {
            bool isValid = true;
            var r = new Regex("^([a-z]|[A-Z])([a-z]|[A-Z][0-9])*", RegexOptions.Compiled);
            if (r.IsMatch(lex)) isValid = true;
            else isValid = false;

            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = true;
            var r = new Regex("^[0-9]+(\\.[0-9]+)?$", RegexOptions.Compiled);
            if (r.IsMatch(lex)) isValid = true;
            else isValid = false;
            return isValid;
        }

        bool isStringLiteral(string lex)
        {
            bool isValid = false;
            if (lex[0] == '"' && lex[(lex.Length - 1)] == '"')
                isValid = true;
            return isValid;
        }

        bool IsComment(string lex)
        {
            return (lex.StartsWith("/*") && lex.EndsWith("*/"));
        }

    }
}