using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class ItemEntry : Window
    {
        public ItemEntry()
        {
            ItemEntryViewModel.Errors = 0;
            InitializeComponent();
            TxtItemName.Focus();
        }
        //public ItemDetail(ItemDTO itemDto)
        //{
        //    ItemDetailViewModel.Errors = 0;
        //    InitializeComponent();           
        //    Messenger.Default.Send<ItemDTO>(itemDto);
        //    Messenger.Reset();
        //}
        public ItemEntry(ItemQuantityDTO itemQtyDto, WarehouseDTO warehouseDto)
        {
            ItemEntryViewModel.Errors = 0;
            InitializeComponent();

            //var itqtyId = itemQtyDto != null ? itemQtyDto.Id : 0;

            //int[] ids = { itqtyId, warehouseDto.Id };
            //Messenger.Default.Send<int[]>(ids);

            Messenger.Default.Send<ItemQuantityDTO>(itemQtyDto);
            Messenger.Default.Send<WarehouseDTO>(warehouseDto);

            Messenger.Reset();
        }
        public ItemEntry(ItemQuantityDTO itemQtyDto, WarehouseDTO warehouseDto, System.Windows.Visibility itemsQtyVisibility)
        {
            ItemEntryViewModel.Errors = 0;
            InitializeComponent();
            //TxtBlockItemsQuantity.Visibility = itemsQtyVisibility;
            //TxtItemsQuantity.Visibility = itemsQtyVisibility;

            Messenger.Default.Send<ItemQuantityDTO>(itemQtyDto);
            Messenger.Default.Send<WarehouseDTO>(warehouseDto);

            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ItemEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ItemEntryViewModel.Errors -= 1;
        }

        private void WdwItemDetail_Loaded(object sender, RoutedEventArgs e)
        {
            TxtItemName.Focus();
        }

        private void ItemDetail_OnClosing(object sender, CancelEventArgs e)
        {
            ItemEntryViewModel.CleanUp();
        }

    }
}
