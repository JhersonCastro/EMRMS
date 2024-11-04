using EMRMS.Utilities;
using EMRMS.Utilities.Loader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

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
            string query = "SELECT name,nickname, avatar FROM users WHERE UserID = @UserID";
            var T = SQLCON.ExecuteQuery(query,
                new Dictionary<string, object> { { "@UserID", idUser2Search } });
            txtNickname.Text = T[0]["name"].ToString() + "\n@" + T[0]["nickname"].ToString();
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
