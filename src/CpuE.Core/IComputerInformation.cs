namespace CpuE.Core
{
    public interface IComputerInformation
    {
        ComputerModel GetAllData();

        ComputerModel Update(ComputerModel computerModel);
    }
}