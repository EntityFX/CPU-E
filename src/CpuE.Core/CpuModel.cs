using System;

namespace CpuE.Core
{


    public class CpuModel
    {
        public string Name { get; set; }

        public string CodeName { get; set; }

        public string Architecture { get; set; }

        public string Specification { get; set; }

        public ushort Technology { get; set; }

        public string TechnologyName { get => $"{Technology} nm"; }

        public CpuFamilyModelStepping CpuFamilyModelStepping { get; set; }

        public string InstructionsConcatenated { get => String.Join(", ", Instructions); }

        public string[] Instructions { get; set; }

        public CpuClocks CpuClocks { get; set; }

        public CpuCaches Caches { get; set; }

        public uint NumberOfCores { get; set; }

        public uint NumberOfLogicalProcessors { get; set; }
        public string Manufacturer { get; set; }
    }
}
