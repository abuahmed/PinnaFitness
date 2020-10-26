using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight.Messaging;


namespace PinnaFit.WPF.Views
{
    /// <summary>
    /// Interaction logic for ReportViewerCommon.xaml
    /// </summary>
    public partial class ReportViewerCommon : Window
    {
        public ReportViewerCommon()
        {
            InitializeComponent();
        }
        ReportDocument reportDocument;
        public ReportViewerCommon(ReportDocument report)
        {
            InitializeComponent();
            reportDocument = report;
            Messenger.Default.Send<ReportDocument>(reportDocument);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            crvReportViewer.ViewerCore.ReportSource = reportDocument;
        }

    }
}
