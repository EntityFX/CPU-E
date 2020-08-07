using System.Collections.Generic;

namespace CpuE.Core
{
    public interface IDevicePropertiesAdapter
    {
        IEnumerable<IDictionary<string, object>> GetMultipleProperties(string category);

        IDictionary<string, object> GetProperties(string category);
    }
}