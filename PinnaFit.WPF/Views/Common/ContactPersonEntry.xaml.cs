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
    /// Interaction logic for ContactPersonEntry.xaml
    /// </summary>
    public partial class ContactPersonEntry : Window
    {
        public ContactPersonEntry()
        {
            ContactPersonViewModel.Errors = 0;
            InitializeComponent();
        }

        public ContactPersonEntry(ContactPersonDTO contactPersonDTO)
        {
            ContactPersonViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ContactPersonDTO>(contactPersonDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ContactPersonViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ContactPersonViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtContactName.Focus();
        }

        private void ContactPersonEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //ContactPersonViewModel.CleanUp();
        }
        private void WdwContactPerson_Initialized(object sender, System.EventArgs e)
        {
            //Height = winheight;
        }
    }
}
