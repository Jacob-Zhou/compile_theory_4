using compile_theory_4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{
	enum ActionType
	{
		SHIFT,
		REDUCE,
		ACCEPT,
		CHANGE,
		ERROR
	}

	class Action
	{
		public Action(ActionType action, int value = 0, string information = "")
		{
			this.Kind = action;
			this.value = value;
			this.information = information;
		}

		public ActionType Kind { get; set; }
		public int value { get; set; }
		public string information { get; set; }
	}

	class FixAction
	{
		public FixAction(ActionType kind, int value = 0, TokenKind? missToken = null)
		{
			Kind = kind;
			this.value = value;
			this.missToken = missToken;
		}

		public ActionType Kind { get; set; }
		public int value { get; set; }
		public TokenKind? missToken { get; set; }
	}

	class ActionTable
	{

		static public Dictionary<int, Dictionary<TokenKind, Action>> actions = new Dictionary<int, Dictionary<TokenKind, Action>>();

		static public Action action(int state, TokenKind terminalSymbol)
		{
			if (actions.ContainsKey(state))
			{
				if (actions[state].ContainsKey(terminalSymbol))
				{
					return actions[state][terminalSymbol];
				}
				else
				{
					switch (state)
					{
						//E0
						case 0:
							if(terminalSymbol == TokenKind.END)
							{
								return new Action(ActionType.ACCEPT);
							}
							else
							{
								return new Action(ActionType.ERROR, 0, "应为 \"{ \"" );
							}

						//E1
						case 1:
						case 5:
						case 24:
							if (terminalSymbol == TokenKind.END)
							{
								return new Action(ActionType.ERROR, 8, "应为表达式");
							}
							else
							{
								return new Action(ActionType.ERROR, 1, "只有赋值语句可用作表达式");
							}

						//E2
						case 2:
						case 3:
							return new Action(ActionType.ERROR, 2, "程序应该被包含在主程序括号中");

						//E3
						case 4:
							return new Action(ActionType.ERROR, 3, "应输入 \"( \"");

						//E4
						case 22:
							return new Action(ActionType.ERROR, 4, "应输入 \"; \"");

						//E5
						case 6:
							return new Action(ActionType.ERROR, 5, "应输入 \"= \"");

						//E6
						case 8:
						case 11:
							return new Action(ActionType.ERROR, 6, "应输入 \"} \"");

						//E7
						case 10:
						case 12:
						case 18:
						case 27:
						case 28:
						case 31:
						case 32:
							if (terminalSymbol == TokenKind.END)
							{
								if(state == 18)
								{
									return new Action(ActionType.ERROR, 9, "应输入 \") \"");
								}
								else
								{
									return new Action(ActionType.ERROR, 8, "应为表达式");
								}
							}
							else
							{
									return new Action(ActionType.ERROR, 7, string.Format("表达式项 \"{0} \" 无效", Parser.TokenKindToString(terminalSymbol)));
							}

						//E9
						case 17:
							return new Action(ActionType.ERROR, 9, "应输入 \") \"");

						//E5
						case 25:
						case 26:
							return new Action(ActionType.ERROR, 5, "应输入 \"= \"");

						//EF
						case 7:
						case 9:
						case 13:
						case 14:
						case 15:
						case 16:
						case 19:
						case 20:
						case 21:
						case 23:
						case 29:
						case 30:
						case 35:
						case 36:
						case 37:
						case 38:
						case 33:
						case 34:
							return new Action(ActionType.ERROR, -1);

					}
					return new Action(ActionType.ERROR, -1, "TO DO");
				}
			}
			else
			{
				return new Action(ActionType.ERROR, -1, "不存在的状态");
			}
		}

		static public FixAction getFixAction(int state)
		{
			switch (state)
			{
				case 0:
					return new FixAction(ActionType.CHANGE, 1, TokenKind.LBRA);
				case 1:
				case 5:
				case 9:
					return new FixAction(ActionType.REDUCE, 3);
				case 2:
					return new FixAction(ActionType.ERROR);
				case 3:
					return new FixAction(ActionType.REDUCE, 0);
				case 4:
					return new FixAction(ActionType.CHANGE, 10, TokenKind.LPAR);
				case 6:
					return new FixAction(ActionType.CHANGE, 12, TokenKind.EQU);
				case 7:
					return new FixAction(ActionType.REDUCE, 6);
				case 8:
					return new FixAction(ActionType.CHANGE, 13, TokenKind.RBRA);
				case 10:
					return new FixAction(ActionType.CHANGE, 17, TokenKind.BOOL);//?
				case 11:
					return new FixAction(ActionType.CHANGE, 19, TokenKind.RBRA);
				case 12:
				case 27:
				case 28:
					return new FixAction(ActionType.CHANGE, 20, TokenKind.ID);//?
				case 13:
					return new FixAction(ActionType.REDUCE, 1);
				case 14:
					return new FixAction(ActionType.REDUCE, 2);
				case 15:
					return new FixAction(ActionType.REDUCE, 13);
				case 16:
					return new FixAction(ActionType.REDUCE, 14);
				case 17:
					return new FixAction(ActionType.CHANGE, 24, TokenKind.LPAR);
				case 18:
					return new FixAction(ActionType.REDUCE, 9);//?
				case 19:
					return new FixAction(ActionType.REDUCE, 1);
				case 20:
					return new FixAction(ActionType.REDUCE, 13);
				case 21:
					return new FixAction(ActionType.REDUCE, 14);
				case 22:
					return new FixAction(ActionType.CHANGE, 29, TokenKind.SEMI);
				case 23:
					return new FixAction(ActionType.REDUCE, 12);
				case 24:
					return new FixAction(ActionType.CHANGE, 6, TokenKind.ID);//?
				case 25:
					return new FixAction(ActionType.CHANGE, 31, TokenKind.EQU);
				case 26:
					return new FixAction(ActionType.CHANGE, 32, TokenKind.EQU);
				case 29:
					return new FixAction(ActionType.REDUCE, 4);
				case 30:
					return new FixAction(ActionType.REDUCE, 5);
				case 31:
				case 32:
					return new FixAction(ActionType.CHANGE, 35, TokenKind.ID);//?
				case 33:
					return new FixAction(ActionType.REDUCE, 10);
				case 34:
					return new FixAction(ActionType.REDUCE, 11);
				case 35:
					return new FixAction(ActionType.REDUCE, 13);
				case 36:
					return new FixAction(ActionType.REDUCE, 14);
				case 37:
					return new FixAction(ActionType.REDUCE, 8);
				case 38:
					return new FixAction(ActionType.REDUCE, 7);
			}
			return null;
		}
	}
}
