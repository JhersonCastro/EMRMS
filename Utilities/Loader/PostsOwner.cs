using System;
using System.Collections.Generic;
using System.Linq;
using EMRMS.Utilities;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using EMRMS.Users;
using Microsoft.UI.Xaml.Controls;
using EMRMS.Utilities.PostAndComments;

namespace EMRMS.Utilities.Loader
{
    class PostsOwner
    {
       public static void Get(int UserId, StackPanel stack)
        {
            var posts = GetPoster(UserId);
            foreach (var post in posts)
            {
                int postId = post.Key;
                var (dateTime, body, mediaItems) = post.Value;

                // Crea una instancia de la página con los parámetros necesarios
                var postPage = new postTemplate();
                postPage.InitializePost(postId, dateTime, body, mediaItems);
                // Agrega el contenido de la página al StackPanel
                stack.Children.Add((UIElement)postPage);
            }
        }

        private static Dictionary<int, (DateTime dateTime,string Body, List<MediaItem> MediaItems)> GetPoster(int UserId)
        {
            string query = "SELECT * FROM posts WHERE UserId = @UserID ORDER BY POSTDATE DESC";
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
                posts[postID] = (dateTime:postDate,Body: body, MediaItems: mediaItems);
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
                    Path = Const.SourceDocFolder + item["fileUri"],
                    FileType = item["fileType"].ToString(),
                    VideoPath = Const.SourceDocFolder + item["fileUri"]
                });
            }
            return mediaItems;
        }

    }
}
