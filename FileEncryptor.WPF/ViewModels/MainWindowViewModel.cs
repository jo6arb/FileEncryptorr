using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Infrastructure.Commands.Base;
using FileEncryptor.WPF.Services.Interface;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {

        private const  string __EncryptedFileSuffix = ".encrypted";

        private readonly IUserDialog _UserDialog;
        private readonly IEncryptor _Encryptor;

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

        #region ProgressValued : double - Значение прогресса

        /// <summary>Значение прогресса</summary>
        private double _ProgressValue;

        /// <summary>Значение прогресса</summary>
        public double ProgressValue { get => _ProgressValue; set => Set(ref _ProgressValue, value); }

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
        private async void OnEncryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if(file is null) return;

            var default_file_name = file.FullName + __EncryptedFileSuffix;
            if(!_UserDialog.SaveFile("Выбор файл для сохранения", out var destination_path, default_file_name)) return;

            var timer = Stopwatch.StartNew();

            var progress = new Progress<double>(persent => ProgressValue = persent);

            ((Command) EncryptCommand).Executable = false;
            ((Command) DecryptCommand).Executable = false;
            ((Command) SelectFileCommand).Executable = false;
            try
            {
                await _Encryptor.EncryptAsync(file.FullName, destination_path, Password, Progress:progress);
            }
            catch (OperationCanceledException )
            {
            }
            ((Command)EncryptCommand).Executable = true;
            ((Command)DecryptCommand).Executable = true;
            ((Command)SelectFileCommand).Executable = true;

            timer.Stop();

            _UserDialog.Information("Шифрование", $"Шифрование файла прошло успешно за {timer.Elapsed.TotalSeconds:0.##} секунд");
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
        private async void OnDecryptCommandExecuted(object p)
        {
            var file = p as FileInfo ?? SelectedFile;
            if (file is null) return;

            var default_file_name = file.FullName.EndsWith(__EncryptedFileSuffix) 
                ? file.FullName.Substring(0, file.FullName.Length - __EncryptedFileSuffix.Length)
                : file.FullName;
            if (!_UserDialog.SaveFile("Выбор файл для дешифрования", out var destination_path, default_file_name)) return;

            var timer = Stopwatch.StartNew();

            var progress = new Progress<double>(percent => ProgressValue = percent);

            ((Command)EncryptCommand).Executable = false;
            ((Command)DecryptCommand).Executable = false;
            ((Command)SelectFileCommand).Executable = false;
            var decryption_task = _Encryptor.DencryptAsync(file.FullName, destination_path, Password, Progress: progress);
            var success = false;
            try
            {
                success = await decryption_task;
            }
            catch (OperationCanceledException exception)
            {
               
            }
            ((Command)EncryptCommand).Executable = true;
            ((Command)DecryptCommand).Executable = true;
            ((Command)SelectFileCommand).Executable = true;

            timer.Stop();
            if(success)
                _UserDialog.Information("Шифрование", $"Дешифрование файла выполнено успешно за {timer.Elapsed.TotalSeconds:0.##} секунд");
            else
                _UserDialog.Warning("Шифрование", "Ошибка при дешифровке файла: указан неверный пароль.");
        }

        #endregion

        #endregion

        public MainWindowViewModel(IUserDialog UserDialog, IEncryptor Encryptor)
        {
            _UserDialog = UserDialog;
            _Encryptor = Encryptor;
        }
    }
}