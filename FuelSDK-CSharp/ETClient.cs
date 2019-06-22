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

        private IFuelSDKConfiguration configuration;
        public string AuthToken { get; private set; }
        public Soap SoapClient { get; private set; }
        public string InternalAuthToken { get; private set; }
        public string RefreshKey { get; private set; }
        public DateTime AuthTokenExpiration { get; private set; }
        public JObject Jwt { get; private set; }
        public string EnterpriseId { get; private set; }
        public string OrganizationId { get; private set; }
        private string stackKey;

        public bool UseOAuth2Authentication
        {
            get { return !String.IsNullOrEmpty(configuration.UseOAuth2Authentication) && Convert.ToBoolean(configuration.UseOAuth2Authentication); }
        }

        [Obsolete(StackKeyErrorMessage)]
        public string Stack
        {
            get
            {
                if (stackKey != null)
                    return stackKey;

                stackKey = GetStackFromSoapEndPoint(new Uri(configuration.SoapEndPoint));
                return stackKey;
            }
            private set
            {
                stackKey = value;
            }
        }

        private static DateTime stackKeyExpiration;

        private const string StackKeyErrorMessage = "Tenant specific endpoints doesn't support Stack Key property and this will property will be deprecated in next major release";

        public class RefreshState
        {
            public string RefreshKey { get; set; }
            public string EnterpriseId { get; set; }
            public string OrganizationId { get; set; }
            public string Stack { get; set; }
        }

        public ETClient(string jwt)
            : this(new NameValueCollection { { "jwt", jwt } }, null) { }

        internal ETClient(IFuelSDKConfiguration configuration, ISoapClientFactory soapClientFactory, RefreshState refreshState = null)
        {
            this.configuration = configuration;
            this.SoapClient = soapClientFactory.Create();
        }

        public ETClient(NameValueCollection parameters = null, RefreshState refreshState = null) 
        {
            this.configuration = new ConfigFileWithParametersOverwriteConfigurationProvider().Get(parameters);
            // If JWT URL Parameter Used
            var organizationFind = false;
            if (refreshState != null)
            {
                RefreshKey = refreshState.RefreshKey;
                EnterpriseId = refreshState.EnterpriseId;
                OrganizationId = refreshState.OrganizationId;
                Stack = refreshState.Stack;
                RefreshToken();
            }
            else if (parameters != null && parameters.AllKeys.Contains("jwt") && !string.IsNullOrEmpty(parameters["jwt"]))
            {
                if (string.IsNullOrEmpty(configuration.AppSignature))
                    throw new Exception("Unable to utilize JWT for SSO without appSignature: Must be provided in config file or passed when instantiating ETClient");
                var encodedJWT = parameters["jwt"].ToString().Trim();
                var decodedJWT = DecodeJWT(encodedJWT, configuration.AppSignature);
                var parsedJWT = JObject.Parse(decodedJWT);
                AuthToken = parsedJWT["request"]["user"]["oauthToken"].Value<string>().Trim();
                AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedJWT["request"]["user"]["expiresIn"].Value<string>().Trim()));
                InternalAuthToken = parsedJWT["request"]["user"]["internalOauthToken"].Value<string>().Trim();
                RefreshKey = parsedJWT["request"]["user"]["refreshToken"].Value<string>().Trim();
                Jwt = parsedJWT;
                var orgPart = parsedJWT["request"]["organization"];
                if (orgPart != null)
                {
                    EnterpriseId = orgPart["enterpriseId"].Value<string>().Trim();
                    OrganizationId = orgPart["id"].Value<string>().Trim();
                    Stack = orgPart["stackKey"].Value<string>().Trim();
                }
            }
            else
            {
                RefreshToken();
                organizationFind = true;
            }

            SoapClient = new SoapClientFactory(configuration, AuthToken, SDKVersion, this).Create();

            // Find Organization Information
            if (organizationFind)
            {
                var rr = SoapClient.Retrieve(new RetrieveRequest1(new RetrieveRequest { ObjectType = "BusinessUnit", Properties = new[] { "ID", "Client.EnterpriseID" } }));
                if (rr.OverallStatus == "OK" && rr.Results.Length > 0)
                {
                    EnterpriseId = rr.Results[0].Client.EnterpriseID.ToString();
                    OrganizationId = rr.Results[0].ID.ToString();
                    Stack = "";  //StackKey.Instance.Get(long.Parse(EnterpriseId), this);
                }
            }
        }

        internal string FetchRestAuth()
        {
            var returnedRestAuthEndpoint = new ETEndpoint { AuthStub = this, Type = "restAuth" }.Get();
            if (returnedRestAuthEndpoint.Status && returnedRestAuthEndpoint.Results.Length == 1)
                return ((ETEndpoint)returnedRestAuthEndpoint.Results[0]).URL;
            else
                throw new Exception("REST auth endpoint could not be determined");
        }

        private string DecodeJWT(string jwt, string key)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var json = decoder.Decode(jwt, key, true);
            return json;
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
            if (UseOAuth2Authentication)
            {
                RefreshTokenWithOauth2(force);
                return;
            }

            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(AuthToken) && DateTime.Now.AddSeconds(300) <= AuthTokenExpiration && !force)
                return;

            // Get an internalAuthToken using clientId and clientSecret
            var authEndpoint = new AuthEndpointUriBuilder(configuration).Build();

            // Build the request
            var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json;
                if (!string.IsNullOrEmpty(RefreshKey))
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"refreshToken\": \"{2}\", \"scope\": \"cas:{3}\" , \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret, RefreshKey, InternalAuthToken);
                else
                    json = string.Format("{{\"clientId\": \"{0}\", \"clientSecret\": \"{1}\", \"accessType\": \"offline\"}}", configuration.ClientId, configuration.ClientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
                responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            InternalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
            AuthToken = parsedResponse["accessToken"].Value<string>().Trim();
            AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
            RefreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
        }

        private void RefreshTokenWithOauth2(bool force = false)
        {
            // workaround to support TLS 1.2 in .NET 4.0
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            // RefreshToken
            if (!string.IsNullOrEmpty(AuthToken) && DateTime.Now.AddSeconds(300) <= AuthTokenExpiration && !force)
                return;

            var authEndpoint = configuration.AuthenticationEndPoint + "/v2/token";
            var request = (HttpWebRequest)WebRequest.Create(authEndpoint.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                dynamic payload = new JObject();
                payload.client_id = configuration.ClientId;
                payload.client_secret = configuration.ClientSecret;
                payload.grant_type = "client_credentials";
                if (!string.IsNullOrEmpty(configuration.AccountId))
                    payload.account_id = configuration.AccountId;
                if (!string.IsNullOrEmpty(configuration.Scope))
                    payload.scope = configuration.Scope;

                streamWriter.Write(payload.ToString());
            }

            // Get the response
            string responseFromServer = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
                responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            AuthToken = parsedResponse["access_token"].Value<string>().Trim();
            InternalAuthToken = parsedResponse["access_token"].Value<string>().Trim();
            AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expires_in"].Value<string>().Trim()));
            configuration.SoapEndPoint = parsedResponse["soap_instance_url"].Value<string>().Trim() + "service.asmx";
            configuration.RestEndPoint = parsedResponse["rest_instance_url"].Value<string>().Trim();
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