using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace FuelSDK
{
    class UserAgentClientMessageInspector : IClientMessageInspector
    {
        private readonly string sdkVersion;

        public UserAgentClientMessageInspector(string sdkVersion)
        {
            this.sdkVersion = sdkVersion;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            var httpRequest = new HttpRequestMessageProperty();
            httpRequest.Headers.Add(HttpRequestHeader.UserAgent, sdkVersion);
            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);

            return null;
        }
    }
}
