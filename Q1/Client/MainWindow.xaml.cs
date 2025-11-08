using System.Collections.ObjectModel;
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
        private readonly ObservableCollection<Start> _stars = new();

        public MainWindow()
        {
            InitializeComponent();
            StarListBox.ItemsSource = _stars;
            NationalityComboBox.SelectedIndex = 0; // Set default to "Viet Nam"
        }

        private void AddToListButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StarNameTextBox.Text))
            {
                MessageBox.Show("Star Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var star = new Start
            {
                Name = StarNameTextBox.Text,
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text,
                Male = IsMaleCheckBox.IsChecked,
                Nationality = NationalityComboBox.SelectedItem == null ? null : ((System.Windows.Controls.ComboBoxItem)NationalityComboBox.SelectedItem).Content?.ToString(),
                Dob = DobDatePicker.SelectedDate
            };

            _stars.Add(star);

            // Clear input fields
            StarNameTextBox.Clear();
            DescriptionTextBox.Clear();
            NationalityComboBox.SelectedIndex = 0; // Reset to default "Viet Nam"
            IsMaleCheckBox.IsChecked = false;
            DobDatePicker.SelectedDate = null;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_stars.Count == 0)
            {
                MessageBox.Show("No stars to send. Please add stars to the list first.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ServerHost, ServerPort);
                    using (var stream = client.GetStream())
                    {
                        // Serialize stars to JSON
                        var json = System.Text.Json.JsonSerializer.Serialize(_stars);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        await stream.WriteAsync(data, 0, data.Length);
                        
                        // Read response from server
                        var responseBuffer = new byte[1024];
                        var bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                        
                        if (bytesRead > 0)
                        {
                            var response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                            
                            if (response.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("Sent to server success", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Server response: {response}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
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
