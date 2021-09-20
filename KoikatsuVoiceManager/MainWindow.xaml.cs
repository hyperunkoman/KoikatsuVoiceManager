using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using UnityPlugin;
using System.Diagnostics;

namespace KoikatsuVoiceManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, VoiceUsage> VoiceUsages => DataManager.Instance.VoiceUsages;

        public MainWindow()
        {
            InitializeComponent();
            GC.Collect();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            DataManager.Instance.ReloadAll();
        }
    }
}
