namespace FuelSDK
{
    public interface IConfigurationProvider
    {
        string AppSignature { get; set; }
        string AuthenticationEndPoint { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string RestEndPoint { get; set; }
        string SoapEndPoint { get; set; }
    }
}