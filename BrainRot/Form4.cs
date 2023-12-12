using CefSharp;
using CefSharp.WinForms;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Text.RegularExpressions;

namespace BrainRot
{
    public partial class Form4 : Form
    {
        ChromiumWebBrowser chromeBrowser;
        Timer InitCheker;
        String Url;
        String ApiKey;
        String VideoId;
            
        public Form4(String Url)
        {
            this.BackColor = System.Drawing.Color.Magenta;
            this.TopMost = true;
            this.Url = Url;
            Url = GetYouTubeEmbedLink(Url);
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            
            InitializeComponent();
            TimerStart();
        
            

         


        }
        
        private void LoadHtml(string htmlContent)
        {
            chromeBrowser.LoadHtml(htmlContent, "https://www.youtube.com/");
        }
        private void TimerStart()
        {
            InitCheker = new Timer();
            InitCheker.Interval = 100;
            InitCheker.Tick+= InitChecker_;
            InitCheker.Start();
        }

        private void InitChecker_(object sender, EventArgs e)
        {
            if (Cef.IsInitialized==true)
            {
                chromeBrowser = new ChromiumWebBrowser();
                Controls.Add(chromeBrowser);
                chromeBrowser.Dock = DockStyle.Fill;
                LoadHtml($"<html><head><iframe width='100%' height='100%' src='{Url}'></iframe></head></html>");
                InitCheker.Stop();
            }
            
        }


        static string GetYouTubeEmbedLink(string youtubeUrl)
        {
            try
            {
                // Regular expression to match YouTube video ID in various URL formats
                Regex regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})", RegexOptions.IgnoreCase);

                Match match = regex.Match(youtubeUrl);

                if (match.Success)
                {
                    // Extract the video ID from the match
                    string videoId = match.Groups[1].Value;

                    // Construct the embeddable URL
                    string embedLink = $"https://www.youtube.com/embed/{videoId}";

                    return embedLink;
                }
                else
                {
                    Console.WriteLine("Invalid YouTube URL");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

    }
}
