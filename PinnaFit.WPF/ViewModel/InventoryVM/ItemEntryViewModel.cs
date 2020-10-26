#region MyRegion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Properties;
using PinnaFit.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MessageBox = System.Windows.MessageBox;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class ItemEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IItemService _itemService;
        private static IItemQuantityService _itemQuantityService;
        private ItemDTO _selectedItem;
        private ItemQuantityDTO _selectedItemQuantity, _selectedItemQuantityOld;
        private WarehouseDTO _selectedWarehouse;
        private ICommand _saveItemViewCommand, _closeItemViewCommand, _addNewCategoryCommand, _addNewUomCommand;
        private ObservableCollection<CategoryDTO> _categories, _unitOfMeasure;
        private string _quantityEditVisibility;
        private decimal? _currentQuantity, _itemPreviousQty;
        #endregion

        #region Constructor
        public ItemEntryViewModel()
        {
            CleanUp();
            _itemService = new ItemService();
            _itemQuantityService = new ItemQuantityService();

            CheckRoles();

            LoadCategories();
            SelectedCategory = Categories.FirstOrDefault();
            LoadUoMs();
            SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault();

            Messenger.Default.Register<ItemQuantityDTO>(this, (message) =>
            {
                if (message != null)
                {
                    SelectedItemQuantityOld = _itemQuantityService.Find(message.Id.ToString()) ??
                                              new ItemQuantityDTO()
                    {
                        QuantityOnHand = 0,
                        ItemId = message.ItemId,
                        WarehouseId = message.WarehouseId
                    };
                }

            });

            Messenger.Default.Register<WarehouseDTO>(this, (message) =>
            {
                SelectedWarehouse = message;
            });


            if (_selectedItem == null)
            {
                _selectedItem = GetNewSelectedItem();
                _itemPreviousQty = 0;
            }
            QuantityEditVisibility = "Collapsed";
        }

        public static void CleanUp()
        {
            if (_itemService != null)
                _itemService.Dispose();
            if (_itemQuantityService != null)
                _itemQuantityService.Dispose();
        }
        #endregion

        #region Properties
        public ItemDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged<ItemDTO>(() => SelectedItem);
                if (SelectedItem != null)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedItem.CategoryId);
                    SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault(c => c.Id == SelectedItem.UnitOfMeasureId);
                }
            }
        }
        public ItemQuantityDTO SelectedItemQuantityOld
        {
            get { return _selectedItemQuantityOld; }
            set
            {
                _selectedItemQuantityOld = value;
                RaisePropertyChanged<ItemQuantityDTO>(() => SelectedItemQuantityOld);
                if (SelectedItemQuantityOld != null)
                {
                    SelectedItemQuantity = SelectedItemQuantityOld;
                }
            }
        }
        public ItemQuantityDTO SelectedItemQuantity
        {
            get { return _selectedItemQuantity; }
            set
            {
                _selectedItemQuantity = value;
                RaisePropertyChanged<ItemQuantityDTO>(() => SelectedItemQuantity);
                if (SelectedItemQuantity != null)
                {
                    _itemPreviousQty = SelectedItemQuantity.QuantityOnHand;
                    SelectedItem = _itemService.Find(SelectedItemQuantity.ItemId.ToString());
                    CurrentQuantity = SelectedItemQuantity.QuantityOnHand;
                }
            }
        }
        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);
                if (SelectedWarehouse != null)
                {
                    if (SelectedWarehouse.DisplayName != "All")
                    {
                        QuantityEditVisibility = "Visible";
                    }
                }
            }
        }
        public string QuantityEditVisibility
        {
            get { return _quantityEditVisibility; }
            set
            {
                _quantityEditVisibility = value;
                RaisePropertyChanged<string>(() => QuantityEditVisibility);
            }
        }
        public decimal? CurrentQuantity
        {
            get { return _currentQuantity; }
            set
            {
                _currentQuantity = value;
                RaisePropertyChanged<decimal?>(() => CurrentQuantity);
            }
        }

        #endregion

        #region Commands
        public ICommand SaveCloseItemViewCommand
        {
            get { return _saveItemViewCommand ?? (_saveItemViewCommand = new RelayCommand<Object>(ExecuteSaveItemViewCommand, CanSave)); }
        }
        private void ExecuteSaveItemViewCommand(object obj)
        {
            try
            {
                SelectedItem.CategoryId = SelectedCategory.Id;
                SelectedItem.UnitOfMeasureId = SelectedUnitOfMeasure.Id;

                var stat = _itemService.InsertOrUpdate(SelectedItem);
                if (stat == string.Empty)
                {
                    SelectedItem.Number = _itemService.GetItemNumber(SelectedItem.Id);
                    stat = _itemService.InsertOrUpdate(SelectedItem);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);

                    #region Change Item Qty after adding a new PI

                    if (QuantityEditVisibility != null && QuantityEditVisibility == "Visible" && CurrentQuantity!=null &&
                        _itemPreviousQty != CurrentQuantity && SelectedWarehouse != null && SelectedWarehouse.Id != -1)
                    {
                        var itemQty = new ItemQuantityDTO
                        {
                            WarehouseId = SelectedWarehouse.Id,
                            ItemId = SelectedItem.Id,
                            QuantityOnHand = CurrentQuantity
                        };
                        var stat2 = _itemQuantityService.InsertOrUpdate(itemQty, true);

                        if (stat2 == string.Empty)
                            CloseWindow(obj);
                        else
                            MessageBox.Show(
                                "item detail saved successfully but updating quantity failed, try again..." +
                                Environment.NewLine + stat2, "save error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        
                        CloseWindow(obj);
                    }
                    #endregion
                }
                else
                    MessageBox.Show("Got Problem while saving item, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                       MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem saving Item..." +
                            Environment.NewLine + exception.Message +
                            Environment.NewLine + exception.InnerException);
            }
        }

        public ICommand CloseItemViewCommand
        {
            get
            {
                return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            //new NotificationWindow().Show();
            //string str = "title", title2 = "title", text2 = "text";

            //var nic = new NotifyIcon
            //{
            //    Text = str,
            //    BalloonTipText = str,
            //    BalloonTipTitle = title2,
            //    Visible = true
            //};
            //nic.Icon = new System.Drawing.Icon("/PinnaFit.WPF;component/Resources/AppIcon.ico");
            //nic.ShowBalloonTip(1000, title2, text2, ToolTipIcon.Info);

            //var tbi = new TaskbarIcon
            //{
            //    Icon = new System.Drawing.Icon("../../Resources/AppIcon.ico"),
            //    ToolTipText = "hello world"
            //};
            //new TaskbarIcon().ShowBalloonTip("hello", "hello world", new System.Drawing.Icon("../../Resources/AppIcon.ico"));

        }
        #endregion

        public ItemDTO GetNewSelectedItem()
        {
            var selectedItem = new ItemDTO
            {
                //Number = _itemService.GetItemNumber(),
                //TotalCurrentQty = 0,
                //PurchasePrice = 0,
                //SalePrice = 0,
                SafeQuantity = 10,
                CategoryId = SelectedCategory.Id,
                UnitOfMeasureId = SelectedUnitOfMeasure.Id
            };

            return selectedItem;
        }

        #region Categories
        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.Category);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Categories = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedCategory);
            }
        }
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Categories);
            }
        }
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.Category);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();//should also get the latest updates in each row
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedItem.CategoryId = SelectedCategory.Id;
            }
        }
        #endregion

        #region Unit Of Measures
        private CategoryDTO _selectedUnitOfMeasure;
        public CategoryDTO SelectedUnitOfMeasure
        {
            get { return _selectedUnitOfMeasure; }
            set
            {
                _selectedUnitOfMeasure = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedUnitOfMeasure);
            }
        }

        public void LoadUoMs()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.UnitMeasure);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            UnitOfMeasures = new ObservableCollection<CategoryDTO>(categoriesList);
        }
        public ObservableCollection<CategoryDTO> UnitOfMeasures
        {
            get { return _unitOfMeasure; }
            set
            {
                _unitOfMeasure = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => UnitOfMeasures);
            }
        }
        public ICommand AddNewUomCommand
        {
            get
            {
                return _addNewUomCommand ?? (_addNewUomCommand = new RelayCommand(ExcuteAddNewUomCommand));
            }
        }
        private void ExcuteAddNewUomCommand()
        {
            var category = new Categories(NameTypes.UnitMeasure);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadUoMs();
                SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedUnitOfMeasure != null) SelectedItem.UnitOfMeasureId = SelectedUnitOfMeasure.Id;
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        #endregion

        #region Previlege Visibility
        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}
