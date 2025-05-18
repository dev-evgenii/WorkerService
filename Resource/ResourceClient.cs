using System.Text.Json;
using WorkerService.Helpers;

namespace WorkerService
{
    public class ResourceClient
    {
        private readonly string _token;
        public ResourceClient(string token)
        {
            _token = token;
        }

        public List<Resource> Get()
        {
            var res = new List<Resource>();
            var resources = JsonSerializer.Deserialize<List<Resource>>(RequestHelper.Get(UrlHelper.Resource, _token).Result);
            if (resources != null)
            {
                res.AddRange(resources);
            }

            return res;
        }

        public string Post(string requestData)
        {
            return RequestHelper.Post(UrlHelper.Resource, _token, requestData).Result;
        }

        public string Delete(int id)
        {
            return RequestHelper.Delete(UrlHelper.Resource, _token, id).Result;
        }
    }
}