using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum TransactionStatus
    {
        [Description("አዲስ")]
        New,
        [Description("Draft")]
        Draft,
        [Description("Order Status")]
        Order,
        [Description("Posted Status")]
        Posted,
        [Description("Posted With Less Stock")]
        PostedWithLessStock,
        [Description("Completed")]
        Completed,
        [Description("Closed")]
        Closed,
        [Description("Approved")]
        Approved,
        [Description("Archived")]
        Archived,
        [Description("Cancel")]
        Canceled,
        [Description("On Process")]
        OnProcess,
        [Description("ተልኳል")]
        Sent,
        [Description("Delivery Confirmed")]
        DeliveryConfirmed,
        [Description("ተቀብሏል")]
        Received,
        [Description("Refunded Status")]
        Refunded,
        [Description("ተጠይቋል")]
        Requested,

    }
}