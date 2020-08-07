﻿/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	Copyright (C) 2010 Paul Werelds
  Copyright (C) 2011 Roland Reinl <roland-reinl@gmx.de>
	
*/

using System.Collections.Generic;
using Jotai.Collections;

namespace Jotai.Hardware.HDD {
   
  [NamePrefix("INTEL SSD"), 
   RequireSmart(0xE1), RequireSmart(0xE8), RequireSmart(0xE9)]
  internal class SSDIntel : AbstractHarddrive {

    private static readonly IEnumerable<SmartAttribute> smartAttributes =
      new List<SmartAttribute> {

      new SmartAttribute(0x01, SmartNames.ReadErrorRate),
      new SmartAttribute(0x03, SmartNames.SpinUpTime),
      new SmartAttribute(0x04, SmartNames.StartStopCount, RawToInt),
      new SmartAttribute(0x05, SmartNames.ReallocatedSectorsCount),
      new SmartAttribute(0x09, SmartNames.PowerOnHours, RawToInt),
      new SmartAttribute(0x0C, SmartNames.PowerCycleCount, RawToInt),
      new SmartAttribute(0xAA, SmartNames.AvailableReservedSpace),
      new SmartAttribute(0xAB, SmartNames.ProgramFailCount),
      new SmartAttribute(0xAC, SmartNames.EraseFailCount),
      new SmartAttribute(0xB8, SmartNames.EndToEndError),
      new SmartAttribute(0xC0, SmartNames.UnsafeShutdownCount), 
      new SmartAttribute(0xE1, SmartNames.HostWrites, 
        (byte[] r, byte v, IReadOnlyArray<IParameter> p) 
          => { return RawToInt(r, v, p) / 0x20; }, 
        SensorType.Data, 0),
      new SmartAttribute(0xE8, SmartNames.RemainingLife, 
        null, SensorType.Level, 0),
      new SmartAttribute(0xE9, SmartNames.MediaWearOutIndicator),
      new SmartAttribute(0xF1, SmartNames.HostWrites,
        (byte[] r, byte v, IReadOnlyArray<IParameter> p) 
          => { return RawToInt(r, v, p) / 0x20; }, 
        SensorType.Data, 0),
      new SmartAttribute(0xF2, SmartNames.HostReads, 
        (byte[] r, byte v, IReadOnlyArray<IParameter> p) 
          => { return RawToInt(r, v, p) / 0x20; }, 
        SensorType.Data, 1),      
    };

    public SSDIntel(ISmart smart, string name, string firmwareRevision, 
      int index, ISettings settings)
      : base(smart, name, firmwareRevision, index, smartAttributes, settings) {}
  }
}
