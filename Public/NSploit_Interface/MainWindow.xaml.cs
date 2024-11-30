using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using XenoUI;

namespace Interface
{
    public partial class MainWindow : Window
    {
        private ClientsWindow _clientsWindow;
        public MainWindow()
        {
            InitializeComponent();
            InitializeEditor();

            Module.StartServer();
        }

        private void InitializeEditor()
        {
            var editor = new Editor();
            editor.Initialize();

            var tabItem = new TabItem
            {
                Header = $"Untitled {TabControl.Items.Count + 1}",
                Style = (Style)FindResource("Tab")
            };

            var grid = new Grid();
            var webView = editor.WebView;

            if (webView != null)
            {
                webView.Height = 420;
                webView.VerticalAlignment = VerticalAlignment.Bottom;
                webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                grid.Children.Add(webView);
                tabItem.Content = grid;

                TabControl.Items.Add(tabItem);
                TabControl.SelectedItem = tabItem;
            }
            else
            {
                MessageBox.Show("Failed to initialize the editor!");
            }
        }

        private void RemoveEditor(TabItem tabItem)
        {
            if (tabItem.Content is Grid grid && grid.Children[0] is WebView2 webView)
            {
                var editorToRemove = Editor.GetActiveEditors().FirstOrDefault(editor => editor.WebView == webView);
                if (editorToRemove != null)
                {
                    Editor.GetActiveEditors().Remove(editorToRemove);
                }
            }
        }

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && FindParent<TabItem>(button) is TabItem tabItem && TabControl.Items.Count != 1)
            {
                TabControl.Items.Remove(tabItem);
                RemoveEditor(tabItem);
            }
        }

        private void AddTab(object sender, RoutedEventArgs e) => InitializeEditor();

        private T? FindParent<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null && !(obj is T))
                obj = VisualTreeHelper.GetParent(obj);

            return obj as T;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (Process Close = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "taskkill.exe",
                    Arguments = $"/F /IM NSploit_Interface.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            })
            {
                Close.Start();
                Close.WaitForExit();
            }
        }

        private void Attach_Click(object sender, RoutedEventArgs e)
        {
            /*   Process proc = new Process();
               proc.StartInfo.FileName = $"{Environment.CurrentDirectory}\\Injector.exe";
               proc.StartInfo.UseShellExecute = true;
               proc.StartInfo.Verb = "runas";
               proc.Start(); */
            try
            {

                if (_clientsWindow == null)
                {
                    _clientsWindow = new ClientsWindow();
                }

                ToggleWindowVisibility(_clientsWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Clients Window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedItem is TabItem selectedTab &&
                selectedTab.Content is Grid grid &&
                grid.Children[0] is WebView2 webView)
            {
                var selectedEditor = Editor.GetActiveEditors().FirstOrDefault(editor => editor.WebView == webView);
                if (selectedEditor != null)
                {
                    var script = await selectedEditor.GetEditor();
                    if (string.IsNullOrEmpty(script))
                    {
                        MessageBox.Show("Please enter a script to execute.");
                        return;
                    }

                    script = script.Replace("\r", "");
                    // aaaaaaaaaaaa
                    ExecuteScript(script);
                }
            }
        }

        private void Titlebar_LBD(object sender, MouseButtonEventArgs e) => DragMove();

        private void Control_LBU(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                switch (element.Name)
                {
                    case "Close":
                        Application.Current.Shutdown();
                        break;
                    case "Minimize":
                        WindowState = WindowState.Minimized;
                        break;
                }
            }
        }
        public void ExecuteScript(string scriptContent)
        {
            if (_clientsWindow == null)
            {
                MessageBox.Show("Clients window has not been initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_clientsWindow.ActiveClients.Any())
            {
                MessageBox.Show("No clients are currently selected. Please select at least one client before attempting to execute the script.", "No Client Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string status = _clientsWindow.GetCompilableStatus(scriptContent);
            if (status != "success")
                MessageBox.Show(status, "Compiler Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
                _clientsWindow.ExecuteScript(scriptContent);
        }
        private void ToggleWindowVisibility(Window window)
        {
            if (window.IsVisible)
                window.Hide();
            else
                window.Show();
        }
    }
}
