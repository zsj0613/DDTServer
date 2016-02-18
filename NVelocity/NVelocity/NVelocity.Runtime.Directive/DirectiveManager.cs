using System;
using System.Collections;

namespace NVelocity.Runtime.Directive
{
	public class DirectiveManager : IDirectiveManager
	{
		private IDictionary name2Type = Hashtable.Synchronized(new Hashtable());

		public virtual void Register(string directiveTypeName)
		{
			directiveTypeName = directiveTypeName.Replace(';', ',');
			Type type = Type.GetType(directiveTypeName, false, false);
			if (type == null)
			{
				throw new System.Exception("Could not resolve type " + directiveTypeName);
			}
			Directive directive = (Directive)Activator.CreateInstance(type);
			this.name2Type[directive.Name] = type;
		}

		public virtual Directive Create(string name, Stack directiveStack)
		{
			Type type = (Type)this.name2Type[name];
			Directive result;
			if (type != null)
			{
				result = (Directive)Activator.CreateInstance(type);
			}
			else
			{
				if (directiveStack.Count != 0)
				{
					Directive directive = (Directive)directiveStack.Peek();
					if (directive.SupportsNestedDirective(name))
					{
						result = directive.CreateNestedDirective(name);
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public virtual bool Contains(string name)
		{
			return this.name2Type.Contains(name);
		}
	}
}
