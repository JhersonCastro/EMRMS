using EMRMS.Users;
using System;
using System.Collections.Generic;

namespace EMRMS.Utilities.Loader
{
    public enum TypeUser
    {
        Owner,
        Friend,
        NonFriend
    }
    internal class Post
    {
        public static Dictionary<int, (DateTime dateTime, string Body, List<MediaItem> MediaItems)> GetPoster(int UserId, TypeUser typeUser)
        {
            string query = "";
            switch (typeUser)
            {
                case TypeUser.Owner:
                    query = "SELECT * FROM posts WHERE UserId = @UserID ORDER BY POSTDATE DESC";
                    break;
                case TypeUser.Friend:
                    query = "SELECT * FROM posts WHERE UserId = @UserID AND PRIVACITY IN('public','only friends') ORDER BY POSTDATE DESC";
                    break;
                case TypeUser.NonFriend:
                    query = "SELECT * FROM posts WHERE UserId = @UserID AND PRIVACITY = 'public' ORDER BY POSTDATE DESC";
                    break;
                default:
                    break;
            }

            var t = SQLCON.ExecuteQuery(query, new Dictionary<string, object> { { "@UserID", UserId } });

            // Cambia la estructura del diccionario
            Dictionary<int, (DateTime dateTime, string Body, List<MediaItem> MediaItems)> posts =
                new Dictionary<int, (DateTime dateTime, string Body, List<MediaItem> MediaItems)>();

            foreach (var item in t)
            {
                int postID = int.Parse(item["PostID"].ToString());
                DateTime postDate = DateTime.Parse(item["POSTDATE"].ToString());
                string body = item["BODY"].ToString(); // Obtener el cuerpo del post
                List<MediaItem> mediaItems = GetItemPost(postID);

                // Almacenar en el diccionario usando la fecha como clave
                posts[postID] = (dateTime: postDate, Body: body, MediaItems: mediaItems);
            }

            return posts; // Retornar el diccionario de publicaciones.
        }

        private static List<Users.MediaItem> GetItemPost(int postId)
        {
            var t = SQLCON.ExecuteQuery(
                "SELECT fileType, fileUri FROM files WHERE PostID = @PostID",
                new Dictionary<string, object> { { "@PostID", postId } });

            List<Users.MediaItem> mediaItems = new List<Users.MediaItem>();
            foreach (var item in t)
            {
                mediaItems.Add(new Users.MediaItem
                {
                    Path = item["fileUri"].ToString(),
                    FileType = item["fileType"].ToString(),
                    VideoPath = item["fileUri"].ToString()
                });
            }
            return mediaItems;
        }
    }
}
