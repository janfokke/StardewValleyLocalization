using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged;

namespace StardewValleyLocalization.Dialogs
{
    /// <summary>
    ///     Interaction logic for NewProjectDialog.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class NewProjectDialog : MetroWindow
    {
        private readonly string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalizationProjects");

        public NewProjectDialog()
        {
            DataContext = this;
            InitializeComponent();
            ProjectPath = defaultPath;
            ProjectName = GetProjectNameSuggestion();
        }

        public string ProjectDir { get; private set; }

        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }

        public bool ButtonEnabled => !string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(ProjectPath);

        private string GetProjectNameSuggestion()
        {
            Directory.CreateDirectory(defaultPath);

            for (var i = 1;; i++)
            {
                var projectName = $"project_{i}";
                if (Directory.Exists(Path.Combine(defaultPath, projectName))) continue;
                return projectName;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ProjectPath = folderBrowserDialog.SelectedPath;
        }

        private void OkeButton_Click(object sender, RoutedEventArgs e)
        {
            var illegalInFileName = new Regex(@"[\\/:*?""<>|]");
            if (illegalInFileName.IsMatch(NameTextBox.Text))
            {
                ShowError("Name cannot contain any of the following characters: \\/:*?\"<>|");
                return;
            }

            if (!Directory.Exists(ProjectPath))
            {
                ShowError("Project Path doesn't exist");
                return;
            }

            if (Directory.Exists(Path.Combine(ProjectPath, ProjectName)))
            {
                ShowError("Project already exists");
                return;
            }

            ProjectDir = Path.Combine(PathTextBox.Text, NameTextBox.Text);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
    }
}