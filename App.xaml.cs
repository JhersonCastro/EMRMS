using EMRMS.Utilities;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        //If config.json doesn't exist or it's broken, the default lang is english
        public static string language = "us";
        public static string sqlConn = "Server=sql.freedb.tech;Port=3306;Database=freedb_SocialNetwork;Uid=freedb_BucketSocial;Pwd=p@fy&sgb&Bg2Q9q";
        public static Dictionary<int, String> Users = new Dictionary<int, String>();

        public App()
        {
            this.InitializeComponent();

            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(projectDirectory);

            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Source\Avatars");
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Source\Doctype");


            language = ConfigurationManager.AppSettings["Language"];

            var sql = SQLCON.ExecuteQuery("SELECT UserID, nickname FROM users LIMIT 25;");
            sql.ForEach(x => Users.Add(int.Parse(x["UserID"].ToString()), x["nickname"].ToString()));
        }


        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        public Window m_window;

        public void updateLanguage(string lang)
        {
            AddUpdateAppSettings("Language", lang);
            language = ConfigurationManager.AppSettings["Language"];
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                    settings.Add(key, value);
                else
                    settings[key].Value = value;

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
