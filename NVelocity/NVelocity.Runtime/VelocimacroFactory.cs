using NVelocity.Runtime.Directive;
using System;
using System.Collections;

namespace NVelocity.Runtime
{
	public class VelocimacroFactory
	{
		private class Twonk
		{
			private VelocimacroFactory enclosingInstance;

			public Template template;

			public long modificationTime;

			public VelocimacroFactory Enclosing_Instance
			{
				get
				{
					return this.enclosingInstance;
				}
			}

			public Twonk(VelocimacroFactory enclosingInstance)
			{
				this.InitBlock(enclosingInstance);
			}

			private void InitBlock(VelocimacroFactory enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
		}

		private IRuntimeServices rsvc = null;

		private VelocimacroManager vmManager = null;

		private bool replaceAllowed = false;

		private bool addNewAllowed = true;

		private bool templateLocal = false;

		private bool blather = false;

		private bool autoReloadLibrary = false;

		private ArrayList macroLibVec = null;

		private Hashtable libModMap;

		private bool TemplateLocalInline
		{
			get
			{
				return this.templateLocal;
			}
			set
			{
				this.templateLocal = value;
			}
		}

		private bool AddMacroPermission
		{
			set
			{
				bool flag = this.addNewAllowed;
				this.addNewAllowed = value;
			}
		}

		private bool ReplacementPermission
		{
			set
			{
				bool flag = this.replaceAllowed;
				this.replaceAllowed = value;
			}
		}

		private bool Blather
		{
			get
			{
				return this.blather;
			}
			set
			{
				this.blather = value;
			}
		}

		private bool Autoload
		{
			get
			{
				return this.autoReloadLibrary;
			}
			set
			{
				this.autoReloadLibrary = value;
			}
		}

		public VelocimacroFactory(IRuntimeServices rs)
		{
			this.rsvc = rs;
			this.libModMap = new Hashtable();
			this.vmManager = new VelocimacroManager(this.rsvc);
		}

		public void InitVelocimacro()
		{
			lock (this)
			{
				this.ReplacementPermission = true;
				this.Blather = true;
				this.LogVMMessageInfo("Velocimacro : initialization starting.");
				this.vmManager.NamespaceUsage = false;
				object property = this.rsvc.GetProperty("velocimacro.library");
				if (property != null)
				{
					if (property is ArrayList)
					{
						this.macroLibVec = (ArrayList)property;
					}
					else if (property is string)
					{
						this.macroLibVec = new ArrayList();
						this.macroLibVec.Add(property);
					}
					for (int i = 0; i < this.macroLibVec.Count; i++)
					{
						string text = (string)this.macroLibVec[i];
						if (text != null && !text.Equals(""))
						{
							this.vmManager.RegisterFromLib = true;
							this.LogVMMessageInfo("Velocimacro : adding VMs from VM library template : " + text);
							try
							{
								Template template = this.rsvc.GetTemplate(text);
								VelocimacroFactory.Twonk twonk = new VelocimacroFactory.Twonk(this);
								twonk.template = template;
								twonk.modificationTime = template.LastModified;
								this.libModMap[text] = twonk;
							}
							catch (System.Exception ex)
							{
								this.LogVMMessageInfo(string.Concat(new object[]
								{
									"Velocimacro : error using  VM library template ",
									text,
									" : ",
									ex
								}));
							}
							this.LogVMMessageInfo("Velocimacro :  VM library template macro registration complete.");
							this.vmManager.RegisterFromLib = false;
						}
					}
				}
				this.AddMacroPermission = true;
				if (!this.rsvc.GetBoolean("velocimacro.permissions.allow.inline", true))
				{
					this.AddMacroPermission = false;
					this.LogVMMessageInfo("Velocimacro : allowInline = false : VMs can not be defined inline in templates");
				}
				else
				{
					this.LogVMMessageInfo("Velocimacro : allowInline = true : VMs can be defined inline in templates");
				}
				this.ReplacementPermission = false;
				if (this.rsvc.GetBoolean("velocimacro.permissions.allow.inline.to.replace.global", false))
				{
					this.ReplacementPermission = true;
					this.LogVMMessageInfo("Velocimacro : allowInlineToOverride = true : VMs defined inline may replace previous VM definitions");
				}
				else
				{
					this.LogVMMessageInfo("Velocimacro : allowInlineToOverride = false : VMs defined inline may NOT replace previous VM definitions");
				}
				this.vmManager.NamespaceUsage = true;
				this.TemplateLocalInline = this.rsvc.GetBoolean("velocimacro.permissions.allow.inline.local.scope", false);
				if (this.TemplateLocalInline)
				{
					this.LogVMMessageInfo("Velocimacro : allowInlineLocal = true : VMs defined inline will be local to their defining template only.");
				}
				else
				{
					this.LogVMMessageInfo("Velocimacro : allowInlineLocal = false : VMs defined inline will be  global in scope if allowed.");
				}
				this.vmManager.TemplateLocalInlineVM = this.TemplateLocalInline;
				this.Blather = this.rsvc.GetBoolean("velocimacro.messages.on", true);
				if (this.Blather)
				{
					this.LogVMMessageInfo("Velocimacro : messages on  : VM system will output logging messages");
				}
				else
				{
					this.LogVMMessageInfo("Velocimacro : messages off : VM system will be quiet");
				}
				this.Autoload = this.rsvc.GetBoolean("velocimacro.library.autoreload", false);
				if (this.Autoload)
				{
					this.LogVMMessageInfo("Velocimacro : autoload on  : VM system will automatically reload global library macros");
				}
				else
				{
					this.LogVMMessageInfo("Velocimacro : autoload off  : VM system will not automatically reload global library macros");
				}
				this.rsvc.Info("Velocimacro : initialization complete.");
			}
		}

