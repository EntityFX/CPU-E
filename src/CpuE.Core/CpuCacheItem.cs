namespace CpuE.Core
{
    public class CpuCacheItem
    {
        public byte Level { get; set; }

        public uint? NumberOfCores { get; set; }

        public string NumberOfCoresString { get => $"{(NumberOfCores != null ? "x " + NumberOfCores : "")}"; }

        public uint Size { get; set; }

        public uint LineSize { get; set; }

        public string SizeString { get => $"{Size / 1024} KBytes"; }

        public byte Associativity { get; set; }

        public string AssociativityWay { get => $"{Associativity}-way"; }

        public string Descriptor
        {
            get => $"{AssociativityWay} set associative, {LineSize}-byte line size";
        }

        public override string ToString()
        {
            return $"{(NumberOfCores != null ? NumberOfCores + " x" : "")} {Size / 1024} KBytes";
        }
    }
}
