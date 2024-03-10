using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechInventory._src.database;
using Word = Microsoft.Office.Interop.Word;

namespace TechInventory
{
    internal class Report
    {
        Word.Application app = new Word.Application();
        Word.Document doc;

        ~Report()
        {
            doc.Saved = true;
            try { app.Quit(); }
            catch { }
        }

        public void CheckTechGen(IList<Equipment> Equipment)
        {
            if (Equipment != null)
            {
                doc = app.Documents.Add(Template: $@"C:\Users\danb9\Desktop\C#\TechInventory\Templates\Act.docx", Visible: true);

            

                //Word.Range dateTime = doc.Bookmarks["DateTime"].Range;
                //dateTime.Text = DateTime.Now.ToString();

                Word.Table table = doc.Bookmarks["Table"].Range.Tables[1];
                int currPage = 1;
                foreach (var item in Equipment)
                {
                    int page = doc.ComputeStatistics(Word.WdStatistic.wdStatisticPages);

                    Word.Row row = table.Rows.Add();
                    if (page > currPage) //Если запись не влезает на текущею страницу
                    {
                        row.Range.InsertBreak();
                        table = doc.Tables[doc.Tables.Count];

                        doc.Tables[1].Rows[1].Range.Copy();
                        row.Range.Paste();

                        int rowIndexToDelete = -1;
                        for (int i = 1; i <= table.Rows.Count; i++)
                        {
                            if (table.Rows[i].Range.Text.Contains("[текст] [текст] [текст] [текст] [текст]"))
                            {
                                rowIndexToDelete = i;
                                break;
                            }
                        }

                        // Удаляем строку, если найдена
                        if (rowIndexToDelete != -1)
                        {
                            table.Rows[rowIndexToDelete].Delete();
                        }

                        //table.Rows[2].Delete(); //Удаляем пустую строку после заголовка

                        currPage = page;
                        row = table.Rows.Add();
                    }

                    row.Cells[1].Range.Text = item.EquipmentName;
                    row.Cells[2].Range.Text = item.Count.ToString();
                    //row.Cells[3].Range.InsertBefore(item.PurchaseDate.ToString());
                    //row.Cells[4].Range.Text = item.PurchaseDate.ToString();
                    //row.Cells[5].Range.Text = item.SerialNumber;
                    //row.Cells[6].Range.Text = item.EquipmentType;
                }
                doc.Bookmarks["Table"].Range.Tables[1].Rows[2].Delete(); //Удаляем строку [текст] [текст] [текст] [текст] в таблице

                app.Visible = true;

                //// Сохранение измененного файла
                //string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Укажите путь для сохранения файла
                //string fileName = "Акт приема-передачи оборудования.docx"; // Укажите имя файла
                //string fullPath = System.IO.Path.Combine(savePath, fileName);
                //doc.SaveAs(fullPath);
                //doc.Close();

                //// Вывод сообщения о готовности файла
                //Console.WriteLine("Файл успешно сохранен.");
            }
        }

    }
}
