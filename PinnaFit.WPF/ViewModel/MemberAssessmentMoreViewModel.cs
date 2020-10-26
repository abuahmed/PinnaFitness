using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;

namespace PinnaFit.WPF.ViewModel
{
    public class MemberAssessmentMoreViewModel : ViewModelBase
    {
        #region Fields
        private MemberAssessmentMoreDTO _selectedMemberAssessmentMore;
        private ICommand _saveMemberAssessmentMoreViewCommand, _closeMemberAssessmentMoreViewCommand, _resetMemberAssessmentMoreViewCommand;
        #endregion

        #region Constructor
        public MemberAssessmentMoreViewModel()
        {
            CheckRoles();
            Messenger.Default.Register<MemberAssessmentMoreDTO>(this, (message) =>
            {
                SelectedMemberAssessmentMore = message;
            });
        }

        #endregion

        #region Public Properties

        public MemberAssessmentMoreDTO SelectedMemberAssessmentMore
        {
            get { return _selectedMemberAssessmentMore; }
            set
            {
                _selectedMemberAssessmentMore = value;
                RaisePropertyChanged<MemberAssessmentMoreDTO>(() => SelectedMemberAssessmentMore);
            }
        }


        #endregion

        #region Commands

        public ICommand SaveMemberAssessmentMoreViewCommand
        {
            get { return _saveMemberAssessmentMoreViewCommand ?? (_saveMemberAssessmentMoreViewCommand = new RelayCommand<object>(SaveMemberAssessmentMore, CanSave)); }
        }
        private void SaveMemberAssessmentMore(object obj)
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

        public ICommand ResetMemberAssessmentMoreViewCommand
        {
            get { return _resetMemberAssessmentMoreViewCommand ?? (_resetMemberAssessmentMoreViewCommand = new RelayCommand(ResetMemberAssessmentMore)); }
        }
        private void ResetMemberAssessmentMore()
        {
            SelectedMemberAssessmentMore = new MemberAssessmentMoreDTO();
        }

        public ICommand CloseMemberAssessmentMoreViewCommand
        {
            get { return _closeMemberAssessmentMoreViewCommand ?? (_closeMemberAssessmentMoreViewCommand = new RelayCommand<Object>(CloseWindow)); }
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

        public void GetMemberAssessmentMoreDetail()
        {
            var criteria = new SearchCriteria<MemberAssessmentMoreDTO>();
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