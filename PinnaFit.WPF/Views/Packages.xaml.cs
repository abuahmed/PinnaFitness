using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for FacilitySubscriptions.xaml
    /// </summary>
    public partial class Packages : Window
    {
        public Packages()
        {
            PackagesViewModel.Errors = 0;
            InitializeComponent();
        }
        //public FacilitySubscriptions(FacilitySubscriptionTypes subscriptionType)
        //{
        //    FacilitySubscriptionViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<FacilitySubscriptionTypes>(subscriptionType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) PackagesViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) PackagesViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void FacilitySubscriptions_OnClosing(object sender, CancelEventArgs e)
        {
            PackagesViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }
    }
}
