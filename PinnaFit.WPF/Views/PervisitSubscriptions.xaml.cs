using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for PervisitSubscriptions.xaml
    /// </summary>
    public partial class PervisitSubscriptions : Window
    {
        public PervisitSubscriptions()
        {
            PervisitSubscriptionViewModel.Errors = 0;
            InitializeComponent();
        }
        public PervisitSubscriptions(MemberDTO pervisit)
        {
            PervisitSubscriptionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<MemberDTO>(pervisit);
            Messenger.Reset();
        }
        //public PervisitSubscriptions(PervisitSubscriptionTypes subscriptionType)
        //{
        //    PervisitSubscriptionViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<PervisitSubscriptionTypes>(subscriptionType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) PervisitSubscriptionViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) PervisitSubscriptionViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void PervisitSubscriptions_OnClosing(object sender, CancelEventArgs e)
        {
            PervisitSubscriptionViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }
    }
}
