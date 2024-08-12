using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
                MaxWidth = (int)((int)txtTitle.Text.Length * txtTitle.FontSize + 50);              
            else
                MaxWidth = (int)((int)txtSubtitle.Text.Length * txtSubtitle.FontSize + 50);

            MaxHeight = (int)(txtTitle.FontSize + txtSubtitle.FontSize + btnCheck.FontSize + 150);

            AppWindow.Resize(new SizeInt32(MaxWidth, MaxHeight));
            win = previousWindows;
            win.AppWindow.Hide();
        }

        private void DialogWindows_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            AppWindow.Resize(new SizeInt32(MaxWidth, MaxHeight));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            win.AppWindow.Show();
            this.Close();
        }
    }
}
