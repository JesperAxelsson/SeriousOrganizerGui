using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SeriousOrganizerGui
{
    /// <summary>
    /// Interaction logic for AddLabelDialog.xaml
    /// </summary>
    public partial class RenameFolderDialog : Window
    {

        private DirEntry _dirEntry;

        public RenameFolderDialog(DirEntry dirEntry)
        {
            InitializeComponent();

            _dirEntry = dirEntry;
            txt_new_name.Text = dirEntry.Name;
            txt_new_name.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ... Test for F5 key.
            if (e.Key == Key.Enter)
            {
                Rename();
            }

            if (e.Key == Key.Escape)
            {
                Rename();
            }
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            Rename();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private static bool ContainsInvalidPathChars(string path)
        {
            return path.Any(c => Path.GetInvalidPathChars().Contains(c));
        }

        private static bool ContainsInvalidFileNameChars(string path)
        {
            return path.Any(c => Path.GetInvalidFileNameChars().Contains(c));
        }

        private static bool PathIsValid(string path)
        {
            return !ContainsInvalidFileNameChars(path) && !ContainsInvalidPathChars(path);
        }



        public void Rename()
        {
            var name = txt_new_name.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Error("Name may not be only whitespace!");
                return;
            }

            if (File.Exists(_dirEntry.Path))
            {
                var parent = Directory.GetParent(_dirEntry.Path);
                var newPath = Path.Combine(parent.FullName, name);

                if (!PathIsValid(name))
                {
                    //Error path already exists!
                    Error("Filename contains invalid characters!");
                    return;
                }

                if (File.Exists(newPath))
                {
                    Error("File already exists!");
                    return;
                }


                File.Move(_dirEntry.Path, newPath);
            }
            else if (Directory.Exists(_dirEntry.Path))
            {
                var parent = Directory.GetParent(_dirEntry.Path);
                var newPath = Path.Combine(parent.FullName, name);

                if (!PathIsValid(name))
                {
                    //Error path already exists!
                    Error("Path contains invalid characters");
                    return;
                }

                if (Directory.Exists(newPath))
                {
                    Error("Direcory already exists!");
                    return;
                }

                Directory.Move(_dirEntry.Path, newPath);
            }
            else
            {
                // Path does not exist!
                Error("No file with this is name is found!");
            }

            this.Close();
        }

        private void Cancel()
        {
            this.Close();
        }

        private void Error(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            this.Close();
        }
    }
}
