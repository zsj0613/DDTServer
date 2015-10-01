using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Util.Introspection;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class Foreach : Directive
	{
		private enum EnumType
		{
			Unknown = -1,
			Array = 1,
			Iterator,
			Dictionary,
			Collection,
			Enumeration,
			Enumerable
		}

		private static readonly string[] SectionNames;

		private string counterName;

		private int counterInitialValue;

		private string elementKey;

		public override string Name
		{
			get
			{
				return "foreach";
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
				return DirectiveType.BLOCK;
			}
		}

		static Foreach()
		{
			Foreach.SectionNames = Enum.GetNames(typeof(ForeachSectionEnum));
			Array.Sort(Foreach.SectionNames, CaseInsensitiveComparer.Default);
			for (int i = 0; i < Foreach.SectionNames.Length; i++)
			{
				Foreach.SectionNames[i] = Foreach.SectionNames[i].ToLower();
			}
		}

		public override bool SupportsNestedDirective(string name)
		{
			int num = Array.BinarySearch<string>(Foreach.SectionNames, name.ToLower());
			return num >= 0;
		}

		public override Directive CreateNestedDirective(string name)
		{
			name = name.ToLower();
			Directive result;
			if (name == "between")
			{
				result = new ForeachBetweenSection();
			}
			else if (name == "odd")
			{
				result = new ForeachOddSection();
			}
			else if (name == "even")
			{
				result = new ForeachEvenSection();
			}
			else if (name == "nodata")
			{
				result = new ForeachNoDataSection();
			}
			else if (name == "before")
			{
				result = new ForeachBeforeSection();
			}
			else if (name == "after")
			{
				result = new ForeachAfterSection();
			}
			else if (name == "beforeall")
			{
				result = new ForeachBeforeAllSection();
			}
			else if (name == "afterall")
			{
				result = new ForeachAfterAllSection();
			}
			else
			{
				if (!(name == "each"))
				{
					throw new NotSupportedException("Foreach directive error: Nested directive not supported: " + name);
				}
				result = new ForeachEachSection();
			}
			return result;
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);
			this.counterName = this.rsvc.GetString("directive.foreach.counter.name");
			this.counterInitialValue = this.rsvc.GetInt("directive.foreach.counter.initial.value");
			this.elementKey = node.GetChild(0).FirstToken.Image.Substring(1);
		}

		private IEnumerator GetIterator(IInternalContextAdapter context, INode node)
		{
			object obj = node.GetChild(2).Value(context);
			IEnumerator result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				Foreach.EnumType enumType = Foreach.EnumType.Unknown;
				IntrospectionCacheData introspectionCacheData = context.ICacheGet(this);
				Type type = obj.GetType();
				if (introspectionCacheData != null && introspectionCacheData.ContextData == type)
				{
					enumType = (Foreach.EnumType)introspectionCacheData.Thingy;
				}
				if (enumType == Foreach.EnumType.Unknown)
				{
					if (obj.GetType().IsArray)
					{
						enumType = Foreach.EnumType.Array;
					}
					else if (obj is IDictionary)
					{
						enumType = Foreach.EnumType.Dictionary;
					}
					else if (obj is ICollection)
					{
						enumType = Foreach.EnumType.Collection;
					}
					else if (obj is IEnumerable)
					{
						enumType = Foreach.EnumType.Enumerable;
					}
					else if (obj is IEnumerator)
					{
						enumType = Foreach.EnumType.Enumeration;
					}
					if (enumType != Foreach.EnumType.Unknown)
					{
						introspectionCacheData = new IntrospectionCacheData(type, enumType);
						context.ICachePut(this, introspectionCacheData);
					}
				}
				switch (enumType)
				{
				case Foreach.EnumType.Array:
					result = ((Array)obj).GetEnumerator();
					return result;
				case Foreach.EnumType.Dictionary:
					result = ((IDictionary)obj).GetEnumerator();
					return result;
				case Foreach.EnumType.Collection:
					result = ((ICollection)obj).GetEnumerator();
					return result;
				case Foreach.EnumType.Enumeration:
					this.rsvc.Warn(string.Concat(new object[]
					{
						"Warning! The reference ",
						node.GetChild(2).FirstToken.Image,
						" is an Enumeration in the #foreach() loop at [",
						base.Line,
						",",
						base.Column,
						"] in template ",
						context.CurrentTemplateName,
						". Because it's not resetable, if used in more than once, this may lead to unexpected results."
					}));
					result = (IEnumerator)obj;
					return result;
				case Foreach.EnumType.Enumerable:
					result = ((IEnumerable)obj).GetEnumerator();
					return result;
				}
				this.rsvc.Warn(string.Concat(new object[]
				{
					"Could not determine type of enumerator (",
					obj.GetType().Name,
					") in #foreach loop for ",
					node.GetChild(2).FirstToken.Image,
					" at [",
					base.Line,
					",",
					base.Column,
					"] in template ",
					context.CurrentTemplateName
				}));
				result = null;
			}
			return result;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			IEnumerator iterator = this.GetIterator(context, node);
			INode child = node.GetChild(3);
			INode[][] array = this.PrepareSections(child);
			bool flag = array != null;
			bool result;
			if (iterator == null && !flag)
			{
				result = true;
			}
			else
			{
				int num = this.counterInitialValue;
				object obj = context.Get(this.elementKey);
				object obj2 = context.Get(this.counterName);
				if (iterator != null && iterator.MoveNext())
				{
					do
					{
						object current = iterator.Current;
						context.Put(this.counterName, num);
						context.Put(this.elementKey, current);
						if (!flag)
						{
							child.Render(context, writer);
						}
						else
						{
							if (num == this.counterInitialValue)
							{
								this.ProcessSection(ForeachSectionEnum.BeforeAll, array, context, writer);
							}
							else
							{
								this.ProcessSection(ForeachSectionEnum.Between, array, context, writer);
							}
							this.ProcessSection(ForeachSectionEnum.Before, array, context, writer);
							if ((num - this.counterInitialValue) % 2 == 0)
							{
								this.ProcessSection(ForeachSectionEnum.Odd, array, context, writer);
							}
							else
							{
								this.ProcessSection(ForeachSectionEnum.Even, array, context, writer);
							}
							this.ProcessSection(ForeachSectionEnum.Each, array, context, writer);
							this.ProcessSection(ForeachSectionEnum.After, array, context, writer);
						}
						num++;
					}
					while (iterator.MoveNext());
				}
				if (flag)
				{
					if (num > this.counterInitialValue)
					{
						this.ProcessSection(ForeachSectionEnum.AfterAll, array, context, writer);
					}
					else
					{
						this.ProcessSection(ForeachSectionEnum.NoData, array, context, writer);
					}
				}
				if (obj2 != null)
				{
					context.Put(this.counterName, obj2);
				}
				else
				{
					context.Remove(this.counterName);
				}
				if (obj != null)
				{
					context.Put(this.elementKey, obj);
				}
				else
				{
					context.Remove(this.elementKey);
				}
				result = true;
			}
			return result;
		}

		private void ProcessSection(ForeachSectionEnum sectionEnumType, INode[][] sections, IInternalContextAdapter context, TextWriter writer)
		{
			if (sections[(int)sectionEnumType] != null)
			{
				INode[] array = sections[(int)sectionEnumType];
				for (int i = 0; i < array.Length; i++)
				{
					INode node = array[i];
					node.Render(context, writer);
				}
			}
		}

		private INode[][] PrepareSections(INode node)
		{
			bool flag = false;
			int num = 0;
			ArrayList[] array = new ArrayList[Foreach.SectionNames.Length];
			int childrenCount = node.ChildrenCount;
			for (int i = 0; i < childrenCount; i++)
			{
				INode child = node.GetChild(i);
				ASTDirective aSTDirective = child as ASTDirective;
				if (aSTDirective != null && Array.BinarySearch<string>(Foreach.SectionNames, aSTDirective.DirectiveName) > -1)
				{
					flag = true;
					num = (int)Enum.Parse(typeof(ForeachSectionEnum), aSTDirective.DirectiveName, true);
				}
				else
				{
					if (array[num] == null)
					{
						array[num] = new ArrayList();
					}
					array[num].Add(child);
				}
			}
			INode[][] result;
			if (!flag)
			{
				result = null;
			}
			else
			{
				INode[][] array2 = new INode[array.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array2[i] = (array[i].ToArray(typeof(INode)) as INode[]);
					}
				}
				result = array2;
			}
			return result;
		}
	}
}
