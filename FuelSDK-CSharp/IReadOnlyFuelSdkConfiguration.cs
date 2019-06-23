using System;

namespace FuelSDK
{
    public interface IReadOnlyFuelSdkConfiguration
    {
        /// <summary>
        /// Gets or sets the app signature.
        /// </summary>
        /// <value>The app signature.</value>
        string AppSignature { get; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        string ClientId { get; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        string ClientSecret { get; }

        /// <summary>
        /// Gets or sets the SOAP end point.
        /// </summary>
        /// <value>The SOAP end point.</value>
        string SoapEndPoint { get; }

        /// <summary>
        /// Gets or sets the authentification end point.
        /// </summary>
        /// <value>The authentification end point.</value>
        string AuthenticationEndPoint { get; }

        /// <summary>
        /// Gets or sets the REST end point.
        /// </summary>
        /// <value>The REST end point.</value>
        string RestEndPoint { get; }

        /// <summary>
        /// Gets or sets the Authentication Mode.
        /// </summary>
        /// <value>Authentication Mode</value>
        string UseOAuth2Authentication { get; }

        /// <summary>
        /// Gets or sets the Account Id.
        /// </summary>
        /// <value>Authentication Mode</value>
        string AccountId { get; }

        /// <summary>
        /// Gets or sets the Authentication Mode.
        /// </summary>
        /// <value>Authentication Mode</value>
        string Scope { get; }

        string SDKVersion { get; }
    }
}