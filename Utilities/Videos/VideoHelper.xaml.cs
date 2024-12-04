using Microsoft.UI.Xaml;
using System;
using Windows.Media.Core;

namespace EMRMS.Utilities.Videos
{

    public sealed partial class VideoHelper : Window
    {

        public VideoHelper(string uri)
        {
            this.InitializeComponent();
            mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(uri));
            mediaPlayerElement.MediaPlayer.Play();
            this.Closed += (sender, e) =>
            {
                mediaPlayerElement.Source = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            };
        }
    }
}
