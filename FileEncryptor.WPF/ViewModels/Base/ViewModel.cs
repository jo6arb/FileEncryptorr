using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileEncryptor.WPF.Annotations;

namespace FileEncryptor.WPF.ViewModels.Base
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}