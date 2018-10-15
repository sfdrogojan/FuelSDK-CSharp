using System;
using System.Collections.Concurrent;

namespace FuelSDK
{
    public class StackKey
    {
        ConcurrentDictionary<long, string> values;
        static ETClient client;

        private static readonly Lazy<StackKey> lazy =
            new Lazy<StackKey>(() => new StackKey());

        public static StackKey Instance(ETClient etClient)
        {
            client = etClient;
            return lazy.Value;
        }

        private StackKey()
        {
            values = new ConcurrentDictionary<long, string>();
        }

        public string Get(long enterpriseId)
        {
            return values.GetOrAdd(enterpriseId, (eId) => {
                var restAuth = client.FetchRestAuth();
                var userInfo = new UserInfo(restAuth);
                userInfo.AuthStub = client;     
                return ((UserInfo)userInfo.Get().Results[0]).StackKey;
            });
        }
    }
}
