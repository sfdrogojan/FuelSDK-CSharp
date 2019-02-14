using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace FuelSDK
{
    class AddHeadersEndpointBehavior : IEndpointBehavior
    {
        private readonly string internalAuthToken;
        private readonly string sdkVersion;

        public AddHeadersEndpointBehavior(string internalAuthToken, string sdkVersion)
        {
            this.internalAuthToken = internalAuthToken;
            this.sdkVersion = sdkVersion;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new OAuthClientMessageInspector(this.internalAuthToken));
            clientRuntime.MessageInspectors.Add(new UserAgentClientMessageInspector(this.sdkVersion));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
