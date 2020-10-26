using System.ComponentModel;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Equipments.xaml
    /// </summary>
    public partial class Equipments : Window
    {
        public Equipments()
        {
            EquipmentViewModel.Errors = 0;
            InitializeComponent();
        }
        //public Equipments(EquipmentTypes equipmentType)
        //{
        //    EquipmentViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<EquipmentTypes>(equipmentType);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EquipmentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EquipmentViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }

        private void Equipments_OnClosing(object sender, CancelEventArgs e)
        {
            EquipmentViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            //TxtName.Focus();
        }
    }
}
