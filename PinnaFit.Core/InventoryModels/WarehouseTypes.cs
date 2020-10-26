using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum WarehouseTypes
    {
        [Description("Only Storage")]
        StoreOnly = 0,
        [Description("Store and Sell")]
        StoreSell = 1,
        [Description("Store and Process")]
        StoreProcess = 2,
        [Description("Store and Use")]
        StoreUse = 3
    }
}