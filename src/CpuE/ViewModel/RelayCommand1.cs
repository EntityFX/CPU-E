using System;

namespace CpuE.ViewModel
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute) : base(execute) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute) { }
    }
}
