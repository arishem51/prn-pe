# WPF (Windows Presentation Foundation) Guide

## Introduction

WPF is a UI framework for building desktop applications in .NET. It uses XAML (eXtensible Application Markup Language) for declarative UI design and C# for code-behind logic.

## Project Structure

- **XAML files** (`.xaml`): Define the UI layout and appearance
- **Code-behind files** (`.xaml.cs`): Handle events and business logic
- **App.xaml**: Application-level resources and startup configuration

## Common Components and Use Cases

### 1. TextBox
**Purpose**: Single-line text input

**XAML Example**:
```xml
<TextBox x:Name="NameTextBox" Height="30" Margin="0,0,0,15"/>
```

**Code-behind Usage**:
```csharp
// Get value
string name = NameTextBox.Text;

// Set value
NameTextBox.Text = "John Doe";

// Clear
NameTextBox.Clear();

// Check if empty
if (string.IsNullOrWhiteSpace(NameTextBox.Text))
{
    // Handle empty input
}
```

**Common Use Cases**:
- User name input
- Search fields
- Form data entry
- Configuration values

---

### 2. ComboBox
**Purpose**: Dropdown list for selecting one option from multiple choices

**XAML Example**:
```xml
<ComboBox x:Name="NationalityComboBox" Height="30">
    <ComboBoxItem Content="Viet Nam"/>
    <ComboBoxItem Content="Japan"/>
    <ComboBoxItem Content="American"/>
</ComboBox>
```

**Code-behind Usage**:
```csharp
// Set default selection
NationalityComboBox.SelectedIndex = 0; // Select first item

// Get selected value
if (NationalityComboBox.SelectedItem != null)
{
    string selected = ((ComboBoxItem)NationalityComboBox.SelectedItem).Content?.ToString();
}

// Clear selection
NationalityComboBox.SelectedItem = null;

// Reset to default
NationalityComboBox.SelectedIndex = 0;
```

**Common Use Cases**:
- Country/region selection
- Category selection
- Status dropdowns
- Configuration options

---

### 3. DatePicker
**Purpose**: Date selection with calendar popup

**XAML Example**:
```xml
<DatePicker x:Name="DobDatePicker" Height="30"/>
```

**Code-behind Usage**:
```csharp
// Get selected date
DateTime? selectedDate = DobDatePicker.SelectedDate;

// Set date
DobDatePicker.SelectedDate = DateTime.Now;

// Clear selection
DobDatePicker.SelectedDate = null;

// Check if date is selected
if (DobDatePicker.SelectedDate.HasValue)
{
    DateTime date = DobDatePicker.SelectedDate.Value;
}
```

**Common Use Cases**:
- Birth date selection
- Start/end date ranges
- Deadline selection
- Event scheduling

---

### 4. CheckBox
**Purpose**: Boolean option (checked/unchecked)

**XAML Example**:
```xml
<CheckBox x:Name="IsMaleCheckBox" Content="Is male"/>
```

**Code-behind Usage**:
```csharp
// Get checked state
bool isChecked = IsMaleCheckBox.IsChecked ?? false;

// Set checked state
IsMaleCheckBox.IsChecked = true;
IsMaleCheckBox.IsChecked = false;

// Clear (unchecked)
IsMaleCheckBox.IsChecked = false;
```

**Common Use Cases**:
- Boolean flags (male/female, active/inactive)
- Feature toggles
- Agreement acceptance
- Option selection

---

### 5. Button
**Purpose**: Trigger actions when clicked

**XAML Example**:
```xml
<Button x:Name="AddButton" Content="Add To List" 
        Height="40" 
        Background="#E0E0E0"
        Click="AddButton_Click"/>
```

**Code-behind Usage**:
```csharp
private void AddButton_Click(object sender, RoutedEventArgs e)
{
    // Handle button click
    MessageBox.Show("Button clicked!");
}

// Enable/disable button
AddButton.IsEnabled = false;

// Change button text
AddButton.Content = "Processing...";
```

**Common Use Cases**:
- Form submission
- Navigation
- Action triggers (Save, Delete, Cancel)
- Confirmation dialogs

---

### 6. ListBox
**Purpose**: Display a scrollable list of items

**XAML Example**:
```xml
<ListBox x:Name="StarListBox" 
         BorderThickness="0"
         Background="Transparent"/>
```

**Code-behind Usage**:
```csharp
// Bind to collection
private readonly ObservableCollection<Star> _stars = new();
StarListBox.ItemsSource = _stars;

// Add item
_stars.Add(new Star { Name = "John" });

// Remove item
_stars.Remove(selectedStar);

// Get selected item
Star selected = StarListBox.SelectedItem as Star;

// Clear all items
_stars.Clear();
```

