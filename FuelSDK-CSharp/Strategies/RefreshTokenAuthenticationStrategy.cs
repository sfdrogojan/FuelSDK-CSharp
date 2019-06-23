namespace FuelSDK.Strategies
{
    internal class RefreshTokenAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly ETClient etClient;
        private readonly ETClient.RefreshState refreshState;

        public RefreshTokenAuthenticationStrategy(ETClient etClient, ETClient.RefreshState refreshState)
        {
            this.etClient = etClient;
            this.refreshState = refreshState;
        }

        public void ObtainToken()
        {
            etClient.RefreshKey = refreshState.RefreshKey;
            etClient.EnterpriseId = refreshState.EnterpriseId;
            etClient.OrganizationId = refreshState.OrganizationId;
            etClient.Stack = refreshState.Stack;
            etClient.AuthService.RefreshToken();
        }
    }
}