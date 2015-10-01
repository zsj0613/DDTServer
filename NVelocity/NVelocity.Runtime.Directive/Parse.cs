using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using System;
using System.IO;
using System.Text;

namespace NVelocity.Runtime.Directive
{
	public class Parse : Directive
	{
		public override string Name
		{
			get
			{
				return "parse";
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

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			bool result;
			object obj;
			if (!this.AssertArgument(node))
			{
				result = false;
			}
			else if (!this.AssertNodeHasValue(node, context, out obj))
			{
				result = false;
			}
			else
			{
				string arg = obj.ToString();
				this.AssertTemplateStack(context);
				Resource.Resource currentResource = context.CurrentResource;
				string encoding;
				if (currentResource != null)
				{
					encoding = currentResource.Encoding;
				}
				else
				{
					encoding = (string)this.rsvc.GetProperty("input.encoding");
				}
				Template template = this.GetTemplate(arg, encoding, context);
				result = (template != null && this.RenderTemplate(template, arg, writer, context));
			}
			return result;
		}

		private bool AssertArgument(INode node)
		{
			bool result = true;
			if (node.GetChild(0) == null)
			{
				this.rsvc.Error("#parse() error :  null argument");
				result = false;
			}
			return result;
		}

		private bool AssertNodeHasValue(INode node, IInternalContextAdapter context, out object value)
		{
			bool result = true;
			value = node.GetChild(0).Value(context);
			if (value == null)
			{
				this.rsvc.Error("#parse() error :  null argument");
				result = false;
			}
			return result;
		}

		private bool AssertTemplateStack(IInternalContextAdapter context)
		{
			bool result = true;
			object[] templateNameStack = context.TemplateNameStack;
			if (templateNameStack.Length >= this.rsvc.GetInt("directive.parse.max.depth", 20))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < templateNameStack.Length; i++)
				{
					stringBuilder.Append(" > " + templateNameStack[i]);
				}
				this.rsvc.Error(string.Concat(new object[]
				{
					"Max recursion depth reached (",
					templateNameStack.Length,
					") File stack:",
					stringBuilder
				}));
				result = false;
			}
			return result;
		}

		private Template GetTemplate(string arg, string encoding, IInternalContextAdapter context)
		{
			Template result = null;
			try
			{
				result = this.rsvc.GetTemplate(arg, encoding);
			}
			catch (ResourceNotFoundException var_1_18)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					"#parse(): cannot find template '",
					arg,
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
			catch (ParseErrorException var_2_92)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					"#parse(): syntax error in #parse()-ed template '",
					arg,
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
					"#parse() : arg = ",
					arg,
					".  Exception : ",
					ex
				}));
				result = null;
			}
			return result;
		}

		private bool RenderTemplate(Template template, string arg, TextWriter writer, IInternalContextAdapter context)
		{
			bool result = true;
			try
			{
				context.PushCurrentTemplateName(arg);
				((SimpleNode)template.Data).Render(context, writer);
			}
			catch (System.Exception ex)
			{
				if (ex is MethodInvocationException)
				{
					throw;
				}
				this.rsvc.Error(string.Concat(new object[]
				{
					"Exception rendering #parse( ",
					arg,
					" )  : ",
					ex
				}));
				result = false;
			}
			finally
			{
				context.PopCurrentTemplateName();
			}
			return result;
		}
	}
}
