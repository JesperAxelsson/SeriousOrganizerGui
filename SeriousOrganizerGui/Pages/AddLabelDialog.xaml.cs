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
    /// Interaction logic for AddLabelDialog.xaml
    /// </summary>
    public partial class AddLabelDialog : Window
    {
        public AddLabelDialog()
        {
            InitializeComponent();
        }

        private void BtnAddLabel_Click(object sender, RoutedEventArgs e)
        {
            var name = txt_newlabel.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            DataClient.Label.Add(name);
            this.Close();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
