﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{
	class Error
	{
		public int line { get; set; }
		public int lineOffset { get; set; }
		public int length { get; set; }
		public ErrorKind kind { get; set; }
		public string value { get; set; }
		public string infomation { get; set; }
	}
}
