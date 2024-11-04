using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Media.Core;

namespace EMRMS.Utilities.Videos
{
    public sealed partial class VideoHelper : Window
    {
        public VideoHelper(string uri)
        {
            this.InitializeComponent();
            webViewer.Source = new Uri(uri);

            this.Closed += (sender, e) =>
            {
                webViewer.Source = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            };
        }

        private void webViewer_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.IsWebMessageEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            sender.CoreWebView2.Settings.AreDevToolsEnabled = false;
        }
    }
}
