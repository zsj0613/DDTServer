using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using System;
using System.IO;
using System.Text;

namespace NVelocity
{
	public class Template : Resource
	{
		private System.Exception errorCondition = null;

		public override bool Process()
		{
			this.data = null;
			Stream stream = null;
			this.errorCondition = null;
            stream = this.ObtainStream();
			if (stream != null)
			{
                try
				{
					StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding(this.encoding));
					this.data = this.rsvc.Parse(reader, this.name);
					this.InitDocument();
					return true;
				}
				catch (IOException innerException)
				{
					string exceptionMessage = "Template.process : Unsupported input encoding : " + this.encoding + " for template " + this.name;
					throw this.errorCondition = new ParseErrorException(exceptionMessage, innerException);
				}
				catch (ParseException ex)
				{
					throw this.errorCondition = new ParseErrorException(ex.Message, ex);
				}
				catch (System.Exception ex2)
				{
					this.errorCondition = ex2;
					throw;
				}
				finally
				{
					stream.Close();
				}
			}
            throw this.errorCondition = new ResourceNotFoundException("Unknown resource error for resource " + this.name);
		}

		public void InitDocument()
		{
			InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(new VelocityContext());
			try
			{
				internalContextAdapterImpl.PushCurrentTemplateName(this.name);
				((SimpleNode)this.data).Init(internalContextAdapterImpl, this.rsvc);
			}
			finally
			{
				internalContextAdapterImpl.PopCurrentTemplateName();
			}
		}

		public void Merge(IContext context, TextWriter writer)
		{
			if (this.errorCondition != null)
			{
				throw this.errorCondition;
			}
			if (this.data != null)
			{
				InternalContextAdapterImpl internalContextAdapterImpl = new InternalContextAdapterImpl(context);
				try
				{
					internalContextAdapterImpl.PushCurrentTemplateName(this.name);
					internalContextAdapterImpl.CurrentResource = this;
					((SimpleNode)this.data).Render(internalContextAdapterImpl, writer);
				}
				finally
				{
					internalContextAdapterImpl.PopCurrentTemplateName();
					internalContextAdapterImpl.CurrentResource = null;
				}
				return;
			}
			string message = "Template.merge() failure. The document is null, most likely due to parsing error.";
			this.rsvc.Error(message);
			throw new System.Exception(message);
		}

		protected virtual Stream ObtainStream()
		{
			Stream resourceStream;
			try
			{
				resourceStream = this.resourceLoader.GetResourceStream(this.name);
			}
			catch (ResourceNotFoundException ex)
			{
				this.errorCondition = ex;
				throw;
			}
			return resourceStream;
		}
	}
}
