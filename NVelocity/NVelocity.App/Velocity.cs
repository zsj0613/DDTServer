using Commons.Collections;
using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;
using System.Text;

namespace NVelocity.App
{
	public class Velocity
	{
		public static void Init()
		{
			RuntimeSingleton.Init();
		}

		public static void Init(string propsFilename)
		{
			RuntimeSingleton.Init(propsFilename);
		}

		public static void Init(ExtendedProperties p)
		{
			RuntimeSingleton.Init(p);
		}

		public static void SetProperty(string key, object value)
		{
			RuntimeSingleton.SetProperty(key, value);
		}

		public static void AddProperty(string key, object value)
		{
			RuntimeSingleton.AddProperty(key, value);
		}

		public static void ClearProperty(string key)
		{
			RuntimeSingleton.ClearProperty(key);
		}

		public static void SetExtendedProperties(ExtendedProperties value)
		{
			RuntimeSingleton.Configuration = value;
		}

		public static object GetProperty(string key)
		{
			return RuntimeSingleton.GetProperty(key);
		}

		public static bool Evaluate(IContext context, TextWriter writer, string logTag, string instring)
		{
			return Velocity.Evaluate(context, writer, logTag, new StringReader(instring));
		}

		[Obsolete("Use the overload that takes a TextReader")]
		public static bool Evaluate(IContext context, TextWriter writer, string logTag, Stream instream)
		{
			TextReader reader = null;
			string text = null;
			try
			{
				text = RuntimeSingleton.getString("input.encoding", "ISO-8859-1");
				reader = new StreamReader(new StreamReader(instream, Encoding.GetEncoding(text)).BaseStream);
			}
			catch (IOException innerException)
			{
				string exceptionMessage = "Unsupported input encoding : " + text + " for template " + logTag;
				throw new ParseErrorException(exceptionMessage, innerException);
			}
			return Velocity.Evaluate(context, writer, logTag, reader);
		}

		public static bool Evaluate(IContext context, TextWriter writer, string logTag, TextReader reader)
		{
			SimpleNode simpleNode = null;
			try
			{
				simpleNode = RuntimeSingleton.Parse(reader, logTag);
			}
			catch (ParseException ex)
			{
				throw new ParseErrorException(ex.Message, ex);
			}
			bool result;
			if (simpleNode != null)
			{
				InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(context);
				internalContextAdapterImpl.PushCurrentTemplateName(logTag);
				try
				{
					try
					{
						simpleNode.Init(internalContextAdapterImpl, RuntimeSingleton.RuntimeServices);
					}
					catch (System.Exception ex2)
					{
						RuntimeSingleton.Error(string.Concat(new object[]
						{
							"Velocity.evaluate() : init exception for tag = ",
							logTag,
							" : ",
							ex2
						}));
					}
					simpleNode.Render(internalContextAdapterImpl, writer);
				}
				finally
				{
					internalContextAdapterImpl.PopCurrentTemplateName();
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool InvokeVelocimacro(string vmName, string logTag, string[] parameters, IContext context, TextWriter writer)
		{
			bool result;
			if (vmName == null || parameters == null || context == null || writer == null || logTag == null)
			{
				RuntimeSingleton.Error("Velocity.invokeVelocimacro() : invalid parameter");
				result = false;
			}
			else if (!RuntimeSingleton.IsVelocimacro(vmName, logTag))
			{
				RuntimeSingleton.Error("Velocity.invokeVelocimacro() : VM '" + vmName + "' not registered.");
				result = false;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder("#");
				stringBuilder.Append(vmName);
				stringBuilder.Append("(");
				for (int i = 0; i < parameters.Length; i++)
				{
					stringBuilder.Append(" $");
					stringBuilder.Append(parameters[i]);
				}
				stringBuilder.Append(" )");
				try
				{
					bool flag = Velocity.Evaluate(context, writer, logTag, stringBuilder.ToString());
					result = flag;
					return result;
				}
				catch (System.Exception arg)
				{
					RuntimeSingleton.Error("Velocity.invokeVelocimacro() : error " + arg);
				}
				result = false;
			}
			return result;
		}

		[Obsolete("Use the overload that takes an encoding")]
		public static bool MergeTemplate(string templateName, IContext context, TextWriter writer)
		{
			return Velocity.MergeTemplate(templateName, RuntimeSingleton.getString("input.encoding", "ISO-8859-1"), context, writer);
		}

		public static bool MergeTemplate(string templateName, string encoding, IContext context, TextWriter writer)
		{
			Template template = RuntimeSingleton.GetTemplate(templateName, encoding);
			bool result;
			if (template == null)
			{
				RuntimeSingleton.Error("Velocity.parseTemplate() failed loading template '" + templateName + "'");
				result = false;
			}
			else
			{
				template.Merge(context, writer);
				result = true;
			}
			return result;
		}

		public static Template GetTemplate(string name)
		{
			return RuntimeSingleton.GetTemplate(name);
		}

		public static Template GetTemplate(string name, string encoding)
		{
			return RuntimeSingleton.GetTemplate(name, encoding);
		}

		public static bool ResourceExists(string templateName)
		{
			return RuntimeSingleton.GetLoaderNameForResource(templateName) != null;
		}

		public static void Warn(object message)
		{
			RuntimeSingleton.Warn(message);
		}

		public static void Info(object message)
		{
			RuntimeSingleton.Info(message);
		}

		public static void Error(object message)
		{
			RuntimeSingleton.Error(message);
		}

		public static void Debug(object message)
		{
			RuntimeSingleton.Debug(message);
		}

		public static void SetApplicationAttribute(object key, object value)
		{
			RuntimeSingleton.RuntimeServices.SetApplicationAttribute(key, value);
		}

		[Obsolete("Use ResourceExists(String) instead")]
		public static bool TemplateExists(string resourceName)
		{
			return Velocity.ResourceExists(resourceName);
		}
	}
}
