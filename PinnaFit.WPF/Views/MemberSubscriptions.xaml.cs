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
    /// Interaction logic for MemberSubscriptions.xaml
    /// </summary>
    public partial class MemberSubscriptions : Window
    {
        public MemberSubscriptions()
        {
            MemberSubscriptionViewModel.Errors = 0;
            InitializeComponent();
        }
        public MemberSubscriptions(MemberDTO member)
        {
            MemberSubscriptionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<MemberDTO>(member);
            Messenger.Reset();
        }
        public MemberSubscriptions(MemberDTO member,bool isRenewal)
        {
            MemberSubscriptionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<bool>(isRenewal);
            Messenger.Default.Send<MemberDTO>(member);
            Messenger.Reset();
        }
        //public MemberSubscriptions(MemberSubscriptionTypes subscriptionType)
        //{
        //    MemberSubscriptionViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<MemberSubscriptionTypes>(subscriptionType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) MemberSubscriptionViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) MemberSubscriptionViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void MemberSubscriptions_OnClosing(object sender, CancelEventArgs e)
        {
            MemberSubscriptionViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }
    }
}
