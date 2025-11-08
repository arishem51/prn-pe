using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ServerHost = "localhost";
        private const int ServerPort = 5000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ServerHost, ServerPort);
                    using (var stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes("Hello World");
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}