namespace FuelSDK
{
    public interface IFuelSDKConfiguration
    {
        /// <summary>
        /// Gets or sets the app signature.
        /// </summary>
        /// <value>The app signature.</value>
        string AppSignature { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the SOAP end point.
        /// </summary>
        /// <value>The SOAP end point.</value>
        string SoapEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the authentification end point.
        /// </summary>
        /// <value>The authentification end point.</value>
        string AuthenticationEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the REST end point.
        /// </summary>
        /// <value>The REST end point.</value>
        string RestEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the Authenticaton Mode.
        /// </summary>
        /// <value>Authenticaton Mode</value>
        string UseOAuth2Authentication { get; set; }

        /// <summary>
        /// Gets or sets the Account Id.
        /// </summary>
        /// <value>Authenticaton Mode</value>
        string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the Authenticaton Mode.
        /// </summary>
        /// <value>Authenticaton Mode</value>
        string Scope { get; set; }

        /// <summary>
        /// Sets the AuthenticationEndPoint to the default value if it is not set and returns the updated instance.
        /// </summary>
        /// <param name="defaultAuthEndpoint">The default auth endpoint</param>
        /// <returns>The updated <see cref="FuelSDKConfigurationSection"/> instance</returns>
        IFuelSDKConfiguration WithDefaultAuthEndpoint(string defaultAuthEndpoint);

        IFuelSDKConfiguration WithDefaultRestEndpoint(string defaultRestEndpoint);

        IFuelSDKConfiguration Clone();
    }
}