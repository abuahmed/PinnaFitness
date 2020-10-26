using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum RoleTypes
    {
        [Description("Members Mgmt")]
        MemberEntry,
        [Description("Trainers Mgmt")]
        TrainersEntry,
        [Description("Equipments Mgmt")]
        EquipmentsEntry,

        [Description("Subscription Mgmt")]
        SubscriptionEntry,
        [Description("Facility Mgmt")]
        FacilityEntry,
        [Description("Package Mgmt")]
        PackageEntry, 
       
        [Description("Schedule Mgmt")]
        ScheduleMgmt,
        [Description("Attendance Mgmt")]
        AttendanceMgmt,
        [Description("Assessment Mgmt")]
        AssessmentMgmt,

        [Description("Members List")]
        MembersList,

        [Description("Users Mgt")]
        UsersMgmt,
        [Description("Users Privilege Mgmt")]
        UsersPrivilegeMgmt,
        [Description("Backup and Restore Mgmt")]
        BackupRestore,
        [Description("Company Mgmt")]
        CompanyMgmt,
        [Description("Store Mgmt")]
        WarehouseMgmt,

        [Description("Members Edit")]
        MemberEdit,
        [Description("Members Delete")]
        MemberDelete,
        [Description("Members Id Card")]
        MemberIdCard,

        [Description("Adding Packages")]
        PackageAdd,
        [Description("Editing Packages")]
        PackageEdit,
        [Description("Renewing Packages")]
        PackageRenewal,
        [Description("Extending Packages")]
        PackageExtension,
        [Description("Editing Package Expiry Date")]
        PackageEndDateEdit,

        [Description("Attendance Edit")]
        AttendanceEdit,
        [Description("Attendance Delete")]
        AttendanceDelete,

        [Description("DashBoard Management")]
        DashBoardMgmt,

        [Description("Items Management")]
        ItemsMgmt,
        [Description("OnHand Management")]
        OnHandMgmt,

        [Description("Receive Stock ")]
        ReceiveStock,
        [Description("Sell Stock")]
        SellStock,
        [Description("Transfer Stock")]
        TransferStock,

        [Description("Transfer Request")]
        TransferRequest,
        [Description("Transfer Send")]
        TransferSend,
        [Description("Transfer Receive")]
        TransferReceive,

        [Description("Expense Entry")]
        ExpenseEntry,
        [Description("Expense Edit")]
        ExpenseEdit,
        [Description("Expense Delete")]
        ExpenseDelete,

        [Description("Suppliers Entry")]
        SupplierEntry,

        [Description("Maraki Reports")]
        PosReports,

    }
}
