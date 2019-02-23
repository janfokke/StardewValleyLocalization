using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PropertyChanged;
using XnbConvert;

namespace StardewValleyLocalization
{
    [AddINotifyPropertyChangedInterface]
    internal class XnbFile
    {
        private readonly object _initializeMutex = new object();
        private IReadOnlyCollection<Content> _content;
        private Type _contentKeyType;

        private bool _initialized;

        public XnbFile(Project project, string relativePath)
        {
            RelativePath = relativePath;
            FullPath = Path.Combine(project.ProjectDirectoryPath, relativePath);
            var directoryInfo = new DirectoryInfo(FullPath);
            Name = directoryInfo.Name;
        }

        public IReadOnlyCollection<Content> Content
        {
            get
            {
                Initialize();
                return _content.Where(i =>
                {
                    var b = !Settings.Instance.IgnoredContent.Contains(i.ContentPath);
                    return b;
                }).ToList();
            }
        }

        /// <summary>
        ///     The name of the content file including the .xnb extension
        /// </summary>
        public string Name { get; }


        public string RelativePath { get; }
        public string FullPath { get; set; }

        /// <summary>
        ///     Indicates whether the XnbFile has changed
        /// </summary>
        public bool Changed { get; set; }

        public void Initialize()
        {
            lock (_initializeMutex)
            {
                if (_initialized)
                    return;
                _initialized = true;

                var xnbDeserializer = new XnbDeserializer();
                var xnbFileBytes = File.ReadAllBytes(FullPath);
                var result = xnbDeserializer.Deserialize<object>(xnbFileBytes);
                _contentKeyType = result.Content.GetType().GetGenericArguments()[0];
                var keyValuePairs = result.Content as IDictionary;

                var items = new List<Content>();
                foreach (DictionaryEntry keyValuePair in keyValuePairs)
                {
                    var fileName = Path.GetFileName(FullPath);
                    var globalFileName = fileName.Split('.').First();
                    var globalFilePath = Path.Combine(Path.GetDirectoryName(RelativePath), globalFileName);

                    var keyPath = $"{globalFilePath}:{keyValuePair.Key}";
                    var parser = ParserFactory.Instance.FindParser(keyPath);
                    items.Add(new Content(this, parser, keyValuePair.Key, (string) keyValuePair.Value, keyPath));
                }

                _content = new ReadOnlyCollection<Content>(items);
            }
        }

        public void Save()
        {
            var genericType = typeof(Dictionary<,>).MakeGenericType(_contentKeyType, typeof(string));
            dynamic dictionary = Activator.CreateInstance(genericType);
            foreach (var c in _content)
            {
                genericType.GetMethod("Add")
                    .Invoke(dictionary, new[] {c.Index, c.Result});
                c.Save();
            }

            byte[] bytes = XnbSerializer.Instance.Serialize(dictionary);
            File.WriteAllBytes(FullPath, bytes);
            Changed = false;
        }
    }
}