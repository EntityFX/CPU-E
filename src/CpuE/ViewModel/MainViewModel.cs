using CpuE.Adapters;
using CpuE.Core;
using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace CpuE.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IComputerInformation computerInformation;

        private System.Timers.Timer t = new System.Timers.Timer(1000);

        private object _stdLock = new  { };

        private readonly SynchronizationContext SyncContext = AsyncOperationManager.SynchronizationContext;

        public MainViewModel()
        {
            t.Elapsed += T_Elapsed;
           // OnUpdate += MainViewModel_OnUpdate;
        }

        private void MainViewModel_OnUpdate(ComputerModel e)
        {
            Computer = computerInformation.Update(Computer);
            CoreSpeed = $"{Computer.Cpus.RootCpu.CpuClocks.CoreSpeed} MHz";
            BusSpeed = $"{Computer.Cpus.RootCpu.CpuClocks.BusSpeed} MHz";
            Multiplier = Computer.Cpus.RootCpu.CpuClocks.Multiplier;
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            //lock (_stdLock)
            //{
                var allCPus = computerInformation.Update(Computer);
                SyncContext.Post(ea => MainViewModel_OnUpdate(
                   allCPus
                ), null);

            //}

        }

        private CpuClocks cpuclocks;

        public ComputerModel Computer
        {
            get { return Get<ComputerModel>(); }
            set { Set(value); }
        }

        public string CoreSpeed
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string BusSpeed
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public decimal Multiplier
        {
            get { return Get<decimal>(); }
            set { Set(value); }
        }


        public void Init()
        {
           // computerInformation =  new WindowsManagementComputerInformation();
            computerInformation = new CpuIdComputerInformation();
            Computer = computerInformation.GetAllData();
            MainViewModel_OnUpdate(Computer);
            t.Start();
        }
    }
}
