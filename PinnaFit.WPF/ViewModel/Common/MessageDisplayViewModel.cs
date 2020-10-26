using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFit.WPF.ViewModel
{
    public class MessageDisplayViewModel : ViewModelBase
    {
        private ICommand _closeWindowCommand;
        private string _daysNumber, _daysText, _fontColor;
        private int _days;

        #region Fields


        #endregion

        #region Constructor
        public MessageDisplayViewModel()
        {
            FontColor = "Black";
            DaysNumber = "0";
            DaysText = "Days Left";

            Messenger.Default.Register<int>(this, (message) =>
            {
                DaysNumber = message.ToString();
            });
        }


        #endregion

        #region Public Properties
        public int Days
        {
            get { return _days; }
            set
            {
                _days = value;
                RaisePropertyChanged<int>(() => Days);
            }
        }
        public string DaysNumber
        {
            get { return _daysNumber; }
            set
            {
                _daysNumber = value;
                RaisePropertyChanged<string>(() => DaysNumber);

                Days = Convert.ToInt32(DaysNumber);
                if (Days <= 0)
                {
                    DaysText = "ቀኖች አልፏል";
                    FontColor = "Red";
                }
                else
                {
                    DaysText = "ቀኖች ቀሩት";
                    FontColor = "Yellow";
                }
            }
        }
        public string DaysText
        {
            get { return _daysText; }
            set
            {
                _daysText = value;
                RaisePropertyChanged<string>(() => DaysText);
            }
        }
        public string FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                RaisePropertyChanged<string>(() => FontColor);
            }
        }
        #endregion

        #region Commands
        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<object>(CloseWindow));
            }
        }
        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        #endregion
    }
}