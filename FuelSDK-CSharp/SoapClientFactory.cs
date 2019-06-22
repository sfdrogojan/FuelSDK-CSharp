using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FuelSDK.EndpointBehaviors;

namespace FuelSDK
{
    public class SoapClientFactory : ISoapClientFactory
    {
        private readonly IFuelSDKConfiguration configuration;
        private readonly string authToken;
        private readonly string sdkVersion;
        private static DateTime soapEndPointExpiration;
        private static string fetchedSoapEndpoint;
        private const long cacheDurationInMinutes = 10;
        private readonly ETClient etClient;

        public SoapClientFactory(IFuelSDKConfiguration configuration, string authToken, string sdkVersion, ETClient etClient)
        {
            this.configuration = configuration;
            this.authToken = authToken;
            this.sdkVersion = sdkVersion;
            this.etClient = etClient;
        }

        public Soap Create()
        {
            FetchSoapEndpoint();
            var binding = GetSoapBinding();
            var endpointAddress = new EndpointAddress(new Uri(configuration.SoapEndPoint));

#if NET40
            ChannelFactory<Soap> channelFactory = new ChannelFactory<Soap>(binding, endpointAddress);
            channelFactory.Endpoint.Behaviors.Add(new AddHeadersEndpointBehavior(authToken, sdkVersion));
#else
            ChannelFactory<Soap> channelFactory = new ChannelFactory<Soap>(new BasicHttpsBinding(BasicHttpsSecurityMode.Transport), endpointAddress);
            channelFactory.Endpoint.EndpointBehaviors.Add(new AddHeadersEndpointBehavior(authToken, sdkVersion));
#endif

            return channelFactory.CreateChannel();
        }

        private void FetchSoapEndpoint()
        {
            if (string.IsNullOrEmpty(configuration.SoapEndPoint) || (DateTime.Now > soapEndPointExpiration && fetchedSoapEndpoint != null))
            {
                try
                {
                    var grSingleEndpoint = new ETEndpoint { AuthStub = etClient, Type = "soap" }.Get();
                    if (grSingleEndpoint.Status && grSingleEndpoint.Results.Length == 1)
                    {
                        // Find the appropriate endpoint for the account
                        configuration.SoapEndPoint = ((ETEndpoint)grSingleEndpoint.Results[0]).URL;
                        fetchedSoapEndpoint = configuration.SoapEndPoint;
                        soapEndPointExpiration = DateTime.Now.AddMinutes(cacheDurationInMinutes);
                    }
                    else
                        configuration.SoapEndPoint = DefaultEndpoints.Soap;
                }
                catch (Exception ex)
                {
                    configuration.SoapEndPoint = DefaultEndpoints.Soap;
                }
            }
        }

        private Binding GetSoapBinding()
        {
            BindingElementCollection bindingCollection = new BindingElementCollection();
            bindingCollection.AddRange(new TextMessageEncodingBindingElement
                {
                    MessageVersion = MessageVersion.Soap12WSAddressingAugust2004,
                    ReaderQuotas =
                    {
                        MaxDepth = 32,
                        MaxStringContentLength = int.MaxValue,
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue
                    }
                },
                new HttpsTransportBindingElement
                {
                    TransferMode = TransferMode.Buffered,
                    MaxReceivedMessageSize = 655360000,
                    MaxBufferSize = 655360000,
                    KeepAliveEnabled = true
                });

            return new CustomBinding(bindingCollection)
            {
                Name = "UserNameSoapBinding",
                Namespace = "Core.Soap",
                CloseTimeout = new TimeSpan(0, 50, 0),
                OpenTimeout = new TimeSpan(0, 50, 0),
                ReceiveTimeout = new TimeSpan(0, 50, 0),
                SendTimeout = new TimeSpan(0, 50, 0)
            };
        }
    }
}