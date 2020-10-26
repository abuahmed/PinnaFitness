using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum NameTypes
    {
        [Description("Item Category")]
        Category = 0,
        [Description("Unit Of Measure")]
        UnitMeasure = 1,
        [Description("Client Category")]
        ClientCategory = 2,
        [Description("Equipment Category")]
        EquipmentCategory = 3,
        [Description("City")]
        City = 4,
        [Description("SubCity")]
        SubCity = 5
    }
}
