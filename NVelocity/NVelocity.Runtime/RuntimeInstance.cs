using Commons.Collections;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Log;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using NVelocity.Util;
using NVelocity.Util.Introspection;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NVelocity.Runtime
{
	public class RuntimeInstance : IRuntimeServices, IRuntimeLogger
	{
		private DefaultTraceListener debugOutput = new DefaultTraceListener();

		private VelocimacroFactory vmFactory = null;

		private ILogSystem logSystem;

		private SimplePool parserPool;

		private bool initialized;

		private ExtendedProperties overridingProperties = null;

		private ExtendedProperties configuration;

		private IResourceManager resourceManager = null;

		private Introspector introspector = null;

		private Hashtable applicationAttributes = null;

		private IUberspect uberSpect;

		private IDirectiveManager directiveManager;

		public ExtendedProperties Configuration
		{
			get
			{
				return this.configuration;
			}
			set
			{
				if (this.overridingProperties == null)
				{
					this.overridingProperties = value;
				}
				else if (this.overridingProperties != value)
				{
					this.overridingProperties.Combine(value);
				}
			}
		}

		public Introspector Introspector
		{
			get
			{
				return this.introspector;
			}
		}

		public IUberspect Uberspect
		{
			get
			{
				return this.uberSpect;
			}
		}

		public RuntimeInstance()
		{
			this.logSystem = new PrimordialLogSystem();
			this.configuration = new ExtendedProperties();
			this.vmFactory = new VelocimacroFactory(this);
			this.introspector = new Introspector(this);
			this.applicationAttributes = new Hashtable();
		}

		public void Init()
		{
			lock (this)
			{
				if (!this.initialized)
				{
					this.initializeProperties();
					this.initializeLogger();
					this.initializeResourceManager();
					this.initializeDirectives();
					this.initializeParserPool();
					this.initializeIntrospection();
					this.vmFactory.InitVelocimacro();
					this.initialized = true;
				}
			}
		}

		private void initializeIntrospection()
		{
			string @string = this.GetString("runtime.introspector.uberspect");
			if (@string == null || @string.Length <= 0)
			{
				string message = "It appears that no class was specified as the Uberspect.  Please ensure that all configuration information is correct.";
				this.Error(message);
				throw new System.Exception(message);
			}
			object obj;
			try
			{
				obj = SupportClass.CreateNewInstance(Type.GetType(@string));
			}
			catch (System.Exception)
			{
				string message = "The specified class for Uberspect (" + @string + ") does not exist (or is not accessible to the current classlaoder.";
				this.Error(message);
				throw new System.Exception(message);
			}
			if (!(obj is IUberspect))
			{
				string message = "The specified class for Uberspect (" + @string + ") does not implement org.apache.velocity.util.introspector.Uberspect. Velocity not initialized correctly.";
				this.Error(message);
				throw new System.Exception(message);
			}
			this.uberSpect = (IUberspect)obj;
			if (this.uberSpect is UberspectLoggable)
			{
				((UberspectLoggable)this.uberSpect).RuntimeLogger = this;
			}
			this.uberSpect.Init();
		}

		private void setDefaultProperties()
		{
			try
			{
				this.configuration.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("NVelocity.Runtime.Defaults.nvelocity.properties"));
			}
			catch (System.Exception ex)
			{
				this.debugOutput.WriteLine("Cannot get NVelocity Runtime default properties!\n" + ex.Message);
				this.debugOutput.Flush();
			}
		}

		public void SetProperty(string key, object value)
		{
			if (this.overridingProperties == null)
			{
				this.overridingProperties = new ExtendedProperties();
			}
			this.overridingProperties.SetProperty(key, value);
		}

		public void AddProperty(string key, object value)
		{
			if (this.overridingProperties == null)
			{
				this.overridingProperties = new ExtendedProperties();
			}
			this.overridingProperties.AddProperty(key, value);
		}

		public void ClearProperty(string key)
		{
			if (this.overridingProperties != null)
			{
				this.overridingProperties.ClearProperty(key);
			}
		}

		public object GetProperty(string key)
		{
			return this.configuration.GetProperty(key);
		}

		private void initializeProperties()
		{
			if (!this.configuration.IsInitialized())
			{
				this.setDefaultProperties();
			}
			if (this.overridingProperties != null)
			{
				this.configuration.Combine(this.overridingProperties);
			}
		}

		public void Init(ExtendedProperties p)
		{
			this.overridingProperties = ExtendedProperties.ConvertProperties(p);
			this.Init();
		}

		public void Init(string configurationFile)
		{
			this.overridingProperties = new ExtendedProperties(configurationFile);
			this.Init();
		}

		private void initializeResourceManager()
		{
			IResourceManager resourceManager = (IResourceManager)this.applicationAttributes["resource.manager.class"];
			string @string = this.GetString("resource.manager.class");
			if (resourceManager == null && @string != null && @string.Length > 0)
			{
				object obj;
				try
				{
					Type type = Type.GetType(@string);
					obj = Activator.CreateInstance(type);
				}
				catch (System.Exception)
				{
					string message = "The specified class for Resourcemanager (" + @string + ") does not exist.";
					this.Error(message);
					throw new System.Exception(message);
				}
				if (!(obj is IResourceManager))
				{
					string message = "The specified class for ResourceManager (" + @string + ") does not implement ResourceManager. NVelocity not initialized correctly.";
					this.Error(message);
					throw new System.Exception(message);
				}
				this.resourceManager = (IResourceManager)obj;
				this.resourceManager.Initialize(this);
			}
			else
			{
				if (resourceManager == null)
				{
					string message = "It appears that no class was specified as the ResourceManager.  Please ensure that all configuration information is correct.";
					this.Error(message);
					throw new System.Exception(message);
				}
				this.resourceManager = resourceManager;
				this.resourceManager.Initialize(this);
			}
		}

		private void initializeLogger()
		{
			if (this.logSystem is PrimordialLogSystem)
			{
				PrimordialLogSystem primordialLogSystem = (PrimordialLogSystem)this.logSystem;
				this.logSystem = new NullLogSystem();
				if (this.logSystem == null)
				{
					this.logSystem = new NullLogSystem();
				}
				else
				{
					primordialLogSystem.DumpLogMessages(this.logSystem);
				}
			}
		}

		private void initializeDirectives()
		{
			this.initializeDirectiveManager();
			ExtendedProperties extendedProperties = new ExtendedProperties();
			try
			{
				extendedProperties.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("NVelocity.Runtime.Defaults.directive.properties"));
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("Error loading directive.properties! Something is very wrong if these properties aren't being located. Either your Velocity distribution is incomplete or your Velocity jar file is corrupted!\n" + ex.Message);
			}
			IEnumerator enumerator = extendedProperties.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string directiveTypeName = (string)enumerator.Current;
				this.directiveManager.Register(directiveTypeName);
			}
			string[] stringArray = this.configuration.GetStringArray("userdirective");
			for (int i = 0; i < stringArray.Length; i++)
			{
				this.directiveManager.Register(stringArray[i]);
			}
		}

		private void initializeDirectiveManager()
		{
			string text = this.configuration.GetString("directive.manager");
			if (text == null)
			{
				throw new System.Exception("Looks like there's no 'directive.manager' configured. NVelocity can't go any further");
			}
			text = text.Replace(';', ',');
			Type type = Type.GetType(text, false, false);
			if (type == null)
			{
				throw new System.Exception(string.Format("The type {0} could not be resolved", text));
			}
			this.directiveManager = (IDirectiveManager)Activator.CreateInstance(type);
		}

		private void initializeParserPool()
		{
			int @int = this.GetInt("parser.pool.size", 20);
			this.parserPool = new SimplePool(@int);
			for (int i = 0; i < @int; i++)
			{
				this.parserPool.put(this.CreateNewParser());
			}
			this.Info("Created: " + @int + " parsers.");
		}

		public Parser.Parser CreateNewParser()
		{
			return new Parser.Parser(this)
			{
				Directives = this.directiveManager
			};
		}

		public SimpleNode Parse(TextReader reader, string templateName)
		{
			return this.Parse(reader, templateName, true);
		}

		public SimpleNode Parse(TextReader reader, string templateName, bool dumpNamespace)
		{
			SimpleNode result = null;
            Parser.Parser parser = (Parser.Parser)this.parserPool.get();
			bool flag = false;
			if (parser == null)
			{
				this.Error("Runtime : ran out of parsers. Creating new.   Please increment the parser.pool.size property. The current value is too small.");
				parser = this.CreateNewParser();
				if (parser != null)
				{
					flag = true;
				}
			}
			if (parser != null)
			{
				try
				{
					if (dumpNamespace)
					{
						this.DumpVMNamespace(templateName);
					}
					result = parser.Parse(reader, templateName);
				}
				finally
				{
					if (!flag)
					{
						this.parserPool.put(parser);
					}
				}
			}
			else
			{
				this.Error("Runtime : ran out of parsers and unable to create more.");
			}
			return result;
		}

		public Template GetTemplate(string name)
		{
			return this.GetTemplate(name, this.GetString("input.encoding", "ISO-8859-1"));
		}

		public Template GetTemplate(string name, string encoding)
		{
			return (Template)this.resourceManager.GetResource(name, ResourceType.Template, encoding);
		}

		public ContentResource GetContent(string name)
		{
			return this.GetContent(name, this.GetString("input.encoding", "ISO-8859-1"));
		}

		public ContentResource GetContent(string name, string encoding)
		{
			return (ContentResource)this.resourceManager.GetResource(name, ResourceType.Content, encoding);
		}

		public string GetLoaderNameForResource(string resourceName)
		{
			return this.resourceManager.GetLoaderNameForResource(resourceName);
		}

		private bool showStackTrace()
		{
			return this.configuration.IsInitialized() && this.GetBoolean("runtime.log.warn.stacktrace", false);
		}

		private void Log(LogLevel level, object message)
		{
			string message2;
			if (this.showStackTrace() && (message is System.Exception || message is System.Exception))
			{
				message2 = StringUtils.StackTrace((System.Exception)message);
			}
			else
			{
				message2 = message.ToString();
			}
			this.logSystem.LogVelocityMessage(level, message2);
		}

		public void Warn(object message)
		{
			this.Log(LogLevel.Warn, message);
		}

		public void Info(object message)
		{
			this.Log(LogLevel.Info, message);
		}

		public void Error(object message)
		{
			this.Log(LogLevel.Error, message);
		}

		public void Debug(object message)
		{
			this.Log(LogLevel.Debug, message);
		}

		public string GetString(string key, string defaultValue)
		{
			return this.configuration.GetString(key, defaultValue);
		}

		public Directive.Directive GetVelocimacro(string vmName, string templateName)
		{
			return this.vmFactory.GetVelocimacro(vmName, templateName);
		}

		public bool AddVelocimacro(string name, string macro, string[] argArray, string sourceTemplate)
		{
			return this.vmFactory.AddVelocimacro(name, macro, argArray, sourceTemplate);
		}

		public bool IsVelocimacro(string vmName, string templateName)
		{
			return this.vmFactory.IsVelocimacro(vmName, templateName);
		}

		public bool DumpVMNamespace(string namespace_Renamed)
		{
			return this.vmFactory.DumpVMNamespace(namespace_Renamed);
		}

		public string GetString(string key)
		{
			return this.configuration.GetString(key);
		}

		public int GetInt(string key)
		{
			return this.configuration.GetInt(key);
		}

		public int GetInt(string key, int defaultValue)
		{
			return this.configuration.GetInt(key, defaultValue);
		}

		public bool GetBoolean(string key, bool def)
		{
			return this.configuration.GetBoolean(key, def);
		}

		public object GetApplicationAttribute(object key)
		{
			return this.applicationAttributes[key];
		}

		public object SetApplicationAttribute(object key, object o)
		{
			this.applicationAttributes[key] = o;
			return o;
		}
	}
}
