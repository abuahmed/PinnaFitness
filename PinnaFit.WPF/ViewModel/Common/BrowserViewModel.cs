using GalaSoft.MvvmLight;

namespace PinnaFit.WPF.ViewModel
{
    public class BrowserViewModel : ViewModelBase
    {
        #region Fields
        
       
        #endregion

        #region Constructor
        public BrowserViewModel()
        {
           
        }


        #endregion

        #region Public Properties

       
        #endregion

        #region Commands
       
        
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
