﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTRHacker.Core.ProjectileMaker.Parse.AST
{
	public class Expr_BTuple : Expression
	{
		public Expression A { get; set; }
		public Expression B { get; set; }
		public Expr_BTuple(int off) : base(off)
		{

		}
	}
}
