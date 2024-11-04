using EMRMS.Utilities.PostAndComments;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace EMRMS.Utilities.Loader
{
    internal class PostNoOwner
    {


        public static void Get(int userID, int user2Search, StackPanel stack)
        {
            TypeUser typeUser;
            //Find if the user is a friend or not
            string query = "SELECT * FROM friend WHERE " +
                 "((UserID = @UserID AND FriendID = @user2Search)" +
                 "OR (UserID = @user2Search AND FriendID = @UserID))";

            var t = SQLCON.ExecuteQuery(query,
                new Dictionary<string, object> {
                    { "@UserID", userID },
                    { "@user2Search", user2Search }
                });
            if (t.Count == 0)
                typeUser = TypeUser.NonFriend;
            else
                typeUser = TypeUser.Friend;

            var posts = Post.GetPoster(user2Search, typeUser);
            foreach (var post in posts)
            {
                int postId = post.Key;
                var (dateTime, body, mediaItems) = post.Value;

                var postPage = new postTemplate();
                postPage.InitializePost(stack, postId, dateTime, body, mediaItems, typeUser);
                stack.Children.Add((UIElement)postPage);
            }

        }
    }
}
