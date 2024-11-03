using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DialogWindows : Window
    {
        private int MaxWidth = 600;
        private int MaxHeight = 300;
        private Window win;
        public DialogWindows(string title, string subtitle, Window previousWindows)
        {
            this.InitializeComponent();
            txtTitle.Text = title;
            txtSubtitle.Text = subtitle;

            if (txtTitle.Text.Length * txtTitle.FontSize > txtSubtitle.Text.Length * txtSubtitle.FontSize)
                MaxWidth = (int)(txtTitle.Text.Length * txtTitle.FontSize + 50);
            else
                MaxWidth = (int)(txtSubtitle.Text.Length * txtSubtitle.FontSize + 50);

            MaxHeight = (int)(txtTitle.FontSize + txtSubtitle.FontSize + btnCheck.FontSize + 150);

            AppWindow.Resize(new SizeInt32(MaxWidth, MaxHeight));
            if (previousWindows != null)
            {
                win = previousWindows;
                win.AppWindow.Hide();
            }
            this.Activate();
        }

        private void DialogWindows_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            AppWindow.Resize(new SizeInt32(MaxWidth, MaxHeight));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (win != null)
                win.AppWindow.Show();
            this.Close();
        }
    }
}
