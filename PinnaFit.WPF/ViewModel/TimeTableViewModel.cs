using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class TimeTableViewModel : ViewModelBase
    {
        #region Fields
        private static ITimeTableService _timeTableService;
        private IEnumerable<TimeTableDTO> _memberAssessments;
        private ObservableCollection<ServiceDTO> _members;
        private ObservableCollection<ServiceDTO> _services;
        private ServiceDTO _selectedService;
        private ObservableCollection<TimeTableDTO> _filteredTimeTables;
        private TimeTableDTO _selectedTimeTable;
        private ICommand _addNewTimeTableViewCommand, _saveTimeTableViewCommand, _deleteTimeTableViewCommand;
        private string _searchText, _timeTableText;
        private DateTime _classBegins, _classEnds;
        #endregion

        #region Constructor
        public TimeTableViewModel()
        {
            CleanUp();
            _timeTableService = new TimeTableService();

            GetLiveTrainers();
            CheckRoles();
            GetLiveServices();
            GetLiveTimeTables();

            ClassBegins = DateTime.Now;
            ClassEnds = DateTime.Now;

            TimeTableText = "Time Table Entry";
        }
        public static void CleanUp()
        {
            if (_timeTableService != null)
                _timeTableService.Dispose();
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

                try
                {
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        //Services = new ObservableCollection<ServiceDTO>
                        //(ServiceList.Where(c => c.ServiceDetail.ToLower().Contains(value.ToLower())).ToList());
                    }
                    //else
                    //Services = new ObservableCollection<ServiceDTO>(ServiceList);
                }
                catch
                {
                    MessageBox.Show("Problem searching Service");
                    //Services = new ObservableCollection<ServiceDTO>(ServiceList);
                }

            }
        }
        public string TimeTableText
        {
            get { return _timeTableText; }
            set
            {
                _timeTableText = value;
                RaisePropertyChanged<string>(() => TimeTableText);
            }
        }
        public ObservableCollection<ServiceDTO> ServiceList
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => ServiceList);
            }
        }
        public ObservableCollection<ServiceDTO> Services
        {
            get { return _services; }
            set
            {
                _services = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => Services);
            }
        }
        public TimeTableDTO SelectedTimeTable
        {
            get { return _selectedTimeTable; }
            set
            {
                _selectedTimeTable = value;
                RaisePropertyChanged<TimeTableDTO>(() => SelectedTimeTable);
                if (SelectedTimeTable != null && SelectedTimeTable.Id != 0)
                {
                    SelectedService = SelectedTimeTable.ServiceId != 0 ? Services.FirstOrDefault(d => d.Id == SelectedTimeTable.ServiceId) : null;
                    ClassBegins = Convert.ToDateTime(SelectedTimeTable.ClassBegins);
                    ClassEnds = Convert.ToDateTime(SelectedTimeTable.ClassEnds);

                    try
                    {
                        if (SelectedTimeTable.Trainers != null)
                        {
                            IList<TrainerDTO> selectedTrainersList = SelectedTimeTable.Trainers.Select(userroles => userroles.Trainer).ToList();
                            SelectedTrainers = new ObservableCollection<TrainerDTO>(selectedTrainersList);
                        }

                        GetLiveFilteredTrainers(); //FilteredTrainers = new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
                    }
                    catch { }
                }
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
                    GetLiveTrainers();
            }
        }
        public IEnumerable<TimeTableDTO> TimeTableList
        {
            get { return _memberAssessments; }
            set
            {
                _memberAssessments = value;
                RaisePropertyChanged<IEnumerable<TimeTableDTO>>(() => TimeTableList);
            }
        }
        public ObservableCollection<TimeTableDTO> TimeTables
        {
            get { return _filteredTimeTables; }
            set
            {
                _filteredTimeTables = value;
                RaisePropertyChanged<ObservableCollection<TimeTableDTO>>(() => TimeTables);

                if (TimeTables != null && TimeTables.Any())
                    SelectedTimeTable = TimeTables.FirstOrDefault();
                else
                    AddNewTimeTable();
            }
        }
        public DateTime ClassBegins
        {
            get { return _classBegins; }
            set
            {
                _classBegins = value;
                RaisePropertyChanged<DateTime>(() => ClassBegins);
            }
        }
        public DateTime ClassEnds
        {
            get { return _classEnds; }
            set
            {
                _classEnds = value;
                RaisePropertyChanged<DateTime>(() => ClassEnds);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewTimeTableViewCommand
        {
            get { return _addNewTimeTableViewCommand ?? (_addNewTimeTableViewCommand = new RelayCommand(AddNewTimeTable)); }
        }
        private void AddNewTimeTable()
        {
            SelectedService = Services.FirstOrDefault();
            SelectedTimeTable = new TimeTableDTO()
            {
                ClassBegins = DateTime.Now.ToShortTimeString(),
                ClassEnds = DateTime.Now.ToShortTimeString()
            };

            SelectedTrainers = new ObservableCollection<TrainerDTO>();
            //GetLiveTrainers();
            GetLiveFilteredTrainers(); //FilteredTrainers = new ObservableCollection<TrainerDTO>(Trainers.ToList());

            AllTrainersChecked = true;
            AddTrainerEnability = false;
            RemoveTrainerEnability = false;
        }



        public ICommand SaveTimeTableViewCommand
        {
            get { return _saveTimeTableViewCommand ?? (_saveTimeTableViewCommand = new RelayCommand<Object>(SaveTimeTable, CanSave)); }
        }
        private void SaveTimeTable(object obj)
        {
            try
            {
                var newObject = SelectedTimeTable.Id;

                SelectedTimeTable.ServiceId = SelectedService.Id;
                SelectedTimeTable.ClassBegins = ClassBegins.ToString("hh:mm tt");//HH:mm:ss
                SelectedTimeTable.ClassEnds = ClassEnds.ToString("hh:mm tt");

                SelectedTimeTable.Trainers = new List<TrainerTimeTableDTO>();
                foreach (var trainer in SelectedTrainers)
                {
                    SelectedTimeTable.Trainers.Add(new TrainerTimeTableDTO { TimeTableId = SelectedTimeTable.Id, Trainer = trainer });
                }

                var stat = _timeTableService.InsertOrUpdate(SelectedTimeTable);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                {
                    GetLiveTimeTables();
                    //TimeTables.Insert(0, SelectedTimeTable);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteTimeTableViewCommand
        {
            get { return _deleteTimeTableViewCommand ?? (_deleteTimeTableViewCommand = new RelayCommand<Object>(DeleteTimeTable, CanSave)); }
        }
        private void DeleteTimeTable(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedTimeTable.Enabled = false;
                    var stat = _timeTableService.Disable(SelectedTimeTable);
                    if (stat == string.Empty)
                    {
                        TimeTables.Remove(SelectedTimeTable);
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

            ServiceList = new ObservableCollection<ServiceDTO>(
                new ServiceService(true).GetAll(criteria).OrderBy(i => i.Id)
                .ToList());
            Services = new ObservableCollection<ServiceDTO>(ServiceList);
        }
        public void GetLiveTimeTables()
        {
            var criteria = new SearchCriteria<TimeTableDTO>();
            
            TimeTableList = _timeTableService.GetAll(criteria).OrderBy(i => i.Id).ToList();
            TimeTables = new ObservableCollection<TimeTableDTO>(TimeTableList);
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
                        GetLiveFilteredTrainers(); //FilteredTrainers = new ObservableCollection<TrainerDTO>();
                    }
                    else
                    {
                        SelectedTrainers = new ObservableCollection<TrainerDTO>();
                        GetLiveTrainers();
                        GetLiveFilteredTrainers(); //FilteredTrainers = new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
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
                GetLiveFilteredTrainers(); //FilteredTrainers = new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
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
                GetLiveFilteredTrainers();
                    //FilteredTrainers = new ObservableCollection<TrainerDTO>(Trainers.Except(SelectedTrainers));
            }
            catch
            {
                MessageBox.Show("Can't Remove Trainer");
            }
        }

        private void GetLiveTrainers()
        {
            var trians = _timeTableService.GetTrainerServices()
                .Where(s => SelectedService != null && s.ServiceId == SelectedService.Id)
                .Select(tra => tra.Trainer).ToList();
            Trainers = new ObservableCollection<TrainerDTO>(trians);
        }

        private void GetLiveFilteredTrainers()
        {
            IList<TrainerDTO> filtTra = new List<TrainerDTO>();
            foreach (var trainerDTO in Trainers)
            {
                if (SelectedTrainers != null)
                {
                    var train = SelectedTrainers.FirstOrDefault(tr => tr.DisplayName == trainerDTO.DisplayName);
                    if(train==null)
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
