using System;

namespace PinnaFit.Core
{
    public class MemberDetail : CommonFieldsA
    {
        public string Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set { SetValue(() => FullName, value); }
        }
        public string Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }
        public string Age
        {
            get { return GetValue(() => Age); }
            set { SetValue(() => Age, value); }
        }
        public string Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }
        public string Mobile
        {
            get { return GetValue(() => Mobile); }
            set { SetValue(() => Mobile, value); }
        }

        public string Facility
        {
            get { return GetValue(() => Facility); }
            set { SetValue(() => Facility, value); }
        }
        public string Subscription
        {
            get { return GetValue(() => Subscription); }
            set { SetValue(() => Subscription, value); }
        }

        public decimal Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }
        public DateTime StartDate
        {
            get { return GetValue(() => StartDate); }
            set { SetValue(() => StartDate, value); }
        }
        public DateTime EndDate
        {
            get { return GetValue(() => EndDate); }
            set { SetValue(() => EndDate, value); }
        }
    }
}