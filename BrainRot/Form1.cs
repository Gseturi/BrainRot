using Microsoft.Web.Services3.Security.Utility;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xamarin.Forms;
using BrainRot;
namespace BrainRot
{
    public partial class Form1 : Form
    {
        private Timer mousePositionTimer;
        System.Drawing.Point MousePoint;
        System.Drawing.Point start;
        System.Drawing.Point End;
        CookieContainer cookieContainer;
        public delegate void AddCookies();
        bool isinitialazed=false;
        private List<Uri> BackUrl;
        private Uri current;
        private List<Uri> FrontUrl;
        private int Page;
        public Form1()
        {
            BackUrl = new List<Uri>();
            FrontUrl = new List<Uri>();
            AddCookies adder;

            Uri x;
            UriKind temp;
            Uri.TryCreate("https://www.google.com/", UriKind.Absolute, out x);
            InitializeComponent();
            webView21.Dock = DockStyle.Fill;
            webView21.CoreWebView2InitializationCompleted += Initialized;
            
              webView21.Source = x;
            current = x;
            BackUrl.Add(x);
            //
            this.BackColor = System.Drawing.Color.Magenta;
            this.TopMost = true; 
            //
           mousePositionTimer = new Timer();
           mousePositionTimer.Interval = 100; // Set the interval in milliseconds (adjust as needed)
                                              //  mousePositionTimer.Tick += MousePositionTimer_Tick;
            mousePositionTimer.Tick += Checker;
            webView21.NavigationStarting += ChangeSource;
           mousePositionTimer.Start();
            
        }

        private void Checker(object sender, EventArgs e)
        {
            if (isinitialazed==true) {
                if (current.ToString() != webView21.Source.ToString() && BackUrl.Contains(webView21.Source) == false)
                {
                    BackUrl.Add(current);
                    Page = BackUrl.Count - 1;
                }
                current = webView21.Source;
                if (HookKeyboard.ArrowKey == -1)
                {
                    Uri temp = webView21.Source;
                    Page = Page - 1;
                    webView21.Source = BackUrl[Page];
                    BackUrl.Remove(temp);
                    FrontUrl.Add(temp);

                }
            }
        }

        private void ChangeSource(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
          //  Console.WriteLine(webView21.Source.ToString());
        }

        private void Initialized(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {   
            if (e.IsSuccess)
            {
                Microsoft.Web.WebView2.Core.CoreWebView2Cookie some = webView21.CoreWebView2.CookieManager.CreateCookie("some", "some1", "google.com", "/");
               // webView21.CoreWebView2.WebResourceRequested += WebResourceRequested;
                isinitialazed = true;
            }
        }
        private void addcookie(Microsoft.Web.WebView2.Core.CoreWebView2Cookie x)
        {
          webView21.CoreWebView2.CookieManager.AddOrUpdateCookie(x);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            webView21.Size = new System.Drawing.Size(this.Width,this.Height);

        }
        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        private void WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            // Check if Ctrl key is pressed and the left mouse button is clicked
            if (Control.ModifierKeys == Keys.Control && HookMouse.MouseOneIsDown) // 1 represents left mouse button
            {
                // Cancel the request to prevent opening a new tab
                e.Response = webView21.CoreWebView2.Environment.CreateWebResourceResponse(null, 204, "No Content", null);
            }
        }
        private void MousePositionTimer_Tick(object sender, EventArgs e)
        {
            Console.Write(HookKeyboard.CTRPressed);
            Control temp = this;
       
           
            // Get the current mouse position
            System.Drawing.Point mousePosition = Control.MousePosition;
            Console.WriteLine(IsMouseOverControl(webView21, mousePosition));

            // Check if the mouse is over the axWindowsMediaPlayer control
            if (IsMouseOverControl(webView21, mousePosition) == true && HookKeyboard.CTRPressed == true)
            {
                webView21.Visible = true;
                this.Visible = true;

                temp = this;
            }
            else if (IsMouseOverControl(webView21, mousePosition) == true && HookKeyboard.CTRPressed == false)
            {
                temp = this;
                webView21.Visible = false;
                this.Visible = false;




            }
            else if (IsMouseOverControl(webView21, mousePosition) == false && HookKeyboard.CTRPressed == false)
            {
                temp = this;
                webView21.Visible = true;
                this.Visible = true;


            }
            else if (IsMouseOverControl(webView21, mousePosition) == false && HookKeyboard.CTRPressed == true)
            {
                webView21.Visible = true;
                this.Visible = true;


            }
           
        }
        private bool IsMouseOverControl(Control control, System.Drawing.Point mousePosition)
        {
            return control.Bounds.Contains(control.PointToClient(mousePosition));
        }
        private void webView22_Click(object sender, EventArgs e)
        {

        }

        private void webView21_Click(object sender, EventArgs e)
        {
            webView21.Reload();
        }
        
    }
}
