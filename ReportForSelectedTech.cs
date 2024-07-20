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
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace TechInventory
{


    internal class ReportForSelectedTech
    {
        private void GenerateReport(IList<Equipment> selectedEquipment, Equipment equipment)
        {
            //Word.Application app = new Word.Application();
            //Word.Document doc;

            //void CheckTechGen(IList<Equipment> Equipment)
            //{
            //    if (Equipment != null)
            //    {
            //        // Создание формы
            //        Form form = new Form();
            //        form.Text = "Выбор оборудования";
            //        form.Width = 600;
            //        form.Height = 400;

            //        // Создание DataGridView
            //        DataGridView dataGridView = new DataGridView();
            //        dataGridView.Dock = DockStyle.Fill;
            //        dataGridView.AutoGenerateColumns = true;

            //        // Создание столбца с чекбоксами
            //        DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            //        checkBoxColumn.HeaderText = "Выбрать";
            //        checkBoxColumn.Name = "SelectColumn";
            //        checkBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            //        // Добавление столбца к DataGridView
            //        dataGridView.Columns.Add(checkBoxColumn);

            //        // Назначение данных для DataGridView
            //        dataGridView.DataSource = Equipment;

            //        // Добавление кнопки "Сохранить"
            //        Button saveButton = new Button();
            //        saveButton.Text = "Сохранить";
            //        saveButton.Dock = DockStyle.Bottom;
            //        saveButton.Click += SaveButton_Click;

            //        // Добавление кнопки на форму
            //        form.Controls.Add(dataGridView);
            //        form.Controls.Add(saveButton);

            //        // Отображение формы
            //        form.ShowDialog();
            //    }
            //}

            //void SaveButton_Click(object sender, EventArgs e)
            //{
            //    // Получаем выбранные строки из DataGridView
            //    DataGrid dataGrid = dataGridView;
            //    // Получение DataGridView из формы
            //    IList<Equipment> selectedEquipment = new List<Equipment>();
            //    foreach (DataGridViewRow row in dataGrid.Rows)
            //    {
            //        DataGridViewCheckBoxCell checkBox = row.Cells["SelectColumn"] as DataGridViewCheckBoxCell;
            //        if (checkBox != null && checkBox.Value != null && (bool)checkBox.Value)
            //        {
            //            selectedEquipment.Add(row.DataBoundItem as Equipment);
            //        }
            //    }

            //    // Добавляем выбранное оборудование в таблицу Word
            //    foreach (var item in selectedEquipment)
            //    {
            //        Word.Table table = doc.Bookmarks["Table"].Range.Tables[1];
            //        Word.Row row = table.Rows.Add();
            //        row.Cells[1].Range.Text = item.EquipmentName;
            //        row.Cells[2].Range.Text = item.Count.ToString();

            //        String Cells_3 = item.PurchaseDate.ToString();
            //        string originalString = Cells_3;
            //        string stringToRemove = " 0:00:00";
            //        string modifiedStringCells_3 = originalString.Replace(stringToRemove, string.Empty);

            //        row.Cells[3].Range.Text = modifiedStringCells_3;
            //        row.Cells[4].Range.Text = item.SerialNumber;
            //        row.Cells[5].Range.Text = item.EquipmentType;
            //    }

            //    // Удаляем пустую строку [текст] [текст] [текст] [текст] [текст] в таблице
            //    doc.Bookmarks["Table"].Range.Tables[1].Rows[2].Delete();

            //    // Сохраняем измененный документ Word
            //    string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //    string fileName = "Акт приема-передачи оборудования.docx";
            //    string fullPath = Path.Combine(savePath, fileName);
            //    doc.SaveAs(fullPath);
            //    doc.Close();

            //    // Выводим сообщение об успешном сохранении
            //    MessageBox.Show("Файл успешно сохранен на рабочем столе.", "Успешное выполнение задачи", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Information);
            //}

        }
    }
}
