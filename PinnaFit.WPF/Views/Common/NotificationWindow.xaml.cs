using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow()
        {
            InitializeComponent();
            //this.Closed += this.NotificationWindowClosed;
            //Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            //{
            //    var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            //    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            //    var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

            //    this.Left = corner.X - this.ActualWidth - 100;
            //    this.Top = corner.Y - this.ActualHeight;
            //}));
        }

        private void BtnNotify_OnClick(object sender, RoutedEventArgs e)
        {
            new ItemEntry().ShowDialog();
        }

        private void NotificationWindow_OnClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NotificationWindowClosed(object sender, EventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                string windowName = window.GetType().Name;

                if (windowName.Equals("NotificationWindow") && window != this)
                {
                    // Adjust any windows that were above this one to drop down
                    if (window.Top < this.Top)
                    {
                        window.Top = window.Top + this.ActualHeight;
                    }
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
        }


        private void btnShowCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            FancyBalloon balloon = new FancyBalloon();
            balloon.BalloonText = "Custom Balloon";

            //show balloon and close it after 4 seconds
            MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
        }

        private void btnHideStandardBalloon_Click(object sender, RoutedEventArgs e)
        {
            MyNotifyIcon.HideBalloonTip();
        }


        private void btnShowStandardBalloon_Click(object sender, RoutedEventArgs e)
        {
            string title = "WPF NotifyIcon";
            string text = "This is a standard balloon";

            MyNotifyIcon.ShowBalloonTip(title, text, BalloonIcon.Info);
            MyNotifyIcon.HideBalloonTip();
        }

        private void btnCloseCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            MyNotifyIcon.CloseBalloon();
        }
    }
}
