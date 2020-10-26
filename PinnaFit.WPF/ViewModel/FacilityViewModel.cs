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
    public class FacilityViewModel : ViewModelBase
    {
        #region Fields
        private static IFacilityService _facilityService;
        private IEnumerable<FacilityDTO> _businessPartners;
        private ObservableCollection<FacilityDTO> _filteredFacilitys;
        private FacilityDTO _selectedFacility;
        private ICommand _addNewFacilityViewCommand, _saveFacilityViewCommand, _deleteFacilityViewCommand;
        private string _searchText, _facilityText;
        #endregion

        #region Constructor
        public FacilityViewModel()
        {
            CleanUp();
            _facilityService = new FacilityService();
            
            GetLiveServices();
            FillFacilityTypes();
            CheckRoles();
            GetLiveFacilities();

            FacilityText = "Training Types";
        }
        public static void CleanUp()
        {
            if (_facilityService != null)
                _facilityService.Dispose();
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
                if (FacilityList != null)
                {
                    try
                    {
                        //if (!string.IsNullOrEmpty(SearchText))
                        //{
                        //    Facilitys = new ObservableCollection<FacilityDTO>
                        //        (FacilityList.Where(c => c.FacilityDetail.ToLower().Contains(value.ToLower())).ToList());
                        //}
                        //else
                        Facilitys = new ObservableCollection<FacilityDTO>(FacilityList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching Facility");
                        Facilitys = new ObservableCollection<FacilityDTO>(FacilityList);
                    }
                }
            }
        }
        public string FacilityText
        {
            get { return _facilityText; }
            set
            {
                _facilityText = value;
                RaisePropertyChanged<string>(() => FacilityText);
            }
        }

        public FacilityDTO SelectedFacility
        {
            get { return _selectedFacility; }
            set
            {
                _selectedFacility = value;
                RaisePropertyChanged<FacilityDTO>(() => SelectedFacility);
                if (SelectedFacility != null)
                {
                    try
                    {
                        if (SelectedFacility.Services != null)
                        {
                            IList<ServiceDTO> selectedServicesList = SelectedFacility.Services.Select(userroles => userroles.Service).ToList();
                            SelectedServices = new ObservableCollection<ServiceDTO>(selectedServicesList);
                        }

                        FilteredServices = new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
                    }
                    catch { }
                }
            }
        }
        public IEnumerable<FacilityDTO> FacilityList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<FacilityDTO>>(() => FacilityList);
            }
        }
        public ObservableCollection<FacilityDTO> Facilitys
        {
            get { return _filteredFacilitys; }
            set
            {
                _filteredFacilitys = value;
                RaisePropertyChanged<ObservableCollection<FacilityDTO>>(() => Facilitys);

                if (Facilitys != null && Facilitys.Any())
                    SelectedFacility = Facilitys.FirstOrDefault();
                else
                    AddNewFacility();
            }
        }
        #endregion

        #region Filter List
        private List<ListDataItem> _facilityTypeList;
        private ListDataItem _selectedFacilityType;

        public List<ListDataItem> FacilityTypeList
        {
            get { return _facilityTypeList; }
            set
            {
                _facilityTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => FacilityTypeList);
            }
        }
        public ListDataItem SelectedFacilityType
        {
            get { return _selectedFacilityType; }
            set
            {
                _selectedFacilityType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedFacilityType);
            }
        }

        private List<ListDataItem> _facilityTypeListForFilter;
        private ListDataItem _selectedFacilityTypeForFilter;

        public List<ListDataItem> FacilityTypeListForFilter
        {
            get { return _facilityTypeListForFilter; }
            set
            {
                _facilityTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => FacilityTypeListForFilter);
            }
        }
        public ListDataItem SelectedFacilityTypeForFilter
        {
            get { return _selectedFacilityTypeForFilter; }
            set
            {
                _selectedFacilityTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedFacilityTypeForFilter);
                GetLiveFacilities();
            }
        }

        public void FillFacilityTypes()
        {
            //var FacilityTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(FacilityTypes));

            //if (FacilityTypes != null && FacilityTypes.Count > 1)
            //FacilityTypeList = FacilityTypes.Skip(1).ToList();

            //FacilityTypeListForFilter = FacilityTypes.ToList();
            //SelectedFacilityTypeForFilter = FacilityTypeListForFilter.FirstOrDefault();
        }


        #endregion

        #region Commands
        public ICommand AddNewFacilityViewCommand
        {
            get { return _addNewFacilityViewCommand ?? (_addNewFacilityViewCommand = new RelayCommand(AddNewFacility)); }
        }
        private void AddNewFacility()
        {
            SelectedFacility = new FacilityDTO();

            SelectedServices = new ObservableCollection<ServiceDTO>();
            GetLiveServices();
            FilteredServices = new ObservableCollection<ServiceDTO>(Services.ToList());

            AllServicesChecked = false;
            AddServiceEnability = false;
            RemoveServiceEnability = false;
        }

        public ICommand SaveFacilityViewCommand
        {
            get { return _saveFacilityViewCommand ?? (_saveFacilityViewCommand = new RelayCommand<Object>(SaveFacility, CanSave)); }
        }
        private void SaveFacility(object obj)
        {
            try
            {
                var newObject = SelectedFacility.Id;
                SelectedFacility.Services = new List<FacilityServiceDTO>();
                foreach (var role in SelectedServices)
                {
                    SelectedFacility.Services.Add(new FacilityServiceDTO { FacilityId = SelectedFacility.Id, Service = role });
                }

                var stat = _facilityService.InsertOrUpdate(SelectedFacility);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Facilitys.Insert(0, SelectedFacility);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteFacilityViewCommand
        {
            get { return _deleteFacilityViewCommand ?? (_deleteFacilityViewCommand = new RelayCommand<Object>(DeleteFacility, CanSave)); }
        }
        private void DeleteFacility(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedFacility.Enabled = false;
                    var stat = _facilityService.Disable(SelectedFacility);
                    if (stat == string.Empty)
                    {
                        Facilitys.Remove(SelectedFacility);
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
        
        public void GetLiveFacilities()
        {
            var criteria = new SearchCriteria<FacilityDTO>();

            FacilityList = _facilityService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Facilitys = new ObservableCollection<FacilityDTO>(FacilityList);
        }

        #region Services
        private ICommand _addServiceViewCommand, _deleteServiceViewCommand;
        private ServiceDTO _selectedService, _selectedServiceToAdd;
        private ObservableCollection<ServiceDTO> _selectedServices;
        private ObservableCollection<ServiceDTO> _roles, _filteredServices;
        private bool _addServiceEnability, _removeServiceEnability, _allServicesChecked;

        public ServiceDTO SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                RaisePropertyChanged<ServiceDTO>(() => SelectedService);
                RemoveServiceEnability = SelectedService != null;
            }
        }
        public ServiceDTO SelectedServiceToAdd
        {
            get { return _selectedServiceToAdd; }
            set
            {
                _selectedServiceToAdd = value;
                RaisePropertyChanged<ServiceDTO>(() => SelectedServiceToAdd);

                AddServiceEnability = SelectedServiceToAdd != null;
            }
        }
        public ObservableCollection<ServiceDTO> SelectedServices
        {
            get { return _selectedServices; }
            set
            {
                _selectedServices = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => SelectedServices);
            }
        }
        public ObservableCollection<ServiceDTO> Services
        {
            get { return _roles; }
            set
            {
                _roles = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => Services);
            }
        }
        public ObservableCollection<ServiceDTO> FilteredServices
        {
            get { return _filteredServices; }
            set
            {
                _filteredServices = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => FilteredServices);
            }
        }
        public bool AddServiceEnability
        {
            get { return _addServiceEnability; }
            set
            {
                _addServiceEnability = value;
                RaisePropertyChanged<bool>(() => AddServiceEnability);
            }
        }
        public bool RemoveServiceEnability
        {
            get { return _removeServiceEnability; }
            set
            {
                _removeServiceEnability = value;
                RaisePropertyChanged<bool>(() => RemoveServiceEnability);
            }
        }
        public bool AllServicesChecked
        {
            get { return _allServicesChecked; }
            set
            {
                _allServicesChecked = value;
                RaisePropertyChanged<bool>(() => AllServicesChecked);

                try
                {
                    if (AllServicesChecked)
                    {
                        SelectedServices = new ObservableCollection<ServiceDTO>(Services);
                        FilteredServices = new ObservableCollection<ServiceDTO>();
                    }
                    else
                    {
                        SelectedServices = new ObservableCollection<ServiceDTO>();
                        FilteredServices = new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
                    }

                }
                catch
                {
                    MessageBox.Show("Can't Remove Service");
                }
            }
        }

        public ICommand AddServiceViewCommand
        {
            get
            {
                return _addServiceViewCommand ?? (_addServiceViewCommand = new RelayCommand(ExcuteAddServiceViewCommand, CanSave));
            }
        }
        private void ExcuteAddServiceViewCommand()
        {
            try
            {
                SelectedServices.Add(SelectedServiceToAdd);
                FilteredServices = new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
            }
            catch
            {
                MessageBox.Show("Can't Save Service");
            }
        }
        public ICommand RemoveServiceViewCommand
        {
            get { return _deleteServiceViewCommand ?? (_deleteServiceViewCommand = new RelayCommand(ExecuteRemoveServiceViewCommand)); }
        }
        private void ExecuteRemoveServiceViewCommand()
        {
            try
            {
                SelectedServices.Remove(SelectedService);
                FilteredServices = new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
            }
            catch
            {
                MessageBox.Show("Can't Remove Service");
            }
        }
        
        private void GetLiveServices()
        {
            Services = new ObservableCollection<ServiceDTO>(_facilityService.GetAllServices().ToList().OrderBy(i => i.Id));
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
