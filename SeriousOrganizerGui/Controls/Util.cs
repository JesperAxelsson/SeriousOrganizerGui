using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SeriousOrganizerGui.Controls
{
    public static class Util
    {
        public static void OpenPathInExplorer(string path)
        {
            if (File.Exists(path) )
            {
                var psi = new ProcessStartInfo("explorer") { CreateNoWindow = true, UseShellExecute = true };
                psi.Arguments = "/select," + path;

                Process.Start(psi);

            }else if (Directory.Exists(path) ){

                var psi = new ProcessStartInfo(path) { CreateNoWindow = true, UseShellExecute = true };

                Process.Start(psi);
            }
            else
            {
                MessageBox.Show("Failed to find file or folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void StartProcess(string command)
        {
            if (File.Exists(command) || Directory.Exists(command))
            {
                var psi = new ProcessStartInfo(command) { CreateNoWindow = true, UseShellExecute = true };
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show("Failed to find file or folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static void DeletePath(IEnumerable< string> paths)
        {
            foreach(var path in paths)
            {
                DeletePath(path);
            }
        }

        public static void DeletePath(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete {path} \n Error: {ex.InnerException?.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static T? FindClickedItem<T>(MenuItem? sender)
            where T : class
        {
            if (sender == null)
            {
                return null;
            }

            var cm = sender.CommandParameter as ContextMenu;
            if (cm == null)
            {
                return null;
            }

            return (cm.PlacementTarget as ListViewItem)?.Content as T;
        }
    }
}
