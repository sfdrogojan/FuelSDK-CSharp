using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FuelSDK
{
    /// <summary>
    /// ETAsset - Represents an asset in the Marketing Cloud account.
    /// </summary>
    public class ETAsset : FuelObject
    {
        /// <summary>
        /// Gets or sets the name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the asset type.
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETAsset"/> class.
        /// </summary>
        public ETAsset()
        {
            Endpoint = ConfigUtil.GetFuelSDKConfigSection().RestEndPoint + "/asset/v1/content/assets{ID}";
            URLProperties = new[] { "ID" };
            RequiredURLProperties = new string[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETAsset"/> class.
        /// </summary>
        /// <param name="obj">Javascript object.</param>
		public ETAsset(JObject obj)
        {
            if (obj["id"] != null)
                ID = int.Parse(CleanRestValue(obj["id"]));
            if (obj["createdDate"] != null)
                CreatedDate = DateTime.Parse(CleanRestValue(obj["createdDate"]));
            if (obj["modifiedDate"] != null)
                ModifiedDate = DateTime.Parse(CleanRestValue(obj["modifiedDate"]));
            if (obj["name"] != null)
                Name = CleanRestValue(obj["name"]);
            if (obj["assetType"] != null)
                AssetType = JsonConvert.DeserializeObject<AssetType>(CleanRestValue(obj["assetType"]));
        }

        /// <summary>
		/// Post this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PostReturn"/>.</returns>
		public PostReturn Post() { return new PostReturn(this); }
        /// <summary>
        /// Delete this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.DeleteReturn"/>.</returns>
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
        public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        /// <summary>
        /// Gets more results.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }
}
