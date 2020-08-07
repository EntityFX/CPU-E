/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2010 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

namespace Jotai.Hardware.CPU
{

   
    public abstract class AMDCPU : GenericCPU
    {

        public enum Microarchitecture
        {
            Unknown,
            K5,
            K6,
            K6_2,
            Sharptooth,
            Athlon,
            Hammer,
            K10,
            Fusion,
            Bobcat,
            Jaguar,
            Puma,
            Bulldozer,
            Piledriver,
            Steamroller,
            Excavator,
            Zen
        }

        private const byte PCI_BUS = 0;
        private const byte PCI_BASE_DEVICE = 0x18;
        private const byte DEVICE_VENDOR_ID_REGISTER = 0;
        private const ushort AMD_VENDOR_ID = 0x1022;

        private readonly Microarchitecture microarchitecture;

        
        public Microarchitecture MicroArchitecture { get { return this.microarchitecture; } }

        public AMDCPU(int processorIndex, CPUID[][] cpuid, ISettings settings) : base(processorIndex, cpuid, settings)
        {
            this.microarchitecture = Microarchitecture.Unknown;
            System.Console.WriteLine(this.family);
            switch (this.family)
            {
                case 0xC:
                    this.microarchitecture = Microarchitecture.K10;
                    break;
                case 0xE:
                    this.microarchitecture = Microarchitecture.Bobcat;
                    break;
                case 0x10:
                    switch (this.model)
                    {
                        case 0x2:
                            this.microarchitecture = Microarchitecture.Jaguar;
                            break;
                        case 0x3:
                            this.microarchitecture = Microarchitecture.Puma;
                            break;
                    }
                    break;
                case 0x12:
                    this.microarchitecture = Microarchitecture.Bobcat;
                    break;
                case 0x15:
                    switch (this.model)
                    {
                        case 0x1:
                            this.microarchitecture = Microarchitecture.Bulldozer;
                            break;
                        case 0x2:
                            this.microarchitecture = Microarchitecture.Piledriver;
                            break;
                        case 0x3:
                            this.microarchitecture = Microarchitecture.Steamroller;
                            break;
                        case 0x4:
                            this.microarchitecture = Microarchitecture.Excavator;
                            break;
                    }
                    break;
            }

            var familyModel = (ushort)((family << 8) + model);
            if (FamilyModel.X86FamilyModelSteppingTable.ContainsKey(familyModel))
            {
                codeName = FamilyModel.X86FamilyModelSteppingTable[familyModel].CodeName;
                technology = FamilyModel.X86FamilyModelSteppingTable[familyModel].Technology;
            }
            else
            {
                codeName = microarchitecture.ToString();
            }
        }

        protected uint GetPciAddress(byte function, ushort deviceId)
        {

            return 0;
        }

    }
}
