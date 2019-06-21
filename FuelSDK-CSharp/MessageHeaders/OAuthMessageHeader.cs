using System.ServiceModel.Channels;
using System.Xml;

namespace FuelSDK.MessageHeaders
{
    class OAuthMessageHeader : MessageHeader
    {
        private readonly string authToken;

        public OAuthMessageHeader(string authToken)
        {
            this.authToken = authToken;
        }

        public override string Name => "fueloauth";

        public override string Namespace => "http://exacttarget.com";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteString(authToken);
        }
    }
}
