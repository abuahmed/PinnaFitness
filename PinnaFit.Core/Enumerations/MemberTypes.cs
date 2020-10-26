using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum MemberTypes
    {
        [Description("All")]
        All,
        [Description("ኖርማል")]
        Normal,
        [Description("ሴቶች ብቻ")]
        WomanOnly,
        [Description("ወንዶች ብቻ(ያለ ሙዚቃ)")]
        ManOnly,
        [Description("የተለየ")]
        Special,
        //[Description("Team Leader")]
        //TeamLeader,
        //[Description("Manager")]
        //Manager,
        //[Description("CEO")]
        //CEO
    }
}