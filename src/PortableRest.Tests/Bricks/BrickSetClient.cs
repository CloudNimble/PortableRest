using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BrickSet.Core;
using PortableRest;

namespace BrickSet
{
    class BrickSetClient : RestClient
    {

        #region Private Members

        /// <summary>
        /// The App ID assigned to you from BrickSet developer account.
        /// </summary>
        internal string ApiId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the BrickSetClient for a given ApiID.
        /// </summary>
        /// <param name="apiId"></param>
        public BrickSetClient(string apiId)
        {
            ApiId = apiId;
            BaseUrl = "http://brickset.com/api/v2.asmx";
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ThemesList> GetThemes()
        {
            var request = new RestRequest("getThemes", HttpMethod.Get)
            {
                ContentType = ContentTypes.Xml,
                IgnoreXmlAttributes = true
            };
            request.AddQueryString("apiKey", ApiId);
            //RWM: Using this version handles null results and gives you access to possible exceptions.
            var results = await SendAsync<ThemesList>(request);
            return results.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<AdditionalImagesList> GetAdditionalImages(int setId)
        {
            var request = new RestRequest("getAdditionalImages", HttpMethod.Get)
            {
                ContentType = ContentTypes.Xml,
                IgnoreXmlAttributes = true
            };
            request.AddQueryString("apiKey", ApiId);
            request.AddQueryString("setID", setId);
            //RWM: Using this version handles null results and gives you access to possible exceptions.
            var results = await SendAsync<AdditionalImagesList>(request);
            return results.Content;
        }

        #endregion


    }
}
