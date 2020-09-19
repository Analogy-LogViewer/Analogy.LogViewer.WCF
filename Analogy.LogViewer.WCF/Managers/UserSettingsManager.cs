using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Analogy.LogViewer.WCF.Managers
{
    public class  UserSettingsManager
    {
        private static readonly Lazy<UserSettingsManager> _instance =
            new Lazy<UserSettingsManager>(() => new UserSettingsManager());
        public static UserSettingsManager UserSettings { get; set; } = _instance.Value;
        private string EventLogSettingFile { get; } = "AnalogyWCFProvider.Settings";
        public UserSettings Settings { get; set; }

        public UserSettingsManager()
        {
            Settings = new UserSettings();
            if (File.Exists(EventLogSettingFile))
            {
                try
                {
                    string data = File.ReadAllText(EventLogSettingFile);
                    Settings = JsonConvert.DeserializeObject<UserSettings>(data);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogCritical( $"Unable to read file {EventLogSettingFile}: {ex}","");
                }
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(EventLogSettingFile, JsonConvert.SerializeObject(Settings));
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogCritical( $"Unable to save file {EventLogSettingFile}: {ex}","");

            }
        }
    }
}
