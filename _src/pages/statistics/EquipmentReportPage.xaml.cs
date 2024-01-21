using System;
using System.Data;
using System.Collections.Generic;
using System.Data.OleDb;
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
using Microsoft.Reporting.WinForms;
using TechInventory._src.database;
using System.Data.SqlClient;

namespace TechInventory._src.pages.statistics
{
    /// <summary>
    /// Логика взаимодействия для EquipmentReportPage.xaml
    /// </summary>
    public partial class EquipmentReportPage : Page
    {
        public EquipmentReportPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Создаем объект DataSet
            DataSet myDataSet = new DataSet();

            // Строка подключения к базе данных SQL Server LocalDB
            string connectionString = @"data source=(LocalDb)\MSSQLLocalDB;initial catalog=TechInventory;integrated security=True";

            // SQL-запрос
            string sqlQuery = "SELECT * FROM EquipmentHistoryWithEmployeesAndEquipmentName";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConn);
                    sqlCommand.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

                    // Заполняем DataSet данными из базы данных
                    da.Fill(myDataSet);
                }
                catch (Exception exMessage)
                {
                    MessageBox.Show(exMessage.Message);
                }
            }

            // Устанавливаем режим обработки на локальный
            MyReportViewer.ProcessingMode = ProcessingMode.Local;

            // Указываем путь к файлу отчета
            MyReportViewer.LocalReport.ReportPath = @"C:\Users\danb9\Desktop\C#\TechInventory\_src\pages\statistics\EquipmentReport.rdlc"; // Замените на фактический путь к вашему отчету

            // Добавляем данные в отчет
            MyReportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet_EpAndEq", myDataSet.Tables[0]));

            // Устанавливаем режим отображения
            MyReportViewer.SetDisplayMode(DisplayMode.PrintLayout);

            // Обновляем отчет
            MyReportViewer.RefreshReport();
        }

    }
}
    

