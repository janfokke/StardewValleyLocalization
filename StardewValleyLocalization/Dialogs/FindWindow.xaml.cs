using System;
using System.Windows;
using MahApps.Metro.Controls;
using StardewValleyLocalization.ViewModel;

namespace StardewValleyLocalization.Dialogs
{
    /// <summary>
    ///     Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : MetroWindow
    {
        private readonly Action<FindCommandModel> _find;

        public FindWindow(Action<FindCommandModel> find)
        {
            _find = find;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }


        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            _find(new FindCommandModel
                {IgnoreCases = IgnoreCaseCheckBox.IsChecked.GetValueOrDefault(false), SearchTerm = NameTextBox.Text});
        }
    }
}