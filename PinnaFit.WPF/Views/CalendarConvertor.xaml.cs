using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for ReportDuration.xaml
    /// </summary>
    public partial class CalendarConvertor : Window
    {
        public CalendarConvertor()
        {
            InitializeComponent();
        }
        public CalendarConvertor(DateTime calDate)
        {
            InitializeComponent();
            Messenger.Default.Send<DateTime>(calDate);
            Messenger.Reset();
        }
       
    }
}
