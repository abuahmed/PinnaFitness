using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for SellItem.xaml
    /// </summary>
    public partial class Duration : Window
    {
        public Duration()
        {
            DurationViewModel.Errors = 0;
            InitializeComponent();
            
        }
        public Duration(ReportTypes reportType)
        {
            DurationViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ReportTypes>(reportType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DurationViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DurationViewModel.Errors -= 1;
        }

        private void WdwSellItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void SellItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            
        }

    }
}
