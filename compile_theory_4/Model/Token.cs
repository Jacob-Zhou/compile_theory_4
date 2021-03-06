﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{
	class Token
	{
		public Token(int offset, string value, TokenKind kind)
		{
			this.offset = offset;
			this.value = value;
			this.kind = kind;
			length = value.Length;
		}
		
		public int offset { get; set; }
		public string value { get; set; }
		public TokenKind kind { get; set; }
		public int length { get; set; }
	}
}
