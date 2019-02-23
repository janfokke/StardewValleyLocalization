using System.ComponentModel;
using PropertyChanged;

namespace StardewValleyLocalization
{
    [AddINotifyPropertyChangedInterface]
    internal class ObservableContent : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}