using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS.Users
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class User : Window
    {
        public User()
        {
            this.InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            };
            var window = this;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add(".jpg");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
                picAvatar.ProfilePicture = new BitmapImage(new Uri(file.Path));
        }
        private List<string> Users = new List<string>()
        {
        "Abyssinian",
        "Aegean",
        "American Bobtail",
    "Bengal",
    "Birman",
    "British Shorthair",
    "Burmese",
    "Chartreux",
    "Cornish Rex",
    "Devon Rex",
    "Egyptian Mau",
    "Exotic Shorthair",
    "Havana Brown",
    "Japanese Bobtail",
    "Korat",
    "Maine Coon",
    "Manx",
    "Norwegian Forest",
    "Ocicat",
    "Oriental",
    "Persian",
    "Pixiebob",
    "Ragdoll",
    "Russian Blue",
    "Scottish Fold",
    "Siamese",
    "Siberian",
    "Singapura",
    "Somali",
    "Sphynx",
    "Tonkinese",
    "Turkish Angora",
    "Turkish Van",
    // ... (otros nombres de usuario)
    "Ziggy"
};


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Length >= 3)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var user in Users)
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
