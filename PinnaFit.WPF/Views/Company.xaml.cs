using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaFit.WPF.ViewModel;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for CompanyDetail.xaml
    /// </summary>
    public partial class Company : Window
    {
        public Company()
        {
            CompanyViewModel.Errors = 0;
            CompanyViewModel.LineErrors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) CompanyViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) CompanyViewModel.Errors -= 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtCustName.Focus();
        }

        private void Company_OnClosing(object sender, CancelEventArgs e)
        {
            CompanyViewModel.CleanUp();
        }
    }
}
