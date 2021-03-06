using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum MemberStatusTypes
    {
        [Description("ጊዜ ያለውና ያለፈበት")]
        All,
        [Description("ቀን ያለው/ላት")]
        Active,
        [Description("ቀን ያለፈበት/ባት")]
        Expired,
        [Description("ትንሽ ጊዜ ያለውና ያለፈበት")]
        FewDaysActiveExpired,
        [Description("ትንሽ ጊዜ ያለው")]
        FewDaysActive,
        [Description("ትንሽ ጊዜ ያለፈበት")]
        FewDaysExpired
    }
    public enum MembershipTypes
    {
        [Description("አዲስና ያደሱ")]
        All,
        [Description("አዲስ የተመዘገቡ")]
        New,
        [Description("ያደሱ")]
        Renewed
    }
    public enum ShiftTypes
    {
        [Description("ጠዋትና ከሰአት")]
        All,
        [Description("ጠዋት")]
        Morning,
        [Description("ከሰአት")]
        Afternoon
    }
}