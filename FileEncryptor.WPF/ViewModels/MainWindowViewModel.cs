using System.IO;
using System.Windows.Input;
using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Services.Interface;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IUserDialog _UserDialog;

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Шифратор";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region Password : string - Пароль

        /// <summary>Пароль</summary>
        private string _Password = "123";

        /// <summary>Пароль</summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        #endregion

        #region SelectedFile : FileInfo - Выбранный файл

        /// <summary>Выбранный файл</summary>
        private FileInfo _SelectedFile;

        /// <summary>Выбранный файл</summary>
        public FileInfo SelectedFile { get => _SelectedFile; set => Set(ref _SelectedFile, value); }

        #endregion

        #region Команды

        #region SelectedFileCommand - команда выбора файла

        private ICommand _SelectFIleCommand;

        public ICommand SelectFileCommand => _SelectFIleCommand ??= new LambdaCommand(OnSelectFileCommandExecute);

        private void OnSelectFileCommandExecute(object Obj)
        {
            if(!_UserDialog.OpenFile("Выбор файла для шифрования", out var file_path)) return;
            var selected_file = new FileInfo(file_path);
            SelectedFile = selected_file.Exists ? selected_file : null;
        }
        
        #endregion

        #region Command EncryptCommand - Команда зашифрования файла

        /// <summary>Команда зашифрования файла</summary>
        private ICommand _EncryptCommand;

        /// <summary>Команда зашифрования файла</summary>
        public ICommand EncryptCommand => _EncryptCommand
            ??= new LambdaCommand(OnEncryptCommandExecuted, CanEncryptCommandExecute);

        /// <summary>Проверка возможности выполнения - Команда зашифрования файла</summary>
        private bool CanEncryptCommandExecute(object p) => (p is FileInfo file && file.Exists || SelectedFile != null) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>Логика выполнения - Команда зашифрования файла</summary>
        private void OnEncryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if(file is null) return;
        }

        #endregion

        #region Command DecryptCommand - Команда расшифрования файла

        /// <summary>Команда расшифрования файла</summary>
        private ICommand _DecryptCommand;

        /// <summary>Команда расшифрования файла</summary>
        public ICommand DecryptCommand => _DecryptCommand
            ??= new LambdaCommand(OnDecryptCommandExecuted, CanDecryptCommandExecute);

        /// <summary>Проверка возможности выполнения - Команда расшифрования файла</summary>
        private bool CanDecryptCommandExecute(object p) => (p is FileInfo file && file.Exists || SelectedFile !=null) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>Логика выполнения - Команда расшифрования файла</summary>
        private void OnDecryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;
        }

        #endregion

        #endregion

        public MainWindowViewModel(IUserDialog UserDialog) { _UserDialog = UserDialog; }
    }
}