using System;
using System.Threading.Tasks;

namespace StardewValleyLocalization
{
    public interface IView
    {
        Task<(bool replace, bool repeat)> AskReplaceFile(string file);
        void ShowError(string message, string title = "Error encountered");
        void Notify(string message, TimeSpan duration);
        bool GetSaveProjectDir(out string path);
        bool GetLoadProjectFilePath(out string path);
        Task<bool?> AskSaveConfirmation(string message);
        void Close();
        void SetClipboard(string value);
        string GetClipboard();
    }
}