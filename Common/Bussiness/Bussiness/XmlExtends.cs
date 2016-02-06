using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace Bussiness
{
	public static class XmlExtends
	{
		public static string ToString(this XElement node, bool check)
		{
			StringBuilder sb = new StringBuilder();
			using (XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings
			{
				CheckCharacters = check,
				OmitXmlDeclaration = true,
				Indent = true
			}))
			{
				node.WriteTo(xw);
			}
			return sb.ToString();
		}
	}
}
