using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CpuE.Core;
using Jotai.Hardware;

namespace CpuE.Console
{

    class Program
    {
        static void Main(string[] args)
        {
            var comp = new Computer();
            comp.Open();


            comp.CpuEnabled = true;
            comp.MainboardEnabled = true;
            comp.RAMEnabled = true;
            comp.GPUEnabled = true;
            var hw = comp.Hardware;

            System.Console.ReadLine();
        }
    }


}
