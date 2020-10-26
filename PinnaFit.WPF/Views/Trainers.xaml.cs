using System;
using System.ComponentModel;
using System.Windows.Media;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Trainers.xaml
    /// </summary>
    public partial class Trainers : Window
    {
        public Trainers()
        {
            TrainerViewModel.Errors = 0;
            InitializeComponent();
        }
        //public Trainers(TrainerTypes businessPartnerType)
        //{
        //    TrainerViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<TrainerTypes>(businessPartnerType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) TrainerViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) TrainerViewModel.Errors -= 1;
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
            TXtCustName.Focus();
        }

        private void Trainers_OnClosing(object sender, CancelEventArgs e)
        {
            TrainerViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //LblAge.Text = "";
            TXtCustName.Focus();
        }
    }
}
