using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;
using XnbConvert;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MenuItem = System.Windows.Controls.MenuItem;

namespace StardewValleyLocalization
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : MetroWindow
    {
        private const string DefaultTitle = "Stardew Valley localization editor";
        public string TitleBinding { get; set; } = DefaultTitle;

        private bool _closing;
        private readonly Dictionary<string, XnbFile> _xnbFileStorage = new Dictionary<string, XnbFile>();
        private XnbFile _currentXnbFile;
        private string _currentContentKey;

        public ObservableCollection<MenuItem> MenuLanguages { get; set; }
        public ObservableCollection<ListViewItem> XnbFiles { get; set; } = new ObservableCollection<ListViewItem>();
        public ObservableCollection<ListViewItem> XnbContentKeys { get; set; } = new ObservableCollection<ListViewItem>();
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MenuLanguages = new ObservableCollection<MenuItem>();

            SaveCommand = new CommandHandler(Save, true);
            LoadCommand = new CommandHandler(Load, true);

            var languageFilters = Settings.Instance.LanguageFilters;
            foreach (var language in languageFilters)
                MenuLanguages.Add(new MenuItem
                {
                    Header = language.Name,
                    Command = new CommandHandler(CreateNewProject, true),
                    CommandParameter = language
                });
        }

        public async void ShowInfoMessage(string message)
        {
            TitleBinding = DefaultTitle + " " + message;
            await Task.Delay(1000);
            TitleBinding = DefaultTitle;
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            if (_closing)
                return;

            if (UnsavedFilesLeft())
            {
                e.Cancel = true;
                switch (await ShowSaveDialog("Save changes before closing?"))
                {
                    case MessageDialogResult.Affirmative:
                        _closing = true;
                        Close();
                        break;
                    case MessageDialogResult.FirstAuxiliary:
                        _closing = true;
                        Save(null);
                        Close();
                        break;
                }
            }

            base.OnClosing(e);
        }

        private async Task<MessageDialogResult> ShowSaveDialog(string message)
        {
            return await this.ShowMessageAsync(message,
                "If you don't save, changes wil be permanently lost.",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings
                {
                    AffirmativeButtonText = "Continue without saving",
                    NegativeButtonText = "Cancel",
                    FirstAuxiliaryButtonText = "Save",
                    AnimateShow = false,
                    AnimateHide = false
                });
        }

        private async void Load(object obj)
        {
            if (UnsavedFilesLeft())
                switch (await ShowSaveDialog("Save changes before loading new project?"))
                {
                    case MessageDialogResult.Negative:
                        return;
                    case MessageDialogResult.FirstAuxiliary:
                        Save(null);
                        break;
                }

            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder to load files from";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadProject(folderBrowserDialog.SelectedPath);
        }

        private void Save(object obj)
        {
            ShowInfoMessage("(Saving)");
            var changedFiles = _xnbFileStorage.Where(x => x.Value.Changed).ToList();
            foreach (var changedFileKeyValuePair in changedFiles)
            foreach (var changedContentKeyValuePair in changedFileKeyValuePair.Value.ChangeableContent)
                changedFileKeyValuePair.Value.OriginalContent[changedContentKeyValuePair.Key] =
                    changedContentKeyValuePair.Value;

            changedFiles.ForEach(x =>
            {
                var xnbSerializer = new XnbSerializer();
                var xnbFile = xnbSerializer.Serialize(x.Value.OriginalContent,
                    flags: XnbFlags.ContentCompressedLzx | XnbFlags.Hidef);
                File.WriteAllBytes(x.Key, xnbFile);
                x.Value.Changed = false;
                x.Value.ChangeableContent.Clear();
            });
        }

        public async void LoadProject(string path)
        {
            try
            {
                OriginalContent.Clear();
                ChangeableContent.Clear();
                _currentContentKey = null;
                _currentXnbFile = null;
                _xnbFileStorage.Clear();
                XnbFiles.Clear();
                XnbContentKeys.Clear();
                var files = Directory.GetFiles(path, "*.xnb", SearchOption.AllDirectories);
                foreach (var file in files)
                    XnbFiles.Add(new ListViewItem
                    {
                        Content = Path.GetFileName(file),
                        Tag = file
                    });
            }
            catch (Exception e)
            {
                await this.ShowMessageAsync("Error encountered",
                    e.Message,
                    MessageDialogStyle.Affirmative, new MetroDialogSettings
                    {
                        AnimateShow = false,
                        AnimateHide = false
                    });
            }
        }

        private bool UnsavedFilesLeft()
        {
            if (_xnbFileStorage == null)
                return false;
            return _xnbFileStorage.Any(x => x.Value.Changed);
        }

        private async void CreateNewProject(object o)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder to save output files";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var languageFilter = o as LanguageFilter;
                var settings = Settings.Instance;
                foreach (var path in settings.DirectoriesThatContainLanguageFiles)
                {
                    var RegexFilters = languageFilter.RegexFilenameFilters.Select(x => { return new Regex(x); })
                        .ToList();

                    string folderPath = Path.Combine(settings.ContentRoot, path);

                    if (!Directory.Exists(folderPath))
                    {
                        await this.ShowMessageAsync(
                            $"{folderPath}\" doesn't exists.",
                            "Make sure the ContentRoot property is configured properly",
                            MessageDialogStyle.Affirmative, new MetroDialogSettings
                            {
                                AnimateShow = false,
                                AnimateHide = false
                            });
                        return;

                    }
                    
                       
                    var files = Directory.GetFiles(folderPath, "*.xnb").Where(x =>
                    {
                        return RegexFilters.Any(r => r.IsMatch(Path.GetFileName(x)));
                    }).ToList();
                    

                    foreach (var filePath in files)
                    {
                        var destPath = Path.Combine(folderBrowserDialog.SelectedPath, path, Path.GetFileName(filePath));
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        if (File.Exists(destPath))
                            switch (await this.ShowMessageAsync(
                                $"A file named \"{Path.GetFileName(destPath)}\" already exists. Do you want to replace it?",
                                "Replacing wil overwrite its contents",
                                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings
                                {
                                    AffirmativeButtonText = "Skip",
                                    NegativeButtonText = "Cancel",
                                    FirstAuxiliaryButtonText = "Replace",

                                    AnimateShow = false,
                                    AnimateHide = false
                                }))
                            {
                                case MessageDialogResult.Affirmative:
                                    continue;
                                case MessageDialogResult.Negative:
                                    return;
                            }
                        File.Copy(filePath, destPath);
                    }
                }

                LoadProject(folderBrowserDialog.SelectedPath);
            }
        }

        private void KeyListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (!(listView.SelectedItem is ListViewItem listViewItem))
                return;
            _currentContentKey = (string) listViewItem.Content;

            OriginalContent.Text = _currentXnbFile.OriginalContent[_currentContentKey];

            if (!_currentXnbFile.ChangeableContent.ContainsKey(_currentContentKey))
                _currentXnbFile.ChangeableContent[_currentContentKey] = _currentXnbFile.OriginalContent[_currentContentKey];
            ChangeableContent.Text = _currentXnbFile.ChangeableContent[_currentContentKey];
        }

        private void ChangeableContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentXnbFile?.ChangeableContent == null)
                return;
            if (_currentXnbFile.ChangeableContent.TryGetValue(_currentContentKey, out var value)
                && value != ChangeableContent.Text)
            {
                _currentXnbFile.Changed = true;

                _currentXnbFile.ChangeableContent[_currentContentKey] = ChangeableContent.Text;
            }
        }

        private void FileListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listview = sender as ListView;
            if (listview.SelectedItem == null) return;

            var listViewItem = listview.SelectedItem as ListViewItem;
            var path = (string) listViewItem.Tag;
            LoadContent(path);
        }

        private void LoadContent(string path)
        {
            if (!_xnbFileStorage.ContainsKey(path))
            {
                var xnbDeserializer = new XnbDeserializer();
                var data = File.ReadAllBytes(path);
                var xnbFile = xnbDeserializer.Deserialize<Dictionary<string, string>>(data);
                _xnbFileStorage[path] = new XnbFile
                {
                    Changed = false, OriginalContent = xnbFile.Content,
                    ChangeableContent = new Dictionary<string, string>()
                };
            }

            _currentXnbFile = _xnbFileStorage[path];
            XnbContentKeys.Clear();
            foreach (var key in _xnbFileStorage[path].OriginalContent.Keys)
                XnbContentKeys.Add(new ListViewItem {Content = key});
        }
    }
}