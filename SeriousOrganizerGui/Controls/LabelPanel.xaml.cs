using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for LabelPanel.xaml
    /// </summary>
    public partial class LabelPanel : UserControl
    {

        public event EventHandler? StateChanged;

        private Lens<Dto.Label, TriStateToggle> _labelLens;

        public LabelPanel()
        {
            InitializeComponent();

            _labelLens = new Lens<Dto.Label, TriStateToggle>(DataClient.Label.Get(), TriStateToggle.Create);
            label_list.ItemsSource = _labelLens.GetSink;
        }

        private void Label_list_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem? item = sender as ListViewItem;
            if (item == null) return;

            var toggler = item.Content as TriStateToggle;

            switch (toggler!.State)
            {
                case TriState.Neutral:
                    toggler.State = TriState.Selected;
                    break;
                case TriState.Selected:
                case TriState.UnSelected:
                    toggler.State = TriState.Neutral;
                    break;
            }

            DataClient.Label.FilterLabel(toggler.Id, (byte)toggler.State);
            LabelStateChanged();
        }

        private void LabelStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged(this, EventArgs.Empty);
            }
        }

        private void Label_list_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem? item = sender as ListViewItem;
            if (item != null)
            {
                var toggler = item.Content as TriStateToggle;

                switch (toggler!.State)
                {
                    case TriState.Neutral:
                        toggler.State = TriState.UnSelected;
                        break;
                    case TriState.Selected:
                    case TriState.UnSelected:
                        toggler.State = TriState.Neutral;
                        break;
                }

                DataClient.Label.FilterLabel(toggler.Id, (byte)toggler.State);
                LabelStateChanged();
            }
        }

        private void remove_label_Button_Click(object sender, RoutedEventArgs e)
        {
            var lbl = label_list.SelectedItem as TriStateToggle;

            if (lbl != null && MessageBox.Show(Window.GetWindow(this), "Are you sure you want to remove label: " + lbl.Name, "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DataClient.Label.Remove(lbl.Id);
                LabelStateChanged();
            }
        }

        private void add_label_Button_Click(object sender, RoutedEventArgs e)
        {
            var select = new AddLabelDialog();
            select.ShowInTaskbar = false;
            select.Owner = Window.GetWindow(this);
            select.ShowDialog();
        }


    }
}
