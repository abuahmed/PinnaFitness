using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;

namespace PinnaFit.WPF.ViewModel
{
    public class EquipmentViewModel : ViewModelBase
    {
        #region Fields
        private static IEquipmentService _equipmentService;
        private IEnumerable<EquipmentDTO> _businessPartners;
        private ObservableCollection<EquipmentDTO> _filteredEquipments;
        private EquipmentDTO _selectedEquipment;
        private ICommand _addNewEquipmentViewCommand, _saveEquipmentViewCommand, _deleteEquipmentViewCommand;
        private string _searchText, _equipmentText;
        #endregion

        #region Constructor
        public EquipmentViewModel()
        {
            CleanUp();
            _equipmentService = new EquipmentService();

            LoadCategories();
            CheckRoles();
            GetLiveEquipments();

            EquipmentText = "Equipments";
        }
        public static void CleanUp()
        {
            if (_equipmentService != null)
                _equipmentService.Dispose();
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
                if (EquipmentList != null)
                {
                    try
                    {
                        //if (!string.IsNullOrEmpty(SearchText))
                        //{
                        //    Equipments = new ObservableCollection<EquipmentDTO>
                        //        (EquipmentList.Where(c => c.EquipmentDetail.ToLower().Contains(value.ToLower())).ToList());
                        //}
                        //else
                        Equipments = new ObservableCollection<EquipmentDTO>(EquipmentList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching equipment");
                        Equipments = new ObservableCollection<EquipmentDTO>(EquipmentList);
                    }
                }
            }
        }
        public string EquipmentText
        {
            get { return _equipmentText; }
            set
            {
                _equipmentText = value;
                RaisePropertyChanged<string>(() => EquipmentText);
            }
        }

        public EquipmentDTO SelectedEquipment
        {
            get { return _selectedEquipment; }
            set
            {
                _selectedEquipment = value;
                RaisePropertyChanged<EquipmentDTO>(() => SelectedEquipment);
                if (SelectedEquipment != null && SelectedEquipment.Category != null )
                {
                    SelectedCategory =
                        Categories.FirstOrDefault(c => c.DisplayName == SelectedEquipment.Category.DisplayName);
                }
            }
        }
        public IEnumerable<EquipmentDTO> EquipmentList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<EquipmentDTO>>(() => EquipmentList);
            }
        }
        public ObservableCollection<EquipmentDTO> Equipments
        {
            get { return _filteredEquipments; }
            set
            {
                _filteredEquipments = value;
                RaisePropertyChanged<ObservableCollection<EquipmentDTO>>(() => Equipments);

                if (Equipments != null && Equipments.Any())
                    SelectedEquipment = Equipments.FirstOrDefault();
                else
                    AddNewEquipment();
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewEquipmentViewCommand
        {
            get { return _addNewEquipmentViewCommand ?? (_addNewEquipmentViewCommand = new RelayCommand(AddNewEquipment)); }
        }
        private void AddNewEquipment()
        {
            SelectedEquipment = new EquipmentDTO();
        }

        public ICommand SaveEquipmentViewCommand
        {
            get { return _saveEquipmentViewCommand ?? (_saveEquipmentViewCommand = new RelayCommand<object>(SaveEquipment, CanSave)); }
        }
        private void SaveEquipment(object obj)
        {
            try
            {
                var newObject = SelectedEquipment.Id;
                SelectedEquipment.CategoryId = SelectedCategory.Id;

                var stat = _equipmentService.InsertOrUpdate(SelectedEquipment);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                {
                    Equipments.Insert(0, SelectedEquipment);
                    SelectedEquipment.Number = _equipmentService.GetEquipmentNumber(SelectedEquipment.Id);
                    stat = _equipmentService.InsertOrUpdate(SelectedEquipment);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteEquipmentViewCommand
        {
            get { return _deleteEquipmentViewCommand ?? (_deleteEquipmentViewCommand = new RelayCommand<Object>(DeleteEquipment, CanSave)); }
        }
        private void DeleteEquipment(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedEquipment.Enabled = false;
                    var stat = _equipmentService.Disable(SelectedEquipment);
                    if (stat == string.Empty)
                    {
                        Equipments.Remove(SelectedEquipment);
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

        public void GetLiveEquipments()
        {
            var criteria = new SearchCriteria<EquipmentDTO>();

            //var stafType = (EquipmentTypes)SelectedEquipmentTypeForFilter.Value;
            //if (stafType != EquipmentTypes.All)
            //    criteria.FiList.Add(b => b.Type == stafType);

            EquipmentList = _equipmentService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Equipments = new ObservableCollection<EquipmentDTO>(EquipmentList);
        }

        #region Categories
        private ICommand _addNewCategoryCommand;
        private ObservableCollection<CategoryDTO> _categories;
        private CategoryDTO _selectedCategory;

        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.EquipmentCategory);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Categories = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        public CategoryDTO SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedCategory);
            }
        }
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Categories);
            }
        }
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.EquipmentCategory);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();//should also get the latest updates in each row
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedEquipment.CategoryId = SelectedCategory.Id;
            }
        }
        #endregion

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