using System.Xml.Serialization;

namespace ModAchievements.Xml
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(RuntimeConfiguration));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (RuntimeConfiguration)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "Button")]
	public class Button
	{

		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }

		[XmlText]
		public string Text { get; set; }
	}


}
