using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StardewValleyLocalization.Exceptions;

namespace StardewValleyLocalization
{
    internal class Project
    {
        private int _lastContentIndex;
        private int _lastFileIndex;

        public Project(string projectFilePath)
        {
            ProjectDirectoryPath = Path.GetDirectoryName(projectFilePath);
            ProjectFilePath = projectFilePath;
        }

        public int LastFileIndex
        {
            get => _lastFileIndex;
            set
            {
                if (value >= 0)
                    _lastFileIndex = value;
            }
        }

        public int LastContentIndex
        {
            get => _lastContentIndex;
            set
            {
                if (value >= 0)
                    _lastContentIndex = value;
            }
        }

        public List<string> Files { get; set; }

        [JsonIgnore] public string ProjectDirectoryPath { get; }

        [JsonIgnore] public string ProjectFilePath { get; }

        [JsonIgnore] public ReadOnlyCollection<XnbFile> XnbFiles { get; set; }

        [JsonIgnore] public bool Changed => XnbFiles.Any(file => file.Changed);

        public static Project Create(string saveDirectory, LanguageFilter languageFilter)
        {
            if (Directory.Exists(saveDirectory)) throw new DirectoryAlreadyExistsException();

            Directory.CreateDirectory(saveDirectory);
            try
            {
                var settings = Settings.Instance;

                var projectFiles = new List<string>();

                if (!Directory.Exists(settings.ContentRoot))
                    throw new DirectoryNotFoundException(
                        "Could not find Stardew Valley's content folder.\nContent folder path can be changed in the config");

                foreach (var relativePath in settings.DirectoriesThatContainLanguageFiles)
                {
                    var regexFilters = languageFilter.RegexFilenameFilters.Select(x => new Regex(x)).ToList();
                    var folderPath = Path.Combine(settings.ContentRoot, relativePath);

                    var xnbFiles = Directory.GetFiles(folderPath, "*.xnb").Where(x =>
                    {
                        return regexFilters.Any(r => r.IsMatch(Path.GetFileName(x)));
                    }).Where(file =>
                    {
                        var fileName = Path.GetFileName(file);
                        var globalFileName = fileName.Split('.').First();

                        var globalFilePath = Path.Combine(relativePath, globalFileName);
                        return !settings.ExcludedFiles.Contains(globalFilePath);
                    }).ToList();

                    foreach (var filePath in xnbFiles)
                    {
                        var relativeDir = Path.Combine(relativePath, Path.GetFileName(filePath));
                        projectFiles.Add(relativeDir);
                        var destPath = Path.Combine(saveDirectory, relativeDir);
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        File.Copy(filePath, destPath);
                    }
                }

                var directoryInfo = new DirectoryInfo(saveDirectory);
                var projectFilePath = Path.Combine(saveDirectory, directoryInfo.Name + ".locproj");
                var project = new Project(projectFilePath) {Files = projectFiles};
                project.Save();

                project.InitializeXnbFiles();
                return project;
            }
            catch
            {
                Directory.Delete(saveDirectory, true);
                throw;
            }
        }

        public void SaveProject()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ProjectFilePath, json);
        }

        public void Save()
        {
            SaveProject();
            XnbFiles?.Where(x => x.Changed).ToList().ForEach(x => x.Save());
        }

        private void InitializeXnbFiles()
        {
            var files = new List<XnbFile>();
            foreach (var file in Files)
                files.Add(new XnbFile(this, file));
            XnbFiles = new ReadOnlyCollection<XnbFile>(files);
        }

        public static Project Load(string projectFilePath)
        {
            var project = new Project(projectFilePath);
            var json = File.ReadAllText(projectFilePath);
            JsonConvert.PopulateObject(json, project);
            project.InitializeXnbFiles();
            return project;
        }
    }
}