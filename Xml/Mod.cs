using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModAchievements.Xml
{
    [XmlRoot(ElementName = "Mod")]
	public class Mod
	{

		[XmlElement(ElementName = "Button")]
		public List<Button> Button { get; set; }

		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }

		[XmlAttribute(AttributeName = "UniqueID")]
		public string UniqueID { get; set; }

		[XmlAttribute(AttributeName = "Version")]
		public string Version { get; set; }

		[XmlText]
		public string Text { get; set; }
	}


}
