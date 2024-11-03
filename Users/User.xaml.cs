using EMRMS.Utilities;
using EMRMS.Utilities.Loader;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS.Users
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class MediaItem
    {
        public string Path { get; set; }
        public string VideoPath { get; set; }
        public string FileType { get; set; }
        public string FileTypeGlyph => FileType == ".mp4" ? "\uE768" : "\uE740"; // Glyphs para video e imagen
    }

    public sealed partial class User : Window
    {
        private ObservableCollection<MediaItem> ImageSources { get; set; } = new ObservableCollection<MediaItem>();

        private int id;
        private const int maxHeight = int.MaxValue, maxWidth = int.MaxValue;
        private const int minHeight = 625, minWidth = 825;
        public User(string id)
        {
            this.InitializeComponent();
            ImageGallery.ItemsSource = ImageSources;

            AppWindow.Resize(new Windows.Graphics.SizeInt32(minWidth, minHeight));
            this.SizeChanged += (sender, e) =>
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32
                    (AppWindow.Size.Width >= minWidth && AppWindow.Size.Width <= maxWidth
                    ? AppWindow.Size.Width : AppWindow.Size.Width < minWidth
                    ? minWidth : maxWidth,

                    AppWindow.Size.Height >= minHeight && AppWindow.Size.Height <= maxHeight
                    ? AppWindow.Size.Height : AppWindow.Size.Height < minHeight
                    ? minHeight : maxHeight)
                    );
            };
            this.id = int.Parse(id);
            var user = SQLCON.ExecuteQuery("SELECT * FROM USERS WHERE UserID = @UserID",
            new Dictionary<string, object> { { "@UserID", this.id } })[0];
            txtNickname.Text = user["name"].ToString() + "\n" + "@"+user["nickname"].ToString();
            try {
                picAvatar.ProfilePicture = new BitmapImage(new Uri(Const.SourceAvatarFolder + user["AVATAR"]));
            }
            catch
            {
                picAvatar.DisplayName = user["name"].ToString();
            }

            this.Title = user["name"].ToString() + " profile";

            PostsOwner.Get(this.id, stackPosts);

            this.Activate();
        }
        private async void Avatar_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            };
            var window = this;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                string[] fileExtension = file.Path.Split('.');
                string newFileName = Guid.NewGuid().ToString() + "." + fileExtension[fileExtension.Length - 1];
                File.Copy(file.Path, Const.SourceAvatarFolder + newFileName);

                picAvatar.ProfilePicture = new BitmapImage(new Uri(Const.SourceAvatarFolder + newFileName));
                SQLCON.ExecuteQuery("UPDATE USERS SET avatar = @Avatar WHERE UserID = @UserID",
                new Dictionary<string, object> { { "@Avatar", newFileName }, { "@UserID", this.id } });
            }
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            
        }

        private async void openMultimediaButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            };
            var window = this;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".mp4");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                string newFileName = Guid.NewGuid().ToString();
                if (file.FileType == ".mp4")
                {
                    Utilities.Videos.VideoThumbnailGen.
                        GenerateThumbnail(file.Path, 
                        Const.SourceDocFolder + "Thumbnail" + newFileName + ".png");

                    File.Copy(file.Path, Const.SourceDocFolder + newFileName + file.FileType);
                    ImageSources.Add(new MediaItem
                    {
                        Path = Directory.GetCurrentDirectory() + @"\Source\Doctype\Thumbnail" + newFileName + ".png",
                        VideoPath = Const.SourceDocFolder + newFileName + file.FileType,
                        FileType = file.FileType
                    });
                }
                else
                {
                    File.Copy(file.Path, Const.SourceDocFolder +newFileName + file.FileType);
                    ImageSources.Add(new MediaItem
                    {
                        Path = Const.SourceDocFolder + newFileName + file.FileType,
                        FileType = file.FileType
                    });
                }



                //SQLCON.ExecuteQuery("UPDATE USERS SET avatar = @Avatar WHERE UserID = @UserID",
                //new Dictionary<string, object> { { "@Avatar", newFileName }, { "@UserID", this.id } });
            }
        }
        private void btnDelete(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var mediaItem = btn.DataContext as MediaItem;

            if (File.Exists(mediaItem.Path))
                File.Delete(mediaItem.Path);
            ImageSources.Remove(mediaItem);
        }
        private void btnCmdSlider(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var mediaItem = btn.DataContext as MediaItem;

            if (mediaItem.FileType == ".mp4")
            {
                Utilities.Videos.VideoHelper videoHelper = new Utilities.Videos.VideoHelper(mediaItem.VideoPath);
                videoHelper.Activate();
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string rtfText;
            post.Document.GetText(TextGetOptions.None, out rtfText);

            SQLCON.ExecuteInsertPost(this.id, rtfText, DateTime.UtcNow, ImageSources.ToList());

            ImageSources.Clear();
            post.Document.SetText(TextSetOptions.None, string.Empty);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Length >= 3)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var user in App.Users)
                {
                    var found = splitText.All(x => { return user.ToLower().Contains(x); });
                    if (found)
                    {
                        suitableItems.Add(user);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No items for you");
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }
    }
}
