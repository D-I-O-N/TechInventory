using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechInventory._src.database;
using Word = Microsoft.Office.Interop.Word;


namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для PageStatisticReport.xaml
    /// </summary>
    /// 



    public partial class PageStatisticReport : Page
    {
        Entities entities = new Entities();
        public PageStatisticReport()
        {
            InitializeComponent();
        }

        private void BackToPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EqAct_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report();
            report.CheckTechGen(entities.Equipment.ToList());

            //var allEquipment = new List<Equipment>();

            //var application = new Word.Application();

            //Word.Document document = application.Documents.Add();

            //Word.Paragraph tableParagraph = document.Paragraphs.Add();
            //Word.Range tableRange = tableParagraph.Range;
            //Word.Table paymentsTable = document.Tables.Add(tableRange, allEquipment.Count()+1, 5);
            //paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle
            //    = Word.WdLineStyle.wdLineStyleSingle;
            //paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //Word.Range cellRange;

            //cellRange = paymentsTable.Cell(1, 1).Range;
            //cellRange.Text = "Наименование оборудования";
            //cellRange = paymentsTable.Cell(1, 2).Range;
            //cellRange.Text = "Кол-во";
            //cellRange = paymentsTable.Cell(1, 3).Range;
            //cellRange.Text = "Дата покупки";
            //cellRange = paymentsTable.Cell(1, 4).Range;
            //cellRange.Text = "Серийный номер";
            //cellRange = paymentsTable.Cell(1, 5).Range;
            //cellRange.Text = "Тип";

            //paymentsTable.Rows[1].Range.Bold = 1;
            //paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment
        }

        private void EqList_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EquipmentReportPage());
        }
    }
}
