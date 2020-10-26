using System.ComponentModel;

namespace PinnaFit.Core.Enumerations
{
    public enum ReportTypes
    {
        [Description("አጠቃላይ ገቢ")]
        AmountSummary = 0,
        [Description("አዲስና ያደሱ አባላት ዝርዝር")]
        NewRenewedList = 1,
        [Description("አቴንዳንስ ዝርዝር")]
        AttendanceList = 2,
        [Description("አቴንዳንስ ማጠቃለያ")]
        AttendanceSummarized = 3,
    }
}