using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;


namespace EMRMS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterWindow : Window
    {
        bool[] _checkers;
        public const int Height = 885, Width = 600;
        public RegisterWindow()
        {
            this.InitializeComponent();
            _checkers = new bool[5];

            this.Closed += (sender, e) =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Activate();
            };
            AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));
            this.SizeChanged += (sender, e) =>
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));
            };
            TeachingTip.Target = null;
            /*Preload*/

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

        private void txtID_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeachingTip.IsOpen = false;
            try
            {
                long.Parse(txtID.Text);
                if (txtID.Text == string.Empty || txtID.Text.Length < 8)
                {
                    TeachingTip.Subtitle = "No se puede dejar vacio o debe tener más de 8 digitos";
                    TeachingTip.IsOpen = true;
                    badgeID.Visibility = Visibility.Visible;
                }
                else
                {
                    TeachingTip.IsOpen = false;
                    badgeID.Visibility = Visibility.Collapsed;
                    _checkers[0] = true;
                }
            }
            catch (Exception)
            {
                badgeID.Visibility = Visibility.Visible;
                TeachingTip.Subtitle = "¡El ID debe ser un numero!";
                TeachingTip.IsOpen = true;
            }
        }
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeachingTip.IsOpen = false;
            TextBox temp = (TextBox)sender;
            if (temp.Text.Length < 5)
            {
                TeachingTip.Subtitle = "Minimo 5 caracteres";
                badgeName.Visibility = Visibility.Visible;
                TeachingTip.IsOpen = true;
                _checkers[temp.Name.Equals(txtName.Name) ? 1 : 2] = false;
            }
            else
            {
                if (_checkers[1] && _checkers[2])
                    badgeName.Visibility = Visibility.Collapsed;
                 
                TeachingTip.IsOpen = false;
                _checkers[temp.Name.Equals(txtName.Name) ? 1 : 2] = true;
            }
        }
        private void txtPsw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TeachingTip.IsOpen = false;
            
            Regex regex = new Regex("^(?=.*\\d)(?=.*[A-Z])(?=.*[a-z]).{8,}$");

            if (!regex.Match(txtPsw.Password).Success || txtPsw.Password.Length < 8)
            {
                TeachingTip.Subtitle = "La contraseña debe tener al menos una mayuscula, un simbolo y un numero, ademas de 8 caracteres";
                TeachingTip.IsOpen = true;
                badgePsw.Visibility = Visibility.Visible;
            }
            else
            {
                TeachingTip.IsOpen = false;
                badgePsw.Visibility = Visibility.Collapsed;
                _checkers[3] = true;
            }
        }
        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            TeachingTip.IsOpen = false;
            if ((DateTime.Now.Year - calendarBirth.Date.Value.Date.Year) < 18)
            {
                TeachingTip.Subtitle = "Debes tener más de 18 años para poder ingresar";
                TeachingTip.IsOpen = true;
                _checkers[4] = false;
                badgeCalendar.Visibility = Visibility.Visible;
            }
            else
            {
                TeachingTip.IsOpen = false;
                badgeCalendar.Visibility = Visibility.Collapsed;
                _checkers[4] = true;
            }
        }
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _checkers)
            {
                if (!item)
                {
                    infoSet.IsOpen = true;
                    var infoBarVisual = ElementCompositionPreview.GetElementVisual(infoSet);
                    var compositor = infoBarVisual.Compositor;
                    var fadeInAnim = compositor.CreateScalarKeyFrameAnimation();
                    fadeInAnim.InsertKeyFrame(0.0f, 0.0f);
                    fadeInAnim.InsertKeyFrame(0.8f, 1.0f);
                    fadeInAnim.Duration = TimeSpan.FromSeconds(1);
                    infoBarVisual.StartAnimation("Opacity", fadeInAnim);
                    return;   
                }
            }
            infoSet.IsOpen = false;
            DialogWindows dialogWindows = new DialogWindows("Registro correcto", "Se pudo concretar el registro", this);

            TaskCompletionSource<bool> _tcs;
            _tcs = new TaskCompletionSource<bool>();
            dialogWindows.Closed += (sender, args) => _tcs.SetResult(true);
            dialogWindows.Activate();
            await _tcs.Task;
            
            this.Close();
        }
    }
}
