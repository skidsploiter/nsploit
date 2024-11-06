using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using System.Windows;

namespace Interface
{
    public class Module
    {
        private static IHost? host;
        private static readonly MemoryStream ScriptS = new MemoryStream();
        private static readonly object StreamLock = new object();

        public static void StartServer()
        {
            host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://localhost:8440");
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/script", async context =>
                        {
                            context.Response.ContentType = "application/json; charset=utf-8";

                            byte[] buffer;
                            lock (StreamLock)
                            {
                                ScriptS.Position = 0;
                                buffer = ScriptS.ToArray();
                            }
                            await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                        });

                        endpoints.MapGet("/clear", async context =>
                        {
                            await Task.Run(() =>
                            {
                                lock (StreamLock)
                                {
                                    ScriptS.SetLength(0);
                                    var data = new { Data = "", Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() };
                                    var options = new JsonSerializerOptions
                                    {
                                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                        WriteIndented = true
                                    };
                                    using (StreamWriter writer = new StreamWriter(ScriptS, Encoding.UTF8, 1024, true))
                                    {
                                        string json = JsonSerializer.Serialize(data, options);
                                        writer.Write(json);
                                    }
                                    ScriptS.Position = 0;
                                }
                            });
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("Script data cleared");
                        });
                    });
                });
            }).Build();
            host.StartAsync();
        }

        public static async void ExecuteScript(string Script)
        {
            if (string.IsNullOrEmpty(Script))
            {
                MessageBox.Show("Please enter a script to execute.");
                return;
            }

            Script = Script.Replace("\r", "");

            await Task.Run(() =>
            {
                lock (StreamLock)
                {
                    ScriptS.SetLength(0);
                    var data = new { Data = Script, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() };
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    };
                    using (StreamWriter writer = new StreamWriter(ScriptS, Encoding.UTF8, 1024, true))
                    {
                        string json = JsonSerializer.Serialize(data, options);
                        writer.Write(json);
                    }
                    ScriptS.Position = 0;
                }
            });
        }
    }
}
