using System.Collections.Generic;

namespace FileEncryptor.WPF.Services.Interface
{
    internal interface IUserDialog
    {
        bool OpenFile(string Title, out string SelectedFIle, string Filter = "Все файлы (*.*)|*.*");
        
        bool SaveFile(string Title, out string SelectedFIle, string DefaultFileName = null, string Filter = "Все файлы (*.*)|*.*");

        void Information(string Title, string Message);

        void Warning(string Title, string Message);

        void Error(string Title, string Message);

    }
}