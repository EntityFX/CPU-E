using CpuE.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuE
{
    public partial class MainWindow : Form
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new MainViewModel(); 
            InitializeComponent();
        }

        private void InitBindings()
        {
            var rootCpuPath = $"{nameof(ViewModel)}.{nameof(ViewModel.AllCpus)}.{nameof(ViewModel.AllCpus.Cpus)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu)}";
            var motherBoardPath = $"{nameof(ViewModel)}.{nameof(ViewModel.AllCpus)}.{nameof(ViewModel.AllCpus.Motherboard)}";
            var biosPath = $"{nameof(ViewModel)}.{nameof(ViewModel.AllCpus)}.{nameof(ViewModel.AllCpus.Bios)}";
            specificationTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Specification)}");
            coreSpeedTextBox.DataBindings.Add(nameof(Text), this, $"{nameof(ViewModel)}.{nameof(ViewModel.CoreSpeed)}");
            multiplierTextBox.DataBindings.Add(nameof(Text), this, $"{nameof(ViewModel)}.{nameof(ViewModel.Multiplier)}");
            busSpeedTextBox.DataBindings.Add(nameof(Text), this, $"{nameof(ViewModel)}.{nameof(ViewModel.BusSpeed)}");
            cpuNameTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Name)}");
            codeNameTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CodeName)}");
            technologyTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.TechnologyName)}");
            cpuCoresTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.NumberOfCores)}");
            cpuThreadsTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.NumberOfLogicalProcessors)}");
            familyTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping.FamilyHex)}");
            cpuModelTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping.ModelHex)}");
            steppingTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.CpuFamilyModelStepping.Stepping)}");
            l1CacheDataTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}");
            l1CacheInstructionsTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Instructions)}");
            level2CacheTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2)}");
            level3CacheTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3)}");
            level1CacheDataWay.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.AssociativityWay)}");
            level1CacheInstructionsWay.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Instructions)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Instructions.AssociativityWay)}");
            level2CacheWay.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2.AssociativityWay)}");
            level3CacheWay.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3.AssociativityWay)}");
            architectueTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Architecture)}");
            instructionsTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.InstructionsConcatenated)}");

            caches_level1CacheDataSizeTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.SizeString)}");
            caches_level1CacheDataMultiplierTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.NumberOfCoresString)}");
            caches_level1CacheDataDescriptorTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.Descriptor)}");


            caches_level1CacheInstructionsSizeTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.SizeString)}");
            caches_level1CacheInstructionsMultiplierTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.NumberOfCoresString)}");
            caches_level1CacheInstructionsDescriptorTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level1Data.Descriptor)}");

            caches_level2CacheSizeTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2.SizeString)}");
            caches_level2CacheMultiplierTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2.NumberOfCoresString)}");
            caches_level2CacheDescriptorTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level2.Descriptor)}");

            caches_level3CacheSizeTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3.SizeString)}");
            caches_level3CacheMultiplierTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3.NumberOfCoresString)}");
            caches_level3CacheDescriptorTextBox.DataBindings.Add(nameof(Text), this, $"{rootCpuPath}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3)}.{nameof(ViewModel.AllCpus.Cpus.RootCpu.Caches.Level3.Descriptor)}");


            mbManufacturerTextBox.DataBindings.Add(nameof(Text), this, $"{motherBoardPath}.{nameof(ViewModel.AllCpus.Motherboard.Manufacturer)}");
            mbModeltextBox.DataBindings.Add(nameof(Text), this, $"{motherBoardPath}.{nameof(ViewModel.AllCpus.Motherboard.Model)}");
            mbVersionTextBox.DataBindings.Add(nameof(Text), this, $"{motherBoardPath}.{nameof(ViewModel.AllCpus.Motherboard.Version)}");

            biosBrandTextBox.DataBindings.Add(nameof(Text), this, $"{biosPath}.{nameof(ViewModel.AllCpus.Bios.Brand)}");
            biosVersionTextBox.DataBindings.Add(nameof(Text), this, $"{biosPath}.{nameof(ViewModel.AllCpus.Bios.Version)}");
            biosDateTextBox.DataBindings.Add(nameof(Text), this, $"{biosPath}.{nameof(ViewModel.AllCpus.Bios.Date)}");
            //mapperTypeNameTextBox.ComboBox.DataBindings.Add(nameof(ComboBox.SelectedValue)
            //    , this, $"{nameof(ViewModel)}.{nameof(ViewModel.MapperType)}", true, DataSourceUpdateMode.OnPropertyChanged);


            //sourceXmlTextBox.DataBindings.Add(nameof(Text), this, $"{nameof(ViewModel)}.{nameof(ViewModel.InputXmlText)}", true, DataSourceUpdateMode.OnPropertyChanged);



            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == null) return;

            //if (e.PropertyName == nameof(ViewModel.SelectedXmlPath))
            //{
            //    ViewModel.UpdateSelectedXml.Execute(ViewModel.SelectedXmlPath);
            //}
        }

        private void InitDataSources()
        {
            //xsltPathComboBox.ComboBox.DisplayMember = "Value";
            //xsltPathComboBox.ComboBox.ValueMember = "Key";
            //xsltPathComboBox.ComboBox.DataSource = ViewModel.XsltPaths;
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ViewModel.Init();
            InitDataSources();
            InitBindings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
