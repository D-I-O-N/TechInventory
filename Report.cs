using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechInventory._src.database;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using DocumentFormat.OpenXml.Drawing;
using TechInventory._src.pages.statistics;

namespace TechInventory
{
    internal class Report
    {
        Word.Application app = new Word.Application();
        Word.Document doc;

        //public void CheckTechGen(IList<Equipment> Equipment)
        //{
        //    if (Equipment != null)
        //    {
        //        doc = app.Documents.Add(Template: $@"C:\Users\danb9\Desktop\C#\TechInventory\Templates\Act.docx", Visible: true);

        //        //string templatePath = $@"Templates\Act.docx";
        //        //string fullWay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);
        //        //doc = app.Documents.Add(Template: fullWay, Visible: true);


        //        int rowCount = 0;

        //        //Word.Range dateTime = doc.Bookmarks["DateTime"].Range;
        //        //dateTime.Text = DateTime.Now.ToString();

        //        Word.Table table = doc.Bookmarks["Table"].Range.Tables[1];
        //        int currPage = 1;


        //        foreach (var item in Equipment)
        //        {
        //            int page = doc.ComputeStatistics(Word.WdStatistic.wdStatisticPages);

        //            Word.Row row = table.Rows.Add();
        //            if (page > currPage) //Если запись не влезает на текущею страницу
        //            {
        //                row.Range.InsertBreak();
        //                table = doc.Tables[doc.Tables.Count];

        //                doc.Tables[1].Rows[1].Range.Copy();
        //                row.Range.Paste();

        //                int rowIndexToDelete = -1;
        //                for (int i = 1; i <= table.Rows.Count; i++)
        //                {
        //                    if (table.Rows[i].Range.Text.Contains("[текст] [текст] [текст] [текст] [текст]"))
        //                    {
        //                        rowIndexToDelete = i;
        //                        break;
        //                    }
        //                }

        //                if (rowIndexToDelete != -1)
        //                {
        //                    table.Rows[rowIndexToDelete].Delete(); // Удаляем пустую строку после заголовка строку, если найдена
        //                }
        //                currPage = page;
        //                row = table.Rows.Add();
        //            }

        //            row.Cells[1].Range.Text = item.EquipmentName;
        //            row.Cells[2].Range.Text = item.Count.ToString();

        //            String Cells_3 = item.PurchaseDate.ToString();
        //            string originalString = Cells_3;
        //            string stringToRemove = " 0:00:00";
        //            string modifiedStringCells_3 = originalString.Replace(stringToRemove, string.Empty);

        //            row.Cells[3].Range.Text = modifiedStringCells_3;
        //            row.Cells[4].Range.Text = item.SerialNumber;
        //            row.Cells[5].Range.Text = item.EquipmentType;

        //            rowCount++;
        //            if (rowCount >= 9) // Если добавлено уже 4 строки, прекращаем цикл
        //            {
        //                break;
        //            }
        //        }
        //        doc.Bookmarks["Table"].Range.Tables[1].Rows[2].Delete(); //Удаляем строку [текст] [текст] [текст] [текст] в таблице
        //        //app.Visible = true;

        //        // Сохранение измененного файла
        //        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //        string fileName = "Акт приема-передачи оборудования.docx";
        //        string fullPath = System.IO.Path.Combine(savePath, fileName);
        //        doc.SaveAs(fullPath);
        //        doc.Close();

        //        MessageBox.Show("Файл успешно сохранен на рабочем столе.", "Успешное выполнение задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //}


        public void CheckTechGen(List<EquipmentItem> Equipment)
        {
            if (Equipment != null)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string templatePath = System.IO.Path.Combine(desktopPath, "C#", "TechInventory", "Templates", "Act.docx");
                //string templatePath = @"C:\Users\danb9\Desktop\C#\TechInventory\Templates\Act.docx";
                doc = app.Documents.Add(Template: templatePath, Visible: true);

                Word.Table table = doc.Bookmarks["Table"].Range.Tables[1];

                //int rowCount = 0;
                int currPage = 1;

                foreach (var item in Equipment)
                {
                    // Проверяем, нужно ли добавить новую страницу
                    int page = doc.ComputeStatistics(Word.WdStatistic.wdStatisticPages);
                    if (page > currPage)
                    {
                        // Добавляем разрыв страницы
                        table.Rows[table.Rows.Count].Range.InsertBreak(Word.WdBreakType.wdPageBreak);

                        // Получаем таблицу на новой странице
                        table = doc.Tables[doc.Tables.Count];

                        // Необходимо скопировать форматирование строки из первой таблицы и применить к новой
                        Word.Row firstRow = doc.Tables[1].Rows[1];
                        //table.Rows[table.Rows.Count].Range.FormattedText = firstRow.Range.FormattedText;

                        // Удаляем пустую строку после заголовка, если найдена
                        int rowIndexToDelete = -1;
                        for (int i = 1; i <= table.Rows.Count; i++)
                        {
                            if (table.Rows[i].Range.Text.Contains("[текст] [текст] [текст] [текст] [текст]"))
                            {
                                rowIndexToDelete = i;
                                break;
                            }
                        }

                        if (rowIndexToDelete != -1)
                        {
                            table.Rows[rowIndexToDelete].Delete();
                        }
                        // А ОНО вообще надо нам?

                        // Обновляем номер текущей страницы
                        currPage = page;
                    }
                    Word.Row row = table.Rows.Add();

                    row.Cells[1].Range.Text = item.EquipmentName;
                    row.Cells[2].Range.Text = item.Count.ToString();

                    String Cells_3 = item.PurchaseDate.ToString();
                    string originalString = Cells_3;
                    string stringToRemove = " 0:00:00";
                    string modifiedStringCells_3 = originalString.Replace(stringToRemove, string.Empty);

                    row.Cells[3].Range.Text = modifiedStringCells_3;
                    /* row.Cells[3].Range.Text = item.PurchaseDate.ToString("dd.MM.yyyy");*/ 
                    row.Cells[4].Range.Text = item.SerialNumber;
                    row.Cells[5].Range.Text = item.EquipmentType;

                    //rowCount++;

                    //// Если добавлено уже 4 строки, прекращаем цикл
                    //if (rowCount >= 4)
                    //{
                    //    break;
                    //}
                }

                doc.Bookmarks["Table"].Range.Tables[1].Rows[2].Delete();

                string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = "Акт приема-передачи оборудования.docx";
                string fullPath = System.IO.Path.Combine(savePath, fileName);
                doc.SaveAs(fullPath);
                doc.Close();

                MessageBox.Show("Файл успешно сохранен на рабочем столе.", "Успешное выполнение задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



    }
}
