using EMRMS.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace EMRMS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterWindow : Window
    {
        bool[] _checkers;

        public const int maxHeight = 900, maxWidth = 800;
        public const int minHeight = 500, minWidth = 600;
        public RegisterWindow()
        {
            this.InitializeComponent();
            _checkers = new bool[5];

            this.Closed += (sender, e) =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Activate();
            };
            AppWindow.Resize(new Windows.Graphics.SizeInt32(maxWidth, maxHeight));
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
            TeachingTip.Target = null;

            changelang(App.language);
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
        private void changelang(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            txtTitle.Text = Properties.Lang.Register;
            txtEmail.Header = Properties.Lang.HeaderEmail;
            txtEmail.PlaceholderText = Properties.Lang.PlaceHolderEmail;
            txtName.Header = Properties.Lang.HeaderName;
            txtName.PlaceholderText = Properties.Lang.PlaceHolderName;
            txtPsw.Header = Properties.Lang.HeaderPassword;
            txtPsw.PlaceholderText = Properties.Lang.PlaceHolderPassword;
            calendarBirth.Header = Properties.Lang.HeaderCalendar;
            calendarBirth.PlaceholderText = Properties.Lang.PlaceHolderCalendar;
            infoSet.Message = Properties.Lang.IncorrectRegister;
            TeachingTip.Title = Properties.Lang.TeachingTipTitleRegister;
        }
        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeachingTip.IsOpen = false;
            _checkers[0] = true;
        }
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeachingTip.IsOpen = false;
            TextBox temp = (TextBox)sender;
            if (temp.Text.Length < 5)
            {
                TeachingTip.Subtitle = Properties.Lang.TeachingTipMin5Chars;
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
                TeachingTip.Subtitle = Properties.Lang.TeachingTipReqPassword;
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
            if ((DateTime.UtcNow.Year - calendarBirth.Date.Value.Date.Year) < 18)
            {
                TeachingTip.Subtitle = Properties.Lang.TeachingTipAdultReq;
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
            
            
            DialogWindows dialogWindows = new DialogWindows(
                Properties.Lang.SuccessfulRegTitle,
                Properties.Lang.SuccessfulRegSubTitle,
                this);
            SQLCON.ExecuteInsertUser(txtNickName.Text,
                txtName.Text,
                calendarBirth.Date.Value.Date,
                txtEmail.Text,
                txtPsw.Password);

            TaskCompletionSource<bool> _tcs;
            _tcs = new TaskCompletionSource<bool>();
            dialogWindows.Closed += (sender, args) => _tcs.SetResult(true);
            dialogWindows.Activate();
            await _tcs.Task;

            this.Close();
        }
    }
}
