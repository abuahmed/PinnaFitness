using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum WeekDays
    {
        [Description("Monday")]
        Mon = 0,
        [Description("Tuesday")]
        Tue = 1,
        [Description("Wednesday")]
        Wed = 2,
        [Description("Thursday")]
        Thr = 3,
        [Description("Friday")]
        Fri = 4,
        [Description("Saturday")]
        Sat = 5,
        [Description("Sunday")]
        Sun = 6,
        [Description("Every Day")]
        EveryDay = 7,
    }
}