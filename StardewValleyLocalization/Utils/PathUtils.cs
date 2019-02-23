using System;
using System.IO;

namespace StardewValleyLocalization.Utils
{
    internal class PathUtils
    {
        public static bool FilePathHasInvalidChars(string path)
        {
            return !string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            if (!Path.IsPathRooted(filespec))
                return filespec;

            if (!Path.IsPathRooted(folder))
                throw new ArgumentException("Must be an absolute Path.", "folder");

            filespec = Path.GetFullPath(filespec).TrimEnd('/', '\\');
            folder = Path.GetFullPath(folder).TrimEnd('/', '\\');

            if (filespec == folder)
                return string.Empty;

            var pathUri = new Uri(filespec);
            var folderUri = new Uri(folder + Path.DirectorySeparatorChar);
            var result = folderUri.MakeRelativeUri(pathUri).ToString();
            result = result.Replace('/', Path.DirectorySeparatorChar);
            result = Uri.UnescapeDataString(result);

            return result;
        }
    }
}