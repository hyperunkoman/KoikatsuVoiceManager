using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace KoikatsuVoiceManager
{
    /// <summary>
    /// VoiceListControl.xaml の相互作用ロジック
    /// </summary>
    public partial class VoiceListControl : UserControl
    {
        ICollectionView m_ViewSource;

        Int32 m_filterByID;
        bool m_filterUnref;

        private void RefreshDataSource()
        {
            m_ViewSource = CollectionViewSource.GetDefaultView(DataManager.Instance.VoiceUsages);
            if(m_ViewSource != null) m_ViewSource.Filter = List_Filter;
            //m_ViewSource.View.Refresh();
            ui_VoiceListDG.ItemsSource = m_ViewSource;
        }

        public VoiceListControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            m_filterByID = -1;
            m_filterUnref = false;
            RefreshDataSource();
        }

        private bool List_Filter(object obj)
        {
            if (m_filterByID < 0 && !m_filterUnref) return true;
            var data = (KeyValuePair<string, VoiceUsage>)obj;
            if (m_filterUnref && data.Value.ReferenceCount > 0) return false;
            if (m_filterByID >= 0 && data.Value.CharacterID != m_filterByID) return false;
            return true;
        }

        public void Refresh()
        {
            m_ViewSource.Refresh();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            var data = (KeyValuePair<string, VoiceUsage>)((sender as Button).Tag);
            DataManager.PlayAudioClip(data.Value.ArcPath, data.Value.PathID);
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            m_filterByID = -1;
            if (ui_FilterID.Text.Trim().Length != 0) Int32.TryParse(ui_FilterID.Text, out m_filterByID);
            m_filterUnref = ui_FilterUnref.IsChecked.HasValue && ui_FilterUnref.IsChecked.Value;
            ui_VoiceListDG.IsEnabled = false;
            ui_VoiceListDG.Visibility = Visibility.Hidden;
            m_ViewSource.Refresh();
            ui_VoiceListDG.Visibility = Visibility.Visible;
            ui_VoiceListDG.IsEnabled = true;
        }

        private void ui_VoiceListDG_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var data = (KeyValuePair<string, VoiceUsage>)e.Item;
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, (sender as DataGrid).Columns[0], data.Value.ToString()));
        }
    }
}
