using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : Window
    {
        public Services()
        {
            ServiceViewModel.Errors = 0;
            InitializeComponent();
        }
    
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ServiceViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ServiceViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtName.Focus();
        }

        private void Services_OnClosing(object sender, CancelEventArgs e)
        {
            ServiceViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtName.Focus();
        }
    }
}
