using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml.Linq;
using FuelSDK.EndpointBehaviors;
using FuelSDK.Strategies;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{

    /// <summary>
    /// Client interface class which manages the authentication process 
    /// </summary>
    public class ETClient
    {
        public const string SDKVersion = "FuelSDK-C#-v1.2.1";
        internal IFuelSDKConfiguration Configuration { get; set; }
        public string AuthToken { get; internal set; }
        public Soap SoapClient { get; internal set; }
        public string InternalAuthToken { get; internal set; }
        public string RefreshKey { get; internal set; }
        public DateTime AuthTokenExpiration { get; internal set; }
        public JObject Jwt { get; internal set; }
        public string EnterpriseId { get; internal set; }
        public string OrganizationId { get; internal set; }
        private string stackKey;

        public bool UseOAuth2Authentication
        {
            get { return !String.IsNullOrEmpty(Configuration.UseOAuth2Authentication) && Convert.ToBoolean(Configuration.UseOAuth2Authentication); }
        }

        [Obsolete(StackKeyErrorMessage)]
        public string Stack
        {
            get
            {
                if (stackKey != null)
                    return stackKey;

                stackKey = GetStackFromSoapEndPoint(new Uri(Configuration.SoapEndPoint));
                return stackKey;
            }
            internal set
            {
                stackKey = value;
            }
        }

        private static DateTime stackKeyExpiration;
        private readonly IAuthenticationStrategyFactory authenticationStrategyFactory;

        private const string StackKeyErrorMessage = "Tenant specific endpoints doesn't support Stack Key property and this will property will be deprecated in next major release";

        public class RefreshState
        {
            public string RefreshKey { get; set; }
            public string EnterpriseId { get; set; }
            public string OrganizationId { get; set; }
            public string Stack { get; set; }
        }

        internal IAuthService AuthService { get; set; }
        internal IAuthenticationStrategyFactory AuthenticationStrategyFactory { get; set; }
        internal ISoapClientFactory SoapClientFactory { get; set; }

        public ETClient(string jwt)
            : this(new NameValueCollection { { "jwt", jwt } }, null) { }

        internal ETClient(
            IFuelSDKConfiguration configuration, 
            IAuthService authService,
            ISoapClientFactory soapClientFactory,
            IAuthenticationStrategyFactory authenticationStrategyFactory,
            NameValueCollection parameters = null, RefreshState refreshState = null)
        {
            this.Configuration = configuration;
            this.AuthService = authService;
            this.SoapClientFactory = soapClientFactory;
            this.AuthenticationStrategyFactory = authenticationStrategyFactory;

            this.AuthenticationStrategyFactory.Create(this, refreshState, parameters).ObtainToken();
        }

        public ETClient(NameValueCollection parameters = null, RefreshState refreshState = null) 
        {
            this.Configuration = new FuelSDKConfigurationProvider().Get(parameters);
            this.AuthService = new AuthService(Configuration, new HttpWebRequestWrapperFactory(), this);
            this.SoapClientFactory = new SoapClientFactory(Configuration, this);
            this.AuthenticationStrategyFactory = new AuthenticationStrategyFactory();

            this.AuthenticationStrategyFactory.Create(this, refreshState, parameters).ObtainToken();
        }

        internal string FetchRestAuth()
        {
            var returnedRestAuthEndpoint = new ETEndpoint { AuthStub = this, Type = "restAuth" }.Get();
            if (returnedRestAuthEndpoint.Status && returnedRestAuthEndpoint.Results.Length == 1)
                return ((ETEndpoint)returnedRestAuthEndpoint.Results[0]).URL;
            else
                throw new Exception("REST auth endpoint could not be determined");
        }

        private string GetStackFromSoapEndPoint(Uri uri)
        {
            var parts = uri.Host.Split('.');
            if (parts.Length < 2 || !parts[0].Equals("webservice", StringComparison.OrdinalIgnoreCase))
                throw new Exception(StackKeyErrorMessage);
            return (parts[1] == "exacttarget" ? "s1" : parts[1].ToLower());
        }

        public void RefreshToken(bool force = false)
        {
            this.AuthService.RefreshToken(force);
        }

        public FuelReturn AddSubscribersToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, subscriberKey, listIDs); }
        public FuelReturn AddSubscribersToList(string emailAddress, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, null, listIDs); }
        protected FuelReturn ProcessAddSubscriberToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs)
        {
            var sub = new ETSubscriber
            {
                AuthStub = this,
                EmailAddress = emailAddress,
                Lists = listIDs.Select(x => new ETSubscriberList { ID = x }).ToArray(),
            };
            if (subscriberKey != null)
                sub.SubscriberKey = subscriberKey;
            var prAddSub = sub.Post();
            if (!prAddSub.Status && prAddSub.Results.Length > 0 && prAddSub.Results[0].ErrorCode == 12014)
                return sub.Patch();
            return prAddSub;
        }

        public FuelReturn CreateDataExtensions(ETDataExtension[] dataExtensions)
        {
            var cleanedArray = new List<ETDataExtension>();
            foreach (var de in dataExtensions)
            {
                de.AuthStub = this;
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray());
        }


    }

    [Obsolete("ET_Client will be removed in future release. Use ETClient class instead.")]
    public class ET_Client : ETClient
    {
        [Obsolete("This method will be removed in future release.")]
        public FuelReturn CreateDataExtensions(ET_DataExtension[] dataExtensions)
        {
            var cleanedArray = new List<ET_DataExtension>();
            foreach (var de in dataExtensions)
            {
                de.AuthStub = this;
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray());
        }
    }

}