using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace PinnaFit.WPF.ViewModel
{
    public class SellItemEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IItemService _itemService;
        private static IItemQuantityService _itemQuantityService;
        private static ITransactionService _transactionService;
        private ItemDTO _selectedItem;
        private ObservableCollection<ItemDTO> _items;
        private ICommand _addNewTransactionHeaderViewCommand,_saveTransactionHeaderViewCommand, _closeItemViewCommand;
        private TransactionLineDTO _selectedTransactionLine;
        private TransactionHeaderDTO _selectedTransactionHeader;
        private decimal? _storeCurrentQty;
        private bool _freelyGiven;
        private string _transactionText, _sellStockVisibility, _useStockVisibility;
        private TransactionTypes _transactionType;
        #endregion

        #region Constructor
        public SellItemEntryViewModel()
        {
            CleanUp();
            _itemService = new ItemService();
            _itemQuantityService = new ItemQuantityService();
            _transactionService=new TransactionService();
  
            CheckRoles();
            GetLiveItems();

            Messenger.Default.Register<TransactionTypes>(this, (message) =>
            {
                TransactionType = message;
            });
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

        public string TransactionText
        {
            get { return _transactionText; }
            set
            {
                _transactionText = value;
                RaisePropertyChanged<string>(() => TransactionText);
            }
        }
        public string SellStockVisibility
        {
            get { return _sellStockVisibility; }
            set
            {
                _sellStockVisibility = value;
                RaisePropertyChanged<string>(() => SellStockVisibility);
            }
        }
        public string UseStockVisibility
        {
            get { return _useStockVisibility; }
            set
            {
                _useStockVisibility = value;
                RaisePropertyChanged<string>(() => UseStockVisibility);
            }
        }
        public TransactionTypes TransactionType
        {
            get { return _transactionType; }
            set
            {
                _transactionType = value;
                RaisePropertyChanged<TransactionTypes>(() => TransactionType);

                if (TransactionType != TransactionTypes.All)
                {
                    switch (TransactionType)
                    {
                        case TransactionTypes.SellStock:
                            TransactionText = "የእቃ መሸጫ";
                            SellStockVisibility = "Visible";
                            UseStockVisibility = "Collapsed";
                            break;
                        case TransactionTypes.UseStock:
                            TransactionText = "የእቃ ማዘዣ";
                            UseStockVisibility = "Visible";
                            SellStockVisibility = "Collapsed";
                            break;
                    }
                    GetWarehouses();
                    if (Warehouses != null && Warehouses.Any())
                    {
                        SelectedWarehouse = SelectedWarehouse ?? Warehouses.FirstOrDefault();
                    }
                    //Load();
                }
            }
        }
        #region Commands
        public ICommand AddNewTransactionHeaderViewCommand
        {
            get
            {
                return _addNewTransactionHeaderViewCommand ??
                       (_addNewTransactionHeaderViewCommand = new RelayCommand(AddNewTransactionHeader));
            }
        }
        private void AddNewTransactionHeader()
        {
            if (SelectedWarehouse == null || SelectedWarehouse.Id == -1)
                return;

            SelectedTransactionHeader = new TransactionHeaderDTO
            {
                TransactionType = TransactionType,
                Status = TransactionStatus.Posted,
                TransactionDate = DateTime.Now,
                WarehouseId = SelectedWarehouse.Id
            };
        }

        public ICommand SaveTransactionHeaderViewCommand
        {
            get
            {
                return _saveTransactionHeaderViewCommand ??
                       (_saveTransactionHeaderViewCommand = new RelayCommand<Object>(SaveTransactionHeader, CanSave));
            }
        }
        private void SaveTransactionHeader(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Choose Item First");
                    return;
                }
                
                if (SelectedTransactionHeaderLine != null)
                {
                    if (SelectedTransactionHeaderLine.Unit == 0 || SelectedTransactionHeaderLine.Unit>=StoreCurrentQty)
                    {
                        MessageBox.Show("Qty Should be above the current qty");
                        return;
                    }

                    SelectedTransactionHeaderLine.ItemId = SelectedItem.Id;

                    var stat = _transactionService.InsertOrUpdateChild(SelectedTransactionHeaderLine);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Stock Sell Data"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else
                    {
                        stat = _transactionService.Post(SelectedTransactionHeader);
                        if (stat != string.Empty)
                            MessageBox.Show("Can't Post"
                                            + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        else
                        CloseWindow(obj);
                    }
                        
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        
        public ICommand CloseItemViewCommand
        {
            get
            {
                return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow));
            }
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
            //new NotificationWindow().Show();
            //string str = "title", title2 = "title", text2 = "text";

            //var nic = new NotifyIcon
            //{
            //    Text = str,
            //    BalloonTipText = str,
            //    BalloonTipTitle = title2,
            //    Visible = true
            //};
            //nic.Icon = new System.Drawing.Icon("/PinnaFit.WPF;component/Resources/AppIcon.ico");
            //nic.ShowBalloonTip(1000, title2, text2, ToolTipIcon.Info);

            //var tbi = new TaskbarIcon
            //{
            //    Icon = new System.Drawing.Icon("../../Resources/AppIcon.ico"),
            //    ToolTipText = "hello world"
            //};
            //new TaskbarIcon().ShowBalloonTip("hello", "hello world", new System.Drawing.Icon("../../Resources/AppIcon.ico"));

        }
        #endregion

        public TransactionHeaderDTO SelectedTransactionHeader
        {
            get { return _selectedTransactionHeader; }
            set
            {
                _selectedTransactionHeader = value;
                RaisePropertyChanged<TransactionHeaderDTO>(() => SelectedTransactionHeader);
            }
        }

        public TransactionLineDTO SelectedTransactionHeaderLine
        {
            get { return _selectedTransactionLine; }
            set
            {
                _selectedTransactionLine = value;
                RaisePropertyChanged<TransactionLineDTO>(() => SelectedTransactionHeaderLine);
            }
        }

        #region Items
        public decimal? StoreCurrentQty
        {
            get { return _storeCurrentQty; }
            set
            {
                _storeCurrentQty = value;
                RaisePropertyChanged<decimal?>(() => StoreCurrentQty);
            }
        }
        public bool FreelyGiven
        {
            get { return _freelyGiven; }
            set
            {
                _freelyGiven = value;
                RaisePropertyChanged<bool>(() => FreelyGiven);
                try
                {
                    if (FreelyGiven)
                    {
                        SelectedTransactionHeaderLine.EachPrice = 0;
                        SelectedTransactionHeaderLine.LinePrice = 0;
                        SelectedTransactionHeaderLine.IsFree = true;
                    }
                    else
                    {
                        SelectedTransactionHeaderLine.EachPrice = SelectedItem.SalePrice;
                        SelectedTransactionHeaderLine.LinePrice =
                            (decimal) (SelectedItem.SalePrice*SelectedTransactionHeaderLine.Unit);
                        SelectedTransactionHeaderLine.IsFree = false;
                    }
                }catch{}
            }
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
                        SelectedTransactionHeaderLine = new TransactionLineDTO
                        {
                            Transaction = SelectedTransactionHeader,
                            ItemId = SelectedItem.Id,
                            EachPrice = SelectedItem.SalePrice,
                            Unit = 1
                        };
                        if (itQ.QuantityOnHand != null) StoreCurrentQty = itQ.QuantityOnHand;
                    }
                }
            }
        }

        public void GetLiveItems()
        {
            var criteria = new SearchCriteria<ItemDTO>();
            criteria.FiList.Add(i => i.ItemType != ItemTypes.ProcessForSell);// && i.ItemType != ItemTypes.PurchaseProcess);

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
                if(SelectedWarehouse!=null)
                    AddNewTransactionHeader();
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