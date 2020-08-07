/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2014 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/


using System.Collections.Generic;

namespace Jotai.Hardware.CPU
{
    public sealed class FamilyModel
    {
        public static Dictionary<ushort, CPUIDMode> X86FamilyModelSteppingTable = new Dictionary<ushort, CPUIDMode>()
        {
            [0x0609] = new CPUIDMode() { CodeName = "Banias", Technology = 130 },
            [0x060D] = new CPUIDMode() { CodeName = "Dothan", Technology = 90 },
            [0x060E] = new CPUIDMode() { CodeName = "Yonah", Technology = 65 },
            [0x0606] = new CPUIDMode() { CodeName = "Presler/Cedar Mill", Technology = 65 },
            [0x0616] = new CPUIDMode() { CodeName = "Conroe-L", Technology = 65 },
            [0x060F] = new CPUIDMode() { CodeName = "Merom", Technology = 65 },
            [0x061E] = new CPUIDMode() { CodeName = "Clarksfield", Technology = 45 },
            [0x061A] = new CPUIDMode() { CodeName = "Nehalem (Bloomfield)", Technology = 45 },
            [0x062E] = new CPUIDMode() { CodeName = "Nehalem-EX", Technology = 45 },
            [0x0617] = new CPUIDMode() { CodeName = "Penryn", Technology = 45 },
            [0x061D] = new CPUIDMode() { CodeName = "Dunnington", Technology = 45 },
            [0x0625] = new CPUIDMode() { CodeName = "Arrandale", Technology = 32 },
            [0x062A] = new CPUIDMode() { CodeName = "Sandy Bridge", Technology = 32 },
            [0x062D] = new CPUIDMode() { CodeName = "Sandy Bridge (E/EN/EP)", Technology = 32 },
            [0x063A] = new CPUIDMode() { CodeName = "Ivy Bridge", Technology = 22 },
            [0x063C] = new CPUIDMode() { CodeName = "Haswel", Technology = 22 },
            [0x064F] = new CPUIDMode() { CodeName = "Broadwell", Technology = 14 },
            [0x068E] = new CPUIDMode() { CodeName = "Kaby Lake (U/Y)", Technology = 14 },
            [0x069E] = new CPUIDMode() { CodeName = "Coffee Lake", Technology = 14 },
            [0x0625] = new CPUIDMode() { CodeName = "Westmere (Arrandale/Clarksdale)", Technology = 32 },
            [0x062C] = new CPUIDMode() { CodeName = "Westmere (Gulftown)", Technology = 32 },
            [0x064C] = new CPUIDMode() { CodeName = "Cherry Trail", Technology = 14 },

            [0x1004] = new CPUIDMode() { CodeName = "Deneb", Technology = 45 },
            [0x100A] = new CPUIDMode() { CodeName = "Thuban", Technology = 45 },
            [0x1201] = new CPUIDMode() { CodeName = "Liano", Technology = 32 },
            [0x1510] = new CPUIDMode() { CodeName = "Trinity", Technology = 32 },
            [0x1513] = new CPUIDMode() { CodeName = "Richland", Technology = 32 },
            [0x1530] = new CPUIDMode() { CodeName = "Kaveri", Technology = 28 },
            [0x1538] = new CPUIDMode() { CodeName = "Godavari", Technology = 28 },
            [0x1538] = new CPUIDMode() { CodeName = "Carrizo", Technology = 20 },
            [0x1501] = new CPUIDMode() { CodeName = "Bulldozer", Technology = 32 },
            [0x1565] = new CPUIDMode() { CodeName = "Bristol Ridge", Technology = 28 },
            [0x1502] = new CPUIDMode() { CodeName = "Vishera", Technology = 32 },
            [0x1600] = new CPUIDMode() { CodeName = "Kabini", Technology = 28 },
            [0x1630] = new CPUIDMode() { CodeName = "Mullins", Technology = 28 },
        };


    }
}
