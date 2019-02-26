using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StardewValleyLocalization.Dialogs;
using StardewValleyLocalization.ViewModel;
using Clipboard = System.Windows.Clipboard;
using ListBox = System.Windows.Controls.ListBox;

namespace StardewValleyLocalization
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IView
    {
        private readonly FindWindow _findWindow;

        public MainWindow()
        {
            _findWindow = new FindWindow(findCommandModel =>
            {
                this.Invoke(() =>
                    ((LocalizatorViewModel) DataContext).FindInProjectCommand.Execute(findCommandModel));
            });
            OriginalTitle = Title;
            DataContext = new LocalizatorViewModel(this);
        }


        public string OriginalTitle { get; set; }


        public async Task<(bool replace, bool repeat)> AskReplaceFile(string file)
        {
            var result = await this.ShowMessageAsync(
                $"A file named \"{file}\" already exists. Do you want to replace it?",
                "Replacing wil overwrite its contents",
                MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, new MetroDialogSettings
                {
                    AffirmativeButtonText = "Skip",
                    NegativeButtonText = "Cancel",
                    FirstAuxiliaryButtonText = "Replace",
                    SecondAuxiliaryButtonText = "Replace all",
                    AnimateShow = false,
                    AnimateHide = false
                });

            switch (result)
            {
                case MessageDialogResult.Affirmative:
                    return (false, false);
                case MessageDialogResult.Negative:
                    return (false, true);
                case MessageDialogResult.FirstAuxiliary:
                    return (true, false);
                default:
                    return (true, true);
            }
        }

        public void ShowError(string message, string title = "Error encountered")
        {
            this.ShowMessageAsync(title, message,
                MessageDialogStyle.Affirmative, new MetroDialogSettings
                {
                    AnimateShow = false,
                    AnimateHide = false
                });
        }

        public async void Notify(string message, TimeSpan duration)
        {
            Title = message;
            await Task.Delay(duration);
            Title = OriginalTitle;
        }

        public bool GetSaveProjectDir(out string path)
        {
            var dialog = new NewProjectDialog();
            if (dialog.ShowDialog() == true)
            {
                path = dialog.ProjectDir;
                return true;
            }

            path = null;
            return false;
        }

        public bool GetLoadProjectFilePath(out string path)
        {
            var folderBrowserDialog = new OpenFileDialog
                {Title = "Open project", Filter = "Localization Project (*.locproj)|*.locproj", Multiselect = false};
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = folderBrowserDialog.FileName;
                return true;
            }

            path = null;
            return false;
        }

        public async Task<bool?> AskSaveConfirmation(string message)
        {
            var result = await this.ShowMessageAsync(message,
                "If you don't save, changes wil be permanently lost.",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings
                {
                    AffirmativeButtonText = "Continue without saving",
                    NegativeButtonText = "Cancel",
                    FirstAuxiliaryButtonText = "Save",
                    AnimateShow = false,
                    AnimateHide = false
                });
            switch (result)
            {
                case MessageDialogResult.Affirmative:
                    return false;
                case MessageDialogResult.FirstAuxiliary:
                    return true;
                default:
                    return null;
            }
        }

        public void SetClipboard(string value)
        {
            Clipboard.SetText(value);
        }

        public string GetClipboard()
        {
            return Clipboard.GetText();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            ((LocalizatorViewModel) DataContext).CloseCommand.Execute(e);
        }

        private void FindInFile_Click(object sender, RoutedEventArgs e)
        {
            _findWindow.Show();
            _findWindow.Activate();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            _findWindow.Close();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }
}