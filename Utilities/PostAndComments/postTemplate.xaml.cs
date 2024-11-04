using EMRMS.Users;
using EMRMS.Utilities.Loader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS.Utilities.PostAndComments
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class Item
    {
        public string Path { get; set; }
        public string VideoPath { get; set; }
        public string FileType { get; set; }
        public string FileTypeGlyph => FileType == "video" ? "\uE768" : "\uE740"; // Glyphs para video e imagen

        public Visibility visibilityBtnFullScreenVideo => FileType == "video" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility visibilityBtnFullScreenImage => FileType == "image" ? Visibility.Visible : Visibility.Collapsed;
    }
    public sealed partial class postTemplate : Page
    {
        private ObservableCollection<Item> ImageGalleryPost { get; set; } = new ObservableCollection<Item>();
        private int idPost;
        private StackPanel fatherPanel;
        public postTemplate()
        {
            this.InitializeComponent();
        }

        public void InitializePost(StackPanel fatherPanel, int idPost, DateTime dateTime, string Body, List<MediaItem> MediaItems, TypeUser typeUser)
        {
            if (typeUser != TypeUser.Owner)
            {
                btnEdit.IsEnabled = false;
                btnPostDelete.IsEnabled = false;
                btnEdit.Visibility = Visibility.Collapsed;
                btnPostDelete.Visibility = Visibility.Collapsed;
            }
            this.idPost = idPost;
            this.fatherPanel = fatherPanel;
            txtBody.Text = Body;
            txtDateTime.Text = dateTime.ToString();

            foreach (var item in MediaItems)
            {
                gotoFile(item);
            }
        }
        private void gotoFile(MediaItem item)
        {
            if (item.Path == "")
                return;

            if (item.FileType == "video")
            {
                string[] s = item.Path.Split(@"/");
                string n = "";
                for (global::System.Int32 i = 0; i < s.Length - 1; i++)
                    n += s[i] + @"/";

                if (s[s.Length - 1].Contains(".mp4"))
                {
                    n += "Thumbnail" + s[s.Length - 1].Substring(0, s[s.Length - 1].IndexOf(".mp4")) + ".png";
                }
                else if (s[s.Length - 1].Contains(".png"))
                {
                    n += s[s.Length - 1];
                }
                ImageGalleryPost.Add(new Item
                {
                    VideoPath = item.VideoPath,
                    Path = n,
                    FileType = item.FileType,
                });

            }
            else
            {
                ImageGalleryPost.Add(new Item
                {
                    Path = item.Path,
                    FileType = item.FileType,
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
            if (item != null)
            {
                if (item.FileType == "video")
                {
                    Utilities.Videos.VideoHelper videoHelper = new Utilities.Videos.VideoHelper(item.VideoPath);
                    videoHelper.Activate();
                }
                else
                {
                    Utilities.Images.ImageHelper imageHelper = new Utilities.Images.ImageHelper(item.Path);
                    imageHelper.Activate();
                }
            }
        }

        private async void btnPostDelete_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Eliminar Post",
                Content = "¿Estás seguro de que deseas eliminar este post?",
                PrimaryButtonText = "Eliminar",
                CloseButtonText = "Cancelar"
            };

            contentDialog.XamlRoot = btnPostDelete.XamlRoot;
            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SQLCON.ExecuteQuery("DELETE FROM posts WHERE PostID = @PostID",
                    new Dictionary<string, object> { { "@PostID", idPost } });
                fatherPanel.Children.Remove(this);

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //EditWin editWin = new EditWin();
            //editWin.InitializeEditWin(idPost, txtBody.Text, ImageGalleryPost);
            //editWin.Activate();
            string query = "Update posts set Body = @Body where PostID = @PostID";
        }
    }
}
