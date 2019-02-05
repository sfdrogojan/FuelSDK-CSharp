using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK
{
    interface ICachedSoapEndpointProvider
    {
        string GetSoapEndpoint();
    }
}
