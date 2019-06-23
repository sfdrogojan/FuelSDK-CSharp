namespace FuelSDK.Strategies
{
    internal class OAuthAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly ETClient etClient;

        public OAuthAuthenticationStrategy(ETClient etClient)
        {
            this.etClient = etClient;
        }

        public void ObtainToken()
        {
            etClient.AuthService.RefreshToken();
            etClient.SoapClient = etClient.SoapClientFactory.Create();

            var rr = etClient.SoapClient.Retrieve(new RetrieveRequest1(new RetrieveRequest { ObjectType = "BusinessUnit", Properties = new[] { "ID", "Client.EnterpriseID" } }));
            if (rr.OverallStatus == "OK" && rr.Results.Length > 0)
            {
                etClient.EnterpriseId = rr.Results[0].Client.EnterpriseID.ToString();
                etClient.OrganizationId = rr.Results[0].ID.ToString();
                etClient.Stack = "";  
            }
        }
    }
}