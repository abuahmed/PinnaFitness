using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.Views;

namespace PinnaFit.WPF.ViewModel
{
    public class ContactPersonViewModel : ViewModelBase
    {
        #region Fields
        private ContactPersonDTO _selectedContactPerson;
        private ICommand _saveContactPersonViewCommand, _closeContactPersonViewCommand, _resetContactPersonViewCommand;
        private ObservableCollection<AddressDTO> _contactPersonAddressDetail;
        private ICommand _contactPersonAddressViewCommand;
        #endregion

        #region Constructor
        public ContactPersonViewModel()
        {
            CheckRoles();
            Messenger.Default.Register<ContactPersonDTO>(this, (message) =>
            {
                SelectedContactPerson = message;
            });
        }

        #endregion

        #region Public Properties

        public ContactPersonDTO SelectedContactPerson
        {
            get { return _selectedContactPerson; }
            set
            {
                _selectedContactPerson = value;
                RaisePropertyChanged<ContactPersonDTO>(() => SelectedContactPerson);

                if (SelectedContactPerson != null)
                    ContactPersonAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedContactPerson.Address
                    };
            }
        }
        public ObservableCollection<AddressDTO> ContactPersonAdressDetail
        {
            get { return _contactPersonAddressDetail; }
            set
            {
                _contactPersonAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactPersonAdressDetail);
            }
        }
        public ICommand ContactPersonAddressViewCommand
        {
            get { return _contactPersonAddressViewCommand ?? (_contactPersonAddressViewCommand = new RelayCommand(ContactPersonAddress)); }
        }

        public void ContactPersonAddress()
        {
            new AddressEntry(SelectedContactPerson.Address).ShowDialog();
        }

        #endregion

        #region Commands

        public ICommand SaveContactPersonViewCommand
        {
            get { return _saveContactPersonViewCommand ?? (_saveContactPersonViewCommand = new RelayCommand<object>(SaveContactPerson, CanSave)); }
        }
        private void SaveContactPerson(object obj)
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

        public ICommand ResetContactPersonViewCommand
        {
            get { return _resetContactPersonViewCommand ?? (_resetContactPersonViewCommand = new RelayCommand(ResetContactPerson)); }
        }
        private void ResetContactPerson()
        {
            SelectedContactPerson = new ContactPersonDTO
            {
                Sex = Sex.Male
            };
        }

        public ICommand CloseContactPersonViewCommand
        {
            get { return _closeContactPersonViewCommand ?? (_closeContactPersonViewCommand = new RelayCommand<Object>(CloseWindow)); }
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

        public void GetContactPersonDetail()
        {
            var criteria = new SearchCriteria<ContactPersonDTO>();
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