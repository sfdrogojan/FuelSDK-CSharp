using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;
using FuelSDK.MessageHeaders.Models;

namespace FuelSDK.MessageHeaders
{
    public class SecurityMessageHeader : MessageHeader
    {
        public UsernameToken UsernameToken { get; set; }

        public override string Name => "Security";

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        public override bool MustUnderstand => true;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UsernameToken));
            serializer.Serialize(writer, this.UsernameToken);
        }
    }
}