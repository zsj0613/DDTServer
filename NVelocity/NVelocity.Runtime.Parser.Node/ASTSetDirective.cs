using NVelocity.App.Events;
using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTSetDirective : SimpleNode
	{
		private string leftReference = "";

		private INode right;

		private ASTReference left;

		internal bool blather = false;

		private ASTReference LeftHandSide
		{
			get
			{
				return (ASTReference)base.GetChild(0).GetChild(0).GetChild(0);
			}
		}

		private INode RightHandSide
		{
			get
			{
				return base.GetChild(0).GetChild(0).GetChild(1).GetChild(0);
			}
		}

		public ASTSetDirective(int id) : base(id)
		{
		}

		public ASTSetDirective(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.right = this.RightHandSide;
			this.left = this.LeftHandSide;
			this.blather = this.rsvc.GetBoolean("runtime.log.invalid.references", true);
			this.leftReference = this.left.FirstToken.Image.Substring(1);
			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			object obj = this.right.Value(context);
			bool result;
			if (obj == null)
			{
				if (this.blather)
				{
					EventCartridge eventCartridge = context.EventCartridge;
					bool flag = true;
					if (eventCartridge != null)
					{
						flag = eventCartridge.ShouldLogOnNullSet(this.left.Literal, this.right.Literal);
					}
					if (flag)
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"RHS of #set statement is null. Context will not be modified. ",
							context.CurrentTemplateName,
							" [line ",
							base.Line,
							", column ",
							base.Column,
							"]"
						}));
					}
				}
				result = false;
			}
			else
			{
				if (this.left.ChildrenCount == 0)
				{
					context.Put(this.leftReference, obj);
				}
				else
				{
					this.left.SetValue(context, obj);
				}
				result = true;
			}
			return result;
		}
	}
}
