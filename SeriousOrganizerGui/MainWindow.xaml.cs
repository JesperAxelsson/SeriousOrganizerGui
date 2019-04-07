using SeriousOrganizerGui.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using SeriousOrganizerGui.Data;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace SeriousOrganizerGui
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataClient.Connect();


            //_client.Connect();
            //_dirEntryProvider = new DirEntryProvider(_client);
            //_turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            //_client.SendTextSearchChanged(""); // Reset search text
            //_turbo.Update();

            //dir_list.ItemsSource = _turbo;
            //label_list.ItemsSource = _labelList;
            //UpdateLabels();

            //var bin = new Binding("");
            //bin.Mode = 
        }

     
    }
}
