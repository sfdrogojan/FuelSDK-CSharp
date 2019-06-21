using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using FuelSDK.MessageInspectors;

namespace FuelSDK.EndpointBehaviors
{
    class AddHeadersEndpointBehavior : IEndpointBehavior
    {
        private readonly string authToken;
        private readonly string sdkVersion;

        public AddHeadersEndpointBehavior(string authToken, string sdkVersion)
        {
            this.authToken = authToken;
            this.sdkVersion = sdkVersion;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
#if NET40
            clientRuntime.MessageInspectors.Add(new OAuthClientMessageInspector(this.authToken));
            clientRuntime.MessageInspectors.Add(new UserAgentClientMessageInspector(this.sdkVersion));
#else
            clientRuntime.ClientMessageInspectors.Add(new OAuthClientMessageInspector(this.authToken));
            clientRuntime.ClientMessageInspectors.Add(new UserAgentClientMessageInspector(this.sdkVersion));
#endif

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
