using System.Collections.Generic;

namespace FileEncryptor.WPF.Services.Interface
{
    internal interface IUserDialog
    {
        bool OpenFile(string Title, out string SelectedFIle, string Filter = "Все файлы (*.*)|*.*");
        bool OpenFiles(string Title, out IEnumerable<string> SelectedFIles, string Filter = "Все файлы (*.*)|*.*");
    }
}