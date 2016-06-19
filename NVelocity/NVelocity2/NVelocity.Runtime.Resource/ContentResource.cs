using NVelocity.Exception;
using System;
using System.IO;
using System.Text;

namespace NVelocity.Runtime.Resource
{
	public class ContentResource : Resource
	{
		public override bool Process()
		{
			StreamReader streamReader = null;
			bool result;
			try
			{
				StringWriter stringWriter = new StringWriter();
				streamReader = new StreamReader(new StreamReader(this.resourceLoader.GetResourceStream(this.name), System.Text.Encoding.GetEncoding(this.encoding)).BaseStream);
				char[] buffer = new char[1024];
				int count;
				while ((count = streamReader.Read(buffer, 0, 1024)) > 0)
				{
					stringWriter.Write(buffer, 0, count);
				}
				this.data = stringWriter.ToString();
				result = true;
			}
			catch (ResourceNotFoundException ex)
			{
				throw ex;
			}
			catch (System.Exception ex2)
			{
				this.rsvc.Error("Cannot process content resource : " + ex2.ToString());
				result = false;
			}
			finally
			{
				if (streamReader != null)
				{
					try
					{
						streamReader.Close();
					}
					catch (System.Exception)
					{
					}
				}
			}
			return result;
		}
	}
}
