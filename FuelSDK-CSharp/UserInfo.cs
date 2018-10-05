using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK
{
    public class UserInfo : FuelObject
    {
        public string StackKey { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
		public string URL { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETEndpoint"/> class.
        /// </summary>
		public UserInfo()
        {
            Endpoint = "https://mcqs5fr9-7kpqw-50jf9sxdwpr8y.auth-qa3.marketingcloudqaapis.com" + "/v2/userInfo";
            URLProperties = new string[0];
            RequiredURLProperties = new string[0];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETEndpoint"/> class.
        /// </summary>
        /// <param name="obj">Javascript Object.</param>
		public UserInfo(JObject obj)
        {
            if(obj["organization"]["stack_key"] != null)
            {
                StackKey = obj["organization"]["stack_key"].ToString();
            }
        }
        /// Get this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
        public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        /// <summary>
        /// Gets the more results.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }
}
