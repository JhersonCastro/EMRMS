using EMRMS.Utilities;
using EMRMS.Utilities.Loader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS.Users
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class nonOwner : Window
    {
        public nonOwner(int idUser, int idUser2Search)
        {
            this.InitializeComponent();
            string query = "SELECT name,nickname, avatar FROM Users WHERE UserID = @UserID";
            var T = SQLCON.ExecuteQuery(query, 
                new Dictionary<string, object> { { "@UserID", idUser2Search } });
            txtNickname.Text = T[0]["name"].ToString()+"\n@"+T[0]["nickname"].ToString();
            if (T[0]["avatar"].ToString() == "default.png")
            {
                picAvatar.ProfilePicture = null;
                picAvatar.DisplayName = T[0]["name"].ToString();
            }
            else
            {
                picAvatar.DisplayName = null;
                picAvatar.ProfilePicture =
                    new BitmapImage(new Uri
                    (Const.SourceAvatarFolder + T[0]["avatar"].ToString()));
                PostNoOwner.Get(idUser, idUser2Search, stackPosts);
            }
        }
    }
}
