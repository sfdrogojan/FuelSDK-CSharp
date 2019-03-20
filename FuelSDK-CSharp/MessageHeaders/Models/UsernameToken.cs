using System.Xml.Serialization;

namespace FuelSDK.MessageHeaders.Models
{
    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class UsernameToken
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public string Username { get; set; }
        [XmlElement]
        public string Password { get; set; }
    }
}