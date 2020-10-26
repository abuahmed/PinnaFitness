using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Messaging;
using mshtml;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using MessageBox = System.Windows.MessageBox;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for MessageDisplay.xaml
    /// </summary>
    public partial class MessageDisplay : Window
    {
        public MessageDisplay()
        {
            InitializeComponent();
        }
        public MessageDisplay(string days)
        {
            InitializeComponent();
            Messenger.Default.Send<int>(Convert.ToInt32(days));
            Messenger.Reset();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


    }
}
