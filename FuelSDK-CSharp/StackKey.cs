using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FuelSDK
{
    public class StackKey
    {
        ConcurrentDictionary<long, string> values;

        private static readonly Lazy<StackKey> lazy =
            new Lazy<StackKey>(() => new StackKey());

        public static StackKey Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private StackKey()
        {
            values = new ConcurrentDictionary<long, string>();
        }

        public string Get(long enterpriseId, ETClient client)
        {
            return values.GetOrAdd(enterpriseId, (eId) => {
                var restAuth = client.FetchRestAuth();
                var userInfo = new UserInfo(restAuth)
                {
                    AuthStub = client
                };

                var userInfoGetResult = userInfo.Get();
                if (userInfoGetResult.Results != null && userInfoGetResult.Results.Length > 0)
                {
                    return ((UserInfo)userInfoGetResult.Results[0]).StackKey;
                }
                return null;
            });
        }
    }
}
