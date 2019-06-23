using System.Collections.Specialized;

namespace FuelSDK.Strategies
{
    internal interface IAuthenticationStrategyFactory
    {
        IAuthenticationStrategy Create(ETClient etClient, ETClient.RefreshState refreshState, NameValueCollection parameters);
    }
}