		public bool AddVelocimacro(string name, string macroBody, string[] argArray, string sourceTemplate)
		{
			bool result;
			if (name == null || macroBody == null || argArray == null || sourceTemplate == null)
			{
				this.LogVMMessageWarn("Velocimacro : VM addition rejected : programmer error : arg null");
				result = false;
			}
			else if (!this.CanAddVelocimacro(name, sourceTemplate))
			{
				result = false;
			}
			else
			{
				lock (this)
				{
					this.vmManager.AddVM(name, macroBody, argArray, sourceTemplate);
				}
				if (this.blather)
				{
					string text = "#" + argArray[0];
					text += "(";
					for (int i = 1; i < argArray.Length; i++)
					{
						text += " ";
						text += argArray[i];
					}
					text += " ) : source = ";
					text += sourceTemplate;
					this.LogVMMessageInfo("Velocimacro : added new VM : " + text);
				}
				result = true;
			}
			return result;
		}

		private bool CanAddVelocimacro(string name, string sourceTemplate)
		{
			bool result;
			if (this.Autoload)
			{
				for (int i = 0; i < this.macroLibVec.Count; i++)
				{
					string text = (string)this.macroLibVec[i];
					if (text.Equals(sourceTemplate))
					{
						result = true;
						return result;
					}
				}
			}
			if (!this.addNewAllowed)
			{
				this.LogVMMessageWarn("Velocimacro : VM addition rejected : " + name + " : inline VMs not allowed.");
				result = false;
			}
			else
			{
				if (!this.templateLocal)
				{
					if (this.IsVelocimacro(name, sourceTemplate) && !this.replaceAllowed)
					{
						this.LogVMMessageWarn("Velocimacro : VM addition rejected : " + name + " : inline not allowed to replace existing VM");
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private void LogVMMessageInfo(string s)
		{
			if (this.blather)
			{
				this.rsvc.Info(s);
			}
		}

		private void LogVMMessageWarn(string s)
		{
			if (this.blather)
			{
				this.rsvc.Warn(s);
			}
		}

		public bool IsVelocimacro(string vm, string sourceTemplate)
		{
			bool result;
			lock (this)
			{
				if (this.vmManager.get(vm, sourceTemplate) != null)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public Directive.Directive GetVelocimacro(string vmName, string sourceTemplate)
		{
			VelocimacroProxy velocimacroProxy = null;
			lock (this)
			{
				velocimacroProxy = this.vmManager.get(vmName, sourceTemplate);
				if (velocimacroProxy != null && this.Autoload)
				{
					string libraryName = this.vmManager.GetLibraryName(vmName, sourceTemplate);
					if (libraryName != null)
					{
						try
						{
							VelocimacroFactory.Twonk twonk = (VelocimacroFactory.Twonk)this.libModMap[libraryName];
							if (twonk != null)
							{
								Template template = twonk.template;
								long modificationTime = twonk.modificationTime;
								long lastModified = template.ResourceLoader.GetLastModified(template);
								if (lastModified > modificationTime)
								{
									this.LogVMMessageInfo("Velocimacro : autoload reload for VMs from VM library template : " + libraryName);
									twonk.modificationTime = lastModified;
									template = this.rsvc.GetTemplate(libraryName);
									twonk.template = template;
									twonk.modificationTime = template.LastModified;
								}
							}
						}
						catch (System.Exception ex)
						{
							this.LogVMMessageInfo(string.Concat(new object[]
							{
								"Velocimacro : error using  VM library template ",
								libraryName,
								" : ",
								ex
							}));
						}
						velocimacroProxy = this.vmManager.get(vmName, sourceTemplate);
					}
				}
			}
			return velocimacroProxy;
		}

		public bool DumpVMNamespace(string namespace_Renamed)
		{
			return this.vmManager.DumpNamespace(namespace_Renamed);
		}
	}
}
