using CpuE.Core;
using Jotai.Hardware.CPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CpuE.Adapters
{

    public class WindowsManagementComputerInformation : IComputerInformation
    {
        private IDevicePropertiesAdapter devicePropertiesAdapter;

        public WindowsManagementComputerInformation()
        {
            devicePropertiesAdapter = new ManagementDevicePropertiesAdapter();
        }


        public ComputerModel GetAllData()
        {
            CpuModel[] cpus = null;
            try
            {
                var cpusProperties = devicePropertiesAdapter.GetMultipleProperties("Win32_Processor");

                cpus = cpusProperties.Select(cp =>
                {
                    var cpu = new CpuModel()
                    {
                        Specification = cp["Name"].ToString() ?? "Unknown",
                        CpuClocks = new CpuClocks()
                        {
                            CoreSpeed = cp["CurrentClockSpeed"] != null ? Convert.ToSingle((uint)(cp["CurrentClockSpeed"])) : 0f,
                            BusSpeed = cp["ExtClock"] != null ? Convert.ToSingle((uint)(cp["ExtClock"])) : 0f,
                        },
                        NumberOfCores = cp["NumberOfCores"] != null ? (uint)(cp["NumberOfCores"]) : 1,
                        NumberOfLogicalProcessors = cp["NumberOfLogicalProcessors"] != null ? (uint)(cp["NumberOfLogicalProcessors"]) : 1,
                        Architecture = ArchitectureHelper.GetArchitecture((ushort)cp["Architecture"], (ushort)cp["DataWidth"]),
                        Manufacturer = cp["Manufacturer"]?.ToString()
                    };
                    cpu = SetModelFamilyStepping(cpu, cp["Caption"].ToString());

                    SetSupportedInstructions(cpu, (ushort)cp["Architecture"]);

                    SetCaches(cpu);

                    SetCpuDetails(cpu);
                    return cpu;
                }).ToArray();

                WindowsProcessor.GetSupportedInstructions();
            }
            finally
            {
                if (cpus == null || cpus.Length == 0)
                {
                    cpus = new CpuModel[] { new CpuModel()
                    {
                        CpuClocks = new CpuClocks()
                        {

                        },
                        Caches = new CpuCaches()
                        {
                            Level1Data = new CpuCacheItem(),
                            Level1Instructions = new CpuCacheItem(),
                            Level2 = new CpuCacheItem(),
                            Level3 = new CpuCacheItem()
                        },
                        CpuFamilyModelStepping = new CpuFamilyModelStepping() {}
                        },

                    };
                }
            }


            return new ComputerModel()
            {
                Cpus = new AllCpus()
                {
                    Cpus = cpus,
                    RootCpu = cpus[0],
                    TotalOfCores = cpus.Length > 1 ? (uint)cpus.Sum(c => c.NumberOfCores) : cpus[0].NumberOfCores,
                    TotalOfLogicalProcessors = cpus.Length > 1 ? (uint)cpus.Sum(c => c.NumberOfLogicalProcessors) : cpus[0].NumberOfLogicalProcessors,
                }

            };
        }

        private void SetSupportedInstructions(CpuModel cpu, ushort architecture)
        {
            var cpuFeatures = WindowsProcessor.GetSupportedInstructions().ToList();

            if (architecture == 9 )
            {
                cpuFeatures.Add(cpu.Manufacturer == "GenuineIntel" ? "EM64T" : "x86-64");
            }

            cpu.Instructions = cpuFeatures.ToArray();
        }

        private static CpuCaches SetCaches(CpuModel cpu)
        {
            var cachesData = WindowsProcessor.GetCacheByLevel();
            var caches = new CpuCaches();
            if (cachesData.ContainsKey(1))
            {
                var l1Cache = cachesData[1];
                WindowsProcessor.SYSTEM_LOGICAL_PROCESSOR_INFORMATION? l1Data = l1Cache.FirstOrDefault(c => c.ProcessorInformation.Cache.Type == WindowsProcessor.PROCESSOR_CACHE_TYPE.CacheData);
                if (l1Data != null)
                {
                    caches.Level1Data = MapCacheItem(l1Data, cpu.NumberOfCores);
                }

                WindowsProcessor.SYSTEM_LOGICAL_PROCESSOR_INFORMATION? l1Instruction = l1Cache.FirstOrDefault(c => c.ProcessorInformation.Cache.Type == WindowsProcessor.PROCESSOR_CACHE_TYPE.CacheInstruction);
                if (l1Instruction != null)
                {
                    caches.Level1Instructions = MapCacheItem(l1Instruction, cpu.NumberOfCores);
                }
            }

            if (cachesData.ContainsKey(2))
            {
                var l2Cache = cachesData[2];
                caches.Level2 = MapCacheItem(l2Cache.FirstOrDefault(), cpu.NumberOfCores);
            }

            if (cachesData.ContainsKey(3))
            {
                var l3Cache = cachesData[3];
                caches.Level3 = MapCacheItem(l3Cache.FirstOrDefault());
            }

            cpu.Caches = caches;

            return caches;
        }

        private static CpuCacheItem MapCacheItem(WindowsProcessor.SYSTEM_LOGICAL_PROCESSOR_INFORMATION? l1Instruction, uint? numberOfCores = null)
        {
            var l1DataItem = l1Instruction.Value.ProcessorInformation.Cache;
            return new CpuCacheItem()
            {
                Associativity = l1DataItem.Associativity,
                Level = 1,
                Size = l1DataItem.Size,
                NumberOfCores = numberOfCores
            };
        }

        private static void SetCpuDetails(CpuModel cpu)
        {
            ushort fms = (ushort)(cpu.CpuFamilyModelStepping.FullFamily * 0x100 + cpu.CpuFamilyModelStepping.FullModel);
            if (FamilyModel.X86FamilyModelSteppingTable.ContainsKey(fms))
            {
                var fmsData = FamilyModel.X86FamilyModelSteppingTable[fms];
                cpu.CodeName = fmsData.CodeName;
                cpu.Technology = fmsData.Technology;
            }
        }

        private CpuModel SetModelFamilyStepping(CpuModel cpu, string text)
        {
            if (!text.Contains("Family") || !text.Contains("Model") || !text.Contains("Stepping"))
            {
                cpu.CpuFamilyModelStepping = new CpuFamilyModelStepping()
                {
                    FullFamily = 0,
                    FullModel = 0,
                    Stepping = 0
                };
            }

            var mfsCOmponents = text.Split(' ');

            cpu.CpuFamilyModelStepping = new CpuFamilyModelStepping()
            {
                FullFamily = byte.Parse(mfsCOmponents[2]),
                FullModel = byte.Parse(mfsCOmponents[4]),
                Stepping = byte.Parse(mfsCOmponents[6])
            };


            return cpu;
        }

        public ComputerModel Update(ComputerModel computerModel)
        {
            return computerModel;
        }

        private class WindowsProcessor
        {
            [DllImport("kernel32.dll")]
            public static extern int GetCurrentThreadId();

            [DllImport("Kernel32")]
            static extern bool IsProcessorFeaturePresent(ProcessorFeature processorFeature);

            [DllImport("kernel32.dll")]
            private static extern long GetEnabledXStateFeatures();

            enum ProcessorFeature : uint
            {
                PF_FLOATING_POINT_PRECISION_ERRATA = 0,
                PF_FLOATING_POINT_EMULATED = 1,
                PF_COMPARE_EXCHANGE_DOUBLE = 2,
                PF_MMX_INSTRUCTIONS_AVAILABLE = 3,
                PF_PPC_MOVEMEM_64BIT_OK = 4,
                PF_ALPHA_BYTE_INSTRUCTIONS = 5,
                PF_XMMI_INSTRUCTIONS_AVAILABLE = 6,
                PF_3DNOW_INSTRUCTIONS_AVAILABLE = 7,
                PF_RDTSC_INSTRUCTION_AVAILABLE = 8,
                PF_PAE_ENABLED = 9,
                PF_XMMI64_INSTRUCTIONS_AVAILABLE = 10,
                PF_SSE_DAZ_MODE_AVAILABLE = 11,
                PF_NX_ENABLED = 12,
                PF_SSE3_INSTRUCTIONS_AVAILABLE = 13,
                PF_COMPARE_EXCHANGE128 = 14,
                PF_COMPARE64_EXCHANGE128 = 15,
                PF_CHANNELS_ENABLED = 16,
                PF_XSAVE_ENABLED = 17,
                PF_SECOND_LEVEL_ADDRESS_TRANSLATION = 20,
                PF_VIRT_FIRMWARE_ENABLED = 21,
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            private struct GROUP_AFFINITY
            {
                public UIntPtr Mask;

                [MarshalAs(UnmanagedType.U2)]
                public ushort Group;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U2)]
                public ushort[] Reserved;
            }

            [DllImport("kernel32", SetLastError = true)]
            private static extern Boolean SetThreadGroupAffinity(IntPtr hThread, ref GROUP_AFFINITY GroupAffinity, ref GROUP_AFFINITY PreviousGroupAffinity);

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESSORCORE
            {
                public byte Flags;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct NUMANODE
            {
                public uint NodeNumber;
            }

            public enum PROCESSOR_CACHE_TYPE
            {
                CacheUnified,
                CacheInstruction,
                CacheData,
                CacheTrace
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CACHE_DESCRIPTOR
            {
                public byte Level;
                public byte Associativity;
                public ushort LineSize;
                public uint Size;
                public PROCESSOR_CACHE_TYPE Type;
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION
            {
                [FieldOffset(0)]
                public PROCESSORCORE ProcessorCore;
                [FieldOffset(0)]
                public NUMANODE NumaNode;
                [FieldOffset(0)]
                public CACHE_DESCRIPTOR Cache;
                [FieldOffset(0)]
                private UInt64 Reserved1;
                [FieldOffset(8)]
                private UInt64 Reserved2;
            }

            public enum LOGICAL_PROCESSOR_RELATIONSHIP
            {
                RelationProcessorCore,
                RelationNumaNode,
                RelationCache,
                RelationProcessorPackage,
                RelationGroup,
                RelationAll = 0xffff
            }

            public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
            {
#pragma warning disable 0649
                public UIntPtr ProcessorMask;
                public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
                public SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION ProcessorInformation;
#pragma warning restore 0649
            }

            [DllImport(@"kernel32.dll", SetLastError = true)]
            public static extern bool GetLogicalProcessorInformation(IntPtr Buffer, ref uint ReturnLength);

            public static IEnumerable<string> GetSupportedInstructions()
            {
                var features = System.Enum.GetValues(typeof(ProcessorFeature)).OfType<ProcessorFeature>().Select(f =>
                {
                    var isAvailable = IsProcessorFeaturePresent(f);
                    if (!isAvailable) return null;
                    switch (f)
                    {
                        case ProcessorFeature.PF_MMX_INSTRUCTIONS_AVAILABLE:
                            return "MMX";
                        case ProcessorFeature.PF_XMMI_INSTRUCTIONS_AVAILABLE:
                            return "SSE";
                        case ProcessorFeature.PF_3DNOW_INSTRUCTIONS_AVAILABLE:
                            return "3DNow!";
                        case ProcessorFeature.PF_XMMI64_INSTRUCTIONS_AVAILABLE:
                            return "SSE2";
                        case ProcessorFeature.PF_SSE3_INSTRUCTIONS_AVAILABLE:
                            return "SSE3";
                        default:
                            return null;
                    }
                }).Where(f => f != null).ToList();

                bool hasAvx = false;
                try
                {
                    hasAvx = (GetEnabledXStateFeatures() & 4) != 0;
                }
                catch
                {
                    hasAvx = false;
                }

                if (hasAvx)
                {
                    features.Add("AVX");
                }

                return features;
            }


            public static Dictionary<byte, WindowsProcessor.SYSTEM_LOGICAL_PROCESSOR_INFORMATION[]> GetCacheByLevel()
            {
                var info = WindowsProcessor.LogicalProcessorInformation;
                return info.Where(c => c.Relationship == WindowsProcessor.LOGICAL_PROCESSOR_RELATIONSHIP.RelationCache && ((Int64)c.ProcessorMask & (Int64)1) != 0).GroupBy(g => g.ProcessorInformation.Cache.Level).ToDictionary(kv => kv.Key, kv => kv.ToArray());
            }

            private const int ERROR_INSUFFICIENT_BUFFER = 122;

            private static SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] _logicalProcessorInformation = null;

            public static SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] LogicalProcessorInformation
            {
                get
                {
                    if (_logicalProcessorInformation != null)
                        return _logicalProcessorInformation;

                    uint ReturnLength = 0;

                    GetLogicalProcessorInformation(IntPtr.Zero, ref ReturnLength);

                    if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                    {
                        IntPtr Ptr = Marshal.AllocHGlobal((int)ReturnLength);
                        try
                        {
                            if (GetLogicalProcessorInformation(Ptr, ref ReturnLength))
                            {
                                int size = Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                                int len = (int)ReturnLength / size;
                                _logicalProcessorInformation = new SYSTEM_LOGICAL_PROCESSOR_INFORMATION[len];
                                IntPtr Item = Ptr;

                                for (int i = 0; i < len; i++)
                                {
                                    _logicalProcessorInformation[i] = (SYSTEM_LOGICAL_PROCESSOR_INFORMATION)Marshal.PtrToStructure(Item, typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                                    Item += size;
                                }

                                return _logicalProcessorInformation;
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(Ptr);
                        }
                    }
                    return null;
                }
            }
        }
    }
}