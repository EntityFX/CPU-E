namespace CpuE.Core
{
    public class AllCpus
    {
        public CpuModel[] Cpus { get; set; }

        public CpuModel RootCpu { get; set; }

        public uint TotalOfCores { get; set; }

        public uint TotalOfLogicalProcessors { get; set; }
    }
}
