using System;
using CrystalDecisions.CrystalReports.Engine;

namespace PinnaFit.Core
{
    public static class ReportUtility
    {
        public static string GetEthCalendar(DateTime gregordate, bool longformat)
        {

            var nextisleapyear = DateTime.IsLeapYear(gregordate.Year + 1);


            var lastdate = new DateTime(gregordate.Year, 9, 10);

            if (nextisleapyear)
                lastdate = new DateTime(gregordate.Year, 9, 11);//will be 11 if the next year is a leap year

            var difference = lastdate.DayOfYear;
            int dayx = 0, monthx = 0, yearx = 2000;
            if (gregordate.DayOfYear > difference) //is in between meskerem 1 and tahasas 22
            {
                difference = gregordate.DayOfYear - difference;

                dayx = difference % 30;
                monthx = difference / 30;
                yearx = gregordate.Year - 7;

            }
            else //is in between tahasas 22 and meskerem 1
            {
                int yearlength = 365;
                if (nextisleapyear)
                    yearlength = 366;
                difference = gregordate.DayOfYear + (yearlength - difference);//will be 366 if the next year is a leap year

                dayx = difference % 30;
                monthx = difference / 30;
                yearx = gregordate.Year - 8;
            }

            string date = dayx.ToString();
            string month = GetAmhMonth(monthx);
            if (dayx == 0)
            {
                month = GetAmhMonth(monthx - 1);
                monthx = monthx - 1;
                dayx = 30;
            }
            string days = dayx.ToString();
            string months = (monthx + 1).ToString();
            if (dayx < 10)
                days = "0" + dayx;
            if ((monthx + 1) < 10)
                months = "0" + (monthx + 1).ToString();
            //return month + " " + dayx.ToString() + " " + yearx.ToString() + " -- " + dayx.ToString() + "/" + (monthx + 1).ToString() + "/" + yearx.ToString();

            if (longformat)
                return GetAmhMonth(monthx) + " " + days + " " + yearx.ToString();
            else
                return days + "" + months + "" + yearx.ToString();
        }
        public static string GetEthCalendarFormated(DateTime gregordate, string separator)
        {
            string amhCalender = GetEthCalendar(gregordate, false);
            return amhCalender.Substring(0, 2) + separator + amhCalender.Substring(2, 2) + separator + amhCalender.Substring(4);
        }
        public static DateTime GetGregorCalendar(int ethyear, int ethmonth, int ethday)
        {
            int yearr = ethyear;
            var begindate = new DateTime(yearr, 9, 11);
            int noOfDays = (ethmonth - 1) * 30 + ethday;

            if (noOfDays <= 112)
            {
                begindate = DateTime.IsLeapYear(yearr + 8) ? new DateTime(yearr + 7, 9, 11) : new DateTime(yearr + 7, 9, 10);
            }
            else
            {
                begindate = DateTime.IsLeapYear(yearr + 7) ? new DateTime(yearr + 7, 9, 11) : new DateTime(yearr + 7, 9, 10);
            }

            return ConfirmGregorDate(begindate.AddDays(noOfDays), ethyear, ethmonth, ethday);

        }
        public static string GetAmhMonth(int month)
        {
            string[] amhmonths = { "መስከረም", "ጥቅምት", "ሕዳር", "ታህሳስ", "ጥር", "የካቲት", "መጋቢት", "ሚያዚያ", "ግንቦት", "ሰኔ", "ሐምሌ", "ነሃሴ", "ጳጉሜ" };
            return amhmonths[month];

        }
        public static string GetEngMonth(int month)
        {
            string[] amhmonths = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return amhmonths[month];
        }

        public static DateTime ConfirmGregorDate(DateTime beginDate, int ethyear, int monthx, int dayx)
        {
            string days = dayx.ToString();
            string months = monthx.ToString();
            if (dayx < 10)
                days = "0" + dayx;
            if (monthx < 10)
                months = "0" + monthx.ToString();

            var ethCal = days + "" + months + "" + ethyear.ToString();
            
            string newEthioDate = GetEthCalendar(beginDate, false);

            if (ethCal == newEthioDate)
                return beginDate;

            newEthioDate = GetEthCalendar(beginDate.AddDays(-1), false);
            if (ethCal == newEthioDate)
                return beginDate.AddDays(-1);

            newEthioDate = GetEthCalendar(beginDate.AddDays(1), false);
            if (ethCal == newEthioDate)
                return beginDate.AddDays(1);

            return beginDate;
        }
    }
}
