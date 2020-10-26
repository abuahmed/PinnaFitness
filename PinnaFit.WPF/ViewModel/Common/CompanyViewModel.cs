using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class CompanyViewModel : ViewModelBase
    {
        #region Fields
        private static ICompanyService _companyService;
        private CompanyDTO _selectedCompany;
        private ICommand _saveCompanyViewCommand;
        #endregion

        #region Constructor

        public CompanyViewModel()
        {
            CleanUp();
            _companyService = new CompanyService();

            SelectedCompany = _companyService.GetCompany() ?? new CompanyDTO()
            {
                Address = new AddressDTO()
                {
                    Country = "ኢትዮጲያ",
                    City = "አዲስ አበባ"
                }
            };
        }

        public static void CleanUp()
        {
            if (_companyService != null)
                _companyService.Dispose();
        }

        #endregion

        #region Properties

        public CompanyDTO SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                RaisePropertyChanged<CompanyDTO>(() => SelectedCompany);
                if (SelectedCompany != null)
                {
                    LetterHeadImage = ImageUtil.ToImage(SelectedCompany.Header);
                    LetterFootImage = ImageUtil.ToImage(SelectedCompany.Footer);
                }
            }
        }

        #endregion

        #region Commands
        public ICommand SaveCompanyViewCommand
        {
            get { return _saveCompanyViewCommand ?? (_saveCompanyViewCommand = new RelayCommand<Object>(ExecuteSaveCompanyViewCommand, CanSave)); }
        }
        private void ExecuteSaveCompanyViewCommand(object obj)
        {
            try
            {
                if (LetterHeadImage.UriSource != null)
                    SelectedCompany.Header = ImageUtil.ToBytes(LetterHeadImage);
                if (LetterFootImage.UriSource != null)
                    SelectedCompany.Footer = ImageUtil.ToBytes(LetterFootImage);

                if (SelectedCompany != null && _companyService.InsertOrUpdate(SelectedCompany) == string.Empty)
                    CloseWindow(obj);
                else
                    MessageBox.Show("Got Problem while saving, try again...", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.Message, "error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        #region Letter Head
        private BitmapImage _letterHeadImage, _letterFootImage;
        private ICommand _showLetterHeaderImageCommand, _showLetterFooterImageCommand;

        public BitmapImage LetterHeadImage
        {
            get { return _letterHeadImage; }
            set
            {
                _letterHeadImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterHeadImage);
            }
        }
        public ICommand ShowLetterHeaderImageCommand
        {
            get { return _showLetterHeaderImageCommand ?? (_showLetterHeaderImageCommand = new RelayCommand(ExecuteShowLetterHeaderImageViewCommand)); }
        }
        private void ExecuteShowLetterHeaderImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterHeadImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
            }
        }

        public BitmapImage LetterFootImage
        {
            get { return _letterFootImage; }
            set
            {
                _letterFootImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterFootImage);
            }
        }
        public ICommand ShowLetterFooterImageCommand
        {
            get { return _showLetterFooterImageCommand ?? (_showLetterFooterImageCommand = new RelayCommand(ExecuteShowLetterFooterImageViewCommand)); }
        }
        private void ExecuteShowLetterFooterImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterFootImage = new BitmapImage(new Uri(file.FileName, true));// new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine()
        {
            return LineErrors == 0;

        }
        #endregion
    }
}
