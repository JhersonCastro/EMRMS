using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.PointOfService;
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
        public const int minHeight = 500, minWidth = 550;
        public const int maxHeight = 800, maxWidth = 600;
        public MainWindow()
        {
            this.InitializeComponent();
            Users.SampleUser sampleUser = new Users.SampleUser();
            sampleUser.Activate();
            AppWindow.Resize(new Windows.Graphics.SizeInt32(minWidth, minHeight));
            this.SizeChanged += (sender, e) =>
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32
                    (AppWindow.Size.Width >= minWidth && AppWindow.Size.Width <= maxWidth
                    ? AppWindow.Size.Width : AppWindow.Size.Width < minWidth
                    ? minWidth : maxWidth,

                    AppWindow.Size.Height >= minHeight && AppWindow.Size.Height <= maxHeight
                    ? AppWindow.Size.Height : AppWindow.Size.Height < minHeight
                    ? minHeight : maxHeight)
                    );
            };
            changeLang("en");
            #region Preload Animations (idk why i need do this for this shit working)
            var infoBarVisual = ElementCompositionPreview.GetElementVisual(infoSet);
            var compositor = infoBarVisual.Compositor;
            var fadeInAnim = compositor.CreateScalarKeyFrameAnimation();
            fadeInAnim.InsertKeyFrame(0.0f, 0.0f);
            fadeInAnim.InsertKeyFrame(0.8f, 1.0f);
            fadeInAnim.Duration = TimeSpan.FromSeconds(1);
            infoBarVisual.StartAnimation("Opacity", fadeInAnim);
            #endregion
        }
        public void changeLang(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            txtLogInUp.Text = Properties.Lang.Login;
            
            txtIDLogin.Header 
                = Properties.Lang.HeaderID;
            txtIDLogin.PlaceholderText
                = Properties.Lang.PlaceHolderID;

            pswBox.Header =
                Properties.Lang.HeaderPassword;
            pswBox.PlaceholderText =
                Properties.Lang.PlaceHolderPassword;

            btnLgn.Content =
                Properties.Lang.Login;
            btnRegister.Content =
                Properties.Lang.Register;

            infoSet.Message = Properties.Lang.WrongCredentials;
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
            if (false)
            {

            }
            else
            {
                infoSet.IsOpen = true;
                var infoBarVisual = ElementCompositionPreview.GetElementVisual(infoSet);
                var compositor = infoBarVisual.Compositor;
                var fadeInAnim = compositor.CreateScalarKeyFrameAnimation();
                fadeInAnim.InsertKeyFrame(0.0f, 0.0f);
                fadeInAnim.InsertKeyFrame(0.8f, 1.0f);
                fadeInAnim.Duration = TimeSpan.FromSeconds(1);
                infoBarVisual.StartAnimation("Opacity", fadeInAnim);
            }

        }
    }
}
