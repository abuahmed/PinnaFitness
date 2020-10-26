using System;
using System.ComponentModel;
using System.Windows.Controls;
using PinnaFit.WPF.ViewModel;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for SalesDetailList.xaml
    /// </summary>
    public partial class AttendanceListUC : UserControl
    {
        public AttendanceListUC()
        {
            InitializeComponent();
        }
        //public AttendanceListUC(TransactionTypes transactionType, ItemDTO itemDto)
        //{
        //    InitializeComponent();
        //    Messenger.Default.Send<TransactionTypes>(transactionType);
        //    Messenger.Default.Send<ItemDTO>(itemDto);
        //    Messenger.Reset();
        //}

        private void AttendanceList_OnClosing(object sender, CancelEventArgs e)
        {
            MembersListViewModel.CleanUp();
        }
    }
}
