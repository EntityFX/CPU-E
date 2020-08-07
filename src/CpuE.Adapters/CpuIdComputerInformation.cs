using CpuE.Core;
using Jotai.Hardware;
using Jotai.Hardware.CPU;
using Jotai.Hardware.Mainboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CpuE.Adapters
{
    public class CpuIdComputerInformation : IComputerInformation
    {
        Computer computer = new Computer();

        private IHardware[] hardware;

        public ComputerModel GetAllData()
        {
            computer.Open();
            computer.CpuEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;
            computer.GPUEnabled = true;
            hardware = computer.Hardware;
            CpuModel[] cpus = new CpuModel[] { };
            var hwd = hardware.OfType<GenericCPU>().ToArray();

            var hwCpus = hwd.OfType<GenericCPU>();

            var hwBios = hardware.OfType<Mainboard>()?.FirstOrDefault()?.BIOS;

            cpus = hwCpus.Select(cp =>
            {
                var l1CacheData = cp.Caches.ContainsKey(CacheLevels.Level1) ? cp.Caches[CacheLevels.Level1].FirstOrDefault(ch => ch.CacheType == CacheType.Data) : null;
                var l1CacheInstructions = cp.Caches.ContainsKey(CacheLevels.Level1) ? cp.Caches[CacheLevels.Level1].FirstOrDefault(ch => ch.CacheType == CacheType.Instructions) : null;
                var l2Cache = cp.Caches.ContainsKey(CacheLevels.Level2) ? cp.Caches[CacheLevels.Level2].FirstOrDefault() : null;
                var l3Cache = cp.Caches.ContainsKey(CacheLevels.Level3) ? cp.Caches[CacheLevels.Level3].FirstOrDefault() : null;

                IntelCPU intelCpu = cp as IntelCPU;
                var specificCpu = (intelCpu != null) ? intelCpu : ((cp is AMDCPU amdCPu) ? amdCPu : cp);

                var cpuModel=  new CpuModel()
                {
                    Architecture = ArchitectureHelper.GetArchitecture((uint)(specificCpu.Is64bit ? 9 : 0), 0),
                    Name = specificCpu.Name,
                    CodeName = specificCpu.CodeName,
                    Specification = cp.BrandString,
                    NumberOfCores = (uint)cp.Cores,
                    NumberOfLogicalProcessors = (uint)cp.Threads,
                    Technology = cp.Technology,
                    CpuClocks = new CpuClocks()
                    {
                    },
                    CpuFamilyModelStepping = new CpuFamilyModelStepping()
                    {
                        FullFamily = (byte)cp.Family,
                        Family = (byte)cp.FamilyId,
                        FullModel = (byte)cp.Model,
                        Stepping = (byte)cp.Stepping,
                    },
                    Instructions = GetInstructions(specificCpu, cp.InstructionsExtensions),
                    Caches = new CpuCaches()
                    {
                        Level1Data = l1CacheData != null ? new CpuCacheItem()
                        {
                            Level = 1,
                            Associativity = (byte)l1CacheData.Associativity,
                            Size = l1CacheData.SizeKbytes * 1024,
                            LineSize = l1CacheData.LineSize,
                            NumberOfCores = (uint)cp.Cores
                        } : new CpuCacheItem() { },
                        Level1Instructions = l1CacheInstructions != null ? new CpuCacheItem()
                        {
                            Level = 1,
                            Associativity = (byte)l1CacheInstructions.Associativity,
                            Size = l1CacheInstructions.SizeKbytes * 1024,
                            LineSize = l1CacheInstructions.LineSize,
                            NumberOfCores = (uint)cp.Cores
                        } : new CpuCacheItem() { },
                        Level2 = l2Cache != null ? new CpuCacheItem()
                        {
                            Level = 2,
                            Associativity = (byte)l2Cache.Associativity,
                            Size = l2Cache.SizeKbytes * 1024,
                            LineSize = l2Cache.LineSize,
                            NumberOfCores = (uint)cp.Cores
                        } : new CpuCacheItem() { },
                        Level3 = l3Cache != null ? new CpuCacheItem()
                        {
                            Level = 3,
                            Associativity = (byte)l3Cache.Associativity,
                            LineSize = l3Cache.LineSize,
                            Size = l3Cache.SizeKbytes * 1024
                        } : new CpuCacheItem() { },
                    }
                };

                SetCpuClocks(cp, hwBios, cpuModel.CpuClocks);

                return cpuModel;
            }).ToArray();


            Motherboard motherboard = null;

            if (hwBios != null && hwBios.Board != null)
            {
                motherboard = new Motherboard()
                {
                    Manufacturer = hwBios.Board.ManufacturerName,
                    Model = hwBios.Board.ProductName,
                    Version = hwBios.Board.Version
                };
            } else
            {
                motherboard = new Motherboard()
                {
                    Manufacturer = string.Empty,
                    Model = string.Empty,
                    Version = string.Empty
                };
            }

            Bios bios = null;
            if (hwBios != null && hwBios.BIOS != null)
            {
                bios = new Bios()
                {
                    Brand = hwBios.BIOS.Vendor,
                    Date = hwBios.BIOS.Date,
                    Version = hwBios.BIOS.Version
                };
            } else
            {
                bios = new Bios()
                {
                    Brand = string.Empty,
                    Date = string.Empty,
                    Version = string.Empty
                };
            }

            return new ComputerModel()
            {
                Cpus = new AllCpus()
                {
                    Cpus = cpus,
                    RootCpu = cpus?[0],
                    TotalOfCores = cpus.Length > 1 ? (uint)cpus.Sum(c => c.NumberOfCores) : (cpus?[0]?.NumberOfCores ?? 0),
                    TotalOfLogicalProcessors = cpus.Length > 1 ? (uint)cpus.Sum(c => c.NumberOfLogicalProcessors) : (cpus?[0]?.NumberOfLogicalProcessors ?? 0),
                },
                Motherboard = motherboard,
                Bios = bios
            };
        }

        private void SetCpuClocks(GenericCPU cp, SMBIOS hwBios, CpuClocks cpuClocks)
        {
            cpuClocks.CoreSpeed = (float)(Math.Round(cp.TimeStampCounterFrequency, 2));
            cpuClocks.BusSpeed = hwBios?.Processor.ExternalClock ?? 0;

            if (cpuClocks.BusSpeed > 0)
            {
                cpuClocks.Multiplier = Convert.ToDecimal(Math.Round(cpuClocks.CoreSpeed / cpuClocks.BusSpeed, MidpointRounding.ToEven));
                cpuClocks.BusSpeed = (float)Math.Round(cpuClocks.CoreSpeed / (float)cpuClocks.Multiplier, 2);
            }
        }

        private string[] GetInstructions(GenericCPU cpu, InstructionsExtensions instructionsExtensions)
        {
            var instructionsList = new List<string>();
            if (instructionsExtensions.MMX)
            {
                instructionsList.Add("MMX");
            }
            if (instructionsExtensions.MMXExt)
            {
                instructionsList.Add("MMX+");
            }
            if (instructionsExtensions._3DNow)
            {
                instructionsList.Add("3DNow!");
            }
            if (instructionsExtensions._3DNowExt)
            {
                instructionsList.Add("3DNow!+");
            }
            if (instructionsExtensions.SSE)
            {
                instructionsList.Add("SSE");
            }
            if (instructionsExtensions.SSE2)
            {
                instructionsList.Add("SSE2");
            }
            if (instructionsExtensions.SSE3)
            {
                instructionsList.Add("SSE3");
            }
            if (instructionsExtensions.SSSE3)
            {
                instructionsList.Add("SSSE3");
            }
            if (instructionsExtensions.SSE4a)
            {
                instructionsList.Add("SSE4a");
            }
            if (instructionsExtensions.SSE4_1)
            {
                instructionsList.Add("SSE4.1");
            }
            if (instructionsExtensions.SSE4_2)
            {
                instructionsList.Add("SSE4.2");
            }

            if (cpu.Is64bit)
            {
                if (cpu is IntelCPU)
                {
                    instructionsList.Add("EM64T");
                }
                else
                {
                    instructionsList.Add("x86-64");
                }
            }

            if (instructionsExtensions.FMA4)
            {
                instructionsList.Add("FMA4");
            }
            if (instructionsExtensions.AVX)
            {
                instructionsList.Add("AVX");
            }
            if (instructionsExtensions.FMA3)
            {
                instructionsList.Add("FMA3");
            }
            if (instructionsExtensions.AES)
            {
                instructionsList.Add("AES");
            }
            if (instructionsExtensions.AVX2)
            {
                instructionsList.Add("AVX2");
            }
            if (instructionsExtensions.AVX512F)
            {
                instructionsList.Add("AVX512F");
            }
            return instructionsList.ToArray();
        }

        public ComputerModel Update(ComputerModel computerModel)
        {
            var hwd = hardware.OfType<GenericCPU>().ToArray();

            var hwCpus = hwd.OfType<GenericCPU>();
            var hwBios = hardware.OfType<Mainboard>()?.FirstOrDefault()?.BIOS;

            var cpuIndex = 0;
            foreach (var hwCpu in hwCpus)
            {
                hwCpu.Update();
                SetCpuClocks(hwCpu, hwBios, computerModel.Cpus.Cpus[cpuIndex].CpuClocks);
                cpuIndex++;
            }

            return computerModel;
        }
    }
}