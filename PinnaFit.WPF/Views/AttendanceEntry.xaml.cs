using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for MemberAttendances.xaml
    /// </summary>
    public partial class AttendanceEntry : Window
    {
        public AttendanceEntry()
        {
            AttendanceEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public AttendanceEntry(int attendanceId)
        {
            AttendanceEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<int>(attendanceId);
            Messenger.Reset();
        }
        public AttendanceEntry(IEnumerable<MemberDTO> membersList)
        {
            AttendanceEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<IEnumerable<MemberDTO>>(membersList);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AttendanceEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AttendanceEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.Focus();
            //TxtBarCode.Focus();
        }

        private void MemberAttendances_OnClosing(object sender, CancelEventArgs e)
        {
            AttendanceEntryViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = "";
            LstItemsAutoCompleteBox.Focus();
        }

        private void TxtBarCode_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var str = e.Key.ToString();
            if (str == "Return")
            {
                //Messenger.Default.Send<string>(TxtBarCode.Text);
                //TxtBarCode.Text = "";
            }
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
