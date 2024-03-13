using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diplom
{
    public class YourApiClient
    {
        private const string BaseUrl = "https://your-django-api-url.com/";

        public async Task<string> GetItemsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                HttpResponseMessage response = await client.GetAsync("api/items/");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}