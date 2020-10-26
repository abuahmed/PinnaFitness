using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum PaymentMethods
    {
        [Description("Cash")]
        Cash = 0,
        [Description("Credit")]
        Credit = 1,
        [Description("Check")]
        Check = 2
    }
    public enum PaymentStatus
    {
        [Description("Cash Not Deposited")]
        NotDeposited = 0,
        [Description("Not Cleared")]
        NotCleared = 1,
        [Description("Cleared")]
        Cleared = 2,
        [Description("No Payment")]//For Draft Status
        NoPayment = 3,
        [Description("Refunded")]
        Refund = 4,
    }
    public enum PaymentTypes
    {
        //We may only need CashIn & CashOut the others will be replaced by PaymentMethod and Sale/Purchase properties
        [Description("For Sale")]
        Sale = 0,
        [Description("For Purchase")]
        Purchase = 1,
        [Description("Cash Loan")]
        CashIn = 2,
        [Description("Expense")]
        CashOut = 3,
        [Description("Sales Credit")]
        SaleCredit = 4,
        [Description("Purchase Credit")]
        PurchaseCredit = 5
    }

    //public enum CreditLimitTypes
    //{
    //    [Description("By Amount")]
    //    Amount,
    //    [Description("By Transactions")]
    //    Transactions,
    //    [Description("By Both")]
    //    Both
    //}

    //public enum TransactionPaymentStatus
    //{
    //    [Description("Partially Paid")]
    //    PartiallyPaid = 0,
    //    [Description("Fully Paid")]
    //    FullyPaid = 1,
    //    [Description("No Payment")]
    //    NoPayment = 2
    //}
    //public enum PaymentListTypes
    //{
    //    All,
    //    [Description("Cleared")]
    //    Cleared,
    //    [Description("Not Cleared")]
    //    NotCleared,
    //    [Description("OverDue Payments")]
    //    NotClearedandOverdue,
    //    [Description("Cash Not Deposited")]
    //    NotDeposited,
    //    [Description("Cash Deposited Not Cleared")]
    //    DepositedNotCleared,
    //    [Description("Cash Deposited and Cleared")]
    //    DepositedCleared,
    //    [Description("Credit Not Cleared")]
    //    CreditNotCleared,
    //    [Description("Check Not Cleared")]
    //    CheckNotCleared,
    //    [Description("Check Cleared")]
    //    CheckCleared

    //}
    //public enum InvoiceTerms
    //{
    //    [Description("Immediate")]
    //    Immediate,
    //    [Description("After Delivery")]
    //    AfterDelivery,
    //    [Description("After Order Delivered")]
    //    AfterOrderDelivered,
    //    [Description("Customer Schedule After Delivery")]
    //    CustomerScheduleAfterDelivery,
    //    [Description("Do Not Invoice")]
    //    DoNotInvoice

    //}
}
