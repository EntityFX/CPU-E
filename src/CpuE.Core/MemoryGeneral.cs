namespace CpuE.Core
{
    public class MemoryGeneral
    {
        public string Type { get; set; }

        public int TypeCode { get; set; }

        public long SizeBytes { get; set; }

        public string UnitSize { get { return ToUnitSize(); } }

        public int Channels { get; set; }

        public string ToUnitSize(string unit = "MBytes")
        {
            switch (unit)
            {
                case "KBytes":
                    return $"{(SizeBytes / 1024)} {unit}";
                case "MBytes":
                    return $"{(SizeBytes / 1024 / 1024 )} {unit}";
                case "GBytes":
                    return $"{(SizeBytes / 1024 / 1024 / 1024)} {unit}";
                default:
                    return $"{(SizeBytes / 1024 / 1024)}";
            }
        }
    }
}