using NVelocity.App.Events;
using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Util.Introspection;
using System;
using System.Reflection;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTIdentifier : SimpleNode
	{
		private string identifier = "";

		protected Info uberInfo;

		public ASTIdentifier(int id) : base(id)
		{
		}

		public ASTIdentifier(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.identifier = base.FirstToken.Image;
			this.uberInfo = new Info(context.CurrentTemplateName, base.Line, base.Column);
			return data;
		}

		public override object Execute(object o, IInternalContextAdapter context)
		{
			IVelPropertyGet velPropertyGet = null;
			try
			{
				Type type = o.GetType();
				IntrospectionCacheData introspectionCacheData = context.ICacheGet(this);
				if (introspectionCacheData != null && introspectionCacheData.ContextData == type)
				{
					velPropertyGet = (IVelPropertyGet)introspectionCacheData.Thingy;
				}
				else
				{
					velPropertyGet = this.rsvc.Uberspect.GetPropertyGet(o, this.identifier, this.uberInfo);
					if (velPropertyGet != null && velPropertyGet.Cacheable)
					{
						introspectionCacheData = new IntrospectionCacheData(type, velPropertyGet);
						context.ICachePut(this, introspectionCacheData);
					}
				}
			}
			catch (System.Exception ex)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					"ASTIdentifier.execute() : identifier = ",
					this.identifier,
					" : ",
					ex
				}));
			}
			object result;
			if (velPropertyGet == null)
			{
				result = null;
			}
			else
			{
				try
				{
					result = velPropertyGet.Invoke(o);
					return result;
				}
				catch (TargetInvocationException ex2)
				{
					EventCartridge eventCartridge = context.EventCartridge;
					string message;
					if (eventCartridge != null)
					{
						try
						{
							result = eventCartridge.HandleMethodException(o.GetType(), velPropertyGet.MethodName, ex2.InnerException);
							return result;
						}
						catch (System.Exception)
						{
							message = string.Format("Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw an exception", new object[]
							{
								velPropertyGet.MethodName,
								(o != null) ? o.GetType().FullName : "",
								this.uberInfo.TemplateName,
								this.uberInfo.Line,
								this.uberInfo.Column
							});
							throw new MethodInvocationException(message, ex2.InnerException, velPropertyGet.MethodName);
						}
					}
					message = string.Format("Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw an exception", new object[]
					{
						velPropertyGet.MethodName,
						(o != null) ? o.GetType().FullName : "",
						this.uberInfo.TemplateName,
						this.uberInfo.Line,
						this.uberInfo.Column
					});
					throw new MethodInvocationException(message, ex2.InnerException, velPropertyGet.MethodName);
				}
				catch (ArgumentException)
				{
					result = null;
					return result;
				}
				catch (System.Exception ex)
				{
					this.rsvc.Error(string.Concat(new object[]
					{
						"ASTIdentifier() : exception invoking method for identifier '",
						this.identifier,
						"' in ",
						o.GetType(),
						" : ",
						ex
					}));
				}
				result = null;
			}
			return result;
		}
	}
}
