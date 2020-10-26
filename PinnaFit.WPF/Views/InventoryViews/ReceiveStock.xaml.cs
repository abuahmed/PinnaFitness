using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for StockReceives.xaml
    /// </summary>
    public partial class ReceiveStock : Window
    {
        public ReceiveStock()
        {
            ReceiveStockViewModel.Errors = 0;
            InitializeComponent();
        }
        public ReceiveStock(TransactionTypes type)
        {
            ReceiveStockViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<TransactionTypes>(type);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ReceiveStockViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ReceiveStockViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtWeight.Focus();
        }

        private void StockReceives_OnClosing(object sender, CancelEventArgs e)
        {
            ReceiveStockViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtWeight.Focus();
        }

        private void FocusControls()
        {
            LstItemsAutoCompleteBox.Focus();
        }
    }
}
