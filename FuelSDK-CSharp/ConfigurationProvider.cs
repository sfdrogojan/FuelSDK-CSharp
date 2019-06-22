using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FuelSDK;

namespace FuelSDK
{
    interface ConfigurationProvider
    {
        IFuelSDKConfiguration Get(NameValueCollection parameters);
    }
}
