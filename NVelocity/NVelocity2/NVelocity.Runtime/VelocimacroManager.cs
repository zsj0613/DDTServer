using NVelocity.Context;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Util;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime
{
	public class VelocimacroManager
	{
		protected internal class MacroEntry
		{
			private VelocimacroManager enclosingInstance;

			internal string macroname;

			internal string[] argarray;

			internal string macrobody;

			internal string sourcetemplate;

			internal SimpleNode nodeTree = null;

			internal VelocimacroManager manager = null;

			internal bool fromLibrary = false;

			public bool FromLibrary
			{
				get
				{
					return this.fromLibrary;
				}
				set
				{
					this.fromLibrary = value;
				}
			}

			public SimpleNode NodeTree
			{
				get
				{
					return this.nodeTree;
				}
			}

			public string SourceTemplate
			{
				get
				{
					return this.sourcetemplate;
				}
			}

			public VelocimacroManager Enclosing_Instance
			{
				get
				{
					return this.enclosingInstance;
				}
			}

			private void InitBlock(VelocimacroManager enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			internal MacroEntry(VelocimacroManager enclosingInstance, VelocimacroManager vmm, string vmName, string macroBody, string[] argArray, string sourceTemplate)
			{
				this.InitBlock(enclosingInstance);
				this.macroname = vmName;
				this.argarray = argArray;
				this.macrobody = macroBody;
				this.sourcetemplate = sourceTemplate;
				this.manager = vmm;
			}

			internal VelocimacroProxy CreateVelocimacro(string namespace_Renamed)
			{
				return new VelocimacroProxy
				{
					Name = this.macroname,
					ArgArray = this.argarray,
					Macrobody = this.macrobody,
					NodeTree = this.nodeTree,
					Namespace = namespace_Renamed
				};
			}

			internal void setup(IInternalContextAdapter ica)
			{
				if (this.nodeTree == null)
				{
					this.parseTree(ica);
				}
			}

			internal void parseTree(IInternalContextAdapter ica)
			{
				try
				{
					TextReader reader = new StringReader(this.macrobody);
					this.nodeTree = this.Enclosing_Instance.rsvc.Parse(reader, "VM:" + this.macroname, true);
					this.nodeTree.Init(ica, null);
				}
				catch (System.Exception e)
				{
					this.Enclosing_Instance.rsvc.Error("VelocimacroManager.parseTree() : exception " + this.macroname + " : " + StringUtils.StackTrace(e));
				}
			}
		}

		private IRuntimeServices rsvc = null;

		private static string GLOBAL_NAMESPACE = "";

		private bool registerFromLib = false;

		private Hashtable namespaceHash;

		private Hashtable libraryMap;

		private bool namespacesOn = true;

		private bool inlineLocalMode = false;

		public bool NamespaceUsage
		{
			set
			{
				this.namespacesOn = value;
			}
		}

		public bool RegisterFromLib
		{
			set
			{
				this.registerFromLib = value;
			}
		}

		public bool TemplateLocalInlineVM
		{
			set
			{
				this.inlineLocalMode = value;
			}
		}

		internal VelocimacroManager(IRuntimeServices rs)
		{
			this.InitBlock();
			this.rsvc = rs;
			this.AddNamespace(VelocimacroManager.GLOBAL_NAMESPACE);
		}

		private void InitBlock()
		{
			this.namespaceHash = new Hashtable();
			this.libraryMap = new Hashtable();
		}

		public bool AddVM(string vmName, string macroBody, string[] argArray, string ns)
		{
			VelocimacroManager.MacroEntry macroEntry = new VelocimacroManager.MacroEntry(this, this, vmName, macroBody, argArray, ns);
			macroEntry.FromLibrary = this.registerFromLib;
			bool flag = true;
			if (this.registerFromLib)
			{
				SupportClass.PutElement(this.libraryMap, ns, ns);
			}
			else
			{
				flag = this.libraryMap.ContainsKey(ns);
			}
			bool result;
			if (!flag && this.UsingNamespaces(ns))
			{
				Hashtable @namespace = this.GetNamespace(ns, true);
				SupportClass.PutElement(@namespace, vmName, macroEntry);
				result = true;
			}
			else
			{
				VelocimacroManager.MacroEntry macroEntry2 = (VelocimacroManager.MacroEntry)this.GetNamespace(VelocimacroManager.GLOBAL_NAMESPACE)[vmName];
				if (macroEntry2 != null)
				{
					macroEntry.FromLibrary = macroEntry2.FromLibrary;
				}
				SupportClass.PutElement(this.GetNamespace(VelocimacroManager.GLOBAL_NAMESPACE), vmName, macroEntry);
				result = true;
			}
			return result;
		}

		public VelocimacroProxy get(string vmName, string namespace_Renamed)
		{
			VelocimacroProxy result;
			if (this.UsingNamespaces(namespace_Renamed))
			{
				Hashtable @namespace = this.GetNamespace(namespace_Renamed, false);
				if (@namespace != null)
				{
					VelocimacroManager.MacroEntry macroEntry = (VelocimacroManager.MacroEntry)@namespace[vmName];
					if (macroEntry != null)
					{
						result = macroEntry.CreateVelocimacro(namespace_Renamed);
						return result;
					}
				}
			}
			VelocimacroManager.MacroEntry macroEntry2 = (VelocimacroManager.MacroEntry)this.GetNamespace(VelocimacroManager.GLOBAL_NAMESPACE)[vmName];
			if (macroEntry2 != null)
			{
				result = macroEntry2.CreateVelocimacro(namespace_Renamed);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public bool DumpNamespace(string ns)
		{
			bool result;
			lock (this)
			{
				if (this.UsingNamespaces(ns))
				{
					Hashtable hashtable = this.namespaceHash;
					Hashtable hashtable2 = (Hashtable)hashtable[ns];
					hashtable.Remove(ns);
					if (hashtable2 == null)
					{
						result = false;
					}
					else
					{
						hashtable2.Clear();
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private Hashtable GetNamespace(string ns)
		{
			return this.GetNamespace(ns, false);
		}

		private Hashtable GetNamespace(string ns, bool addIfNew)
		{
			Hashtable hashtable = (Hashtable)this.namespaceHash[ns];
			if (hashtable == null && addIfNew)
			{
				hashtable = this.AddNamespace(ns);
			}
			return hashtable;
		}

		private Hashtable AddNamespace(string ns)
		{
			Hashtable hashtable = new Hashtable();
			object newValue;
			Hashtable result;
			if ((newValue = SupportClass.PutElement(this.namespaceHash, ns, hashtable)) != null)
			{
				SupportClass.PutElement(this.namespaceHash, ns, newValue);
				result = null;
			}
			else
			{
				result = hashtable;
			}
			return result;
		}

		private bool UsingNamespaces(string ns)
		{
			return this.namespacesOn && this.inlineLocalMode;
		}

		public string GetLibraryName(string vmName, string ns)
		{
			string result;
			if (this.UsingNamespaces(ns))
			{
				Hashtable @namespace = this.GetNamespace(ns, false);
				if (@namespace != null)
				{
					VelocimacroManager.MacroEntry macroEntry = (VelocimacroManager.MacroEntry)@namespace[vmName];
					if (macroEntry != null)
					{
						result = null;
						return result;
					}
				}
			}
			VelocimacroManager.MacroEntry macroEntry2 = (VelocimacroManager.MacroEntry)this.GetNamespace(VelocimacroManager.GLOBAL_NAMESPACE)[vmName];
			if (macroEntry2 != null)
			{
				result = macroEntry2.SourceTemplate;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
