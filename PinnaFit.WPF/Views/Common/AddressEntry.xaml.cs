using System;
using System.ComponentModel;
using System.Windows.Media;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for AddressEntry.xaml
    /// </summary>
    public partial class AddressEntry : Window
    {
        public AddressEntry()
        {
            AddressViewModel.Errors = 0;
            InitializeComponent();
        }

        public AddressEntry(AddressDTO addressDTO)
        {
            AddressViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<AddressDTO>(addressDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AddressViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AddressViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtMobile.Focus();
        }

        private void AddressEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //AddressViewModel.CleanUp();
        }
        private void WdwAddress_Initialized(object sender, System.EventArgs e)
        {
            //Height = winheight;
        }
    }
}
