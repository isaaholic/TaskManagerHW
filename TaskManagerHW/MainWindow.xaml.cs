using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManagerHW
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; }
        public List<string> BlackList { get; set; }
        public ICommand BlackListCommand { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Processes = new(Process.GetProcesses());
            BlackList = new();
            BlackListCommand = new RelayCommand(ExecuteBlackListCommand);

            var timer = new Timer();
            timer.Interval = 2500;

            timer.Elapsed += RefreshProcesses;
            timer.Elapsed += ElapsedBlackList;
            timer.Start();
        }

        private void ExecuteBlackListCommand(object? parameter)
        {
            if (Datas.SelectedItem == null)
                return;

            if (Datas.SelectedItem is Process proc)
            {
                if (!BlackList.Contains(proc.ProcessName))
                    BlackList.Add(proc.ProcessName);
            }
        }

        private void RefreshProcesses(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Processes.Clear();
                foreach (var proc in Process.GetProcesses())
                    Processes.Add(proc);
            }
            );

        }

        private void ElapsedBlackList(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var proc in Processes)
                {
                    if (BlackList.Any(x => x == proc.ProcessName))
                    {
                        try
                        {
                            proc.Kill();
                            Processes.Remove(proc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message); ;
                        }
                    }
                }
            }
            );

        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Datas.SelectedItem == null)
                return;

            if (Datas.SelectedItem is Process proc)
            {
                try
                {
                    proc.Kill();
                    Processes.Remove(proc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTxt.Text))
            {
                MessageBox.Show("Process name must be not empty");
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new(SearchTxt.Text);

                startInfo.WindowStyle = ProcessWindowStyle.Minimized;

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BLTxt.Text))
            {
                MessageBox.Show("Enter Process Name");
                return;
            }

            BlackList.Add(BLTxt.Text);
        }
    }
}
