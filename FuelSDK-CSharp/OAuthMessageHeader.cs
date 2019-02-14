using System.ServiceModel.Channels;
using System.Xml;

namespace FuelSDK
{
    class OAuthMessageHeader : MessageHeader
    {
        private readonly string internalAuthToken;

        public OAuthMessageHeader(string internalAuthToken)
        {
            this.internalAuthToken = internalAuthToken;
        }

        public override string Name => "oAuth";

        public override string Namespace => "http://exacttarget.com";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteElementString("oAuthToken", internalAuthToken);
        }
    }
}
