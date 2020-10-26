using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for MemberAssessments.xaml
    /// </summary>
    public partial class Assessments : Window
    {
        public Assessments()
        {
            AssessmentViewModel.Errors = 0;
            InitializeComponent();
        }
        //public MemberAssessments(MemberAssessmentTypes subscriptionType)
        //{
        //    MemberAssessmentViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<MemberAssessmentTypes>(subscriptionType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AssessmentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AssessmentViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void dtBirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (DtBirthDate.SelectedDate == null) return;
                //int age = DateTime.Now.Subtract(DtBirthDate.SelectedDate.Value).Days;
                //age = (int) (age / 365.25);
                ////try
                ////{
                ////    LblAge.Text = age.ToString().Substring(0, 4);
                ////}
                ////catch
                ////{
                //    LblAge.Text = age.ToString();
                ////}
                ////LblAge.Foreground = Brushes.Black;
            }
            catch
            {

            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtWeight.Focus();
        }

        private void MemberAssessments_OnClosing(object sender, CancelEventArgs e)
        {
            AssessmentViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtWeight.Focus();
        }

        private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }
    }
}
