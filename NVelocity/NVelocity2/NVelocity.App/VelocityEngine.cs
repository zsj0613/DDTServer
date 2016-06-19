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
	public class VelocityEngine
	{
		private RuntimeInstance ri = new RuntimeInstance();

		public VelocityEngine()
		{
		}

		public VelocityEngine(string propsFilename)
		{
			this.ri.Init(propsFilename);
		}

		public VelocityEngine(ExtendedProperties p)
		{
			this.ri.Init(p);
		}

		public void SetExtendedProperties(ExtendedProperties value)
		{
			this.ri.Configuration = value;
		}

		public void Init()
		{
			this.ri.Init();
		}

		public void Init(string propsFilename)
		{
			this.ri.Init(propsFilename);
		}

		public void Init(ExtendedProperties p)
		{
			this.ri.Init(p);
		}

		public void SetProperty(string key, object value)
		{
			this.ri.SetProperty(key, value);
		}

		public void AddProperty(string key, object value)
		{
			this.ri.AddProperty(key, value);
		}

		public void ClearProperty(string key)
		{
			this.ri.ClearProperty(key);
		}

		public object GetProperty(string key)
		{
			return this.ri.GetProperty(key);
		}

		public bool Evaluate(IContext context, TextWriter writer, string logTag, string instring)
		{
			return this.Evaluate(context, writer, logTag, new StringReader(instring));
		}

		[Obsolete("Use the overload that takes an TextReader")]
		public bool Evaluate(IContext context, TextWriter writer, string logTag, Stream instream)
		{
			TextReader reader = null;
			string text = null;
			try
			{
				text = this.ri.GetString("input.encoding", "ISO-8859-1");
				reader = new StreamReader(new StreamReader(instream, Encoding.GetEncoding(text)).BaseStream);
			}
			catch (IOException innerException)
			{
				string exceptionMessage = "Unsupported input encoding : " + text + " for template " + logTag;
				throw new ParseErrorException(exceptionMessage, innerException);
			}
			return this.Evaluate(context, writer, logTag, reader);
		}

		public bool Evaluate(IContext context, TextWriter writer, string logTag, TextReader reader)
		{
			SimpleNode simpleNode = null;
			try
			{
				simpleNode = this.ri.Parse(reader, logTag);
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
						simpleNode.Init(internalContextAdapterImpl, this.ri);
					}
					catch (System.Exception ex2)
					{
						this.ri.Error(string.Concat(new object[]
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

		public bool InvokeVelocimacro(string vmName, string logTag, string[] parameters, IContext context, TextWriter writer)
		{
			bool result;
			if (vmName == null || parameters == null || context == null || writer == null || logTag == null)
			{
				this.ri.Error("VelocityEngine.invokeVelocimacro() : invalid parameter");
				result = false;
			}
			else if (!this.ri.IsVelocimacro(vmName, logTag))
			{
				this.ri.Error("VelocityEngine.invokeVelocimacro() : VM '" + vmName + "' not registered.");
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
					bool flag = this.Evaluate(context, writer, logTag, stringBuilder.ToString());
					result = flag;
				}
				catch (System.Exception arg)
				{
					this.ri.Error("VelocityEngine.invokeVelocimacro() : error " + arg);
					throw;
				}
			}
			return result;
		}

		[Obsolete("Use the overload that takes the encoding as parameter")]
		public bool MergeTemplate(string templateName, IContext context, TextWriter writer)
		{
			return this.MergeTemplate(templateName, this.ri.GetString("input.encoding", "ISO-8859-1"), context, writer);
		}

		public bool MergeTemplate(string templateName, string encoding, IContext context, TextWriter writer)
		{
			Template template = this.ri.GetTemplate(templateName, encoding);
			bool result;
			if (template == null)
			{
				this.ri.Error("Velocity.parseTemplate() failed loading template '" + templateName + "'");
				result = false;
			}
			else
			{
				template.Merge(context, writer);
				result = true;
			}
			return result;
		}

		public Template GetTemplate(string name)
		{
			return this.ri.GetTemplate(name);
		}

		public Template GetTemplate(string name, string encoding)
		{
			return this.ri.GetTemplate(name, encoding);
		}

		public bool TemplateExists(string templateName)
		{
			return this.ri.GetLoaderNameForResource(templateName) != null;
		}

		public void Warn(object message)
		{
			this.ri.Warn(message);
		}

		public void Info(object message)
		{
			this.ri.Info(message);
		}

		public void Error(object message)
		{
			this.ri.Error(message);
		}

		public void Debug(object message)
		{
			this.ri.Debug(message);
		}

		public void SetApplicationAttribute(object key, object value)
		{
			this.ri.SetApplicationAttribute(key, value);
		}
	}
}
