using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CpuE.ViewModel
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T Get<T>([CallerMemberName] string name = null)
        {
            Debug.Assert(name != null, "name != null");
            object value = null;
            if (_properties.TryGetValue(name, out value)) return value == null ? default(T) : (T)value;
            return default(T);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            var args = new PropertyChangedEventArgs(propertyName);
            if (handler != null) handler(this, args);
        }

        /// <summary>
        ///     Sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        protected void Set<T>(T value, [CallerMemberName] string name = null)
        {
            Debug.Assert(name != null, "name != null");
            if (Equals(value, Get<T>(name))) return;
            _properties[name] = value;
            OnPropertyChanged(name);
        }
    }
}
