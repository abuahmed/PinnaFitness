#region MyRegion
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;
#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class WarehouseViewModel : ViewModelBase
    {
        #region Fields
        private static IWarehouseService _warehouseService;
        private ObservableCollection<WarehouseDTO> _filteredWarehouses;
        private WarehouseDTO _selectedWarehouse;
        private ICommand _addNewWarehouseViewCommand, _saveWarehouseViewCommand, _deleteWarehouseViewCommand;
        private bool _defaultCheckBoxEnability;
        #endregion

        #region Constructor
        public WarehouseViewModel()
        {
            CleanUp();
            _warehouseService = new WarehouseService();
            CheckRoles();
            GetLiveWarehouses();
        }

        public static void CleanUp()
        {
            if (_warehouseService != null)
                _warehouseService.Dispose();
        }
        #endregion

        #region Properties
        public ObservableCollection<WarehouseDTO> Warehouses
        {
            get { return _filteredWarehouses; }
            set
            {
                _filteredWarehouses = value;
                RaisePropertyChanged<ObservableCollection<WarehouseDTO>>(() => Warehouses);
                if (Warehouses.Any())
                    SelectedWarehouse = Warehouses.FirstOrDefault();
                else
                    ExecuteAddNewWarehouseViewCommand();
            }
        }

        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);
                if (SelectedWarehouse != null)
                {
                    LetterHeadImage = ImageUtil.ToImage(SelectedWarehouse.Header);
                    LetterFootImage = ImageUtil.ToImage(SelectedWarehouse.Footer);
                }
                else
                    DefaultCheckBoxEnability = true;

            }
        }
        public bool DefaultCheckBoxEnability
        {
            get { return _defaultCheckBoxEnability; }
            set
            {
                _defaultCheckBoxEnability = value;
                RaisePropertyChanged<bool>(() => DefaultCheckBoxEnability);
            }
        }
        //public bool AddNewWarehouseEnability
        //{
        //    get { return _addNewWarehouseEnability; }
        //    set
        //    {
        //        _addNewWarehouseEnability = value;
        //        RaisePropertyChanged<bool>(() => AddNewWarehouseEnability);
        //    }
        //}

        public void GetLiveWarehouses()
        {
            var criteria = new SearchCriteria<WarehouseDTO>();
            
            var warehousesList = _warehouseService.GetAll(criteria);
            Warehouses = new ObservableCollection<WarehouseDTO>(warehousesList);
        }
        #endregion

        #region Commands
        public ICommand AddNewWarehouseViewCommand
        {
            get { return _addNewWarehouseViewCommand ?? (_addNewWarehouseViewCommand = new RelayCommand(ExecuteAddNewWarehouseViewCommand)); }
        }
        private void ExecuteAddNewWarehouseViewCommand()
        {
            SelectedWarehouse = new WarehouseDTO
            {
                Address = new AddressDTO
                {
                    Country = "ኢትዮጲያ",
                    City = "አዲስ አበባ"
                }
            };
        }

        public ICommand SaveWarehouseViewCommand
        {
            get { return _saveWarehouseViewCommand ?? (_saveWarehouseViewCommand = new RelayCommand<Object>(ExecuteSaveWarehouseViewCommand, CanSave)); }
        }
        private void ExecuteSaveWarehouseViewCommand(object obj)
        {
            try
            {
                var newObject = SelectedWarehouse.Id;
                if (LetterHeadImage.UriSource != null)
                    SelectedWarehouse.Header = ImageUtil.ToBytes(LetterHeadImage);
                if (LetterFootImage.UriSource != null)
                    SelectedWarehouse.Footer = ImageUtil.ToBytes(LetterFootImage);

                var stat = _warehouseService.InsertOrUpdate(SelectedWarehouse);
                if (stat != string.Empty)
                {
                    MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + stat, "save error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else if (newObject == 0)
                {
                    //CloseWindow(obj);
                    Warehouses.Insert(0, SelectedWarehouse);
                    SelectedWarehouse.Number = _warehouseService.GetWarehouseNumber(SelectedWarehouse.Id);
                    stat = _warehouseService.InsertOrUpdate(SelectedWarehouse);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
                    
            }
            catch (Exception exception)
            {
                MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + exception.Message, "save error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteWarehouseViewCommand
        {
            get { return _deleteWarehouseViewCommand ?? (_deleteWarehouseViewCommand = new RelayCommand(ExecuteDeleteWarehouseViewCommand, CanSaveLine)); }
        }
        private void ExecuteDeleteWarehouseViewCommand()
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                try
                {
                    SelectedWarehouse.Enabled = false;
                    _warehouseService.Disable(SelectedWarehouse);
                    GetLiveWarehouses();
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't delete the warehouse, may be the warehouse is already in use...", "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion
        
        #region Letter Head
        private BitmapImage _letterHeadImage, _letterFootImage;
        private ICommand _showLetterHeaderImageCommand, _showLetterFooterImageCommand;

        public BitmapImage LetterHeadImage
        {
            get { return _letterHeadImage; }
            set
            {
                _letterHeadImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterHeadImage);
            }
        }
        public ICommand ShowLetterHeaderImageCommand
        {
            get { return _showLetterHeaderImageCommand ?? (_showLetterHeaderImageCommand = new RelayCommand(ExecuteShowLetterHeaderImageViewCommand)); }
        }
        private void ExecuteShowLetterHeaderImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterHeadImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
            }
        }

        public BitmapImage LetterFootImage
        {
            get { return _letterFootImage; }
            set
            {
                _letterFootImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterFootImage);
            }
        }
        public ICommand ShowLetterFooterImageCommand
        {
            get { return _showLetterFooterImageCommand ?? (_showLetterFooterImageCommand = new RelayCommand(ExecuteShowLetterFooterImageViewCommand)); }
        }
        private void ExecuteShowLetterFooterImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterFootImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine()
        {
            return LineErrors == 0;

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

            //StoreNameEnability = UserRoles.AddStores == "Visible";
        }

        #endregion
    }
}
