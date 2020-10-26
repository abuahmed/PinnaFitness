using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Core.Common
{
    public class UserRolesModel : EntityBase
    {
        public UserRolesModel()
        {
            #region Role Visibilities
            
            MemberEntry = CommonUtility.UserHasRole(RoleTypes.MemberEntry) ? "Visible" : "Collapsed";
            TrainersEntry = CommonUtility.UserHasRole(RoleTypes.TrainersEntry) ? "Visible" : "Collapsed";
            EquipmentsEntry = CommonUtility.UserHasRole(RoleTypes.EquipmentsEntry) ? "Visible" : "Collapsed";

            SubscriptionEntry = CommonUtility.UserHasRole(RoleTypes.SubscriptionEntry) ? "Visible" : "Collapsed";
            FacilityEntry = CommonUtility.UserHasRole(RoleTypes.FacilityEntry) ? "Visible" : "Collapsed";
            PackageEntry = CommonUtility.UserHasRole(RoleTypes.PackageEntry) ? "Visible" : "Collapsed";

            ScheduleMgmt = CommonUtility.UserHasRole(RoleTypes.ScheduleMgmt) ? "Visible" : "Collapsed";
            AttendanceMgmt = CommonUtility.UserHasRole(RoleTypes.AttendanceMgmt) ? "Visible" : "Collapsed";
            AttendanceEdit = CommonUtility.UserHasRole(RoleTypes.AttendanceEdit) ? "Visible" : "Collapsed";
            AttendanceDelete = CommonUtility.UserHasRole(RoleTypes.AttendanceDelete) ? "Visible" : "Collapsed";
            AssessmentMgmt = CommonUtility.UserHasRole(RoleTypes.AssessmentMgmt) ? "Visible" : "Collapsed";
            
            MembersList = CommonUtility.UserHasRole(RoleTypes.MembersList) ? "Visible" : "Collapsed";
            
            UsersMgmt = CommonUtility.UserHasRole(RoleTypes.UsersMgmt) ? "Visible" : "Collapsed";
            UsersPrivilegeMgmt = CommonUtility.UserHasRole(RoleTypes.UsersPrivilegeMgmt) ? "Visible" : "Collapsed";
            BackupRestore = CommonUtility.UserHasRole(RoleTypes.BackupRestore) ? "Visible" : "Collapsed";

            Company = CommonUtility.UserHasRole(RoleTypes.CompanyMgmt) ? "Visible" : "Collapsed";
            WarehouseMgmt = CommonUtility.UserHasRole(RoleTypes.WarehouseMgmt) ? "Visible" : "Collapsed";

            MemberEdit = CommonUtility.UserHasRole(RoleTypes.MemberEdit) ? "Visible" : "Collapsed";
            MemberDelete = CommonUtility.UserHasRole(RoleTypes.MemberDelete) ? "Visible" : "Collapsed";
            MemberIdCard = CommonUtility.UserHasRole(RoleTypes.MemberIdCard) ? "Visible" : "Collapsed";

            PackageAdd = CommonUtility.UserHasRole(RoleTypes.PackageAdd) ? "Visible" : "Collapsed";
            PackageEdit = CommonUtility.UserHasRole(RoleTypes.PackageEdit) ? "Visible" : "Collapsed";
            PackageRenewal = CommonUtility.UserHasRole(RoleTypes.PackageRenewal) ? "Visible" : "Collapsed";
            PackageExtension = CommonUtility.UserHasRole(RoleTypes.PackageExtension) ? "Visible" : "Collapsed";
            PackageEndDateEdit = CommonUtility.UserHasRole(RoleTypes.PackageEndDateEdit) ? "Visible" : "Collapsed";

            DashboardMgmt = CommonUtility.UserHasRole(RoleTypes.DashBoardMgmt) ? "Visible" : "Collapsed";

            ItemsMgmt = CommonUtility.UserHasRole(RoleTypes.ItemsMgmt) ? "Visible" : "Collapsed";
            OnHandMgmt = CommonUtility.UserHasRole(RoleTypes.OnHandMgmt) ? "Visible" : "Collapsed";

            ReceiveStock = CommonUtility.UserHasRole(RoleTypes.ReceiveStock) ? "Visible" : "Collapsed";
            SellStock = CommonUtility.UserHasRole(RoleTypes.SellStock) ? "Visible" : "Collapsed";
            TransferStock = CommonUtility.UserHasRole(RoleTypes.TransferStock) ? "Visible" : "Collapsed";

            TransferRequest = CommonUtility.UserHasRole(RoleTypes.TransferRequest) ? "Visible" : "Collapsed";
            TransferSend = CommonUtility.UserHasRole(RoleTypes.TransferSend) ? "Visible" : "Collapsed";
            TransferReceive = CommonUtility.UserHasRole(RoleTypes.TransferReceive) ? "Visible" : "Collapsed";

            PosReports = CommonUtility.UserHasRole(RoleTypes.PosReports) ? "Visible" : "Collapsed";

            ExpenseEntry = CommonUtility.UserHasRole(RoleTypes.ExpenseEntry) ? "Visible" : "Collapsed";
            ExpenseEdit = CommonUtility.UserHasRole(RoleTypes.ExpenseEdit) ? "Visible" : "Collapsed";
            ExpenseDelete = CommonUtility.UserHasRole(RoleTypes.ExpenseDelete) ? "Visible" : "Collapsed";

            SuppliersEntry = CommonUtility.UserHasRole(RoleTypes.SupplierEntry) ? "Visible" : "Collapsed";
            #endregion
        }

        #region Public Properties

        public string Files
        {
            get { return GetValue(() => Files); }
            set { SetValue(() => Files, value); }
        }
        public string MemberEntry
        {
            get { return GetValue(() => MemberEntry); }
            set { SetValue(() => MemberEntry, value); }
        }
        public string TrainersEntry
        {
            get { return GetValue(() => TrainersEntry); }
            set { SetValue(() => TrainersEntry, value); }
        }
        public string EquipmentsEntry
        {
            get { return GetValue(() => EquipmentsEntry); }
            set { SetValue(() => EquipmentsEntry, value); }
        }

        public string SubscriptionEntry
        {
            get { return GetValue(() => SubscriptionEntry); }
            set { SetValue(() => SubscriptionEntry, value); }
        }
        public string FacilityEntry
        {
            get { return GetValue(() => FacilityEntry); }
            set { SetValue(() => FacilityEntry, value); }
        }
        public string PackageEntry
        {
            get { return GetValue(() => PackageEntry); }
            set { SetValue(() => PackageEntry, value); }
        }

        public string Tasks
        {
            get { return GetValue(() => Tasks); }
            set { SetValue(() => Tasks, value); }
        }
        public string ScheduleMgmt
        {
            get { return GetValue(() => ScheduleMgmt); }
            set { SetValue(() => ScheduleMgmt, value); }
        }
        public string AttendanceMgmt
        {
            get { return GetValue(() => AttendanceMgmt); }
            set { SetValue(() => AttendanceMgmt, value); }
        }
        public string AssessmentMgmt
        {
            get { return GetValue(() => AssessmentMgmt); }
            set { SetValue(() => AssessmentMgmt, value); }
        }

        public string Reports
        {
            get { return GetValue(() => Reports); }
            set { SetValue(() => Reports, value); }
        }
        public string MembersList
        {
            get { return GetValue(() => MembersList); }
            set { SetValue(() => MembersList, value); }
        }

        public string Admin
        {
            get { return GetValue(() => Admin); }
            set { SetValue(() => Admin, value); }
        }
        public string UsersMgmt
        {
            get { return GetValue(() => UsersMgmt); }
            set { SetValue(() => UsersMgmt, value); }
        }
        public string UsersPrivilegeMgmt
        {
            get { return GetValue(() => UsersPrivilegeMgmt); }
            set { SetValue(() => UsersPrivilegeMgmt, value); }
        }
        public string BackupRestore
        {
            get { return GetValue(() => BackupRestore); }
            set { SetValue(() => BackupRestore, value); }
        }
        public string Company
        {
            get { return GetValue(() => Company); }
            set { SetValue(() => Company, value); }
        }
        public string WarehouseMgmt
        {
            get { return GetValue(() => WarehouseMgmt); }
            set { SetValue(() => WarehouseMgmt, value); }
        }

        public string MemberEdit
        {
            get { return GetValue(() => MemberEdit); }
            set { SetValue(() => MemberEdit, value); }
        }
        public string MemberDelete
        {
            get { return GetValue(() => MemberDelete); }
            set { SetValue(() => MemberDelete, value); }
        }
        public string MemberIdCard
        {
            get { return GetValue(() => MemberIdCard); }
            set { SetValue(() => MemberIdCard, value); }
        }

        public string PackageAdd
        {
            get { return GetValue(() => PackageAdd); }
            set { SetValue(() => PackageAdd, value); }
        }
        public string PackageEdit
        {
            get { return GetValue(() => PackageEdit); }
            set { SetValue(() => PackageEdit, value); }
        }
        public string PackageRenewal
        {
            get { return GetValue(() => PackageRenewal); }
            set { SetValue(() => PackageRenewal, value); }
        }
        public string PackageExtension
        {
            get { return GetValue(() => PackageExtension); }
            set { SetValue(() => PackageExtension, value); }
        }
        public string PackageEndDateEdit
        {
            get { return GetValue(() => PackageEndDateEdit); }
            set { SetValue(() => PackageEndDateEdit, value); }
        }

        public string AttendanceEdit
        {
            get { return GetValue(() => AttendanceEdit); }
            set { SetValue(() => AttendanceEdit, value); }
        }
        public string AttendanceDelete
        {
            get { return GetValue(() => AttendanceDelete); }
            set { SetValue(() => AttendanceDelete, value); }
        }

        public string DashboardMgmt
        {
            get { return GetValue(() => DashboardMgmt); }
            set { SetValue(() => DashboardMgmt, value); }
        }

        public string ItemsMgmt
        {
            get { return GetValue(() => ItemsMgmt); }
            set { SetValue(() => ItemsMgmt, value); }
        }
        public string OnHandMgmt
        {
            get { return GetValue(() => OnHandMgmt); }
            set { SetValue(() => OnHandMgmt, value); }
        }

        public string ReceiveStock
        {
            get { return GetValue(() => ReceiveStock); }
            set { SetValue(() => ReceiveStock, value); }
        }
        public string SellStock
        {
            get { return GetValue(() => SellStock); }
            set { SetValue(() => SellStock, value); }
        }
        public string TransferStock
        {
            get { return GetValue(() => TransferStock); }
            set { SetValue(() => TransferStock, value); }
        }

        public string TransferRequest
        {
            get { return GetValue(() => TransferRequest); }
            set { SetValue(() => TransferRequest, value); }
        }
        public string TransferSend
        {
            get { return GetValue(() => TransferSend); }
            set { SetValue(() => TransferSend, value); }
        }
        public string TransferReceive
        {
            get { return GetValue(() => TransferReceive); }
            set { SetValue(() => TransferReceive, value); }
        }

        public string PosReports
        {
            get { return GetValue(() => PosReports); }
            set { SetValue(() => PosReports, value); }
        }

        public string ExpenseEntry
        {
            get { return GetValue(() => ExpenseEntry); }
            set { SetValue(() => ExpenseEntry, value); }
        }
        public string ExpenseEdit
        {
            get { return GetValue(() => ExpenseEdit); }
            set { SetValue(() => ExpenseEdit, value); }
        }
        public string ExpenseDelete
        {
            get { return GetValue(() => ExpenseDelete); }
            set { SetValue(() => ExpenseDelete, value); }
        }

        public string SuppliersEntry
        {
            get { return GetValue(() => SuppliersEntry); }
            set { SetValue(() => SuppliersEntry, value); }
        }
        #endregion
    }
}