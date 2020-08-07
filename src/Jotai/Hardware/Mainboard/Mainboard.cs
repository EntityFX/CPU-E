/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Text;
using Jotai.Hardware.LPC;

namespace Jotai.Hardware.Mainboard
{
   
    public class Mainboard : IHardware
    {
        private readonly SMBIOS smbios;
        private readonly string name;
        private string customName;
        private readonly ISettings settings;

        private readonly LMSensors lmSensors;
        private readonly Hardware[] superIOHardware;

        private readonly String manufacturer;
        private readonly String model;

        public Mainboard(SMBIOS smbios, ISettings settings)
        {
            this.settings = settings;
            this.smbios = smbios;

            Manufacturer manufacturer = smbios.Board == null ? Manufacturer.Unknown :
              Identification.GetManufacturer(smbios.Board.ManufacturerName);

            Model model = smbios.Board == null ? Model.Unknown :
              Identification.GetModel(smbios.Board.ProductName);
            
            if (smbios.Board != null)
            {
                if (!string.IsNullOrEmpty(smbios.Board.ProductName))
                {
                    this.model = smbios.Board.ProductName;

                    if (manufacturer == Manufacturer.Unknown)
                    {
                        this.manufacturer = String.Empty;
                    }
                    else
                    {
                        this.manufacturer = manufacturer.ToString();
                    }
                }
                else
                {
                    this.manufacturer = manufacturer.ToString();
                    this.model = String.Empty;
                }
            }
            else
            {
                this.manufacturer = String.Empty;
                this.model = String.Empty;
            }

            String name;
            if (this.manufacturer == String.Empty)
                name = Manufacturer.Unknown.ToString();
            else
                name = this.manufacturer;

            if (this.model != String.Empty)
                name += " " + this.model;

            this.name = name;

            this.customName = settings.GetValue(
              new Identifier(Identifier, "name").ToString(), name);

            ISuperIO[] superIO;
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128))
            {
                this.lmSensors = new LMSensors();
                superIO = lmSensors.SuperIO;
            }
            else
            {

            }

        }

        
        public string Name
        {
            get
            {
                return customName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    customName = value;
                else
                    customName = name;
                settings.SetValue(new Identifier(Identifier, "name").ToString(),
                  customName);
            }
        }

        
        public String Id { get { return Identifier.ToString(); } }

        public Identifier Identifier
        {
            get { return new Identifier("mainboard"); }
        }

        public SMBIOS BIOS
        {
            get { return this.smbios; }
        }

        public HardwareType HardwareType
        {
            get { return HardwareType.Mainboard; }
        }

        public virtual IHardware Parent
        {
            get { return null; }
        }

        public string GetReport()
        {
            StringBuilder r = new StringBuilder();

            r.AppendLine("Mainboard");
            r.AppendLine();
            r.Append(smbios.GetReport());



            byte[] table =
              FirmwareTable.GetTable(FirmwareTable.Provider.ACPI, "TAMG");
            if (table != null)
            {
                GigabyteTAMG tamg = new GigabyteTAMG(table);
                r.Append(tamg.GetReport());
            }

            return r.ToString();
        }

        public void Update() { }

        public void Close()
        {
            if (lmSensors != null)
                lmSensors.Close();
            foreach (Hardware hardware in superIOHardware)
                hardware.Close();
        }

        public IHardware[] SubHardware
        {
            get { return superIOHardware; }
        }

        public ISensor[] Sensors
        {
            get { return new ISensor[0]; }
        }

#pragma warning disable 67
        public event SensorEventHandler SensorAdded;
        public event SensorEventHandler SensorRemoved;
#pragma warning restore 67

        public void Accept(IVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");
            visitor.VisitHardware(this);
        }

        public void Traverse(IVisitor visitor)
        {
            foreach (IHardware hardware in superIOHardware)
                hardware.Accept(visitor);
        }
    }
}
