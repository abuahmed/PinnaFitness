using System.Drawing.Printing;
using CrystalDecisions.CrystalReports.Engine;
using System.Windows.Forms;

namespace PinnaFit.WPF.Reports
{
    public class ReportUtil
    {
        ReportDocument _crReportDocument;
        public void DirectPrinter(ReportDocument cReport)
        {
            var printDialog = new PrintDialog();
            var printDocument = new PrintDocument();

            printDialog.Document = printDocument;
            printDialog.AllowSomePages = true;
            printDialog.AllowCurrentPage = true;

            var dialogue = printDialog.ShowDialog();
            if (dialogue == DialogResult.OK)
            {
                int nCopy = printDocument.PrinterSettings.Copies;
                var sPage = printDocument.PrinterSettings.FromPage;
                var ePage = printDocument.PrinterSettings.ToPage;
                var printerName = printDocument.PrinterSettings.PrinterName;
                _crReportDocument = new ReportDocument();
                _crReportDocument = cReport;
                try
                {
                    _crReportDocument.PrintOptions.PrinterName = printerName;
                    _crReportDocument.Refresh();
                    _crReportDocument.PrintToPrinter(nCopy, false, sPage, ePage);
                }
                catch
                {
                    MessageBox.Show("Error Printing Document");
                }
            }
        }
    }
}

