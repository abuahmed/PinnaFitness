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
    public partial class SellItemEntry : Window
    {
        public SellItemEntry()
        {
            SellItemEntryViewModel.Errors = 0;
            InitializeComponent();
            LstItemsAutoCompleteBox.Focus();
        }
        public SellItemEntry(TransactionTypes type)
        {
            SellItemEntryViewModel.Errors = 0;
            InitializeComponent();
            LstItemsAutoCompleteBox.Focus();
            Messenger.Default.Send<TransactionTypes>(type);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SellItemEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SellItemEntryViewModel.Errors -= 1;
        }

        private void WdwSellItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.Focus();
        }

        private void SellItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            SellItemEntryViewModel.CleanUp();
        }

    }
}
