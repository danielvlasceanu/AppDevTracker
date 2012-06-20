using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// Custom namespaces
using AppDevTracker.DataContext;
using Log = Common.Diagnostics.LogHandler;

namespace AppDevTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool available = this.GetDatabaseConnection();

            // Configure according connectivity status
            this.SetLoginStatus(available);
            this.SetConnectionStatus(available);

            // Save connection status
            Properties.Settings.Default.Connected = available;
            Properties.Settings.Default.Save();
        }

        private bool GetDatabaseConnection()
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Properties.Settings.Default.ADTConnectionString))
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();

                try
                {
                    connection.Open();

                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    // Unspecified datasource or the connection is already open.
                    Properties.Settings.Default.Connected = false;
                    MessageBox.Show("Database connection failed. Contact the application administrator.");

                    return false;
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    // Other Sql internal error or expired sql password.
                    Properties.Settings.Default.Connected = false;
                    MessageBox.Show("Database connection failed. This kind of error usually arises when the authentication password expires\r\nbut can also be caused by other SQL Server internal problems.\r\n\r\nPlease contact the application administrator.");

                    return false;
                }
                finally
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                }
            }
        }
        private void SetLoginStatus(bool status)
        {
            this.spLogin.IsEnabled = status;
        }
        private void SetConnectionStatus(bool status)
        {
            if (status == true)
            {
                tblConnectionStatus.Text = "The database is available and ready.";
                bdrFooter.Background = Brushes.PowderBlue;
            }
            else
            {
                tblConnectionStatus.Text = "The database is NOT available.";
                bdrFooter.Background = Brushes.MistyRose;
            }
        }
    }
}
