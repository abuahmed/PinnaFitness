using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class ServiceViewModel : ViewModelBase
    {
        #region Fields
        private static IServiceService _serviceService;
        private IEnumerable<ServiceDTO> _businessPartners;
        private ObservableCollection<ServiceDTO> _filteredServices;
        private ServiceDTO _selectedService;
        private ICommand _addNewServiceViewCommand, _saveServiceViewCommand, _deleteServiceViewCommand;
        private string _searchText, _subscriptionText;
        #endregion

        #region Constructor
        public ServiceViewModel()
        {
            CleanUp();
            _serviceService = new ServiceService();

            GetLiveTrainers();
            CheckRoles();
            GetLiveServices();

            ServiceText = "Services";
        }
        public static void CleanUp()
        {
            if (_serviceService != null)
                _serviceService.Dispose();
        }
        #endregion

        #region Public Properties
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (ServiceList != null)
                {
                    try
                    {
                        //if (!string.IsNullOrEmpty(SearchText))
                        //{
                        //    Services = new ObservableCollection<ServiceDTO>
                        //        (ServiceList.Where(c => c.ServiceDetail.ToLower().Contains(value.ToLower())).ToList());
                        //}
                        //else
                        Services = new ObservableCollection<ServiceDTO>(ServiceList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching subscription");
                        Services = new ObservableCollection<ServiceDTO>(ServiceList);
                    }
                }
            }
        }
        public string ServiceText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => ServiceText);
            }
        }

        public ServiceDTO SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                RaisePropertyChanged<ServiceDTO>(() => SelectedService);
                if (SelectedService != null)
                {
                    try
                    {
                        if (SelectedService.Trainers != null)
                        {
                            IList<TrainerDTO> selectedTrainersList = SelectedService.Trainers.Select(userroles => userroles.Trainer).ToList();
                            SelectedTrainers = new ObservableCollection<TrainerDTO>(selectedTrainersList);
                        }

                        GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
                    }
                    catch { }
                }
            }
        }
        public IEnumerable<ServiceDTO> ServiceList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<ServiceDTO>>(() => ServiceList);
            }
        }
        public ObservableCollection<ServiceDTO> Services
        {
            get { return _filteredServices; }
            set
            {
                _filteredServices = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => Services);

                if (Services != null && Services.Any())
                    SelectedService = Services.FirstOrDefault();
                else
                    AddNewService();
            }
        }
        #endregion

        #region Filter List



        #endregion

        #region Commands
        public ICommand AddNewServiceViewCommand
        {
            get { return _addNewServiceViewCommand ?? (_addNewServiceViewCommand = new RelayCommand(AddNewService)); }
        }
        private void AddNewService()
        {
            SelectedService = new ServiceDTO();

            SelectedTrainers = new ObservableCollection<TrainerDTO>();
            GetLiveTrainers();
            GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>(Trainers.ToList());

            AllTrainersChecked = true;
            AddTrainerEnability = false;
            RemoveTrainerEnability = false;
        }

        public ICommand SaveServiceViewCommand
        {
            get { return _saveServiceViewCommand ?? (_saveServiceViewCommand = new RelayCommand<Object>(SaveService, CanSave)); }
        }
        private void SaveService(object obj)
        {
            try
            {
                var newObject = SelectedService.Id;

                SelectedService.Trainers = new List<TrainerCourseDTO>();
                foreach (var role in SelectedTrainers)
                {
                    SelectedService.Trainers.Add(new TrainerCourseDTO { ServiceId = SelectedService.Id, Trainer = role });
                }

                var stat = _serviceService.InsertOrUpdate(SelectedService);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Services.Insert(0, SelectedService);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteServiceViewCommand
        {
            get { return _deleteServiceViewCommand ?? (_deleteServiceViewCommand = new RelayCommand<Object>(DeleteService, CanSave)); }
        }
        private void DeleteService(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedService.Enabled = false;
                    var stat = _serviceService.Disable(SelectedService);
                    if (stat == string.Empty)
                    {
                        Services.Remove(SelectedService);
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                             + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                         + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        #endregion

        public void GetLiveServices()
        {
            var criteria = new SearchCriteria<ServiceDTO>();

            ServiceList = _serviceService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Services = new ObservableCollection<ServiceDTO>(ServiceList);
        }

        #region Trainers
        private ICommand _addTrainerViewCommand, _deleteTrainerViewCommand;
        private TrainerDTO _selectedTrainer, _selectedTrainerToAdd;
        private ObservableCollection<TrainerDTO> _selectedTrainers;
        private ObservableCollection<TrainerDTO> _roles, _filteredTrainers;
        private bool _addTrainerEnability, _removeTrainerEnability, _allTrainersChecked;

        public TrainerDTO SelectedTrainer
        {
            get { return _selectedTrainer; }
            set
            {
                _selectedTrainer = value;
                RaisePropertyChanged<TrainerDTO>(() => SelectedTrainer);
                RemoveTrainerEnability = SelectedTrainer != null;
            }
        }
        public TrainerDTO SelectedTrainerToAdd
        {
            get { return _selectedTrainerToAdd; }
            set
            {
                _selectedTrainerToAdd = value;
                RaisePropertyChanged<TrainerDTO>(() => SelectedTrainerToAdd);

                AddTrainerEnability = SelectedTrainerToAdd != null;
            }
        }
        public ObservableCollection<TrainerDTO> SelectedTrainers
        {
            get { return _selectedTrainers; }
            set
            {
                _selectedTrainers = value;
                RaisePropertyChanged<ObservableCollection<TrainerDTO>>(() => SelectedTrainers);
            }
        }
        public ObservableCollection<TrainerDTO> Trainers
        {
            get { return _roles; }
            set
            {
                _roles = value;
                RaisePropertyChanged<ObservableCollection<TrainerDTO>>(() => Trainers);
            }
        }
        public ObservableCollection<TrainerDTO> FilteredTrainers
        {
            get { return _filteredTrainers; }
            set
            {
                _filteredTrainers = value;
                RaisePropertyChanged<ObservableCollection<TrainerDTO>>(() => FilteredTrainers);
            }
        }
        public bool AddTrainerEnability
        {
            get { return _addTrainerEnability; }
            set
            {
                _addTrainerEnability = value;
                RaisePropertyChanged<bool>(() => AddTrainerEnability);
            }
        }
        public bool RemoveTrainerEnability
        {
            get { return _removeTrainerEnability; }
            set
            {
                _removeTrainerEnability = value;
                RaisePropertyChanged<bool>(() => RemoveTrainerEnability);
            }
        }
        public bool AllTrainersChecked
        {
            get { return _allTrainersChecked; }
            set
            {
                _allTrainersChecked = value;
                RaisePropertyChanged<bool>(() => AllTrainersChecked);

                try
                {
                    if (AllTrainersChecked)
                    {
                        SelectedTrainers = new ObservableCollection<TrainerDTO>(Trainers);
                        GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>();
                    }
                    else
                    {
                        SelectedTrainers = new ObservableCollection<TrainerDTO>();
                        GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
                    }

                }
                catch
                {
                    MessageBox.Show("Can't Remove Trainer");
                }
            }
        }

        public ICommand AddTrainerViewCommand
        {
            get
            {
                return _addTrainerViewCommand ?? (_addTrainerViewCommand = new RelayCommand(ExcuteAddTrainerViewCommand, CanSave));
            }
        }
        private void ExcuteAddTrainerViewCommand()
        {
            try
            {
                SelectedTrainers.Add(SelectedTrainerToAdd);
                GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
            }
            catch
            {
                MessageBox.Show("Can't Save Trainer");
            }
        }
        public ICommand RemoveTrainerViewCommand
        {
            get { return _deleteTrainerViewCommand ?? (_deleteTrainerViewCommand = new RelayCommand(ExecuteRemoveTrainerViewCommand)); }
        }
        private void ExecuteRemoveTrainerViewCommand()
        {
            try
            {
                SelectedTrainers.Remove(SelectedTrainer);
                GetLiveFilteredTrainers();// new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
            }
            catch
            {
                MessageBox.Show("Can't Remove Trainer");
            }
        }

        private void GetLiveTrainers()
        {
            Trainers = new ObservableCollection<TrainerDTO>(_serviceService.GetAllTrainers().ToList().OrderBy(i => i.Id));
        }

        private void GetLiveFilteredTrainers()
        {
            IList<TrainerDTO> filtTra = new List<TrainerDTO>();
            foreach (var trainerDTO in Trainers)
            {
                if (SelectedTrainers != null)
                {
                    var train = SelectedTrainers.FirstOrDefault(tr => tr.DisplayName == trainerDTO.DisplayName);
                    if (train == null)
                        filtTra.Add(trainerDTO);
                }
            }
            FilteredTrainers = new ObservableCollection<TrainerDTO>(filtTra);
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        public bool CanSave()
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
