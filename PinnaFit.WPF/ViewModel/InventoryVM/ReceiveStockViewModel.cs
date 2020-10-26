using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace PinnaFit.WPF.ViewModel
{
    public class ReceiveStockViewModel : ViewModelBase
    {
        #region Fields
        
        private TransactionTypes _transactionType;
        private static ITransactionService _transactionService;
        private static IItemService _itemService;
        private IEnumerable<TransactionHeaderDTO> _transactionHeaders;

        private ObservableCollection<ItemDTO> _items;
        private ItemDTO _selectedItem;

        private ObservableCollection<TransactionHeaderDTO> _filteredTransactionHeaders;
        private TransactionHeaderDTO _selectedTransactionHeader;

        private ICommand _addNewTransactionHeaderViewCommand,
            _saveTransactionHeaderViewCommand,
            _deleteTransactionHeaderViewCommand,
            _postTransactionCommand, _unPostTransactionCommand, _showListViewCommand;

        private string _transactionText;
        private bool _saveHeaderEnability, _unPostEnability, _requestEnability, _sendEnability, _receiveEnability;
        #endregion

        #region Constructor

        public ReceiveStockViewModel()
        {
            //FillPeriodCombo();
            
            Messenger.Default.Register<TransactionTypes>(this, (message) =>
            {
                TransactionType = message;
            });
         
        }

        public void Load()
        { 
            CleanUp();
            _transactionService = new TransactionService();
            _itemService = new ItemService();
            
            GetLiveItems();
            
            CheckRoles();
            GetLiveTransactionHeaders();
        }

        public static void CleanUp()
        {
            if (_transactionService != null)
                _transactionService.Dispose();
            if (_itemService != null)
                _itemService.Dispose();
        }

        #endregion
        
        #region Headers

        #region Public Properties

        public bool SaveHeaderEnability
        {
            get { return _saveHeaderEnability; }
            set
            {
                _saveHeaderEnability = value;
                RaisePropertyChanged<bool>(() => SaveHeaderEnability);
            }
        }
        public bool UnPostEnability
        {
            get { return _unPostEnability; }
            set
            {
                _unPostEnability = value;
                RaisePropertyChanged<bool>(() => UnPostEnability);
            }
        }
        public bool RequestEnability
        {
            get { return _requestEnability; }
            set
            {
                _requestEnability = value;
                RaisePropertyChanged<bool>(() => RequestEnability);
            }
        }
        public bool SendEnability
        {
            get { return _sendEnability; }
            set
            {
                _sendEnability = value;
                RaisePropertyChanged<bool>(() => SendEnability);
            }
        }
        public bool ReceiveEnability
        {
            get { return _receiveEnability; }
            set
            {
                _receiveEnability = value;
                RaisePropertyChanged<bool>(() => ReceiveEnability);
            }
        }
        public string TransactionText
        {
            get { return _transactionText; }
            set
            {
                _transactionText = value;
                RaisePropertyChanged<string>(() => TransactionText);
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
                        case TransactionTypes.TransferStock:
                            TransactionText = "የእቃ ዝውውር";
                            StockTransferVisibility = "Visible";
                            //SelectedPeriod = FilterPeriods.FirstOrDefault(t => t.Value == 15);
                            break;
                        case TransactionTypes.RecieveStock:
                            TransactionText = "የእቃ መረከቢያ";
                            StockTransferVisibility = "Collapsed";
                            //SelectedPeriod = FilterPeriods.FirstOrDefault();
                            break;
                    }
                    GetWarehouses();
                    if (Warehouses != null && Warehouses.Any())
                    {
                        SelectedWarehouse = SelectedWarehouse ?? Warehouses.FirstOrDefault();
                    }
                    Load();
                }
            }
        }

        public TransactionHeaderDTO SelectedTransactionHeader
        {
            get { return _selectedTransactionHeader; }
            set
            {
                _selectedTransactionHeader = value;
                RaisePropertyChanged<TransactionHeaderDTO>(() => SelectedTransactionHeader);
                if (SelectedTransactionHeader == null) return;

                switch (SelectedTransactionHeader.Status)
                {
                    case TransactionStatus.Posted:
                        UnPostEnability = true;
                        SaveHeaderEnability = false;
                        ReceiveEnability = false;
                        break;
                    case TransactionStatus.Received:
                        UnPostEnability = true;
                        SaveHeaderEnability = false;
                        RequestEnability = false;
                        SendEnability = false;
                        ReceiveEnability = false;
                        break;
                    case TransactionStatus.Sent:
                        UnPostEnability = true;
                        SaveHeaderEnability = false;
                        RequestEnability = false;
                        SendEnability = false;
                        ReceiveEnability = true;
                        break;
                    case TransactionStatus.Requested:
                        UnPostEnability = true;
                        SaveHeaderEnability = true;
                        RequestEnability = false;
                        SendEnability = true;
                        ReceiveEnability = false;
                        break;
                    case TransactionStatus.New:
                        UnPostEnability = false;
                        SaveHeaderEnability = true;
                        RequestEnability = true;
                        SendEnability = false;
                        ReceiveEnability = false;
                        break;
                }

                if (TransactionType == TransactionTypes.TransferStock)
                    SelectedToWarehouse = ToWarehouses.FirstOrDefault(w => w.Id == SelectedTransactionHeader.ToWarehouseId);

                if (TransactionType == TransactionTypes.RecieveStock && SelectedTransactionHeader.Status == TransactionStatus.New)
                    ReceiveEnability = true;

                StockReceiveDate = SelectedTransactionHeader.TransactionDate;
                GetTransactionLines();
            }
        }

        public IEnumerable<TransactionHeaderDTO> TransactionHeaderList
        {
            get { return _transactionHeaders; }
            set
            {
                _transactionHeaders = value;
                RaisePropertyChanged<IEnumerable<TransactionHeaderDTO>>(() => TransactionHeaderList);
            }
        }

        public ObservableCollection<TransactionHeaderDTO> TransactionHeaders
        {
            get { return _filteredTransactionHeaders; }
            set
            {
                _filteredTransactionHeaders = value;
                RaisePropertyChanged<ObservableCollection<TransactionHeaderDTO>>(() => TransactionHeaders);

                if (TransactionHeaders != null && TransactionHeaders.Any())
                    SelectedTransactionHeader = TransactionHeaders.FirstOrDefault();
                else
                    AddNewTransactionHeader();
            }
        }

        #endregion

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
            if(SelectedWarehouse==null || SelectedWarehouse.Id==-1)
                return;

            SelectedTransactionHeader = new TransactionHeaderDTO
            {
                TransactionType = TransactionType,
                Status = TransactionStatus.New,
                TransactionDate = DateTime.Now,
                WarehouseId = SelectedWarehouse.Id
            };

            StockReceiveDate = DateTime.Now;
            TransactionLines=new ObservableCollection<TransactionLineDTO>();

            if (TransactionType == TransactionTypes.TransferStock)
                SelectedToWarehouse = ToWarehouses.FirstOrDefault();

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
                var newObject = SelectedTransactionHeader.Id;

                if(TransactionLines.Count==0)
                {
                    MessageBox.Show("Add Item First");
                    return;
                }

                SelectedTransactionHeader.TransactionDate = StockReceiveDate;

                if (TransactionType == TransactionTypes.TransferStock && SelectedToWarehouse!=null)
                    SelectedTransactionHeader.ToWarehouseId = SelectedToWarehouse.Id;

                var stat = _transactionService.InsertOrUpdate(SelectedTransactionHeader);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save Stock Receive Data"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    TransactionHeaders.Insert(0, SelectedTransactionHeader);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteTransactionHeaderViewCommand
        {
            get
            {
                return _deleteTransactionHeaderViewCommand ??
                       (_deleteTransactionHeaderViewCommand = new RelayCommand<Object>(DeleteTransactionHeader, CanSave));
            }
        }
        private void DeleteTransactionHeader(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedTransactionHeader.Enabled = false;
                    var stat = _transactionService.Disable(SelectedTransactionHeader);
                    if (stat == string.Empty)
                    {
                        TransactionHeaders.Remove(SelectedTransactionHeader);
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
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand PostTransactionCommand
        {
            get
            {
                return _postTransactionCommand ?? (_postTransactionCommand = new RelayCommand(ExcutePostTransaction));
            }
        }
        private void ExcutePostTransaction()
        {
            try
            {
                if (TransactionLines.Count == 0)
                {
                    MessageBox.Show("No Items To Post, Add Item First....");
                    return;
                }

                if (TransactionType == TransactionTypes.TransferStock && SelectedToWarehouse != null)
                    SelectedTransactionHeader.ToWarehouseId = SelectedToWarehouse.Id;

                var stat = _transactionService.Post(SelectedTransactionHeader);
                if (stat != string.Empty)
                    MessageBox.Show("Can't Post"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    Load();//GetLiveTransactionHeaders();

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't post"
                                  + Environment.NewLine + exception.Message, "Can't post", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }

        }

        public ICommand UnPostTransactionCommand
        {
            get
            {
                return _unPostTransactionCommand ?? (_unPostTransactionCommand = new RelayCommand(ExcuteUnPostTransaction));
            }
        }
        private void ExcuteUnPostTransaction()
        {
            if (MessageBox.Show("Are you Sure You want to Un-post this Transaction?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                var stat = _transactionService.UnPost(SelectedTransactionHeader);
                if (stat != string.Empty)
                    MessageBox.Show("Can't unpost"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    Load();//GetLiveTransactionHeaders();
                
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't unpost!!, please try again, after refreshing the window..."
                                  + Environment.NewLine + exception.Message, "Can't unpost", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand ShowListViewCommand
        {
            get
            {
                return _showListViewCommand ??
                       (_showListViewCommand = new RelayCommand(ShowList));
            }
        }
        private void ShowList()
        {
           Load();
        }
        
        #endregion

        public void GetLiveTransactionHeaders()
        {
            var criteria = new SearchCriteria<TransactionHeaderDTO>
            {
                CurrentUserId = Singleton.User.UserId,
                TransactionType = (int)TransactionType,
                Page = 1,
                PageSize = 5
            };
           
            criteria.FiList.Add(f=>f.WarehouseId==SelectedWarehouse.Id);

            //if(SelectedPeriod!=null && SelectedPeriod.Value!=0)
            //criteria.FiList.Add(f => f.Status == (TransactionStatus)SelectedPeriod.Value);

            TransactionHeaderList = _transactionService.GetAll(criteria).ToList();
            
            TransactionHeaders = new ObservableCollection<TransactionHeaderDTO>(TransactionHeaderList);
        } 
        #endregion

        #region Lines

        #region Fields
        private bool _unitPricePlusTax;
        private string _totalItemsValue, _unitPricePlusTaxVisibility;
        private int _totalItemsCounted;
        private ICommand _addTransactionLineCommand, _editTransactionLineCommand, _deleteTransactionLineCommand;
        private ObservableCollection<TransactionLineDTO> _transactionsLines;
        private TransactionLineDTO _selectedTransactionLine;
        #endregion

        #region Public Properties

        public int TotalItemsCounted
        {
            get { return _totalItemsCounted; }
            set
            {
                _totalItemsCounted = value;
                RaisePropertyChanged<int>(() => TotalItemsCounted);
            }
        }
        public bool UnitPricePlusTax
        {
            get { return _unitPricePlusTax; }
            set
            {
                _unitPricePlusTax = value;
                RaisePropertyChanged<bool>(() => UnitPricePlusTax);
            }
        }
        public string TotalItemsValue
        {
            get { return _totalItemsValue; }
            set
            {
                _totalItemsValue = value;
                RaisePropertyChanged<string>(() => TotalItemsValue);
            }
        }
        public string StockTransferVisibility
        {
            get { return _unitPricePlusTaxVisibility; }
            set
            {
                _unitPricePlusTaxVisibility = value;
                RaisePropertyChanged<string>(() => StockTransferVisibility);
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
        public ObservableCollection<TransactionLineDTO> TransactionLines
        {
            get { return _transactionsLines; }
            set
            {
                _transactionsLines = value;
                RaisePropertyChanged<ObservableCollection<TransactionLineDTO>>(() => TransactionLines);

                if (TransactionLines != null)
                {
                    var lineCounts = TransactionLines.Count;
                    var lineValues = TransactionLines.Sum(s => s.LinePrice);
                    if (SelectedTransactionHeader != null)
                    {
                        SelectedTransactionHeader.CountLines = lineCounts;
                    }
                    TotalItemsCounted = lineCounts;
                    TotalItemsValue = lineValues.ToString("N");
                }

                SelectedTransactionHeaderLine=new TransactionLineDTO();
            }
        }
        #endregion

        public void GetTransactionLines()
        {
            if (SelectedTransactionHeader != null && SelectedTransactionHeader.Id != 0)
            {
                var transactionLinesList = _transactionService.GetChilds(SelectedTransactionHeader.Id, false);
                TransactionLines = new ObservableCollection<TransactionLineDTO>(transactionLinesList);
            }
        }

        #region Commands
        public ICommand AddTransactionLineCommand
        {
            get
            {
                return _addTransactionLineCommand ?? (_addTransactionLineCommand = new RelayCommand<Object>(SaveLine));
            }
        }
        private void SaveLine(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Choose Item First");
                     return;
                }
                if (SelectedTransactionHeaderLine.Unit == 0)
                {
                    MessageBox.Show("Qty Should be above 0");
                    return;
                }
                if (TransactionType == TransactionTypes.TransferStock && SelectedToWarehouse != null)
                    SelectedTransactionHeaderLine.Transaction.ToWarehouseId = SelectedToWarehouse.Id;

                SelectedTransactionHeaderLine.ItemId = SelectedItem.Id;
                
                var stat = _transactionService.InsertOrUpdateChild(SelectedTransactionHeaderLine);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save Stock Receive Data"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                    GetTransactionLines();


                SelectedItem = null;

                var txtBox = obj as RadAutoCompleteBox;
                if (txtBox != null) txtBox.Focus();

                SelectedTransactionHeaderLine = new TransactionLineDTO()
                {
                    Transaction = SelectedTransactionHeader
                };
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem adding transaction item, please try again..." + Environment.NewLine +
                    exception.Message + Environment.NewLine +
                    exception.InnerException);
            }
        }
        
        public ICommand DeleteTransactionLineCommand
        {
            get
            {
                return _deleteTransactionLineCommand ?? (_deleteTransactionLineCommand = new RelayCommand<Object>(DeleteLine));
            }
        }
        private void DeleteLine(object obj)
        {
            if (SelectedTransactionHeaderLine == null || SelectedTransactionHeaderLine.Id == 0)
            {
                MessageBox.Show("First choose Item to delete...", "Problem Deleting");
                return;
            }

            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedTransactionHeaderLine.Enabled = false;

                    var stat = _transactionService.DisableChild(SelectedTransactionHeaderLine);
                    if (stat == string.Empty)
                    {
                        GetTransactionLines();
                        
                        var txtBox = obj as RadAutoCompleteBox;
                        if (txtBox != null) txtBox.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                                        + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + exception.Message + Environment.NewLine +
                                    exception.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        
        #endregion

        #region Filter List

        private ICommand  _stockReceiveDateViewCommand;
        private string  _stockReceiveDateText;
        private DateTime  _stockReceiveDate;
        
        public DateTime StockReceiveDate
        {
            get { return _stockReceiveDate; }
            set
            {
                _stockReceiveDate = value;
                RaisePropertyChanged<DateTime>(() => StockReceiveDate);
                if (StockReceiveDate.Year > 1900)
                {
                    StockReceiveDateText = ReportUtility.GetEthCalendarFormated(StockReceiveDate, "-") +
                                    "(" + StockReceiveDate.ToString("dd-MM-yyyy") + ")";
                }
            }
        }

        public string StockReceiveDateText
        {
            get { return _stockReceiveDateText; }
            set
            {
                _stockReceiveDateText = value;
                RaisePropertyChanged<string>(() => StockReceiveDateText);
            }
        }

        public ICommand StockReceiveDateViewCommand
        {
            get
            {
                return _stockReceiveDateViewCommand ??
                       (_stockReceiveDateViewCommand = new RelayCommand(ExcuteStockReceiveDate));
            }
        }

        public void ExcuteStockReceiveDate()
        {
            var bd = StockReceiveDate;
            if (bd.Year < 1900)
                bd = DateTime.Now;

            var calConv = new CalendarConvertor(bd);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                {
                    StockReceiveDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
                    SelectedTransactionHeader.TransactionDate = StockReceiveDate;
                }
            }
        }

        //private ObservableCollection<ListDataItem> _filterPeriods;
        //private ListDataItem _selectedPeriod;
        //private void FillPeriodCombo()
        //{
        //    IList<ListDataItem> filterPeriods2 = new List<ListDataItem>();
        //    filterPeriods2.Add(new ListDataItem { Display = "All", Value = 0 });
        //    filterPeriods2.Add(new ListDataItem { Display = EnumUtil.GetEnumDesc(TransactionStatus.Requested), Value = 15 });
        //    filterPeriods2.Add(new ListDataItem { Display = EnumUtil.GetEnumDesc(TransactionStatus.Sent), Value = 11 });
        //    filterPeriods2.Add(new ListDataItem { Display = EnumUtil.GetEnumDesc(TransactionStatus.Received), Value = 13 });
        //    FilterPeriods = new ObservableCollection<ListDataItem>(filterPeriods2);
        //}

        //public ObservableCollection<ListDataItem> FilterPeriods
        //{
        //    get { return _filterPeriods; }
        //    set
        //    {
        //        _filterPeriods = value;
        //        RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => FilterPeriods);
        //    }
        //}

        //public ListDataItem SelectedPeriod
        //{
        //    get { return _selectedPeriod; }
        //    set
        //    {
        //        _selectedPeriod = value;
        //        RaisePropertyChanged<ListDataItem>(() => SelectedPeriod);
        //        if (SelectedPeriod != null)
        //        {
                    
        //        }
        //    }
        //}

        #endregion

        #region Items
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
                    SelectedTransactionHeaderLine = new TransactionLineDTO()
                    {
                        Transaction = SelectedTransactionHeader,
                        ItemId = SelectedItem.Id,
                        Unit=1
                    };
                }
            }
        }

        public void GetLiveItems()
        {
            var criteria = new SearchCriteria<ItemDTO>();
            //criteria.FiList.Add(i => i.ItemType != ItemTypes.PurchaseForUse && i.ItemType != ItemTypes.PurchaseProcess);
            criteria.FiList.Add(i => i.ItemType != ItemTypes.ProcessForSell);
            Items = new ObservableCollection<ItemDTO>(_itemService.GetAll(criteria).OrderBy(i => i.Id).ToList());

        }
        #endregion

        #region Warehouse
        private IEnumerable<WarehouseDTO> _warehouses, _toWarehouses;
        private WarehouseDTO _selectedWarehouse,_selectedToWarehouse;
        public IEnumerable<WarehouseDTO> Warehouses
        {
            get { return _warehouses; }
            set
            {
                _warehouses = value;
                RaisePropertyChanged<IEnumerable<WarehouseDTO>>(() => Warehouses);
            }
        }
        public IEnumerable<WarehouseDTO> ToWarehouses
        {
            get { return _toWarehouses; }
            set
            {
                _toWarehouses = value;
                RaisePropertyChanged<IEnumerable<WarehouseDTO>>(() => ToWarehouses);
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
        public WarehouseDTO SelectedToWarehouse
        {
            get { return _selectedToWarehouse; }
            set
            {
                _selectedToWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedToWarehouse);
            }
        }
        public void GetWarehouses()
        {
            if (TransactionType == TransactionTypes.TransferStock)
            {
                Warehouses = new WarehouseService(true).GetAll().ToList();
                ToWarehouses = Singleton.WarehousesList;
            }
            else
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
        private string _transferRequest;
        private string _transferSend;


        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        public string TransferRequest
        {
            get { return _transferRequest; }
            set
            {
                _transferRequest = value;
                RaisePropertyChanged<string>(() => TransferRequest);
            }
        }
        public string TransferSend
        {
            get { return _transferSend; }
            set
            {
                _transferSend = value;
                RaisePropertyChanged<string>(() => TransferSend);
            }
        }
        
        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;

            if (TransactionType == TransactionTypes.RecieveStock)
            {
                TransferRequest = "Collapsed";
                TransferSend = "Collapsed";
            }
            else if (TransactionType == TransactionTypes.TransferStock)
            {
                TransferRequest = "Visible";
                TransferSend = UserRoles.TransferSend;// "Visible";
            }
                
        }

        #endregion

    }
}