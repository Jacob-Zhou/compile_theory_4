using compile_theory_4.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{

	class Parser
	{
		static private Stack<int> stateStack = new Stack<int>();
		static private Stack<Token> tokenStack = new Stack<Token>();
		static private Stack<Process> processStack = new Stack<Process>();
		static private bool hasErr = false;
		static private Process reverse;
		static private Process process;
		static private Token token;

		static private void ReduceHelper(int popCount, TokenKind kind)
		{
			int offset = tokenStack.Peek().offset, length = 0;
			for (int i = 0; i < popCount; i++)
			{
				offset = tokenStack.Peek().offset;
				length += tokenStack.Pop().length;
				stateStack.Pop();
			}
			Token token = new Token(offset, string.Empty, kind);
			token.length = length;
			tokenStack.Push(token);
			var newState = GotoTable.GOTO(stateStack.Peek(), kind);
			if (newState.HasValue)
			{
				stateStack.Push(newState.Value);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		static private void Reduce(int production)
		{
			switch (production)
			{
				case 0:
					ReduceHelper(1, TokenKind.PROGRAM);

					if (!hasErr)
					{
						process = new Process("program", "program -> block");
						process.addDetail(processStack.Pop());
						processStack.Push(process);
					}

					break;
				case 1:
					ReduceHelper(3, TokenKind.BLOCK);

					if (!hasErr)
					{
						process = new Process("block", "block -> { stmts }");
						process.addDetail(new Process("{"));
						process.addDetail(processStack.Pop());
						process.addDetail(new Process("}"));
						processStack.Push(process);
					}

					break;
				case 2:
					ReduceHelper(2, TokenKind.STMTS);

					if (!hasErr)
					{
						process = new Process("stmts", "stmts -> stmt stmts");
						reverse = processStack.Pop();
						process.addDetail(processStack.Pop());
						process.addDetail(reverse);
						processStack.Push(process);
					}

					break;
				case 3:
					ReduceHelper(0, TokenKind.STMTS);

					processStack.Push(new Process("stmts", "stmts -> ε"));

					break;
				case 4:
					ReduceHelper(4, TokenKind.STMT);

					if (!hasErr)
					{
						process = new Process("stmt", "stmt -> ID = expr ;");
						reverse = processStack.Pop();
						process.addDetail(new Process("ID")); //ID
						process.addDetail(new Process("="));
						process.addDetail(reverse); //expr
						process.addDetail(new Process(";"));
						processStack.Push(process);
					}

					break;
				case 5:
					ReduceHelper(5, TokenKind.STMT);

					if (!hasErr)
					{
						process = new Process("stmt", "stmt -> WHILE ( bool ) stmt");
						reverse = processStack.Pop();
						process.addDetail(new Process("WHILE"));
						process.addDetail(new Process("("));
						process.addDetail(processStack.Pop()); //bool
						process.addDetail(new Process(")"));
						process.addDetail(reverse); //stmt
						processStack.Push(process);
					}

					break;
				case 6:
					ReduceHelper(1, TokenKind.STMT);

					if (!hasErr)
					{
						process = new Process("stmt", "stmt -> block");
						process.addDetail(processStack.Pop()); //block
						processStack.Push(process);
					}

					break;
				case 7:
					ReduceHelper(4, TokenKind.BOOL);

					if (!hasErr)
					{
						process = new Process("bool", "bool -> term <= term");
						reverse = processStack.Pop();
						process.addDetail(processStack.Pop()); //term
						process.addDetail(new Process("<="));
						process.addDetail(reverse); //term
						processStack.Push(process);
					}

					break;
				case 8:
					ReduceHelper(4, TokenKind.BOOL);

					if (!hasErr)
					{
						process = new Process("bool", "bool -> term >= term");
						reverse = processStack.Pop();
						process.addDetail(processStack.Pop()); //term
						process.addDetail(new Process(">="));
						process.addDetail(reverse); //term
						processStack.Push(process);
					}

					break;
				case 9:
					ReduceHelper(1, TokenKind.BOOL);

					if (!hasErr)
					{
						process = new Process("bool", "bool -> term");
						process.addDetail(processStack.Pop()); //term
						processStack.Push(process);
					}

					break;
				case 10:
					ReduceHelper(3, TokenKind.EXPR);

					if (!hasErr)
					{
						process = new Process("expr", "expr -> term + term");
						reverse = processStack.Pop();
						process.addDetail(processStack.Pop()); //term
						process.addDetail(new Process("+"));
						process.addDetail(reverse); //term
						processStack.Push(process);
					}

					break;
				case 11:
					ReduceHelper(3, TokenKind.EXPR);

					if (!hasErr)
					{
						process = new Process("expr", "expr -> term - term");
						reverse = processStack.Pop();
						process.addDetail(processStack.Pop()); //term
						process.addDetail(new Process("-"));
						process.addDetail(reverse); //term
						processStack.Push(process);
					}

					break;
				case 12:
					ReduceHelper(1, TokenKind.EXPR);

					if (!hasErr)
					{
						process = new Process("expr", "expr -> term");
						process.addDetail(processStack.Pop()); //term
						processStack.Push(process);
					}

					break;
				case 13:
					ReduceHelper(1, TokenKind.TERM);

					if (!hasErr)
					{
						process = new Process("term", "term -> ID");
						process.addDetail(new Process("ID")); //term
						processStack.Push(process);
					}

					break;
				case 14:
					ReduceHelper(1, TokenKind.TERM);

					if (!hasErr)
					{
						process = new Process("term", "term -> NUM");
						process.addDetail(new Process("NUM")); //term
						processStack.Push(process);
					}

					break;
			}
		}

		static public void parse()
		{
			SourceViewModel.KeepOnlyRead();
			Reset();

			stateStack.Push(0);
			tokenStack.Push(new Token(0, "#", TokenKind.END));

			bool result = false;
			bool breakThrough = false;
			Lexer.Reset();

			Action action;
			FixAction fixAction;

			token = Lexer.LexNext();
			while (true)
			{
				if (token != null)
				{
					if (/*hasErr && token.kind == TokenKind.END || */breakThrough)
					{
						break;
					}
					action = ActionTable.action(stateStack.Peek(), token.kind);
					if (action != null)
					{
						switch (action.Kind)
						{
							case ActionType.ACCEPT:
								result = true;
								break;
							case ActionType.SHIFT:
								stateStack.Push(action.value);
								tokenStack.Push(token);
								token = Lexer.LexNext();
								break;
							case ActionType.REDUCE:
								Reduce(action.value);
								break;
							case ActionType.ERROR:
								fixAction = ActionTable.getFixAction(stateStack.Peek());
								if(action.value != -1)
								{
									ErrorHandle(token, action.information);
								}
								hasErr = true;
								if (fixAction != null)
								{
									switch (fixAction.Kind)
									{
										case ActionType.ACCEPT:
											throw new NotImplementedException();
										case ActionType.CHANGE:
											stateStack.Push(fixAction.value);
											if (fixAction.missToken.HasValue)
											{
												tokenStack.Push(new Token(token.offset, "", fixAction.missToken.Value));
											}
											else
											{
												breakThrough = true;
											}
											break;
										case ActionType.ERROR:
											breakThrough = true;
											break;
										case ActionType.SHIFT:
											token = Lexer.LexNext();
											if(token.kind == TokenKind.END)
											{
												breakThrough = true;
											}
											break;
										case ActionType.REDUCE:
											Reduce(fixAction.value);
											break;
									}
								}
								break;
						}
						if (result)
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}

			if (result && !hasErr)
			{
				StateViewModel.Display("成功");
				if(processStack.Count > 0)
				{
					ProcessViewModel.Add(processStack.Peek());
				}
			}
			else
			{
				StateViewModel.Display("失败");
			}

			SourceViewModel.UnkeepOnlyRead();
		}

		static private void Reset()
		{
			ProcessViewModel.Clear();
			ErrorViewModel.getInstance().clear();
			stateStack.Clear();
			tokenStack.Clear();
			hasErr = false;
		}

		static public string TokenKindToString(TokenKind s)
		{
			switch (s)
			{
				case TokenKind.ADD:
					return "+";
				case TokenKind.SUB:
					return "-";
				case TokenKind.LPAR:
					return "(";
				case TokenKind.RPAR:
					return ")";
				case TokenKind.LBRA:
					return "{";
				case TokenKind.RBRA:
					return "}";
				case TokenKind.LT:
					return "<";
				case TokenKind.GT:
					return ">";
				case TokenKind.EQU:
					return "=";
				case TokenKind.SEMI:
					return ";";
				case TokenKind.END:
					return "#";
				default:
					return s.ToString();
			}
		}

		static private void ErrorHandle(Token t, string info)
		{
			Error err = new Error();
			err.line = SourceViewModel.GetLine(t.offset);
			err.lineOffset = SourceViewModel.GetLineOffset(t.offset);
			err.length = t.length;
			err.infomation = info;
			ErrorViewModel.getInstance().addError(err);
		}
	}
}
