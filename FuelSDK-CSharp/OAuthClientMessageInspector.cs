using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace FuelSDK
{
    class OAuthClientMessageInspector : IClientMessageInspector
    {
        private readonly string internalAuthToken;

        public OAuthClientMessageInspector(string internalAuthToken)
        {
            this.internalAuthToken = internalAuthToken;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            OAuthMessageHeader oAuthMessageHeader = new OAuthMessageHeader(internalAuthToken);
            request.Headers.Add(oAuthMessageHeader);

            return null;
        }
    }
}
