using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KoikatsuVoiceManager
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DataManager.BasePath = KoikatsuVoiceManager.Properties.Settings.Default.KoikatsuPath;
            try
            {
                DataManager.Instance.ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KoikatsuVoiceManager.Properties.Settings.Default.KoikatsuPath = DataManager.BasePath;
            KoikatsuVoiceManager.Properties.Settings.Default.Save();
        }
    }
}
