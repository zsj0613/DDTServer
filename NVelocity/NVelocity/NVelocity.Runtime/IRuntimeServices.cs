using Commons.Collections;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;
using System.IO;

namespace NVelocity.Runtime
{
	public interface IRuntimeServices : IRuntimeLogger
	{
		ExtendedProperties Configuration
		{
			get;
			set;
		}

		IUberspect Uberspect
		{
			get;
		}

		Introspector Introspector
		{
			get;
		}

		void Init();

		void SetProperty(string key, object value);

		void AddProperty(string key, object value);

		void ClearProperty(string key);

		object GetProperty(string key);

		void Init(string configurationFile);

		SimpleNode Parse(TextReader reader, string templateName);

		SimpleNode Parse(TextReader reader, string templateName, bool dumpNamespace);

		Template GetTemplate(string name);

		Template GetTemplate(string name, string encoding);

		ContentResource GetContent(string name);

		ContentResource GetContent(string name, string encoding);

		string GetLoaderNameForResource(string resourceName);

		string GetString(string key, string defaultValue);

        Directive.Directive GetVelocimacro(string vmName, string templateName);

		bool AddVelocimacro(string name, string macro, string[] argArray, string sourceTemplate);

		bool IsVelocimacro(string vmName, string templateName);

		bool DumpVMNamespace(string ns);

		string GetString(string key);

		int GetInt(string key);

		int GetInt(string key, int defaultValue);

		bool GetBoolean(string key, bool def);

		object GetApplicationAttribute(object key);

		object SetApplicationAttribute(object key, object value);
	}
}
