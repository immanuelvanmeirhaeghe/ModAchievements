using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModAchievements.Xml
{
    [XmlRoot(ElementName = "RuntimeConfiguration")]
	public class RuntimeConfiguration
	{

		[ XmlElement(ElementName = "Mod")]
		public List<Mod> Mod { get; set; }
	}

}
