using WorkerService.Helpers;

namespace WorkerService
{
    public class ResourceService
    {
        private readonly ResourceClient _resourceClient;
        public ResourceService(string token)
        {
            _resourceClient = new ResourceClient(token);
        }

        public List<Resource> Get()
        {
            return _resourceClient.Get();
        }

        public Price? GetBestPrice(string resourceType, decimal cpu, decimal ram)
        {
            return PriceClient.EvaluateBestPrice(PriceClient.GetPricesForEvaluate(),
                resourceType, cpu, ram);
        }

        public void Init()
        {
            var prices = PriceClient.Get();

            var db = prices.FirstOrDefault(f => f.Type == ResourceType.DB);
            _resourceClient.Post(RequestHelper.GenerateRequestDataJson(db?.Cpu ?? 1, db?.Ram ?? 1, ResourceType.DB));

            var vm = prices.FirstOrDefault(f => f.Type == ResourceType.VM);
            _resourceClient.Post(RequestHelper.GenerateRequestDataJson(vm?.Cpu ?? 1, vm?.Ram ?? 1, ResourceType.VM));
        }

        public void Add(int cpu, int ram, string type)
        {
            _resourceClient.Post(RequestHelper.GenerateRequestDataJson(cpu, ram, type));
        }

        public void Delete(int id)
        {
            _resourceClient.Delete(id);
        }

        public void DeleteAll()
        {
            foreach (var item in _resourceClient.Get())
            {
                _resourceClient.Delete(item.Id);
            }
        }
    }
}
