using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.Repository.Interfaces;
using PinnaFit.WPF.Models;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;
using PinnaFit.Repository;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.Map;

namespace PinnaFit.WPF.Models
{
    public class SubscriptionModel
    {
        public int Id { get; set; }

        public string MemberName { get; set; }

        public Sex Sex { get; set; }

        public int Age { get; set; }

        public string MobileNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PackageName { get; set; }

        public string FacilityName { get; set; }

        public string SubscriptionName { get; set; }

        public int NoOfAttendances { get; set; }

        public int DaysLeft { get; set; }

        public bool IsExpired { get; set; }

        public ShiftTypes Shift
        {
            get {
                return TransactionDate.TimeOfDay.Hours < 14 ? ShiftTypes.Morning : ShiftTypes.Afternoon;
            }
        }
    }

    public class SubscriptionSumModel
    {
        public string Category { get; set; }

        public string StartDate { get; set; }
        public string StartMonth { get; set; }

        public int TotalNumber
        {
            get
            {
                return SubscriptionModels.Count();
            }
        }
        public int TotalMale
        {
            get
            {
                return SubscriptionModels.Count(m => m.Sex==Sex.Male);
            }
        }
        public int TotalFemale
        {
            get
            {
                return SubscriptionModels.Count(m => m.Sex == Sex.Female);
            }
        }

        public int TotalNumberWithDays
        {
            get
            {
                return SubscriptionModels.Count(s=>s.DaysLeft>=0);
            }
        }
        public int TotalNumberWithNoDays
        {
            get
            {
                return SubscriptionModels.Count(s => s.DaysLeft < 0);
            }
        }
        
        public decimal TotalAmount
        {
            get
            {
                return SubscriptionModels.Sum(s => s.Amount);
            }
        }

        public IEnumerable<SubscriptionModel> SubscriptionModels { get; set; }

        public DateTime TransactionDate { get; set; }
        public ShiftTypes Shift
        {
            get
            {
                return TransactionDate.TimeOfDay.Hours < 14 ? ShiftTypes.Morning : ShiftTypes.Afternoon;
            }
        }

        public decimal TotalUnit
        {
            get
            {
                return SubscriptionModels.Sum(s => s.NoOfAttendances);
            }
        }
    }

    public class Data
    {
        private readonly int _totNumber=0;
        public Data(){}

        public Data(int totalNumber)
        {
            _totNumber = totalNumber;
        }
        public string Category { get; set; }

        public double Value { get; set; }

        public int TotalNumber
        {
            get
            {
                if (_totNumber != 0)
                    return SubscriptionModels.Count()/_totNumber;
                return SubscriptionModels.Count();
            }
        }
        public int TotalMale
        {
            get
            {
                return SubscriptionModels.Count(m => m.Sex == Sex.Male);
            }
        }
        public int TotalFemale
        {
            get
            {
                return SubscriptionModels.Count(m => m.Sex == Sex.Female);
            }
        }

        public decimal TotalValue
        {
            get
            {
                return SubscriptionModels.Sum(s=>s.Amount);
            }
        }
        public IEnumerable<SubscriptionModel> SubscriptionModels { get; set; }
    }
}
