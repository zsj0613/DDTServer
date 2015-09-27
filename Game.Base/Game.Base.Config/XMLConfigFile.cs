using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
namespace Game.Base.Config
{
	public class XMLConfigFile : ConfigElement
	{
		public XMLConfigFile() : base(null)
		{
		}
		protected XMLConfigFile(ConfigElement parent) : base(parent)
		{
		}
		protected bool IsBadXMLElementName(string name)
		{
			return name != null && (name.IndexOf("\\") != -1 || name.IndexOf("/") != -1 || name.IndexOf("<") != -1 || name.IndexOf(">") != -1);
		}
		protected void SaveElement(XmlTextWriter writer, string name, ConfigElement element)
		{
			bool badName = this.IsBadXMLElementName(name);
			if (element.HasChildren)
			{
				if (name == null)
				{
					name = "root";
				}
				if (badName)
				{
					writer.WriteStartElement("param");
					writer.WriteAttributeString("name", name);
				}
				else
				{
					writer.WriteStartElement(name);
				}
				foreach (DictionaryEntry entry in element.Children)
				{
					this.SaveElement(writer, (string)entry.Key, (ConfigElement)entry.Value);
				}
				writer.WriteEndElement();
			}
			else
			{
				if (name != null)
				{
					if (badName)
					{
						writer.WriteStartElement("param");
						writer.WriteAttributeString("name", name);
						writer.WriteString(element.GetString());
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteElementString(name, element.GetString());
					}
				}
			}
		}
		public void Save(FileInfo configFile)
		{
			if (configFile.Exists)
			{
				configFile.Delete();
			}
			XmlTextWriter writer = new XmlTextWriter(configFile.FullName, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument();
			this.SaveElement(writer, null, this);
			writer.WriteEndDocument();
			writer.Close();
		}
		public static XMLConfigFile ParseXMLFile(FileInfo configFile)
		{
			XMLConfigFile root = new XMLConfigFile(null);
			XMLConfigFile result;
			if (!configFile.Exists)
			{
				result = root;
			}
			else
			{
				ConfigElement current = root;
				XmlTextReader reader = new XmlTextReader(configFile.OpenRead());
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (!(reader.Name == "root"))
						{
							if (reader.Name == "param")
							{
								string name = reader.GetAttribute("name");
								if (name != null && name != "root")
								{
									ConfigElement newElement = new ConfigElement(current);
									current[name] = newElement;
									current = newElement;
								}
							}
							else
							{
								ConfigElement newElement = new ConfigElement(current);
								current[reader.Name] = newElement;
								current = newElement;
							}
						}
					}
					else
					{
						if (reader.NodeType == XmlNodeType.Text)
						{
							current.Set(reader.Value);
						}
						else
						{
							if (reader.NodeType == XmlNodeType.EndElement)
							{
								if (reader.Name != "root")
								{
									current = current.Parent;
								}
							}
						}
					}
				}
				reader.Close();
				result = root;
			}
			return result;
		}
	}
}
