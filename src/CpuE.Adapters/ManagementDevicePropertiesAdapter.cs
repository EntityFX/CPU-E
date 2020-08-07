using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using CpuE.Core;
using Jotai.Hardware.CPU;

namespace CpuE.Adapters
{


    public class ManagementDevicePropertiesAdapter : IDevicePropertiesAdapter
    {
        public IEnumerable<IDictionary<string, object>> GetMultipleProperties(string category)
        {
            var searcher = new ManagementObjectSearcher(
                $"select * from {category}");

            try
            {
                return searcher.Get().OfType<ManagementObject>().Select(s =>
                s.Properties.OfType<PropertyData>().ToDictionary(kv => kv.Name, kv => kv.Value));
            }
            catch (Exception)
            {
                //return Enumerable.Empty<IDictionary<string, object>>();
            }

            var a = "";

            return Enumerable.Empty<IDictionary<string, object>>();

        }

        public IDictionary<string, object> GetProperties(string category)
        {
            return GetMultipleProperties(category).FirstOrDefault();
        }
    }
}