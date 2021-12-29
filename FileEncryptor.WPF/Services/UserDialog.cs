using System.Collections.Generic;
using System.Linq;
using FileEncryptor.WPF.Services.Interface;
using Microsoft.Win32;

namespace FileEncryptor.WPF.Services
{
    internal class UserDialog : IUserDialog
    {
        public bool OpenFile(string Title, out string SelectedFIle, string Filter = "Все файлы (*.*)|*.*")
        {
            var file_dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFIle = null;
                return false;
            }

            SelectedFIle = file_dialog.FileName;

            return true;
        }

        public bool OpenFiles(string Title, out IEnumerable<string> SelectedFIles, string Filter = "Все файлы (*.*)|*.*")
        {
            var file_dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFIles = Enumerable.Empty<string>();
                return false;
            }

            SelectedFIles = file_dialog.FileNames;

            return true;

        }
    }
}