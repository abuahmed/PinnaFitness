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
    public partial class TimeTables : Window
    {
        public TimeTables()
        {
            AssessmentViewModel.Errors = 0;
            InitializeComponent();
        }
        //public TimeTables(MemberAssessmentTypes subscriptionType)
        //{
        //    MemberAssessmentViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<MemberAssessmentTypes>(subscriptionType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) TimeTableViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) TimeTableViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtWeight.Focus();
        }

        private void TimeTables_OnClosing(object sender, CancelEventArgs e)
        {
            TimeTableViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtWeight.Focus();
        }
    }
}
