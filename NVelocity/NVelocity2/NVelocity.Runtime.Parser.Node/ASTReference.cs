using NVelocity.App.Events;
using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Exception;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTReference : SimpleNode
	{
		private ReferenceType referenceType;

		private string nullString;

		private string rootString;

		private bool escaped = false;

		private bool computableReference = true;

		private string escPrefix = "";

		private string morePrefix = "";

		private string identifier = "";

		private string literal = null;

		private Stack referenceStack;

		private int numChildren = 0;

		public string RootString
		{
			get
			{
				return this.rootString;
			}
		}

		private string Root
		{
			get
			{
				Token firstToken = base.FirstToken;
				int num = firstToken.Image.IndexOf("\\!");
				string result;
				if (num != -1)
				{
					int length = firstToken.Image.Length;
					int num2 = firstToken.Image.IndexOf('$');
					if (num2 == -1)
					{
						this.rsvc.Error("ASTReference.getRoot() : internal error : no $ found for slashbang.");
						this.computableReference = false;
						this.nullString = firstToken.Image;
						result = this.nullString;
					}
					else
					{
						while (num2 < length && firstToken.Image[num2] != '\\')
						{
							num2++;
						}
						int num3 = num2;
						int num4 = 0;
						while (num2 < length && firstToken.Image[num2++] == '\\')
						{
							num4++;
						}
						this.nullString = firstToken.Image.Substring(0, num3);
						this.nullString += firstToken.Image.Substring(num3, num3 + num4 - 1 - num3);
						this.nullString += firstToken.Image.Substring(num3 + num4);
						this.computableReference = false;
						result = this.nullString;
					}
				}
				else
				{
					this.escaped = false;
					if (firstToken.Image.StartsWith("\\"))
					{
						int num2 = 0;
						int length = firstToken.Image.Length;
						while (num2 < length && firstToken.Image[num2] == '\\')
						{
							num2++;
						}
						if (num2 % 2 != 0)
						{
							this.escaped = true;
						}
						if (num2 > 0)
						{
							this.escPrefix = firstToken.Image.Substring(0, num2 / 2);
						}
						firstToken.Image = firstToken.Image.Substring(num2);
					}
					int num5 = firstToken.Image.LastIndexOf('$');
					if (num5 > 0)
					{
						this.morePrefix += firstToken.Image.Substring(0, num5);
						firstToken.Image = firstToken.Image.Substring(num5);
					}
					this.nullString = this.Literal;
					if (firstToken.Image.StartsWith("$!"))
					{
						this.referenceType = ReferenceType.Quiet;
						if (!this.escaped)
						{
							this.nullString = "";
						}
						if (firstToken.Image.StartsWith("$!{"))
						{
							result = firstToken.Next.Image;
						}
						else
						{
							result = firstToken.Image.Substring(2);
						}
					}
					else if (firstToken.Image.Equals("${"))
					{
						this.referenceType = ReferenceType.Formal;
						result = firstToken.Next.Image;
					}
					else if (firstToken.Image.StartsWith("$"))
					{
						this.referenceType = ReferenceType.Normal;
						result = firstToken.Image.Substring(1);
					}
					else
					{
						this.referenceType = ReferenceType.Runt;
						result = firstToken.Image;
					}
				}
				return result;
			}
		}

		public override string Literal
		{
			get
			{
				string result;
				if (this.literal != null)
				{
					result = this.literal;
				}
				else
				{
					result = base.Literal;
				}
				return result;
			}
		}

		public ASTReference(int id) : base(id)
		{
		}

		public ASTReference(Parser p, int id) : base(p, id)
		{
		}

		public void SetLiteral(string value)
		{
			if (this.literal == null)
			{
				this.literal = value;
			}
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.rootString = this.Root;
			this.numChildren = base.ChildrenCount;
			if (this.numChildren > 0)
			{
				this.identifier = base.GetChild(this.numChildren - 1).FirstToken.Image;
			}
			return data;
		}

		public override object Execute(object o, IInternalContextAdapter context)
		{
			object result;
			if (this.referenceType == ReferenceType.Runt)
			{
				result = null;
			}
			else
			{
				object obj = this.GetVariableValue(context, this.rootString);
				if (context.EventCartridge != null)
				{
					this.referenceStack = new Stack();
					this.referenceStack.Push(obj);
				}
				if (obj == null)
				{
					result = null;
				}
				else
				{
					try
					{
						for (int i = 0; i < this.numChildren; i++)
						{
							obj = base.GetChild(i).Execute(obj, context);
							if (this.referenceStack != null)
							{
								this.referenceStack.Push(obj);
							}
							if (obj == null)
							{
								result = null;
								return result;
							}
						}
						result = obj;
					}
					catch (MethodInvocationException ex)
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"Method ",
							ex.MethodName,
							" threw exception for reference $",
							this.rootString,
							" in template ",
							context.CurrentTemplateName,
							" at  [",
							base.Line,
							",",
							base.Column,
							"]"
						}));
						ex.ReferenceName = this.rootString;
						throw;
					}
				}
			}
			return result;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			bool result;
			if (this.referenceType == ReferenceType.Runt)
			{
				char[] array = this.rootString.ToCharArray();
				writer.Write(array, 0, array.Length);
				result = true;
			}
			else
			{
				object obj = this.Execute(null, context);
				if (this.escaped)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.escPrefix);
					if (obj == null)
					{
						stringBuilder.Append("\\");
					}
					stringBuilder.Append(this.nullString);
					char[] array = stringBuilder.ToString().ToCharArray();
					writer.Write(array, 0, array.Length);
					result = true;
				}
				else
				{
					EventCartridge eventCartridge = context.EventCartridge;
					if (eventCartridge != null && this.referenceStack != null)
					{
						obj = eventCartridge.ReferenceInsert(this.referenceStack, this.nullString, obj);
					}
					if (obj == null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(this.escPrefix);
						stringBuilder.Append(this.escPrefix);
						stringBuilder.Append(this.morePrefix);
						stringBuilder.Append(this.nullString);
						char[] array = stringBuilder.ToString().ToCharArray();
						writer.Write(array, 0, array.Length);
						if (this.referenceType != ReferenceType.Quiet && this.rsvc.GetBoolean("runtime.log.invalid.references", true))
						{
							this.rsvc.Warn(new ReferenceException("reference : template = " + context.CurrentTemplateName, this));
						}
						result = true;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(this.escPrefix);
						stringBuilder.Append(this.morePrefix);
						stringBuilder.Append(obj);
						char[] array = stringBuilder.ToString().ToCharArray();
						writer.Write(array, 0, array.Length);
						result = true;
					}
				}
			}
			return result;
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			object obj = this.Execute(null, context);
			return obj != null && (!(obj is bool) || (bool)obj);
		}

		public override object Value(IInternalContextAdapter context)
		{
			return this.computableReference ? this.Execute(null, context) : null;
		}

		public bool SetValue(IInternalContextAdapter context, object value)
		{
			object obj = this.GetVariableValue(context, this.rootString);
			bool result;
			if (obj == null)
			{
				this.rsvc.Error(new ReferenceException("reference set : template = " + context.CurrentTemplateName, this));
				result = false;
			}
			else
			{
				for (int i = 0; i < this.numChildren - 1; i++)
				{
					obj = base.GetChild(i).Execute(obj, context);
					if (obj == null)
					{
						this.rsvc.Error(new ReferenceException("reference set : template = " + context.CurrentTemplateName, this));
						result = false;
						return result;
					}
				}
				try
				{
					Type type = obj.GetType();
					PropertyInfo propertyInfo = null;
					try
					{
						propertyInfo = this.rsvc.Introspector.GetProperty(type, this.identifier);
						if (propertyInfo == null)
						{
							throw new MethodAccessException();
						}
					}
					catch (MethodAccessException)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(this.identifier);
						if (char.IsLower(stringBuilder[0]))
						{
							stringBuilder[0] = char.ToUpper(stringBuilder[0]);
						}
						else
						{
							stringBuilder[0] = char.ToLower(stringBuilder[0]);
						}
						propertyInfo = this.rsvc.Introspector.GetProperty(type, stringBuilder.ToString());
						if (propertyInfo == null)
						{
							throw;
						}
					}
					object[] index = new object[0];
					propertyInfo.SetValue(obj, value, index);
				}
				catch (MethodAccessException)
				{
					if (!(obj is IDictionary))
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"ASTReference : cannot find ",
							this.identifier,
							" as settable property or key to Map in template = ",
							context.CurrentTemplateName,
							" [",
							base.Line,
							",",
							base.Column,
							"]"
						}));
						result = false;
						return result;
					}
					try
					{
						IDictionary dictionary = (IDictionary)obj;
						dictionary[this.identifier] = value;
					}
					catch (System.Exception ex)
					{
						this.rsvc.Error(string.Concat(new object[]
						{
							"ASTReference Map.put : exception : ",
							ex,
							" template = ",
							context.CurrentTemplateName,
							" [",
							base.Line,
							",",
							base.Column,
							"]"
						}));
						result = false;
						return result;
					}
				}
				catch (TargetInvocationException ex2)
				{
					throw new MethodInvocationException(string.Concat(new object[]
					{
						"ASTReference : Invocation of method '",
						this.identifier,
						"' in  ",
						obj.GetType(),
						" threw exception ",
						ex2.GetBaseException().GetType()
					}), ex2, this.identifier);
				}
				catch (System.Exception ex3)
				{
					this.rsvc.Error(string.Concat(new object[]
					{
						"ASTReference setValue() : exception : ",
						ex3,
						" template = ",
						context.CurrentTemplateName,
						" [",
						base.Line,
						",",
						base.Column,
						"]"
					}));
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}

		public object GetVariableValue(IContext context, string variable)
		{
			return context.Get(variable);
		}
	}
}
