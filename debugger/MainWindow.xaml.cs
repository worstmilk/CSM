using CSM.Commands;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Debugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Type> PacketTypes { get; } = new ObservableCollection<Type>();

        private string _selectedCommand;

        public string SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                _selectedCommand = value;
                OnPropertyChanged(nameof(SelectedCommand));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var types = typeof(CommandBase).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CommandBase)) && !t.IsAbstract);
            foreach (var type in types)
            {
                PacketTypes.Add(type);
            }
        }

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion Property Changed

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get basic information
            var command = ((Type)((ListView)sender).SelectedItem);
            SelectedCommand = $"{command.Name}:";

            // Build the UI
            SelectedCommandPanel.Children.Clear();

            var props = command.GetProperties().ToList();
            foreach (var p in props)
            {
                SelectedCommandPanel.Children.Add(new TextBlock() { Text = $"{p.Name} ({p.PropertyType.Name}):", Margin = new Thickness(0, 0, 0, 3) });

                if (p.PropertyType == typeof(bool))
                {
                    SelectedCommandPanel.Children.Add(new CheckBox() { Margin = new Thickness(0, 0, 0, 5) });
                }
                else if (p.PropertyType.IsEnum)
                {
                    var possibleValues = System.Enum.GetNames(p.PropertyType).ToList();
                    var comboBox = new ComboBox() { Margin = new Thickness(0, 0, 0, 5) };
                    possibleValues.ForEach(x => comboBox.Items.Add(x));

                    SelectedCommandPanel.Children.Add(comboBox);
                }
                else
                {
                    SelectedCommandPanel.Children.Add(new TextBox() { Margin = new Thickness(0, 0, 0, 5) });
                }
            }

            SelectedCommandPanel.Children.Add(new Button() { Content = "Send Packet", Margin = new Thickness(0, 5, 0, 0) });
        }
    }
}
