using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using PropertyChanged;

namespace StardewValleyLocalization.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    internal class LocalizatorViewModel
    {
        private readonly IView _view;

        private bool _closing;
        private int _contentIndex;
        private int _fileIndex;
        private XnbFile _selectedFile;

        public LocalizatorViewModel(IView view)
        {
            _view = view;
            FindInProjectCommand = new DelegateCommand(FindInProject, _ContentAvailable);
            OpenFileInExplorerCommand = new DelegateCommand(OpenFileInExplorer);
            CloseCommand = new DelegateCommand(Close);
            SaveProjectCommand = new DelegateCommand(SaveProject, _ContentAvailable);
            LoadProjectCommand = new DelegateCommand(LoadProject);
            QuickPasteCommand = new DelegateCommand(QuickPaste, _ContentAvailable);
            PreviousContentCommand = new DelegateCommand(PreviousContent, _ContentAvailable);
            NextContentCommand = new DelegateCommand(NextContent, _ContentAvailable);
            InstallCommand = new DelegateCommand(Install, _ContentAvailable);
            InitializeLanguageFilters();
        }

        public Project Project { get; private set; }
        public List<MenuItem> LanguageFilters { get; } = new List<MenuItem>();
        public BindingList<XnbFile> Files { get; set; } = new BindingList<XnbFile>();
        public BindingList<Content> Content { get; set; } = new BindingList<Content>();
        public Content SelectedContent { get; set; }

        public DelegateCommand NextContentCommand { get; }
        public DelegateCommand PreviousContentCommand { get; }
        public DelegateCommand QuickPasteCommand { get; }
        public DelegateCommand LoadProjectCommand { get; }
        public DelegateCommand SaveProjectCommand { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand OpenFileInExplorerCommand { get; }
        public DelegateCommand FindInProjectCommand { get; }
        public DelegateCommand InstallCommand { get; }

        public int FileIndex
        {
            get => _fileIndex;
            set
            {
                _fileIndex = value;
                if (Project != null && value >= 0)
                    Project.LastFileIndex = value;
            }
        }

        public int ContentIndex
        {
            get => _contentIndex;
            set
            {
                _contentIndex = value;


                if (Project != null && value >= 0)
                    Project.LastContentIndex = value;
            }
        }

        public XnbFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                Content.RaiseListChangedEvents = false;
                Content.Clear();
                if (value != null)
                    foreach (var content in value.Content)
                        Content.Add(content);
                _selectedFile = value;
                Content.RaiseListChangedEvents = true;
                Content.ResetBindings();
            }
        }

        public bool ContentAvailable { get; private set; }

        private void Install(object obj)
        {
            _view.Notify("Copying files", TimeSpan.FromSeconds(1));
            Project.Files.ForEach(file =>
            {
                string filePath = Path.Combine(Project.ProjectDirectoryPath, file);
                string targetPath = Path.Combine(Settings.Instance.ContentRoot, file);
                File.Copy(filePath, targetPath, true);
            });
        }

        private void FindInProject(object obj)
        {
            var findCommandModel = obj as FindCommandModel;
            if (findCommandModel == null || Files == null)
                return;


            var fileSkipCount = SelectedFile == null ? 0 : Files.IndexOf(SelectedFile);

            if (Files.Count > 0)
            {
                var files = Files.Skip(fileSkipCount);
                foreach (var file in files)
                {
                    IEnumerable<Content> fileContent;
                    if (file == SelectedFile && SelectedContent != null)
                    {
                        var contentSkipCount = Content.IndexOf(SelectedContent) + 1;
                        if (contentSkipCount == file.Content.Count)
                            continue;
                        fileContent = file.Content.Skip(contentSkipCount);
                    }
                    else
                    {
                        fileContent = file.Content;
                    }

                    foreach (var content in fileContent)
                    {
                        var target = findCommandModel.IgnoreCases ? content.Original.ToLower() : content.Original;

                        var searchTerm = findCommandModel.IgnoreCases
                            ? findCommandModel.SearchTerm.ToLower()
                            : findCommandModel.SearchTerm;

                        if (target.Contains(searchTerm))
                        {
                            SelectedFile = file;
                            SelectedContent = content;
                            return;
                        }
                    }
                }
            }
        }

        private void NextContent(object obj)
        {
            if (Content == null || SelectedFile == null)
                return;


            if (ContentIndex < Content.Count - 1)
                SelectedContent = Content[ContentIndex + 1];
            else if (FileIndex < Files.Count - 1) SelectedFile = Files[FileIndex + 1];
        }

        private void PreviousContent(object obj)
        {
            if (Content == null || SelectedFile == null)
                return;

            if (ContentIndex > 0)
                SelectedContent = Content[ContentIndex - 1];
            else if (FileIndex > 0) SelectedFile = Files[FileIndex - 1];
        }

        private void QuickPaste(object obj)
        {
            if (SelectedContent?.ContentParts != null
                && SelectedContent.ContentParts.Count > 0)
                SelectedContent.ContentParts[0].Content = _view.GetClipboard();

            NextContent(null);
            if (SelectedContent?.ContentParts != null
                && SelectedContent.ContentParts.Count > 0)
                _view.SetClipboard(SelectedContent.ContentParts[0].Content);
        }

        private bool _ContentAvailable(object arg)
        {
            return Content?.Count > 0;
        }

        private async void Close(object obj)
        {
            if (Project == null || _closing)
                return;

            if (Project.Changed)
            {
                var args = obj as CancelEventArgs;
                args.Cancel = true;
                var dialogResult = await _view.AskSaveConfirmation("Save changes before closing?");
                if (dialogResult.HasValue)
                {
                    if (dialogResult.Value) SaveProject(null);
                    _closing = true;
                    _view.Close();
                }
            }
            else
            {
                //Save project basics
                Project.SaveProject();
            }
        }

        private void SaveProject(object obj)
        {
            if (!Project?.Changed ?? false)
                return;
            _view.Notify("Saving", TimeSpan.FromSeconds(1));
            Project?.Save();
        }

        private void OpenFileInExplorer(object obj)
        {
            var fileModel = obj as XnbFile;
            Process.Start(Path.GetDirectoryName(fileModel.FullPath));
        }

        public void InitializeLanguageFilters()
        {
            var newProjectCommand = new DelegateCommand(NewProject);
            var languageFilters = Settings.Instance.LanguageFilters;
            foreach (var language in languageFilters)
                LanguageFilters.Add(new MenuItem
                {
                    Header = language.Name,
                    Command = newProjectCommand,
                    CommandParameter = language
                });
        }

        private async void NewProject(object o)
        {
            if (Project != null)
            {
                if (Project.Changed)
                {
                    var dialogResult = await _view.AskSaveConfirmation("Save changes before creating a new project?");
                    if (dialogResult.HasValue)
                    {
                        if (dialogResult.Value)
                            SaveProject(null);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    //Save project basics
                    Project.SaveProject();
                }
            }

            var languageFilter = o as LanguageFilter;

            if (!_view.GetSaveProjectDir(out var saveDirectory))
                return;

            try
            {
                InitializeProject(Project.Create(saveDirectory, languageFilter));
            }
            catch (Exception exception)
            {
                _view.ShowError(exception.Message);
            }
        }

        private void InitializeProject(Project project)
        {
            Project = project;
            Files.RaiseListChangedEvents = false;
            Files.Clear();
            foreach (var fileModel in Project.XnbFiles)
                Files.Add(fileModel);

            if (project.LastFileIndex < Files.Count - 1) SelectedFile = Files[project.LastFileIndex];
            if (project.LastContentIndex < Content?.Count - 1) SelectedContent = Content[project.LastContentIndex];

            ContentAvailable = true;
            SaveProjectCommand.RaiseCanExecuteChanged();
            NextContentCommand.RaiseCanExecuteChanged();
            PreviousContentCommand.RaiseCanExecuteChanged();
            QuickPasteCommand.RaiseCanExecuteChanged();
            InstallCommand.RaiseCanExecuteChanged();


            Files.RaiseListChangedEvents = true;
            Files.ResetBindings();
        }

        public async void LoadProject(object _)
        {
            if (Project != null)
            {
                if (Project.Changed)
                {
                    var dialogResult = await _view.AskSaveConfirmation("Save changes before loading a new project?");
                    if (dialogResult.HasValue)
                    {
                        if (dialogResult.Value) SaveProject(null);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    //Save project basics
                    Project.SaveProject();
                }
            }

            if (!_view.GetLoadProjectFilePath(out var path))
                return;

            try
            {
                InitializeProject(Project.Load(path));
            }
            catch (Exception e)
            {
                _view.ShowError(e.Message);
            }
        }
    }
}