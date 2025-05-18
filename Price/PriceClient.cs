using System.Text.Json;
using WorkerService.Helpers;

namespace WorkerService
{
    public static class PriceClient
    {
        public static List<Price> Get()
        {
            var priceList = new List<Price>();
            priceList.AddRange(GetPricesForEvaluate());

            return priceList.OrderBy(o => o.Cost)
                            .GroupBy(g => new { g.Type })
                            .Select(s => s.First()).ToList();
        }

        public static List<Price> GetPricesForEvaluate()
        {
            var priceList = new List<Price>();
            var prices = JsonSerializer.Deserialize<List<Price>>(RequestHelper.Get(UrlHelper.Price, string.Empty).Result);
            if (prices != null)
            {
                priceList.AddRange(prices);
            }

            return priceList;
        }

        public static Price? EvaluateBestPrice(List<Price> prices, string resourceType,
            decimal cpu, decimal ram)
        {
            var priceList = new List<Price>();
            if (cpu >= 50 && ram >= 50)
            {
                priceList.AddRange(prices.Where(w => w.Type == resourceType)
                                         .OrderByDescending(o1 => o1.Cpu)
                                         .OrderByDescending(o2 => o2.Ram));
            }
            else
            {
                priceList.AddRange(prices.Where(w => w.Type == resourceType)
                                         .OrderByDescending(o1 => o1.Cost)
                                         .OrderByDescending(o2 => o2.Ram)
                                         .OrderByDescending(o3 => o3.Cpu));
            }

            return priceList.FirstOrDefault();
        }
    }
}
