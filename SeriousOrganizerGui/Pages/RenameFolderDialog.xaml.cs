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
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            var name = txt_new_name.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return;

            if (File.Exists(_dirEntry.Path))
            {
                var parent = Directory.GetParent(_dirEntry.Path);
                var newPath = Path.Combine(parent.FullName, name);

                if (name.Any(c => Path.GetInvalidFileNameChars().Contains(c)) || File.Exists(newPath))
                {
                    //Error path already exists!
                    return;
                }

                File.Move(_dirEntry.Path, newPath);
            }
            else if (Directory.Exists(_dirEntry.Path))
            {
                var parent = Directory.GetParent(_dirEntry.Path);
                var newPath = Path.Combine(parent.FullName, name);

                if (name.Any(c => Path.GetInvalidPathChars().Contains(c)) || Directory.Exists(newPath))
                {
                    //Error path already exists!
                    return;
                }

                Directory.Move(_dirEntry.Path, newPath);
            }
            else
            {
                // Path does not exist!
            }

            this.Close();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
