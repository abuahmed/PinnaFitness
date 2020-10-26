using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Subscriptions.xaml
    /// </summary>
    public partial class Subscriptions : Window
    {
        public Subscriptions()
        {
            SubscriptionViewModel.Errors = 0;
            InitializeComponent();
        }
        public Subscriptions(SubscriptionTypes subscriptionType)
        {
            SubscriptionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<SubscriptionTypes>(subscriptionType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SubscriptionViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SubscriptionViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtName.Focus();
        }

        private void Subscriptions_OnClosing(object sender, CancelEventArgs e)
        {
            SubscriptionViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtName.Focus();
        }
    }
}
