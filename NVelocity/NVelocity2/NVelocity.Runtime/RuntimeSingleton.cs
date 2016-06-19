using Commons.Collections;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;
using System.IO;

namespace NVelocity.Runtime
{
	public class RuntimeSingleton
	{
		private static RuntimeInstance ri = new RuntimeInstance();

		public static IRuntimeServices RuntimeServices
		{
			get
			{
				return RuntimeSingleton.ri;
			}
		}

		public static ExtendedProperties Configuration
		{
			get
			{
				return RuntimeSingleton.ri.Configuration;
			}
			set
			{
				RuntimeSingleton.ri.Configuration = value;
			}
		}

		public static Introspector Introspector
		{
			get
			{
				return RuntimeSingleton.ri.Introspector;
			}
		}

		[Obsolete("Use the RuntimeServices property instead")]
		public static RuntimeInstance RuntimeInstance
		{
			get
			{
				return RuntimeSingleton.ri;
			}
		}

		public static void Init()
		{
			lock (typeof(RuntimeSingleton))
			{
				RuntimeSingleton.ri.Init();
			}
		}

		public static void SetProperty(string key, object value)
		{
			RuntimeSingleton.ri.SetProperty(key, value);
		}

		public static void AddProperty(string key, object value)
		{
			RuntimeSingleton.ri.AddProperty(key, value);
		}

		public static void ClearProperty(string key)
		{
			RuntimeSingleton.ri.ClearProperty(key);
		}

		public static object GetProperty(string key)
		{
			return RuntimeSingleton.ri.GetProperty(key);
		}

		public static void Init(ExtendedProperties p)
		{
			RuntimeSingleton.ri.Init(p);
		}

		public static void Init(string configurationFile)
		{
			RuntimeSingleton.ri.Init(configurationFile);
		}

		private static Parser.Parser CreateNewParser()
		{
			return RuntimeSingleton.ri.CreateNewParser();
		}

		public static SimpleNode Parse(TextReader reader, string templateName)
		{
			return RuntimeSingleton.ri.Parse(reader, templateName);
		}

		public static SimpleNode Parse(TextReader reader, string templateName, bool dumpNamespace)
		{
			return RuntimeSingleton.ri.Parse(reader, templateName, dumpNamespace);
		}

		public static Template GetTemplate(string name)
		{
			return RuntimeSingleton.ri.GetTemplate(name);
		}

		public static Template GetTemplate(string name, string encoding)
		{
			return RuntimeSingleton.ri.GetTemplate(name, encoding);
		}

		public static ContentResource GetContent(string name)
		{
			return RuntimeSingleton.ri.GetContent(name);
		}

		public static ContentResource GetContent(string name, string encoding)
		{
			return RuntimeSingleton.ri.GetContent(name, encoding);
		}

		public static string GetLoaderNameForResource(string resourceName)
		{
			return RuntimeSingleton.ri.GetLoaderNameForResource(resourceName);
		}

		public static void Warn(object message)
		{
			RuntimeSingleton.ri.Warn(message);
		}

		public static void Info(object message)
		{
			RuntimeSingleton.ri.Info(message);
		}

		public static void Error(object message)
		{
			RuntimeSingleton.ri.Error(message);
		}

		public static void Debug(object message)
		{
			RuntimeSingleton.ri.Debug(message);
		}

		public static string getString(string key, string defaultValue)
		{
			return RuntimeSingleton.ri.GetString(key, defaultValue);
		}

		public static Directive.Directive GetVelocimacro(string vmName, string templateName)
		{
			return RuntimeSingleton.ri.GetVelocimacro(vmName, templateName);
		}

		public static bool AddVelocimacro(string name, string macro, string[] argArray, string sourceTemplate)
		{
			return RuntimeSingleton.ri.AddVelocimacro(name, macro, argArray, sourceTemplate);
		}

		public static bool IsVelocimacro(string vmName, string templateName)
		{
			return RuntimeSingleton.ri.IsVelocimacro(vmName, templateName);
		}

		public static bool DumpVMNamespace(string namespace_Renamed)
		{
			return RuntimeSingleton.ri.DumpVMNamespace(namespace_Renamed);
		}

		public static string GetString(string key)
		{
			return RuntimeSingleton.ri.GetString(key);
		}

		public static int GetInt(string key)
		{
			return RuntimeSingleton.ri.GetInt(key);
		}

		public static int GetInt(string key, int defaultValue)
		{
			return RuntimeSingleton.ri.GetInt(key, defaultValue);
		}

		public static bool GetBoolean(string key, bool def)
		{
			return RuntimeSingleton.ri.GetBoolean(key, def);
		}

		public static object GetApplicationAttribute(object key)
		{
			return RuntimeSingleton.ri.GetApplicationAttribute(key);
		}
	}
}
