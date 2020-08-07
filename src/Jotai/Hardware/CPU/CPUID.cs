/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2014 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jotai.Hardware.CPU
{

    public enum Vendor
    {
        Unknown,
        Intel,
        AMD,
    }

    public enum CacheLevels
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    public enum CacheType
    {
        Null,
        Data,
        Instructions,
        Unified
    }

    public enum CacheAssociativity
    {
        Disabled = 0,
        DirectMapped = 0x1,
        TwoWay = 0x2,
        FourWay = 0x4,
        EightWay = 0x6,
        SixteenWay = 0x8,
        ThirtyTwoWay = 0xA,
        FourtyEightWay = 0xB,
        SixtyFourWay = 0xC,
        HundredTwentyEightWay = 0xD,
        Fully = 0xF
    }

    public class CacheItem
    {
        public CacheLevels Level { get; set; }

        public CacheType CacheType { get; set; }

        public uint SizeKbytes { get; set; }

        public CacheAssociativity Associativity { get; set; }

        public ushort LineSize { get; set; }
    }

    public class CPUID
    {

        private readonly int thread;

        private readonly Vendor vendor = Vendor.Unknown;

        private readonly InstructionsExtensions instructionsExtensions = new InstructionsExtensions();

        private readonly Dictionary<CacheLevels, CacheItem[]>  caches = new Dictionary<CacheLevels, CacheItem[]>();

        private readonly string cpuBrandString = "";
        private readonly string name = "";
        private readonly uint familyId;
        private readonly uint[,] cpuidData = new uint[0, 0];
        private readonly uint[,] cpuidExtData = new uint[0, 0];

        private readonly uint family;
        private readonly uint model;
        private readonly uint stepping;

        private readonly uint apicId;

        private readonly uint threadMaskWith;
        private readonly uint coreMaskWith;

        private readonly uint processorId;
        private readonly uint coreId;
        private readonly uint threadId;

        private readonly bool is64bit;

        public const uint CPUID_0 = 0;
        public const uint CPUID_1 = 1;
        public const uint CPUID_4 = 4;
        public const uint CPUID_7 = 7;
        public const uint CPUID_EXT = 0x80000000;

        private static void AppendRegister(StringBuilder b, uint value)
        {
            b.Append((char)((value) & 0xff));
            b.Append((char)((value >> 8) & 0xff));
            b.Append((char)((value >> 16) & 0xff));
            b.Append((char)((value >> 24) & 0xff));
        }

        private static uint NextLog2(long x)
        {
            if (x <= 0)
                return 0;

            x--;
            uint count = 0;
            while (x > 0)
            {
                x >>= 1;
                count++;
            }

            return count;
        }

        public CPUID(int thread)
        {
            this.thread = thread;

            uint maxCpuid = 0;
            uint maxCpuidExt = 0;
            uint maxCpuidCache = 0;

            uint eax, ebx, ecx, edx;

            if (thread >= 64)
                throw new ArgumentOutOfRangeException("thread");
            ulong mask = 1UL << thread;

            if (Opcode.CpuidTx(CPUID_0, 0,
                out eax, out ebx, out ecx, out edx, mask))
            {
                if (eax > 0)
                    maxCpuid = eax;
                else
                    return;

                StringBuilder vendorBuilder = new StringBuilder();
                AppendRegister(vendorBuilder, ebx);
                AppendRegister(vendorBuilder, edx);
                AppendRegister(vendorBuilder, ecx);
                string cpuVendor = vendorBuilder.ToString();
                switch (cpuVendor)
                {
                    case "GenuineIntel":
                        vendor = Vendor.Intel;
                        break;
                    case "AuthenticAMD":
                        vendor = Vendor.AMD;
                        break;
                    default:
                        vendor = Vendor.Unknown;
                        break;
                }
                eax = ebx = ecx = edx = 0;
                if (Opcode.CpuidTx(CPUID_EXT, 0,
                  out eax, out ebx, out ecx, out edx, mask))
                {
                    if (eax > CPUID_EXT)
                        maxCpuidExt = eax - CPUID_EXT;
                    else
                        return;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("thread");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("thread");
            }

            maxCpuid = Math.Min(maxCpuid, 1024);
            maxCpuidExt = Math.Min(maxCpuidExt, 1024);
            maxCpuidCache = Math.Min(maxCpuidCache, 1024);

            cpuidData = new uint[maxCpuid + 1, 4];
            for (uint i = 0; i < (maxCpuid + 1); i++)
                Opcode.CpuidTx(CPUID_0 + i, 0,
                  out cpuidData[i, 0], out cpuidData[i, 1],
                  out cpuidData[i, 2], out cpuidData[i, 3], mask);

            cpuidExtData = new uint[maxCpuidExt + 1, 4];
            for (uint i = 0; i < (maxCpuidExt + 1); i++)
                Opcode.CpuidTx(CPUID_EXT + i, 0,
                  out cpuidExtData[i, 0], out cpuidExtData[i, 1],
                  out cpuidExtData[i, 2], out cpuidExtData[i, 3], mask);

            StringBuilder nameBuilder = new StringBuilder();
            for (uint i = 2; i <= 4; i++)
            {
                if (Opcode.CpuidTx(CPUID_EXT + i, 0,
                  out eax, out ebx, out ecx, out edx, mask))
                {
                    AppendRegister(nameBuilder, eax);
                    AppendRegister(nameBuilder, ebx);
                    AppendRegister(nameBuilder, ecx);
                    AppendRegister(nameBuilder, edx);
                }
            }
            nameBuilder.Replace('\0', ' ');
            cpuBrandString = nameBuilder.ToString().Trim();
            nameBuilder.Replace("(R)", " ");
            nameBuilder.Replace("(TM)", " ");
            nameBuilder.Replace("(tm)", "");
            nameBuilder.Replace("CPU", "");
            nameBuilder.Replace("Quad-Core Processor", "");
            nameBuilder.Replace("Six-Core Processor", "");
            nameBuilder.Replace("Eight-Core Processor", "");
            nameBuilder.Replace("APU with Radeon HD Graphics", "");
            for (int i = 0; i < 10; i++) nameBuilder.Replace("  ", " ");
            name = nameBuilder.ToString();
            if (name.Contains("@"))
                name = name.Remove(name.LastIndexOf('@'));
            name = name.Trim();

            this.familyId = ((cpuidData[1, 0] & 0x0F00) >> 8);
            this.family = ((cpuidData[1, 0] & 0x0FF00000) >> 20) +
              ((cpuidData[1, 0] & 0x0F00) >> 8);
            this.model = ((cpuidData[1, 0] & 0x0F0000) >> 12) +
              ((cpuidData[1, 0] & 0xF0) >> 4);
            this.stepping = (cpuidData[1, 0] & 0x0F);

            this.apicId = (cpuidData[1, 1] >> 24) & 0xFF;

            switch (vendor)
            {
                case Vendor.Intel:
                    uint maxCoreAndThreadIdPerPackage = (cpuidData[1, 1] >> 16) & 0xFF;
                    uint maxCoreIdPerPackage;
                    if (maxCpuid >= 4)
                        maxCoreIdPerPackage = ((cpuidData[4, 0] >> 26) & 0x3F) + 1;
                    else
                        maxCoreIdPerPackage = 1;
                    threadMaskWith =
                      NextLog2(maxCoreAndThreadIdPerPackage / maxCoreIdPerPackage);
                    coreMaskWith = NextLog2(maxCoreIdPerPackage);
                    break;
                case Vendor.AMD:
                    uint corePerPackage;
                    if (maxCpuidExt >= 8)
                        corePerPackage = (cpuidExtData[8, 2] & 0xFF) + 1;
                    else
                        corePerPackage = 1;
                    threadMaskWith = 0;
                    coreMaskWith = NextLog2(corePerPackage);
                    break;
                default:
                    threadMaskWith = 0;
                    coreMaskWith = 0;
                    break;
            }

            if (Opcode.CpuidTx(CPUID_1, 0,
                out eax, out ebx, out ecx, out edx, mask))
            {
                instructionsExtensions.MMX = Convert.ToBoolean((edx >> 23) & 1);
                instructionsExtensions.SSE = Convert.ToBoolean((edx >> 25) & 1);
                instructionsExtensions.SSE2 = Convert.ToBoolean((edx >> 25) & 1);

                instructionsExtensions.SSE3 = Convert.ToBoolean((ecx) & 1);
                instructionsExtensions.SSSE3 = Convert.ToBoolean((ecx >> 9) & 1);
                instructionsExtensions.SSE4_1 = Convert.ToBoolean((ecx >> 19) & 1);
                instructionsExtensions.SSE4_2 = Convert.ToBoolean((ecx >> 20) & 1);

                instructionsExtensions.AVX = Convert.ToBoolean((ecx >> 28) & 1);
                instructionsExtensions.FMA3 = Convert.ToBoolean((ecx >> 12) & 1);
                instructionsExtensions.AES = Convert.ToBoolean((ecx >> 25) & 1);
            }

            if (Opcode.CpuidTx(CPUID_7, 0,
                out eax, out ebx, out ecx, out edx, mask))
            {
                instructionsExtensions.AVX2 = Convert.ToBoolean((ebx >> 5) & 1);
                instructionsExtensions.AVX512F = Convert.ToBoolean((ebx >> 16) & 1);
                instructionsExtensions.TSX = Convert.ToBoolean((ebx >> 11) & 1);
            }

            if (Vendor == Vendor.AMD)
            {
                instructionsExtensions._3DNow = Convert.ToBoolean((cpuidExtData[1, 3] >> 31) & 1);
                instructionsExtensions._3DNowExt = Convert.ToBoolean((cpuidExtData[1, 3] >> 30) & 1);
                instructionsExtensions.MMXExt = Convert.ToBoolean((cpuidExtData[1, 3] >> 22) & 1);


                instructionsExtensions.SSE4a = Convert.ToBoolean((cpuidExtData[1, 2] >> 6) & 1);
                instructionsExtensions.FMA4 = Convert.ToBoolean((cpuidExtData[1, 2] >> 16) & 1);

                PrepareAmdCaches(cpuidExtData);
            }


            if (Vendor == Vendor.Intel)
            {
                PrepareIntelCaches(ref eax, ref ebx, ref ecx, ref edx, mask);
            }

            if (!caches.ContainsKey(CacheLevels.Level2))
            {
                var l2Cache = new CacheItem()
                {
                    Level = CacheLevels.Level2,
                    LineSize = (byte)(cpuidExtData[6, 2] & 0xFF),
                    SizeKbytes = (ushort)(cpuidExtData[6, 2] >> 16 & 0xFFFF)
                };

                var l2Way = (byte)(cpuidExtData[6, 2] >> 12 & 0xFF);

                l2Cache.Associativity = GetAssociativity(l2Way);

                caches.Add(CacheLevels.Level2, new CacheItem[] { l2Cache });
            }


            is64bit = Convert.ToBoolean((cpuidExtData[1, 3] >> 29 & 0x1));

            processorId = (apicId >> (int)(coreMaskWith + threadMaskWith));
            coreId = ((apicId >> (int)(threadMaskWith))
              - (processorId << (int)(coreMaskWith)));
            threadId = apicId
              - (processorId << (int)(coreMaskWith + threadMaskWith))
              - (coreId << (int)(threadMaskWith));
        }

        private static CacheAssociativity GetAssociativity(byte l2Way)
        {
            switch (l2Way)
            {
                case 0x0:
                default:
                    return CacheAssociativity.Disabled; 
                case 0x1:
                    return CacheAssociativity.DirectMapped; 
                case 0x2:
                    return CacheAssociativity.TwoWay; 
                case 0x4:
                    return CacheAssociativity.FourWay; 
                case 0x6:
                    return CacheAssociativity.EightWay; 
                case 0x8:
                    return CacheAssociativity.SixteenWay; 
                case 0xF:
                    return CacheAssociativity.Fully; 
            }
        }

        private void PrepareAmdCaches(uint[,] cpuidExtData)
        {
            var l1DataCacheItem = GetAmdCacheItem(cpuidExtData[5, 2], CacheLevels.Level1, CacheType.Data);
            var l1InstructionsCacheItem = GetAmdCacheItem(cpuidExtData[5, 3], CacheLevels.Level1, CacheType.Instructions);

            if (caches.ContainsKey(CacheLevels.Level1))
            {
                var cachesOfLevel = caches[CacheLevels.Level1].ToList();
                cachesOfLevel.Add(l1DataCacheItem);
                cachesOfLevel.Add(l1InstructionsCacheItem);
                caches[l1DataCacheItem.Level] = cachesOfLevel.ToArray();
            }
            else
            {
                caches[l1DataCacheItem.Level] = new CacheItem[] { l1DataCacheItem, l1InstructionsCacheItem };
            }


        }

        private CacheItem GetAmdCacheItem(uint register, CacheLevels cacheLevels, CacheType cacheType)
        {
            var l1DataSize = register >> 24;
            var waysOfAssociativity = GetAssociativity((byte)(register >> 16 & 0xFF));
            var lineSize = register & 0xFF;

            return new CacheItem()
            {
                Level = cacheLevels,
                LineSize = (ushort)lineSize,
                SizeKbytes = l1DataSize,
                CacheType = cacheType,
                Associativity = waysOfAssociativity
            };
        }

        private void PrepareIntelCaches(ref uint eax, ref uint ebx, ref uint ecx, ref uint edx, ulong mask)
        {
            uint cacheType = 0xFFFF;
            uint leaf = 0;

            while (cacheType != 0)
            {
                if (Opcode.CpuidTx(CPUID_4, leaf,
                out eax, out ebx, out ecx, out edx, mask))
                {
                    cacheType = eax & 0x1f;

                }
                else
                {
                    cacheType = 0;
                }

                if (cacheType == 0) continue;

                var level = (eax >> 5) & 0x07;
                var waysOfAssociativity = ((ebx >> 22) & 0x3FF) + 1;
                var linePartitions = ((ebx >> 12) & 0x3FF) + 1;
                var lineSize = (ushort)((ebx & 0xFFF) + 1);
                var sets = ecx + 1;
                var size = (waysOfAssociativity * linePartitions * lineSize * sets) / 1024;

                var fullAssociative = Convert.ToBoolean((eax >> 9) & 1);

                CacheAssociativity cacheAssociativity = CacheAssociativity.Disabled;

                if (fullAssociative)
                {
                    cacheAssociativity = CacheAssociativity.Fully;
                }
                else if (waysOfAssociativity > 0)
                {
                    cacheAssociativity = (CacheAssociativity)waysOfAssociativity;
                }

                var cacheItem = new CacheItem()
                {
                    Level = (CacheLevels)level,
                    LineSize = lineSize,
                    SizeKbytes = size,
                    CacheType = (CacheType)cacheType,
                    Associativity = cacheAssociativity
                };

                if (caches.ContainsKey(cacheItem.Level))
                {
                    var cachesOfLevel = caches[cacheItem.Level].ToList();
                    cachesOfLevel.Add(cacheItem);
                    caches[cacheItem.Level] = cachesOfLevel.ToArray();
                } else
                {
                    caches[cacheItem.Level] = new CacheItem[] { cacheItem };
                }

                leaf++;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public string BrandString
        {
            get { return cpuBrandString; }
        }

        public int Thread
        {
            get { return thread; }
        }

        public Vendor Vendor
        {
            get { return vendor; }
        }

        public uint Family
        {
            get { return family; }
        }

        public uint FamilyId
        {
            get { return familyId; }
        }

        public uint Model
        {
            get { return model; }
        }

        public uint Stepping
        {
            get { return stepping; }
        }

        public uint ApicId
        {
            get { return apicId; }
        }

        public uint ProcessorId
        {
            get { return processorId; }
        }

        public uint CoreId
        {
            get { return coreId; }
        }

        public uint ThreadId
        {
            get { return threadId; }
        }

        public bool Is64bit
        {
            get { return is64bit; }
        }

        public InstructionsExtensions InstructionsExtensions
        {
            get { return instructionsExtensions; }
        }

        public Dictionary<CacheLevels, CacheItem[]> Caches
        {
            get { return caches; }
        }

        public uint[,] Data
        {
            get { return cpuidData; }
        }

        public uint[,] ExtData
        {
            get { return cpuidExtData; }
        }
    }
}
