using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Interface
{
    public class Editor
    {
        public static List<Editor> ActiveEditors = new List<Editor>();

        public WebView2? WebView { get; private set; }
        public bool IsInitialized { get; private set; }
        private string Latest;

        public Editor()
        {
            ActiveEditors.Add(this);
        }

        public static List<Editor> GetActiveEditors()
        {
            return ActiveEditors;
        }

        public async Task Initialize()
        {
            if (!IsInitialized)
            {
                await InitializeWV2();
                string url = "https://0zayn.github.io/Monaco/"; // Hosted on github, you fuck niggas

                if (url != null)
                {
                    if (WebView != null && WebView.CoreWebView2 != null)
                    {
                        WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                        WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;

                        WebView.CoreWebView2.NavigationCompleted += (sender, args) => { IsInitialized = true; };
                        WebView.CoreWebView2.Navigate(url);
                    }
                    else
                    {
                        MessageBox.Show("WebView or CoreWebView2 is null.");
                    }
                }
                else
                {
                    MessageBox.Show("No link to display was given.");
                }
            }
        }

        public async Task<string> GetEditor()
        {
            if (IsInitialized && WebView != null)
            {
                await WebView.CoreWebView2.ExecuteScriptAsync("window.chrome.webview.postMessage(editor.getValue())");
            }
            return WebView.CoreWebView2 != null ? Latest : string.Empty;
        }

        public async Task SetEditor(string code)
        {
            if (IsInitialized && WebView != null)
            {
                string formatted = Escape(code);
                await WebView.ExecuteScriptAsync($"editor.setValue(`{formatted}`);");
            }
        }

        private async Task InitializeWV2()
        {
            if (WebView == null)
            {
                WebView = new WebView2();
                await WebView.EnsureCoreWebView2Async(null);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Latest = e.TryGetWebMessageAsString();
        }

        private string Escape(string input)
        {
            return input.Replace("\\", "\\\\").Replace("`", "``").Replace("'", "\\'");
        }
    }
}