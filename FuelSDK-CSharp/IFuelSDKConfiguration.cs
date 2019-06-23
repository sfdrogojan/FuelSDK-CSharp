using System;

namespace FuelSDK
{
    internal interface IFuelSDKConfiguration
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
        /// Gets or sets the authentication end point.
        /// </summary>
        /// <value>The authentication end point.</value>
        string AuthenticationEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the REST end point.
        /// </summary>
        /// <value>The REST end point.</value>
        string RestEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the Authentication Mode.
        /// </summary>
        /// <value>Authentication Mode</value>
        string UseOAuth2Authentication { get; set; }

        /// <summary>
        /// Gets or sets the Account Id.
        /// </summary>
        /// <value>Authentication Mode</value>
        string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the Authentication Mode.
        /// </summary>
        /// <value>Authentication Mode</value>
        string Scope { get; set; }
    }
}