**Common Use Cases**:
- Displaying lists of data
- Shopping carts
- Task lists
- History logs

---

### 7. TextBlock
**Purpose**: Display read-only text

**XAML Example**:
```xml
<TextBlock Text="Star's info" FontSize="20" FontWeight="Bold"/>
```

**Code-behind Usage**:
```csharp
// Set text
InfoTextBlock.Text = "Welcome!";

// Change styling
InfoTextBlock.FontSize = 24;
InfoTextBlock.Foreground = Brushes.Red;
```

**Common Use Cases**:
- Labels
- Headers
- Status messages
- Instructions

---

### 8. MessageBox
**Purpose**: Display popup dialogs to the user

**Code-behind Usage**:
```csharp
// Information message
MessageBox.Show("Operation completed successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);

// Warning message
MessageBox.Show("Please fill in all required fields.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);

// Error message
MessageBox.Show($"Error: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);

// Confirmation dialog
MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirm", 
                                          MessageBoxButton.YesNo, 
                                          MessageBoxImage.Question);
if (result == MessageBoxResult.Yes)
{
    // User clicked Yes
}
```

**Common Use Cases**:
- Success/error notifications
- Validation messages
- Confirmation dialogs
- User feedback

---

## Layout Components

### Grid
**Purpose**: Flexible grid-based layout

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    
    <TextBlock Grid.Column="0" Grid.Row="0" Text="Left Top"/>
    <TextBlock Grid.Column="1" Grid.Row="1" Text="Right Bottom"/>
</Grid>
```

### StackPanel
**Purpose**: Stack elements vertically or horizontally

```xml
<StackPanel>
    <TextBox Height="30"/>
    <TextBox Height="30"/>
    <Button Content="Submit"/>
</StackPanel>
```

### Border
**Purpose**: Add borders and padding around content

```xml
<Border BorderBrush="#E0E0E0" BorderThickness="1" Padding="20">
    <TextBlock Text="Content"/>
</Border>
```

---

## Common Patterns

### 1. Form Validation
```csharp
private void SubmitButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(NameTextBox.Text))
    {
        MessageBox.Show("Name is required.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }
    
    // Process form
}
```

### 2. Data Binding with ObservableCollection
```csharp
private readonly ObservableCollection<Item> _items = new();

public MainWindow()
{
    InitializeComponent();
    ListBox.ItemsSource = _items; // Bind once
}

private void AddItem()
{
    _items.Add(new Item { Name = "New Item" }); // UI updates automatically
}
```

### 3. Clearing Form Fields
```csharp
private void ClearForm()
{
    NameTextBox.Clear();
    DescriptionTextBox.Clear();
    ComboBox.SelectedIndex = 0; // Reset to default
    CheckBox.IsChecked = false;
    DatePicker.SelectedDate = null;
}
```

### 4. Async Operations
```csharp
private async void SendButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        SendButton.IsEnabled = false; // Disable during operation
        SendButton.Content = "Sending...";
        
        // Perform async operation
        await SendDataAsync();
        
        MessageBox.Show("Success!", "Info", MessageBoxButton.OK, 
                        MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
    }
    finally
    {
        SendButton.IsEnabled = true;
        SendButton.Content = "Send";
    }
}
```

---

## Best Practices

1. **Naming Convention**: Use descriptive names with component type suffix
   - `NameTextBox`, `SubmitButton`, `ItemsListBox`

2. **Event Handlers**: Follow `ComponentName_EventName` pattern
   - `AddButton_Click`, `NameTextBox_TextChanged`

3. **Validation**: Always validate user input before processing

4. **Error Handling**: Use try-catch blocks for operations that can fail

5. **UI Feedback**: Disable buttons during async operations and show progress

6. **Data Binding**: Use `ObservableCollection` for dynamic lists that need UI updates

7. **Default Values**: Set sensible defaults for form controls

8. **User Experience**: Provide clear error messages and confirmations

---

## Quick Reference

| Component | Get Value | Set Value | Clear |
|-----------|-----------|-----------|-------|
| TextBox | `.Text` | `.Text = "value"` | `.Clear()` |
| ComboBox | `.SelectedItem` | `.SelectedIndex = 0` | `.SelectedItem = null` |
| DatePicker | `.SelectedDate` | `.SelectedDate = DateTime.Now` | `.SelectedDate = null` |
| CheckBox | `.IsChecked` | `.IsChecked = true` | `.IsChecked = false` |
| ListBox | `.SelectedItem` | `.ItemsSource = collection` | `.ItemsSource = null` |

---

## Resources

- [Microsoft WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [WPF Tutorial](https://www.wpf-tutorial.com/)
- [XAML Syntax Guide](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/xaml/)

