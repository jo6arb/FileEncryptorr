using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using FileEncryptor.WPF.Services.Interface;
using FileEncryptor.WPF.Views.Windows;
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

        public bool SaveFile(string Title, out string SelectedFIle, string DefaultFileName = null, string Filter = "Все файлы (*.*)|*.*")
        {
            var file_dialog = new SaveFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (!string.IsNullOrWhiteSpace(DefaultFileName))
                file_dialog.FileName = DefaultFileName;

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFIle = null;
                return false;
            }

            SelectedFIle = file_dialog.FileName;

            return true;
        }

        public void Information(string Title, string Message) => MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Information);

        public void Warning(string Title, string Message) => MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);

        public void Error(string Title, string Message) => MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);

        public (IProgress<double> Progress, IProgress<string> Status, CancellationToken Cancel, Action Close) ShowProgress(string Title)
        {
            var progress_window = new ProgressWindow {Title = Title, Owner = App.FocucedWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner};
            progress_window.Show();
            return (progress_window.ProgressInformer, progress_window.StatusInformer, progress_window.Cancel, progress_window.Close);

        }
    }
}