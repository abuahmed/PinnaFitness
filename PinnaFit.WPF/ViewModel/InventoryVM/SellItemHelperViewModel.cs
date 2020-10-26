#region

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

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class SellItemHelperViewModel : ViewModelBase
    {
        #region Fields

        private static IItemService _itemService;
        private static IItemQuantityService _itemQuantityService;
        private static ITransactionService _transactionService;
        private ItemDTO _selectedItem;
        private ObservableCollection<ItemDTO> _items;
        private ICommand _closeItemViewCommand;

        #endregion

        #region Constructor

        public SellItemHelperViewModel()
        {
            CleanUp();
            _itemService = new ItemService();
            _itemQuantityService = new ItemQuantityService();
            _transactionService = new TransactionService();

            GetLiveItems();
            GetWarehouses();

            if (Warehouses != null && Warehouses.Any())
            {
                SelectedWarehouse = SelectedWarehouse ?? Warehouses.FirstOrDefault();
            }

            CheckRoles();
        }

        public static void CleanUp()
        {
            if (_itemService != null)
                _itemService.Dispose();
            if (_itemQuantityService != null)
                _itemQuantityService.Dispose();
            if (_transactionService != null)
                _transactionService.Dispose();
        }

        #endregion

        #region Commands

        public ICommand CloseItemViewCommand
        {
            get { return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow)); }
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

        #region Items

        private decimal _storeCurrentQty, _forFree, _left, _previouslyBilled, _totallyBilled, _sold;
        private string _toBeBilled;

        public decimal StoreCurrentQty
        {
            get { return _storeCurrentQty; }
            set
            {
                _storeCurrentQty = value;
                RaisePropertyChanged<decimal>(() => StoreCurrentQty);
            }
        }

        public decimal ForFree
        {
            get { return _forFree; }
            set
            {
                _forFree = value;
                RaisePropertyChanged<decimal>(() => ForFree);
                CalculateToBeBilled();
            }
        }

        public decimal Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged<decimal>(() => Left);
                CalculateToBeBilled();
            }
        }

        public decimal PreviouslyBilled
        {
            get { return _previouslyBilled; }
            set
            {
                _previouslyBilled = value;
                RaisePropertyChanged<decimal>(() => PreviouslyBilled);
                CalculateToBeBilled();
            }
        }

        public decimal TotallyBilled
        {
            get { return _totallyBilled; }
            set
            {
                _totallyBilled = value;
                RaisePropertyChanged<decimal>(() => TotallyBilled);
                CalculateToBeBilled();
            }
        }
        
        public decimal Sold
        {
            get { return _sold; }
            set
            {
                _sold = value;
                RaisePropertyChanged<decimal>(() => Sold);
                
            }
        }

        public string ToBeBilled
        {
            get { return _toBeBilled; }
            set
            {
                _toBeBilled = value;
                RaisePropertyChanged<string>(() => ToBeBilled);
            }
        }

        public void CalculateToBeBilled()
        {
            var tobebill = (StoreCurrentQty - ForFree - Left) - (TotallyBilled - PreviouslyBilled);
            if (tobebill > 0)
                ToBeBilled = tobebill.ToString("N0") + " ያልተቆረጠ";
            else
                ToBeBilled = tobebill.ToString("N0") + " ትርፍ የተቆረጠ";

            Sold = StoreCurrentQty - ForFree - Left;
        }

        public ObservableCollection<ItemDTO> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged<ObservableCollection<ItemDTO>>(() => Items);
            }
        }

        public ItemDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged<ItemDTO>(() => SelectedItem);
                if (SelectedItem != null)
                {
                    ItemQuantityDTO itQ = _itemQuantityService.GetByCriteria(SelectedWarehouse.Id, SelectedItem.Id);
                    if (itQ != null)
                    {
                        if (itQ.QuantityOnHand != null) StoreCurrentQty = (decimal) itQ.QuantityOnHand;
                    }
                }
            }
        }

        public void GetLiveItems()
        {
            var criteria = new SearchCriteria<ItemDTO>();
            criteria.FiList.Add(i => i.ItemType != ItemTypes.PurchaseForUse && i.ItemType != ItemTypes.PurchaseProcess);

            Items = new ObservableCollection<ItemDTO>(_itemService.GetAll(criteria).OrderBy(i => i.Id).ToList());
        }

        #endregion

        #region Warehouse

        private IEnumerable<WarehouseDTO> _warehouses;
        private WarehouseDTO _selectedWarehouse;

        public IEnumerable<WarehouseDTO> Warehouses
        {
            get { return _warehouses; }
            set
            {
                _warehouses = value;
                RaisePropertyChanged<IEnumerable<WarehouseDTO>>(() => Warehouses);
            }
        }

        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);
            }
        }

        public void GetWarehouses()
        {
            Warehouses = Singleton.WarehousesList;
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