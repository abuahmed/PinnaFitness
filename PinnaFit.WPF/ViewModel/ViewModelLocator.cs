using PinnaFit.Core;
using PinnaFit.Core.Enumerations;

namespace PinnaFit.WPF.ViewModel
{
    public class ViewModelLocator
    {
        private static Bootstrapper _bootStrapper;

        public ViewModelLocator()
        {
            //Add Code to choose the server/database the user wants to connect to, the line below depends on it
            Singleton.Edition = PinnaFitEdition.ServerEdition;
            if (_bootStrapper == null)
                _bootStrapper = new Bootstrapper();
        }

        public SplashScreenViewModel Splash
        {
            get
            {
                return _bootStrapper.Container.Resolve<SplashScreenViewModel>();
            }
        }
        public CalendarConvertorViewModel CalendarConvertor
        {
            get
            {
                return _bootStrapper.Container.Resolve<CalendarConvertorViewModel>();
            }
        }
        public ActivationViewModel Activation
        {
            get
            {
                return _bootStrapper.Container.Resolve<ActivationViewModel>();
            }
        }
        public DashBoardViewModel DashBoard
        {
            get
            {
                return _bootStrapper.Container.Resolve<DashBoardViewModel>();
            }
        }
        public CategoryViewModel Categories
        {
            get
            {
                return _bootStrapper.Container.Resolve<CategoryViewModel>();
            }
        }
        public DurationViewModel Duration
        {
            get
            {
                return _bootStrapper.Container.Resolve<DurationViewModel>();
            }
        }

        public CompanyViewModel Company
        {
            get
            {
                return _bootStrapper.Container.Resolve<CompanyViewModel>();
            }
        }
        public MemberViewModel Member
        {
            get
            {
                return _bootStrapper.Container.Resolve<MemberViewModel>();
            }
        }
        public MemberEntryViewModel MemberEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<MemberEntryViewModel>();
            }
        }
        public TrainerViewModel Trainer
        {
            get
            {
                return _bootStrapper.Container.Resolve<TrainerViewModel>();
            }
        }
        public EquipmentViewModel Equipment
        {
            get
            {
                return _bootStrapper.Container.Resolve<EquipmentViewModel>();
            }
        }
        public AddressViewModel AddressVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<AddressViewModel>();
            }
        }
        public BusinessPartnerEntryViewModel BusinessPartnerEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<BusinessPartnerEntryViewModel>();
            }
        }
        public ContactPersonViewModel ContactPerson
        {
            get
            {
                return _bootStrapper.Container.Resolve<ContactPersonViewModel>();
            }
        }
        public SubscriptionViewModel Subscription
        {
            get
            {
                return _bootStrapper.Container.Resolve<SubscriptionViewModel>();
            }
        }
        public ServiceViewModel Service
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServiceViewModel>();
            }
        }
        public FacilityViewModel Facility
        {
            get
            {
                return _bootStrapper.Container.Resolve<FacilityViewModel>();
            }
        }
        public PackagesViewModel Packages
        {
            get
            {
                return _bootStrapper.Container.Resolve<PackagesViewModel>();
            }
        }
        public MemberSubscriptionViewModel MemberSubscription
        {
            get
            {
                return _bootStrapper.Container.Resolve<MemberSubscriptionViewModel>();
            }
        }
        public PervisitSubscriptionViewModel PervisitSubscription
        {
            get
            {
                return _bootStrapper.Container.Resolve<PervisitSubscriptionViewModel>();
            }
        }
        
        public AssessmentViewModel MemberAssessment
        {
            get
            {
                return _bootStrapper.Container.Resolve<AssessmentViewModel>();
            }
        }
        public MemberAssessmentMoreViewModel MemberAssessmentMore
        {
            get
            {
                return _bootStrapper.Container.Resolve<MemberAssessmentMoreViewModel>();
            }
        }
        public MembersListViewModel MembersList
        {
            get
            {
                return _bootStrapper.Container.Resolve<MembersListViewModel>();
            }
        }
        public AttendanceListViewModel AttendanceList
        {
            get
            {
                return _bootStrapper.Container.Resolve<AttendanceListViewModel>();
            }
        }
        public AttendanceEntryViewModel MemberAttendance
        {
            get
            {
                return _bootStrapper.Container.Resolve<AttendanceEntryViewModel>();
            }
        }

        public TimeTableViewModel TimeTable
        {
            get
            {
                return _bootStrapper.Container.Resolve<TimeTableViewModel>();
            }
        }

        public LoginViewModel Login
        {
            get
            {
                return _bootStrapper.Container.Resolve<LoginViewModel>();
            }
        }
        public ChangePasswordViewModel ChangePassword
        {
            get
            {
                return _bootStrapper.Container.Resolve<ChangePasswordViewModel>();
            }
        }
        public MainViewModel Main
        {
            get
            {
                return _bootStrapper.Container.Resolve<MainViewModel>();
            }
        }
        public UserViewModel User
        {
            get
            {
                return _bootStrapper.Container.Resolve<UserViewModel>();
            }
        }
        public BackupRestoreViewModel BackupRestore
        {
            get
            {
                return _bootStrapper.Container.Resolve<BackupRestoreViewModel>();
            }
        }
        public ReportViewerViewModel ReportViewerCommon
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReportViewerViewModel>();
            }
        }

        public ItemEntryViewModel ItemEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<ItemEntryViewModel>();
            }
        }
        public SellItemEntryViewModel SellItemEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<SellItemEntryViewModel>();
            }
        }
        public SellItemHelperViewModel SellItemHelper
        {
            get
            {
                return _bootStrapper.Container.Resolve<SellItemHelperViewModel>();
            }
        }
        public ItemViewModel Item
        {
            get
            {
                return _bootStrapper.Container.Resolve<ItemViewModel>();
            }
        }
        public WarehouseViewModel Warehouse
        {
            get
            {
                return _bootStrapper.Container.Resolve<WarehouseViewModel>();
            }
        }
        public OnHandInventoryViewModel OnHandInventory
        {
            get
            {
                return _bootStrapper.Container.Resolve<OnHandInventoryViewModel>();
            }
        }
        public ReceiveStockViewModel ReceiveStock
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReceiveStockViewModel>();
            }
        }
        public ExpenseLoanViewModel ExpenseLoan
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpenseLoanViewModel>();
            }
        }
        public ExpenseLoanEntryViewModel ExpenseLoanEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpenseLoanEntryViewModel>();
            }
        }

        public BrowserViewModel Browser
        {
            get
            {
                return _bootStrapper.Container.Resolve<BrowserViewModel>();
            }
        }
        public MessageDisplayViewModel MessageDisplay
        {
            get
            {
                return _bootStrapper.Container.Resolve<MessageDisplayViewModel>();
            }
        }

        public static void Cleanup()
        {

        }
    }
}