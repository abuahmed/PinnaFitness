using System.ComponentModel;
using System.Windows;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for SalesDetailList.xaml
    /// </summary>
    public partial class MemberList : Window
    {
        public MemberList()
        {
            InitializeComponent();
        }
        //public MembersList(TransactionTypes transactionType, ItemDTO itemDto)
        //{
        //    InitializeComponent();
        //    Messenger.Default.Send<TransactionTypes>(transactionType);
        //    Messenger.Default.Send<ItemDTO>(itemDto);
        //    Messenger.Reset();
        //}

        private void MembersList_OnClosing(object sender, CancelEventArgs e)
        {
            MembersListViewModel.CleanUp();
        }
    }
}
