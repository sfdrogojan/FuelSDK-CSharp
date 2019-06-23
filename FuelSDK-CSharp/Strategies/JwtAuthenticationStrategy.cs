using System;
using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;

namespace FuelSDK.Strategies
{
    internal class JwtAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly ETClient etClient;
        private readonly string encodedJWT;

        public JwtAuthenticationStrategy(ETClient etClient, string encodedJWT)
        {
            this.etClient = etClient;
            this.encodedJWT = encodedJWT;
        }
        public void ObtainToken()
        {
            if (string.IsNullOrEmpty(etClient.Configuration.AppSignature))
                throw new Exception("Unable to utilize JWT for SSO without appSignature: Must be provided in config file or passed when instantiating ETClient");
            var decodedJWT = DecodeJWT(encodedJWT, etClient.Configuration.AppSignature);
            var parsedJWT = JObject.Parse(decodedJWT);
            etClient.AuthToken = parsedJWT["request"]["user"]["oauthToken"].Value<string>().Trim();
            etClient.AuthTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedJWT["request"]["user"]["expiresIn"].Value<string>().Trim()));
            etClient.InternalAuthToken = parsedJWT["request"]["user"]["internalOauthToken"].Value<string>().Trim();
            etClient.RefreshKey = parsedJWT["request"]["user"]["refreshToken"].Value<string>().Trim();
            etClient.Jwt = parsedJWT;
            var orgPart = parsedJWT["request"]["organization"];
            if (orgPart != null)
            {
                etClient.EnterpriseId = orgPart["enterpriseId"].Value<string>().Trim();
                etClient.OrganizationId = orgPart["id"].Value<string>().Trim();
                etClient.Stack = orgPart["stackKey"].Value<string>().Trim();
            }
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
    }
}