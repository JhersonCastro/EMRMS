using EMRMS.Utilities.PostAndComments;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EMRMS.Utilities.Loader
{
    class PostsOwner
    {
        public static void Get(int UserId, StackPanel stack)
        {
            var posts = Post.GetPoster(UserId, TypeUser.Owner);
            foreach (var post in posts)
            {
                int postId = post.Key;
                var (dateTime, body, mediaItems) = post.Value;

                // Crea una instancia de la página con los parámetros necesarios
                var postPage = new postTemplate();
                postPage.InitializePost(stack, postId, dateTime, body, mediaItems, TypeUser.Owner);
                // Agrega el contenido de la página al StackPanel
                stack.Children.Add((UIElement)postPage);
            }
        }
    }
}
