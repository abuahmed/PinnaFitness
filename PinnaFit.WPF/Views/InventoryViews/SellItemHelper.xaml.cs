using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for SellItem.xaml
    /// </summary>
    public partial class SellItemHelper : Window
    {
        public SellItemHelper()
        {
            SellItemHelperViewModel.Errors = 0;
            InitializeComponent();
            LstItemsAutoCompleteBox.Focus();
        }
      
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SellItemHelperViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SellItemHelperViewModel.Errors -= 1;
        }

        private void WdwSellItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.Focus();
        }

        private void SellItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            SellItemHelperViewModel.CleanUp();
        }

    }
}
