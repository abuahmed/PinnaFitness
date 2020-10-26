using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;

namespace PinnaFit.WPF.ViewModel
{
    public class ExpenseLoanEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IBusinessPartnerService _businessPartnerService;
        private static IPaymentService _paymentService;
        //private static IUnitOfWork _unitOfWork;
        private PaymentTypes _paymentType;
        private PaymentDTO _selectedPayment;
        private string _headerText, _businessPartnerPerson;

        private ICommand _addNewPaymentCommand, _savePaymentCommand, _closeExpenseLoanViewCommand,
            _paymentDateViewCommand, _supplierViewCommand;
        #endregion

        #region Constructor
        public ExpenseLoanEntryViewModel()
        {
            CleanUp();
            CheckRoles();
            //_unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            _paymentService=new PaymentService();
            _businessPartnerService = new BusinessPartnerService();

            GetWarehouses();
            GetLiveBusinessPartners();

            Messenger.Default.Register<PaymentTypes>(this, (message) =>
            {
                PaymentType = message;
            });
            Messenger.Default.Register<PaymentDTO>(this, (message) =>
            {
                SelectedPayment = _paymentService.Find(message.Id.ToString());// _unitOfWork.Repository<PaymentDTO>().FindById(message.Id);
            });

        }
        public static void CleanUp()
        {
            //if (_unitOfWork != null)
            //    _unitOfWork.Dispose();
            if (_businessPartnerService != null)
                _businessPartnerService.Dispose();
            if (_paymentService != null)
                _paymentService.Dispose();
        }
        #endregion

        #region Public Properties
        public PaymentTypes PaymentType
        {
            get { return _paymentType; }
            set
            {
                _paymentType = value;
                RaisePropertyChanged<PaymentTypes>(() => PaymentType);
                switch (PaymentType)
                {
                    case PaymentTypes.CashOut:
                        HeaderText = "ወጪ ማስገቢያ";
                        BusinessPartnerPerson = "Given To:";
                        break;
                    case PaymentTypes.CashIn:
                        HeaderText = "Add Cash Loan";
                        BusinessPartnerPerson = "Accepted From:";
                        break;
                }
                if (PaymentType == PaymentTypes.Sale || SelectedPayment != null) return;
                AddNewPayment();
            }
        }
        public PaymentDTO SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                RaisePropertyChanged<PaymentDTO>(() => SelectedPayment);
                if (SelectedPayment != null && SelectedPayment.Id != 0)
                {
                    PaymentType = SelectedPayment.PaymentType;
                    switch (SelectedPayment.PaymentType)
                    {
                        case PaymentTypes.CashOut:
                            HeaderText = "ወጪ ማስተካከያ";
                            break;
                        case PaymentTypes.CashIn:
                            HeaderText = "Edit Cash Loan";
                            break;
                    }
                    if (Warehouses != null)
                        SelectedWarehouse = Warehouses.FirstOrDefault(w => w.Id == SelectedPayment.WarehouseId);
                    if (SelectedPayment.BusinessPartnerId != null)
                        SelectedBusinessPartner =
                            BusinessPartners.FirstOrDefault(b => b.Id == SelectedPayment.BusinessPartnerId);
                }
            }
        }

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }
        public string BusinessPartnerPerson
        {
            get { return _businessPartnerPerson; }
            set
            {
                _businessPartnerPerson = value;
                RaisePropertyChanged<string>(() => BusinessPartnerPerson);
            }
        }
        #endregion

        #region Vendors
        
        private IEnumerable<BusinessPartnerDTO> _businessPartners;
        private ObservableCollection<BusinessPartnerDTO> _filteredBusinessPartners;
        private BusinessPartnerDTO _selectedBusinessPartner;

        public BusinessPartnerDTO SelectedBusinessPartner
        {
            get { return _selectedBusinessPartner; }
            set
            {
                _selectedBusinessPartner = value;
                RaisePropertyChanged<BusinessPartnerDTO>(() => SelectedBusinessPartner);

            }
        }

        public IEnumerable<BusinessPartnerDTO> BusinessPartnerList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<BusinessPartnerDTO>>(() => BusinessPartnerList);
            }
        }
        public ObservableCollection<BusinessPartnerDTO> BusinessPartners
        {
            get { return _filteredBusinessPartners; }
            set
            {
                _filteredBusinessPartners = value;
                RaisePropertyChanged<ObservableCollection<BusinessPartnerDTO>>(() => BusinessPartners);

            }
        }
        public void GetLiveBusinessPartners()
        {
            var criteria = new SearchCriteria<BusinessPartnerDTO>();

            BusinessPartnerList = _businessPartnerService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            BusinessPartners = new ObservableCollection<BusinessPartnerDTO>(BusinessPartnerList);
        } 
        #endregion

        #region Commands
        public ICommand AddNewPaymentCommand
        {
            get
            {
                return _addNewPaymentCommand ?? (_addNewPaymentCommand = new RelayCommand(AddNewPayment));
            }
        }
        private void AddNewPayment()
        {
            if (Warehouses != null)
                SelectedWarehouse = Warehouses.FirstOrDefault();
            else return;

            if (SelectedWarehouse != null && SelectedWarehouse.Id != -1 && SelectedPayment == null)
                SelectedPayment = new PaymentDTO
                {
                    PaymentType = PaymentType,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = PaymentMethods.Cash,
                    Status = PaymentStatus.NotCleared,
                    WarehouseId = SelectedWarehouse.Id
                };
        }

        public ICommand SavePaymentCommand
        {
            get
            {
                return _savePaymentCommand ?? (_savePaymentCommand = new RelayCommand<Object>(SavePayment, CanSave));
            }
        }
        private void SavePayment(object obj)
        {
            SelectedPayment.WarehouseId = SelectedWarehouse.Id;
            if (SelectedBusinessPartner != null)
                SelectedPayment.BusinessPartnerId = SelectedBusinessPartner.Id;

            var stat=_paymentService.InsertOrUpdate(SelectedPayment);
    
            if(string.IsNullOrEmpty(stat))
            CloseWindow(obj);
            else
                MessageBox.Show("Problem on Saving the expense"+Environment.NewLine+stat);
            
        }

        public ICommand CloseExpenseLoanViewCommand
        {
            get
            {
                return _closeExpenseLoanViewCommand ?? (_closeExpenseLoanViewCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        public void CloseWindow(object obj)
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
        public ICommand PaymentDateViewCommand
        {
            get
            {
                return _paymentDateViewCommand ??
                       (_paymentDateViewCommand = new RelayCommand(PaymentDate));
            }
        }

        public void PaymentDate()
        {
            var calConv = new CalendarConvertor(SelectedPayment.PaymentDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedPayment.PaymentDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand SupplierViewCommand
        {
            get
            {
                return _supplierViewCommand ??
                       (_supplierViewCommand = new RelayCommand(SupplierEntry));
            }
        }
        public void SupplierEntry()
        {
            var calConv = new BusinessPartnerEntry();
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                GetLiveBusinessPartners();
            }
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
            Warehouses = Singleton.WarehousesList.Where(w => w.Id != -1).ToList();
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
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
