using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PinnaFit.Core.Enumerations;
using PinnaFit.WPF.Models;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword().Show();
        }

        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Users().Show();
        }

        private void BackupRestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BackupRestore().Show();
        }

        private void Members_Click(object sender, RoutedEventArgs e)
        {
            new Members().Show();
        }

        private void Trainers_Click(object sender, RoutedEventArgs e)
        {
            new Trainers().Show();
        }

        private void Equipments_Click(object sender, RoutedEventArgs e)
        {
            new Equipments().Show();
        }

        private void CompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Company().Show();
        }

        private void StoresMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Warehouses().Show();
        }
        
        private void ProcessMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            //new AttendanceListUC().Show();
        }

        private void TimeTableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new TimeTables().Show();
        }

        private void Subscriptions_Click(object sender, RoutedEventArgs e)
        {
            new Subscriptions().Show();
        }

        private void Facilities_Click(object sender, RoutedEventArgs e)
        {
            new Facility().Show();
        }

        private void FacilitySubscriptions_Click(object sender, RoutedEventArgs e)
        {
            new Packages().Show();
        }

        private void AssesssmentMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Assessments().Show();
        }

        private void MembersListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new MemberList().Show();
        }

        private void NumberSummaryListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var sumutil = new SummaryUtility();
            var mems = sumutil.GetMembers().ToList();
            sumutil.PrintList(mems,MemberStatusTypes.All);
        }
        private void NumberSummaryListActiveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var sumutil = new SummaryUtility();
            var mems = sumutil.GetMembers().ToList();
            sumutil.PrintList(mems, MemberStatusTypes.Active);
        }
        private void NumberSummaryListExpiredMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var sumutil = new SummaryUtility();
            var mems = sumutil.GetMembers().ToList();
            sumutil.PrintList(mems, MemberStatusTypes.Expired);
        }
        private void AmountSummaryListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.AmountSummary).Show();
        }
        private void NewRenewedMembersListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.NewRenewedList).Show();
        }
        private void AttendanceSummaryListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.AttendanceSummarized).Show();
        }
        private void AttendanceListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.AttendanceList).Show();
        }

        private void CalendarConvertor_Click(object sender, RoutedEventArgs e)
        {
            new CalendarConvertor(DateTime.Now.AddYears(-20)).Show();
        }

        private void ServiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Services().Show();
        }

        private void ItemsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Items().Show();
        }

        private void IohMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new OnHandInventories().Show();
        }

        private void StockReceiveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ReceiveStock(TransactionTypes.RecieveStock).Show();
        }

        private void TransferStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ReceiveStock(TransactionTypes.TransferStock).Show();  
        }
        
        private void MarakiReports_Click(object sender, RoutedEventArgs e)
        {
            new Browser().Show();
        }
        
        private void SuppliersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BusinessPartnerEntry().Show();
        }

        private void ExpensesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ExpenseLoans().Show();
        }

        private void MemberEntry_Click(object sender, RoutedEventArgs e)
        {
            new MemberEntry().Show();
        }
    }
}
