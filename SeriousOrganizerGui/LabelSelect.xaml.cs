using SeriousOrganizerGui.Data;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SeriousOrganizerGui
{
    /// <summary>
    /// Interaction logic for LabelSelect.xaml
    /// </summary>
    public partial class LabelSelect : Window
    {
        protected LabelSelect()
        {
            InitializeComponent();

            LabelList.ItemsSource = DataClient.Label.Get();
        }

        public LabelSelect(List<Indexed> entries)
        {
            InitializeComponent();

            LabelList.ItemsSource = DataClient.Label.Get();

            var list = new List<List<int>>();

            foreach (var entry in entries)
            {
                list.Add(DataClient.Label.GetForEntry(entry.Index));
            }
        }


        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
