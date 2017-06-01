using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetEvents
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static HttpResponseMessage httpResponse = null;
        static Guid requestId;

        private static string _apiBaseAddress;
        static string authority;
        private static string apiResourceId;
        private static AuthenticationContext authContext = null;
        private static string clientId;
        private static string aadInstance;
        private static string tenant;

        static void Main(string[] args)
        {
            _apiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
            clientId = ConfigurationManager.AppSettings["ClientId"];
            aadInstance = ConfigurationManager.AppSettings["AADInstance"];
            tenant = ConfigurationManager.AppSettings["Tenant"];
            authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            apiResourceId = ConfigurationManager.AppSettings["ApiResourceId"];
            var result = GetEvents();            
        }

        static public object GetEvents()
        {

            // Reinitializing the HttpClient and setting the Time Out value in order to allow multiple test run in different instances
            client = new HttpClient();
            client.BaseAddress = _apiBaseAddress != null ? new Uri(_apiBaseAddress) : new Uri(ConfigurationManager.AppSettings["ApiBaseAddress"].ToString());

            requestId = Guid.NewGuid();
            authContext = new AuthenticationContext(authority);
            string appKey = ConfigurationManager.AppSettings["AppKey"];
            ClientCredential clientCredential = new ClientCredential(clientId, appKey);
            string tokenResult = string.Empty;
            AuthenticationResult result = null;
            try
            {
                authContext.TokenCache.Clear();
                result = authContext.AcquireToken(apiResourceId, clientCredential);              
            }
            catch (AdalException ex)
            {
                //May be retry
                throw ex;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            string requestUrl = new Uri(_apiBaseAddress).ToString();

            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            var startDate = DateTime.UtcNow.Date;
            queryStrings.Add("dateFloor", startDate.ToString());
            queryStrings.Add("isPublishable", "true");
            queryStrings.Add("skipCount", "0");
            queryStrings.Add("SortBy", "StartDate");
            queryStrings.Add("SortOrder", "asc");

            //Additonal filters
            //queryStrings.Add("dateCeil", startDate.AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59).ToString());
            //queryStrings.Add("eventType", "Live");
            //queryStrings.Add("resultCount", "16");
            
            //Fltering for facet based appraoech
            //List<string> productFilters = new List<string>() { "Cloud Platform", "Office" };
            //string productFilter = GenerateFilterString("product", productFilters);

            var responseString = string.Empty;

            Task.Run(async () =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpResponse = await client.GetAsync(requestUrl + "api/Search?RequestId=" + requestId + queryStrings);

                if (httpResponse.IsSuccessStatusCode)
                {
                    responseString = await httpResponse.Content.ReadAsStringAsync();
                }

            }).GetAwaiter().GetResult();

           
            var jsonresult = JsonConvert.DeserializeObject<SearchResponse>(responseString);

            Console.WriteLine("Total Events :" + jsonresult.TotalResults);
            Console.WriteLine();
            Console.WriteLine("First Event Title:" + jsonresult.Events[0].Title.ToString());
            Console.WriteLine();
            Console.WriteLine("First Event Description:" + jsonresult.Events[0].Description.ToString());

            return jsonresult;

        }


        /// <summary>
        /// Common Method to generate the Filter String/URL string based on the Filter Attribute
        /// </summary>
        /// <param name="filterFor">Filter Attribute Name - eventType, product, category, audience, language</param>
        /// <param name="filterValues">Filter Values</param>
        /// <param name="recordCount">no.of records to fetch</param>
        /// <returns>the generated query</returns>
        static private string GenerateFilterString(string filterFor, List<string> filterValues, int recordCount = 0)
        {
            string filter = string.Empty;
            foreach (string filterValue in filterValues)
            {
                filter += "&" + filterFor + "=" + filterValue;
            }
            filter += "&IsPublishable=true";
            filter += recordCount > 0 ? "&ResultCount=" + recordCount : string.Empty;
            return filter;
        }

    }
}
