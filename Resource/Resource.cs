namespace WorkerService
{
    public class Resource
    {
        public int Cost { get; set; }
        public int Cpu { get; set; }
        public decimal Cpu_load { get; set; }
        public bool Failed { get; set; }
        public string Failed_until { get; set; } = string.Empty;
        public int Id { get; set; }
        public int Ram { get; set; }
        public decimal Ram_load { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
