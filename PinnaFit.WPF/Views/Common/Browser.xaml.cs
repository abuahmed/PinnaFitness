using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Messaging;
using mshtml;
using PinnaFit.Core.Models;
using PinnaFit.WPF.ViewModel;
using MessageBox = System.Windows.MessageBox;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        public Browser()
        {
            InitializeComponent();
            TxtenjazUrl.Text = "http://192.168.1.12/MarakiReports2012/index.php";
        }
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BrwEnjaz.Navigate(TxtenjazUrl.Text);
        }

        private void OpenURLInBrowser(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            try
            {
                BrwEnjaz.Navigate(new Uri(url));

            }
            catch (UriFormatException)
            {
                return;
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxtenjazUrl.Text) || TxtenjazUrl.Text.Equals("about:blank"))
            {
                MessageBox.Show("Enter a valid URL.");
                TxtenjazUrl.Focus();
                return;
            }
            OpenURLInBrowser(TxtenjazUrl.Text);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (BrwEnjaz.CanGoBack)
                BrwEnjaz.GoBack();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (BrwEnjaz.CanGoForward)
                BrwEnjaz.GoForward();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //brwEnjaz.ShowPrintDialog();
            var doc = BrwEnjaz.Document as IHTMLDocument2;
            doc.execCommand("Print", true, null);
        }

        int wid = 100;
        private void btnZOut_Click(object sender, RoutedEventArgs e)
        {
            wid = wid - 2;
            ZoomWebBrowser(wid);
        }

        private void btnZIn_Click(object sender, RoutedEventArgs e)
        {
            wid = wid + 2;
            ZoomWebBrowser(wid);
        }

        private void ZoomWebBrowser(int factor)
        {
            SetZoom(BrwEnjaz, factor / 100);
        }

        public static void SetZoom(WebBrowser webBrowser1, double Zoom)
        {
            try
            {
                //HtmlDocument doc =(HtmlDocument) webBrowser1.Document;

                var doc = webBrowser1.Document as IHTMLDocument2;
                var tags = (IHTMLElementCollection)doc.all.tags("div");
                foreach (IHTMLElement el in tags)
                {
                    string controlName2 = el.getAttribute("tabindex").ToString();
                    if (controlName2 == "17")
                    {
                        //IHTMLElementCollection els = el.children.item;
                        IHTMLElement ell = el.children.item(null, 0);

                        //IHTMLElementCollection els2 = ell.children;
                        IHTMLElement elll = ell.children.item(null, 0);

                        //IHTMLElement elll = ell.children;
                        elll.innerText = "Ethiopia";

                        //IHTMLElementCollection els3 = el.children;
                        IHTMLElement ellll = el.children.item(null, 1);
                        ellll.setAttribute("value", "ETH");
                    }
                }

                //doc.parentWindow.execScript("docoment.body.style.zoom=" + Zoom.ToString().Replace(",", ".") + ";");
            }
            catch { }
        }

        private void brwEnjaz_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wid = 100;
            ZoomWebBrowser(wid);
        }


        private void Browser_OnUnloaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Cleanup");
            //BrowserViewModel.CleanUp();
        }

        private void LstItemsAutoCompleteBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LstItemsAutoCompleteBox.SearchText = string.Empty;
            //LstItemsAutoCompleteBox.Focus();//
        }
    }
}
