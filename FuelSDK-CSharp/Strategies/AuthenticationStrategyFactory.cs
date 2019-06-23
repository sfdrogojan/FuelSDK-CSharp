using System.Collections.Specialized;
using System.Linq;

namespace FuelSDK.Strategies
{
    internal class AuthenticationStrategyFactory : IAuthenticationStrategyFactory
    {
        public IAuthenticationStrategy Create(ETClient etClient, ETClient.RefreshState refreshState, NameValueCollection parameters)
        {
            if (refreshState != null)
            {
                return new RefreshTokenAuthenticationStrategy(etClient, refreshState);
            }

            if (parameters != null && parameters.AllKeys.Contains("jwt") && !string.IsNullOrEmpty(parameters["jwt"]))
            {
                return new JwtAuthenticationStrategy(etClient, parameters["jwt"].ToString().Trim());
            }

            return new OAuthAuthenticationStrategy(etClient);
        }
    }
}