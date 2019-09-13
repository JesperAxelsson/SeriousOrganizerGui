using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SeriousOrganizerGui.Extensions
{
    public static class ListViewExtensions
    {
        public static IEnumerable<T> FindSelectedItems<T>(this ListView listView)
            where T : class
        {
            if (listView.SelectedItems.Count > 0)
            {
                return listView.SelectedItems.Cast<T>();
            }

            return Enumerable.Empty<T>();
        }
    }
}
