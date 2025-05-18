namespace WorkerService.Helpers
{
    public static class RequestHelper
    {
        public static async Task<string> Get(string url, string token)
        {
            string content;

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                var response = await client.GetAsync($"{url}?token={token}");
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var response = await client.GetAsync(url);
                content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }

        public static async Task<string> Post(string url, string token, string requestData)
        {
            string responseContent;

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var httpContent = new StringContent(requestData, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{url}?token={token}", httpContent);
            responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public static async Task<string> Delete(string url, string token, int id)
        {
            string responseContent;

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.DeleteAsync($"{url}/{id}?token={token}");
            responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public static string GenerateRequestDataJson(int cpu, int ram, string resourceType)
        {
            return $"{{\r\n\"cpu\": {cpu},\r\n\"ram\": {ram},\r\n\"type\": \"{resourceType}\"\r\n}}";
        }
    }
}
