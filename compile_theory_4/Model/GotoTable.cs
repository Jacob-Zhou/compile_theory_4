using compile_theory_4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{
	class GotoTable
	{
		static public Dictionary<int, Dictionary<TokenKind, int>> gotos = new Dictionary<int, Dictionary<TokenKind, int>>();

		static public int? GOTO(int state, TokenKind symbol)
		{
			if (gotos.ContainsKey(state))
			{
				if (gotos[state].ContainsKey(symbol))
				{
					return gotos[state][symbol];
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
