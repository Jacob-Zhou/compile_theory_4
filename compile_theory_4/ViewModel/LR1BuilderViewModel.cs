using compile_theory_4.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.ViewModel
{
	class Item
	{
		public Item(List<TokenKind> symbols, int index = 0, TokenKind lookhead = TokenKind.ERROR)
		{
			this.symbols = symbols;
			pointer = 0;
			this.lookhead = lookhead;
			this.index = index;
		}

		public List<TokenKind> symbols { get; set; }
		public int pointer { get; set; } = 0;
		public TokenKind lookhead { get; set; } = TokenKind.END;
		public int index { get; set; }

		public Item MoveOn()
		{
			Item i = new Item(symbols, index, lookhead);
			i.pointer = pointer + 1;
			return i;
		}

		public bool NextIs(TokenKind X)
		{
			if (pointer >= symbols.Count)
			{
				return false;
			}
			else
			{
				return symbols[pointer] == X;
			}
		}

		public TokenKind? GetNext()
		{
			if (pointer >= symbols.Count)
			{
				return null;
			}
			else
			{
				return symbols[pointer];
			}
		}

		public override bool Equals(object obj)
		{
			var i = obj as Item;
			if (i == null)
			{
				return false;
			}
			else
			{
				if (lookhead == i.lookhead && pointer == i.pointer && index == i.index && symbols.Count == i.symbols.Count)
				{
					for (int index = 0; index < symbols.Count; index++)
					{
						if (symbols[index] != i.symbols[index])
						{
							return false;
						}
					}
					return true;
				}
			}
			return base.Equals(obj);
		}
	}

	class LR1BuilderViewModel
	{
		static private List<List<Item>> C;
		static private List<Tuple<int, TokenKind, int>> GO;
		static private HashSet<TokenKind> T = new HashSet<TokenKind>
		{
		TokenKind.IF,
		TokenKind.ELSE,
		TokenKind.WHILE,
		TokenKind.GT,
		TokenKind.ADD,
		TokenKind.SUB,
		TokenKind.EQU,
		TokenKind.LT,
		TokenKind.LBRA,
		TokenKind.RBRA,
		TokenKind.SEMI,
		TokenKind.ID,
		TokenKind.NUM,
		TokenKind.LPAR,
		TokenKind.RPAR,
		TokenKind.PROGRAM,
		TokenKind.BLOCK,
		TokenKind.STMTS,
		TokenKind.STMT,
		TokenKind.BOOL,
		TokenKind.EXPR,
		TokenKind.TERM,
		TokenKind.START,
		TokenKind.END
		};

		static public bool IsNoterminal(TokenKind Kind)
		{
			switch (Kind)
			{
				case TokenKind.PROGRAM:
				case TokenKind.BLOCK:
				case TokenKind.STMTS:
				case TokenKind.STMT:
				case TokenKind.BOOL:
				case TokenKind.EXPR:
				case TokenKind.TERM:
				case TokenKind.START:
					return true;
				default:
					return false;
			}
		}

		static private List<Item> GetItems(TokenKind B)
		{
			switch (B)
			{
				case TokenKind.PROGRAM:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.BLOCK }, 0) };
				case TokenKind.BLOCK:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.LBRA, TokenKind.STMTS, TokenKind.RBRA }, 1) };
				case TokenKind.STMTS:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.STMT, TokenKind.STMTS }, 2), new Item(new List<TokenKind> { }, 3) };
				case TokenKind.STMT:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.ID, TokenKind.EQU, TokenKind.EXPR, TokenKind.SEMI }, 4), new Item(new List<TokenKind> { TokenKind.WHILE, TokenKind.LPAR, TokenKind.BOOL, TokenKind.RPAR, TokenKind.STMT }, 5), new Item(new List<TokenKind> { TokenKind.BLOCK }, 6) };
				case TokenKind.BOOL:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.TERM, TokenKind.LT, TokenKind.EQU, TokenKind.TERM }, 7), new Item(new List<TokenKind> { TokenKind.TERM, TokenKind.GT, TokenKind.EQU, TokenKind.TERM }, 8), new Item(new List<TokenKind> { TokenKind.TERM }, 9) };
				case TokenKind.EXPR:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.EXPR, TokenKind.ADD, TokenKind.EXPR }, 10), new Item(new List<TokenKind> { TokenKind.EXPR, TokenKind.SUB, TokenKind.EXPR }, 11), new Item(new List<TokenKind> { TokenKind.TERM }, 12) };
				case TokenKind.TERM:
					return new List<Item> { new Item(new List<TokenKind> { TokenKind.ID }, 13), new Item(new List<TokenKind> { TokenKind.NUM }, 14) };
				default:
					return null;
			}
		}

		static public void Build()
		{
			C = new List<List<Item>>();
			GO = new List<Tuple<int, TokenKind, int>>();
			Item s = new Item(new List<TokenKind> { TokenKind.PROGRAM }, -1, TokenKind.END);
			C.Add(CLOSURE(new List<Item> { s }));
			int index = 0;
			bool IsContain = false;
			//FileStream fs = File.OpenWrite("Action.txt");
			//StreamWriter sw = new StreamWriter(fs);
			while (true)
			{
				if (index < C.Count)
				{
					var I = C[index];
					foreach (var X in T)
					{
						var _goto = GOTO(I, X);
						if (_goto != null)
						{
							for(int i = 0; i < C.Count; i++)
							{
								var c = C[i];
								IsContain = false;
								if (c.Count == _goto.Count)
								{
									IsContain = true;
									for (int ci = 0; ci < c.Count; ci++)
									{
										if (!c[ci].Equals(_goto[ci]))
										{
											IsContain = false;
											break;
										}
									}
									if (IsContain)
									{
										GO.Add(new Tuple<int, TokenKind, int>(index, X, i));
										break;
									}
								}
							}
							if (!IsContain)
							{
								GO.Add(new Tuple<int, TokenKind, int>(index, X, C.Count));
								C.Add(_goto);
								//sw.Write(C.Count - 1);
								//sw.Write(" :{\n");
								//foreach(var item in _goto)
								//{
								//	sw.Write("\t");
								//	for(int j = 0; j < item.symbols.Count; j++)
								//	{
								//		if(j == item.pointer)
								//		{
								//			sw.Write("@ ");
								//		}
								//		sw.Write(Parser.TokenKindToString(item.symbols[j]) + " ");
								//	}
								//	if(item.symbols.Count == item.pointer)
								//	{
								//		sw.Write("@ ");
								//	}
								//	sw.Write("\t[{0}]\n", Parser.TokenKindToString(item.lookhead));
								//}
								//sw.Write("}\n\n");
								//sw.Flush();
							}
						}
					}
					index++;
				}
				else
				{
					break;
				}
			}
			for(int i = 0; i < C.Count; i++)
			{
				for(int j = 0; j < C[i].Count; j++)
				{
					if(C[i][j].index == -1 && C[i][j].pointer == C[i][j].symbols.Count && C[i][j].lookhead == TokenKind.END)
					{
						if (ActionTable.actions.ContainsKey(i))
						{
							ActionTable.actions[i][TokenKind.END] = new Model.Action(ActionType.ACCEPT);
						}
						else
						{
							ActionTable.actions[i] = new Dictionary<TokenKind, Model.Action>();
							ActionTable.actions[i][TokenKind.END] = new Model.Action(ActionType.ACCEPT);
						}
					}
					else if (C[i][j].pointer == C[i][j].symbols.Count)
					{
						if (ActionTable.actions.ContainsKey(i))
						{
							ActionTable.actions[i][C[i][j].lookhead] = new Model.Action(ActionType.REDUCE, C[i][j].index);
						}
						else
						{
							ActionTable.actions[i] = new Dictionary<TokenKind, Model.Action>();
							ActionTable.actions[i][C[i][j].lookhead] = new Model.Action(ActionType.REDUCE, C[i][j].index);
						}
					}
				}
			}
			foreach(var g in GO)
			{
				if (IsNoterminal(g.Item2))
				{
					if (GotoTable.gotos.ContainsKey(g.Item1))
					{
						GotoTable.gotos[g.Item1][g.Item2] = g.Item3;
					}
					else
					{
						GotoTable.gotos[g.Item1] = new Dictionary<TokenKind, int>();
						GotoTable.gotos[g.Item1][g.Item2] = g.Item3;
					}
				}
				else
				{
					if (ActionTable.actions.ContainsKey(g.Item1))
					{
						ActionTable.actions[g.Item1][g.Item2] = new Model.Action(ActionType.SHIFT, g.Item3);
					}
					else
					{
						ActionTable.actions[g.Item1] = new Dictionary<TokenKind, Model.Action>();
						ActionTable.actions[g.Item1][g.Item2] = new Model.Action(ActionType.SHIFT, g.Item3);
					}
				}
			}
			//FileStream fs = File.OpenWrite("Action.csv");
			//StreamWriter sw = new StreamWriter(fs);
			//List<TokenKind> terminal = new List<TokenKind> {
			//	TokenKind.WHILE,
			//	TokenKind.ID,
			//	TokenKind.NUM,
			//	TokenKind.LBRA,
			//	TokenKind.RBRA,
			//	TokenKind.LPAR,
			//	TokenKind.RPAR,
			//	TokenKind.LT,
			//	TokenKind.GT,
			//	TokenKind.EQU,
			//	TokenKind.ADD,
			//	TokenKind.SUB,
			//	TokenKind.SEMI,
			//	TokenKind.END
			//};
			//List<TokenKind> noterminal = new List<TokenKind> {
			//	TokenKind.PROGRAM,
			//	TokenKind.BLOCK,
			//	TokenKind.STMTS,
			//	TokenKind.STMT,
			//	TokenKind.BOOL,
			//	TokenKind.EXPR,
			//	TokenKind.TERM,
			//};
			//foreach (var t in terminal)
			//{
			//	sw.Write(Parser.TokenKindToString(t) + ", ");
			//}
			//sw.WriteLine();
			//for (int i = 0; i < ActionTable.actions.Count; i++)
			//{
			//	foreach (var t in terminal)
			//	{
			//		if (ActionTable.actions[i].ContainsKey(t))
			//		{
			//			var a = ActionTable.actions[i][t];
			//			switch (a.Kind)
			//			{
			//				case ActionType.ACCEPT:
			//					sw.Write("ACC, ");
			//					break;
			//				case ActionType.REDUCE:
			//					sw.Write("R" + a.value.ToString() + ", ");
			//					break;
			//				case ActionType.SHIFT:
			//					sw.Write("S" + a.value.ToString() + ", ");
			//					break;
			//			}
			//		}
			//		else
			//		{
			//			sw.Write(", ");
			//		}
			//	}
			//	sw.WriteLine();
			//}
			//sw.Flush();
			//fs.Close();
			//fs = File.OpenWrite("Goto.csv");
			//sw = new StreamWriter(fs);

			//foreach (var n in noterminal)
			//{
			//	sw.Write(Parser.TokenKindToString(n) + ", ");
			//}
			//sw.WriteLine();
			//for (int i = 0; i < ActionTable.actions.Count; i++)
			//{
			//	if (GotoTable.gotos.ContainsKey(i))
			//	{
			//		foreach (var n in noterminal)
			//		{
			//			if (GotoTable.gotos[i].ContainsKey(n))
			//			{
			//				sw.Write(GotoTable.gotos[i][n].ToString() + ", ");
			//			}
			//			else
			//			{
			//				sw.Write(", ");
			//			}
			//		}
			//		sw.WriteLine();
			//	}
			//	else
			//	{
			//		foreach (var n in noterminal)
			//		{
			//			sw.Write(", ");
			//		}
			//		sw.WriteLine();
			//	}
			//}
			//sw.Flush();
			//sw.Close();
			//fs.Close();
		}

		static private List<Item> GOTO(List<Item> I, TokenKind X)
		{
			List<Item> J = new List<Item>();
			foreach (var i in I)
			{
				if (i.NextIs(X))
				{
					J.Add(i.MoveOn());
				}
			}
			return CLOSURE(J);
		}

		static private List<Item> CLOSURE(List<Item> I)
		{
			int index = 0;
			if(I.Count == 0)
			{
				return null;
			}
			while (true)
			{
				if (index < I.Count)
				{
					var i = I[index];
					var x = i.GetNext();
					if (x.HasValue)
					{
						if (IsNoterminal(x.Value))
						{
							var B = GetItems(x.Value);
							if (B != null)
							{
								foreach (var b in B)
								{
									if (i.pointer < i.symbols.Count - 1)
									{
										var _first = FIRST(i.symbols.GetRange(i.pointer + 1, i.symbols.Count - (i.pointer + 1)), i.lookhead);
										if (_first != null)
										{
											foreach (var f in _first)
											{
												Item nb = new Item(b.symbols, b.index, f);
												if (!I.Contains(nb))
												{
													I.Add(nb);
												}
											}
										}
									}
									else
									{
										if (!I.Contains(b))
										{
											b.lookhead = i.lookhead;
											I.Add(b);
										}
									}
								}
							}
						}
					}
					index++;
				}
				else
				{
					break;
				}
			}
			return I;
		}

		static private HashSet<TokenKind> FIRST(List<TokenKind> Symbols, TokenKind lookhead)
		{
			HashSet<TokenKind> result = new HashSet<TokenKind>();
			foreach (var s in Symbols)
			{
				if (IsNoterminal(s))
				{
					switch (s)
					{
						case TokenKind.PROGRAM:
							result.Add(TokenKind.LBRA);
							return result;
						case TokenKind.BLOCK:
							result.Add(TokenKind.LBRA);
							return result;
						case TokenKind.STMTS:
							result.Add(TokenKind.ID);
							result.Add(TokenKind.WHILE);
							result.Add(TokenKind.LBRA);
							result.Add(lookhead);
							return result;
						case TokenKind.STMT:
							result.Add(TokenKind.ID);
							result.Add(TokenKind.WHILE);
							result.Add(TokenKind.LBRA);
							return result;
						case TokenKind.BOOL:
						case TokenKind.EXPR:
						case TokenKind.TERM:
							result.Add(TokenKind.ID);
							result.Add(TokenKind.NUM);
							return result;
						default:
							result.Add(TokenKind.LBRA);
							return result;
					}
				}
				else
				{
					result.Add(s);
					return result;
				}
			}
			int i = 0;
			result.Add(lookhead);
			return result;
		}
	}
}
