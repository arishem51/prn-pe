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
                Nationality = string.IsNullOrWhiteSpace(NationalityTextBox.Text) ? null : NationalityTextBox.Text,
                Dob = DobDatePicker.SelectedDate
            };

            _stars.Add(star);

            // Clear input fields
            StarNameTextBox.Clear();
            DescriptionTextBox.Clear();
            NationalityTextBox.Clear();
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
                    }
                }

                MessageBox.Show($"Successfully sent {_stars.Count} star(s) to server.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}