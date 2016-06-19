using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class Include : Directive
	{
		private string outputMsgStart = "";

		private string outputMsgEnd = "";

		public override string Name
		{
			get
			{
				return "include";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override DirectiveType Type
		{
			get
			{
				return DirectiveType.LINE;
			}
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);
			this.outputMsgStart = this.rsvc.GetString("directive.include.output.errormsg.start");
			this.outputMsgStart += " ";
			this.outputMsgEnd = this.rsvc.GetString("directive.include.output.errormsg.end");
			this.outputMsgEnd = " " + this.outputMsgEnd;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			int childrenCount = node.ChildrenCount;
			for (int i = 0; i < childrenCount; i++)
			{
				INode child = node.GetChild(i);
				if (child.Type == 6 || child.Type == 14)
				{
					if (!this.RenderOutput(child, context, writer))
					{
						this.OutputErrorToStream(writer, "error with arg " + i + " please see log.");
					}
				}
				else
				{
					this.rsvc.Error("#include() error : invalid argument type : " + child.ToString());
					this.OutputErrorToStream(writer, "error with arg " + i + " please see log.");
				}
			}
			return true;
		}

		private bool RenderOutput(INode node, IInternalContextAdapter context, TextWriter writer)
		{
			bool result;
			if (node == null)
			{
				this.rsvc.Error("#include() error :  null argument");
				result = false;
			}
			else
			{
				object obj = node.Value(context);
				if (obj == null)
				{
					this.rsvc.Error("#include() error :  null argument");
					result = false;
				}
				else
				{
					string text = obj.ToString();
					Resource.Resource resource = null;
					Resource.Resource currentResource = context.CurrentResource;
					try
					{
						string encoding;
						if (currentResource != null)
						{
							encoding = currentResource.Encoding;
						}
						else
						{
							encoding = (string)this.rsvc.GetProperty("input.encoding");
						}
						resource = this.rsvc.GetContent(text, encoding);
					}
					catch (ResourceNotFoundException)
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"#include(): cannot find resource '",
							text,
							"', called from template ",
							context.CurrentTemplateName,
							" at (",
							base.Line,
							", ",
							base.Column,
							")"
						}));
						throw;
					}
					catch (System.Exception ex)
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"#include(): arg = '",
							text,
							"',  called from template ",
							context.CurrentTemplateName,
							" at (",
							base.Line,
							", ",
							base.Column,
							") : ",
							ex
						}));
					}
					if (resource == null)
					{
						result = false;
					}
					else
					{
						writer.Write((string)resource.Data);
						result = true;
					}
				}
			}
			return result;
		}

		private void OutputErrorToStream(TextWriter writer, string msg)
		{
			if (this.outputMsgStart != null && this.outputMsgEnd != null)
			{
				writer.Write(this.outputMsgStart);
				writer.Write(msg);
				writer.Write(this.outputMsgEnd);
			}
		}
	}
}
