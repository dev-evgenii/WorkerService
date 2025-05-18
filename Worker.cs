namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _token = "private_token";       
        public readonly int _cpuLoadMax = 75;
        public readonly int _memoryLoadMax = 75;
        private readonly int _delta = 10;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {          
            while (!stoppingToken.IsCancellationRequested)
            {
                Calculate();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void Calculate()
        {
            var resourceService = new ResourceService(_token);            
            var currentResources = resourceService.Get();            
            if (!currentResources.Any())
            {
                resourceService.Init();
            }
            else
            {
                Update(ref resourceService, currentResources);
            }          
        }

        private void Update(ref ResourceService resourceService, List<Resource> currentResources)
        {            
            UpdateByType(ref resourceService, ResourceType.VM, currentResources.Where(w => w.Type == ResourceType.VM).ToList());
            UpdateByType(ref resourceService, ResourceType.DB, currentResources.Where(w => w.Type == ResourceType.DB).ToList());
        }

        private void UpdateByType(ref ResourceService resourceService, string resourceType, List<Resource> currentResources)
        {   
            var cpuLoad = currentResources.Sum(item => item.Cpu_load);
            var ramLoad = currentResources.Sum(item => item.Ram_load);            
            var podCount = currentResources.Count;

            _logger.LogInformation("UpdateByType: ResourceType={ResourceType}, PodCount={PodCount}, RamLoad={RamLoad:P2}, CpuLoad={CpuLoad:P2}",
                      resourceType, podCount, ramLoad, cpuLoad);

            decimal cpu = cpuLoad / podCount;
            decimal ram = ramLoad / podCount;

            var bestPrice = resourceService.GetBestPrice(resourceType, cpu, ram);
            if (cpu > _cpuLoadMax ||
                ram > _memoryLoadMax)
            {
                _logger.LogInformation("The value of the parameter is: {ResourceType}.", resourceType);
                     resourceService.Add(bestPrice?.Cpu ?? 1, bestPrice?.Ram ?? 1, resourceType);
            }           
          
            if (podCount > 1 &&
               (cpuLoad / (podCount - 1) < (_cpuLoadMax - _delta) &&
                ramLoad / (podCount - 1) < (_memoryLoadMax - _delta)))
            {
                _logger.LogInformation("Operation: delete resource of type {ResourceType}.", resourceType);

                int id = currentResources.LastOrDefault()?.Id ?? -1;
                if (id != -1)
                 resourceService.Delete(id);
            }

            if (currentResources.Any())
            {
                if (currentResources.Count(w => w.Failed) == currentResources.Count)
                {
                    resourceService.Add(bestPrice?.Cpu ?? 1, bestPrice?.Ram ?? 1, resourceType);
                }
            }         
        }        
    }
}