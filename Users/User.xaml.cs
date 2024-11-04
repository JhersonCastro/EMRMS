using EMRMS.Utilities;
using EMRMS.Utilities.Loader;
using EMRMS.Utilities.PostAndComments;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EMRMS.Users
{
    public class MediaItem
    {
        public string Path { get; set; }
        public string VideoPath { get; set; }
        public string FileType { get; set; }
        public string FileTypeGlyph => FileType == "video" ? "\uE768" : "\uE740"; // Glyphs para video e imagen
        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
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
            var user = SQLCON.ExecuteQuery("SELECT * FROM users WHERE UserID = @UserID",
            new Dictionary<string, object> { { "@UserID", this.id } })[0];
            txtNickname.Text = user["name"].ToString() + "\n" + "@" + user["nickname"].ToString();
            try
            {
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

                await Utilities.Web.UploadFiles.UploadFileAsync(file.Path, "Source/Avatars/", newFileName);

                picAvatar.ProfilePicture = new BitmapImage(new Uri(Const.SourceAvatarFolder + newFileName));
                SQLCON.ExecuteQuery("UPDATE users SET avatar = @Avatar WHERE UserID = @UserID",
                new Dictionary<string, object> { { "@Avatar", newFileName }, { "@UserID", this.id } });
            }
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
                var mediaItem = new MediaItem();

                sendButton.IsEnabled = false;
                if (file.FileType == ".mp4")
                {
                    Utilities.Videos.VideoThumbnailGen.GenerateThumbnail(file.Path,
                        Const.TempFolder + "Thumbnail" + newFileName + ".png");

                    await Utilities.Web.UploadFiles.UploadFileAsync(Const.TempFolder + "Thumbnail" + newFileName + ".png", "Source/Doctype", "Thumbnail" + newFileName + ".png");
                    await Utilities.Web.UploadFiles.UploadFileAsync(file.Path, "Source/Doctype", newFileName + file.FileType);

                    mediaItem.Path = Const.SourceImageFolder + "Thumbnail" + newFileName + ".png";
                    mediaItem.VideoPath = Const.SourceVideoFolder + newFileName + file.FileType;
                    mediaItem.FileType = "video";
                }
                else
                {
                    // Subir el archivo de imagen
                    await Utilities.Web.UploadFiles.UploadFileAsync(file.Path, "Source/Doctype", newFileName + file.FileType);

                    mediaItem.Path = Const.SourceImageFolder + newFileName + file.FileType;
                    mediaItem.FileType = "image";
                }

                sendButton.IsEnabled = true;
                ImageSources.Add(mediaItem);
            }
        }
        private async void btnDelete(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var mediaItem = btn.DataContext as MediaItem;

            try
            {
                await Task.Delay(1000, mediaItem.CancellationTokenSource.Token);

                ImageSources.Remove(mediaItem);
            }
            catch
            {

            }
        }

        private void btnCmdSlider(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var mediaItem = btn.DataContext as MediaItem;

            if (mediaItem.FileType == "video")
            {
                Utilities.Videos.VideoHelper videoHelper = new Utilities.Videos.VideoHelper(mediaItem.VideoPath);
                videoHelper.Activate();
            }
        }
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string rtfText;
            post.Document.GetText(TextGetOptions.None, out rtfText);

            DateTime dateTime = DateTime.UtcNow;
            var files = ImageSources.ToList();

            int lastId = SQLCON.ExecuteInsertPost(this.id, rtfText, dateTime, files);


            var postPage = new postTemplate();
            postPage.InitializePost(stackPosts, lastId, dateTime, rtfText, files, TypeUser.Owner);
            stackPosts.Children.Insert(0, postPage);


            ImageSources.Clear();
            post.Document.SetText(TextSetOptions.None, string.Empty);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Length >= 3)
            {
                var suitableItems = new List<string>();
                var userDictionary = new Dictionary<string, int>();
                var splitText = sender.Text.ToLower().Split(" ");

                foreach (var user in App.Users)
                {
                    var found = splitText.All(x => user.Value.ToLower().Contains(x));
                    if (found)
                    {
                        suitableItems.Add(user.Value);
                        userDictionary[user.Value] = user.Key;
                    }
                }

                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No items for you");
                }

                sender.ItemsSource = suitableItems;
                sender.Tag = userDictionary;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //if (id == user2Search)
            //{
            //    ContentDialog contentDialog = new ContentDialog
            //    {
            //        Title = "Error",
            //        Content = "You can't search yourself",
            //        CloseButtonText = "Ok"
            //    };
            //    contentDialog.XamlRoot = AutoSuggestBox.XamlRoot;
            //    _ = contentDialog.ShowAsync();
            //    return;
            //}
            if (user2Search == -1)
            {
                var nickname = sender.Text;
                var t = SQLCON.ExecuteQuery("SELECT UserID FROM users WHERE nickname = @USERID; ",
                    new Dictionary<string, object> { { "@USERID", nickname } });
                if (t.Count == 0)
                {
                    ContentDialog contentDialog = new ContentDialog
                    {
                        Title = "User not found",
                        Content = "The user you are looking for doesn't exists",
                        CloseButtonText = "Ok"
                    };
                    contentDialog.XamlRoot = AutoSuggestBox.XamlRoot;
                    _ = contentDialog.ShowAsync();
                    return;
                }
                user2Search = int.Parse(t[0]["UserID"].ToString());
                //if (id == user2Search)
                //{
                //    ContentDialog contentDialog = new ContentDialog
                //    {
                //        Title = "Error",
                //        Content = "You can't search yourself",
                //        CloseButtonText = "Ok"
                //    };
                //    contentDialog.XamlRoot = AutoSuggestBox.XamlRoot;
                //    _ = contentDialog.ShowAsync();
                //    return;
                //}
            }
            nonOwner nonOwner = new nonOwner(this.id, user2Search);
            nonOwner.Activate();
            user2Search = -1;
        }
        private int user2Search = -1;
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selectedUserName = args.SelectedItem.ToString();
            var userDictionary = (Dictionary<string, int>)sender.Tag;

            if (userDictionary.TryGetValue(selectedUserName, out int userId))
            {
                user2Search = userId;
            }
        }
    }
}
