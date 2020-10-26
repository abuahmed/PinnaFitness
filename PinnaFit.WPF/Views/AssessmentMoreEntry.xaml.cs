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
    /// Interaction logic for MemberAssessmentMoreEntry.xaml
    /// </summary>
    public partial class MemberAssessmentMoreEntry : Window
    {
        public MemberAssessmentMoreEntry()
        {
            MemberAssessmentMoreViewModel.Errors = 0;
            InitializeComponent();
        }

        public MemberAssessmentMoreEntry(MemberAssessmentMoreDTO memberAssessmentMoreDTO)
        {
            MemberAssessmentMoreViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<MemberAssessmentMoreDTO>(memberAssessmentMoreDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) MemberAssessmentMoreViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) MemberAssessmentMoreViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtMobile.Focus();
        }

        private void MemberAssessmentMoreEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //MemberAssessmentMoreViewModel.CleanUp();
        }
        private void WdwMemberAssessmentMore_Initialized(object sender, System.EventArgs e)
        {
            //Height = winheight;
        }
    }
}
