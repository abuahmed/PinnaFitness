using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class AddressViewModel : ViewModelBase
    {
        #region Fields
        private AddressDTO _selectedAddress;
        private ICommand _saveAddressViewCommand, _closeAddressViewCommand, _resetAddressViewCommand;
        #endregion

        #region Constructor
        public AddressViewModel()
        {
            CheckRoles();
            Messenger.Default.Register<AddressDTO>(this, (message) =>
            {
                SelectedAddress = message;
            });
        }

        #endregion

        #region Public Properties

        public AddressDTO SelectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                _selectedAddress = value;
                RaisePropertyChanged<AddressDTO>(() => SelectedAddress);
            }
        }


        #endregion

        #region Commands

        public ICommand SaveAddressViewCommand
        {
            get { return _saveAddressViewCommand ?? (_saveAddressViewCommand = new RelayCommand<Object>(SaveAddress, CanSave)); }
        }
        private void SaveAddress(object obj)
        {
            try
            {
                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand ResetAddressViewCommand
        {
            get { return _resetAddressViewCommand ?? (_resetAddressViewCommand = new RelayCommand(ResetAddress)); }
        }
        private void ResetAddress()
        {
            SelectedAddress = new AddressDTO
            {
                Country = "ኢትዮጲያ",
                City = "አዲስ አበባ"
            };
        }

        public ICommand CloseAddressViewCommand
        {
            get { return _closeAddressViewCommand ?? (_closeAddressViewCommand = new RelayCommand<Object>(CloseWindow)); }
        }
        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        #endregion

        public void GetAddressDetail()
        {
            var criteria = new SearchCriteria<AddressDTO>();
        }


        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        #endregion

        #region Previlege Visibility
        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}
