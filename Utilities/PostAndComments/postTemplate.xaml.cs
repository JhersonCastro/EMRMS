using ABI.System;
using EMRMS.Users;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS.Utilities.PostAndComments
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class Item
    {
        public string   Path { get; set; }
        public string   VideoPath { get; set; }
        public Visibility visibilityBtnFullScreenVideo { get; set; }
        public Visibility visibilityBtnFullScreenImage { get; set; }
        public string FileType { get; set; }
        public string FileTypeGlyph => FileType == ".mp4" ? "\uE768" : "\uE740"; // Glyphs para video e imagen
    }
    public sealed partial class postTemplate : Page
    {
        private ObservableCollection<Item> ImageGalleryPost { get; set; } = new ObservableCollection<Item>();
        private int idPost;
        public postTemplate()
        {
            this.InitializeComponent();
        }

        public void InitializePost(int idPost, DateTime dateTime, string Body, List<MediaItem> MediaItems)
        {
            this.idPost = idPost;
            txtBody.Text = Body;
            txtDateTime.Text = dateTime.ToString();

            foreach (var item in MediaItems)
            {
                gotoFile(item.Path, item.FileType);
            }
        }
        private void gotoFile(string path, string filetype)
        {
            if (path == "")
                return;
            
            if (filetype == ".mp4")
            {
                string[] s = path.Split(@"\");
                string n = "";
                for (global::System.Int32 i = 0; i < s.Length-1; i++)
                    n += s[i] + @"\";
                
                n += "Thumbnail" + s[s.Length - 1].Substring(0, s[s.Length-1].IndexOf(".mp4")) + ".png";

                ImageGalleryPost.Add(new Item
                {
                    VideoPath = path,
                    Path = n,
                    FileType = filetype
                });
               
            }
            else
            {
                ImageGalleryPost.Add(new Item
                {
                    Path = path,
                    FileType = filetype,
                });
            }

        }


        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dynamicWrapGrid = (VariableSizedWrapGrid)FindName("DynamicWrapGrid");

            if (dynamicWrapGrid != null)
            {
                double imageWidth = 100;

                int columns = (int)(e.NewSize.Width / imageWidth);

                if (columns > 0)
                {
                    dynamicWrapGrid.MaximumRowsOrColumns = columns;
                }
            }
        }


        private void btnFullScreenOrFtp_Click(object sender, RoutedEventArgs e)
        {
            var s = idPost;
            var btn = sender as Button;
            var item = btn.DataContext as Item;
            if (item != null) {
                if (item.FileType == ".mp4")
                {
                    Utilities.Videos.VideoHelper videoHelper = new Utilities.Videos.VideoHelper(item.VideoPath);
                    videoHelper.Activate();
                }
            }
        }
    }
}
