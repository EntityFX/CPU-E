namespace CpuE.Core
{
    public static class ArchitectureHelper
    {
        public static string GetArchitecture(uint arch, uint width)
        {
            switch (arch)
            {
                case 0:
                    return "x86";
                case 1:
                    return "MIPS";
                case 2:
                    return "Alpha";
                case 5 when width == 32:
                    return "arm";
                case 5 when width == 64:
                    return "aarch64";
                case 6:
                    return "IA64";
                case 9:
                    return "x86-64";
                default:
                    return "unknown";
            }
        }
    }
}