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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EMRMS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public const int Height = 500, Width = 550;
        public MainWindow()
        {
            this.InitializeComponent();
            AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));
            SizeChanged += (sender, e) =>
            {
               AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));
            };
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Activate();
            this.Close();
        }

        private void btnLgn_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Logica del login para hacer


        }
    }
}
