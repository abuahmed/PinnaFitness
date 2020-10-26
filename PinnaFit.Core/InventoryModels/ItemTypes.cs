using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum ItemTypes
    {
        [Description("Purchase and Sell")] 
        PurchaseForSell = 0,
        [Description("Purchase and Process")]
        PurchaseProcess = 1,
        [Description("Purchase and Use")]
        PurchaseForUse = 2,
        [Description("Process and Sell")]
        ProcessForSell = 3,
    }
}