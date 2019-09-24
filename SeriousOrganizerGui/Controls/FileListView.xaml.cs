using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Data.Providers;
using SeriousOrganizerGui.Dto;
using SeriousOrganizerGui.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeriousOrganizerGui.Controls
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    public partial class FileListView : UserControl
    {

        public FileListView()
        {
            InitializeComponent();
        }

        public void SetSelectedEntry(int entryIndex)
        {
            var currentProvider = file_list.ItemsSource as ItemProviderTurbo<FileEntry>;
            Console.WriteLine("Selected row: " + entryIndex);

            if (currentProvider is null)
            {
                var provider = new FileEntryProvider(DataClient.Client, entryIndex);
                var foo = new ItemProviderTurbo<FileEntry>(provider);
                foo.Update();
                file_list.ItemsSource = foo;
            }
            else
            {
                (currentProvider.Provider as FileEntryProvider)?.SetDirIndex(entryIndex);
                currentProvider.Update();
                Reset();
            }
        }

        public void Reset()
        {
            file_list.SelectedItem = null;
        }

        private void file_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = sender as ListView;
            if (lv?.SelectedIndex < 0) return;

            var path = ((FileEntry)lv!.SelectedItem).Path;
            if (path == null) return;

            Util.StartProcess(path);
        }

        private void Delete_Files_OnClick(object sender, RoutedEventArgs e)
        {
            var fileEntries = file_list.FindSelectedItems<FileEntry>();
            if (fileEntries.Count() == 0)
            {
                MessageBox.Show($"No files selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult messageResult;

            if (fileEntries.Count() == 1)
            {
                var fileEntry = fileEntries.First();
                messageResult = MessageBox.Show($"Are you sure you want to remove {fileEntry.Path}?", $"Remove file {fileEntry.Name}", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
            else
            {
                messageResult = MessageBox.Show($"Are you sure you want to remove {fileEntries.Count()} files?", $"Remove files", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }

            if (messageResult != MessageBoxResult.OK)
                return;

            // Remove files
            Util.DeletePath(fileEntries.Select(fe => fe.Path));

        }
    }
}
