using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrainRot;

namespace BrainRot
{
    public partial class Form2 : Form
    {
        Point MousePoint;
        Point start;
        Point End;
        //
        private Timer Keypress;
        private Timer mousePositionTimer;
        bool vis;
        bool ClickLocChange=false;
        //
        int count = 0;
        FileInfo[] Files;
        private int rand(int leng)
        {   
         
            int loc =new Random().Next(leng);
            return loc;
        }

        private string DefaultLocation;

        public Form2(string DefaultLocation)
        {
            MousePoint = new Point();
            //
            this.DefaultLocation = DefaultLocation;
            DirectoryInfo d = new DirectoryInfo(DefaultLocation);
            Files = d.GetFiles("*.mp4");
            InitializeComponent();
            this.Visible = true;
            //
            this.FormBorderStyle = FormBorderStyle.None;
            this.TransparencyKey = Color.Magenta;
            this.BackColor = Color.Magenta;
           
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            //
            axWindowsMediaPlayer1.Visible = true;
            //
            this.Location = new System.Drawing.Point(0, 0);
            this.Resize += Form2_Resize;
            axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;
            this.FormBorderStyle = FormBorderStyle.None;
            // use less axWindowsMediaPlayer1.MouseMoveEvent += axWindowsMediaPlayer1_MouseMoveEvent;
            
            axWindowsMediaPlayer1.uiMode = "None";
            int temp = rand(Files.Length);
            axWindowsMediaPlayer1.URL = DefaultLocation+"\\"+Files[temp];
           
            //axWindowsMediaPlayer1.URL = @"C:\Users\User\OneDrive\Desktop\BrainRotVId\Hitman_2023.11.02-06.18.mp4";
            TimerSetupKeyBoard();
            mousePositionTimer = new Timer();
            mousePositionTimer.Interval = 100; // Set the interval in milliseconds (adjust as needed)
            mousePositionTimer.Tick += MousePositionTimer_Tick;
            mousePositionTimer.Tick += MouseMoveForm;

            mousePositionTimer.Start();
            KeyPress += new KeyPressEventHandler(KeyTrack);
            //

            

        }
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            ClickLocChange = true;

        }
        private void TimerSetupKeyBoard()
        {
            if (this.Focused)
            {
                Console.WriteLine("is focused");
            }
            Keypress = new Timer();
            Keypress.Interval = 100;
           //Keypress.Tick += CtrKeypressCheck;

            Keypress.Start();
        }
       



        private void CtrKeypressCheck(object sender,EventArgs e)
        {   
           
            if (HookKeyboard.CTRPressed ==true)
            {
                vis = true;
                
            }
            else if(HookKeyboard.CTRReleased ==false)
            {
                
                
            }
            HookKeyboard.KeyPressed = "";
            // this makes it so that you need to hold down ctr Program.KeyPressed = "";
        }



        private void KeyTrack(object sender, KeyPressEventArgs e)
        {
            throw new NotImplementedException();
            Console.WriteLine(e.KeyChar);
        }

        private void MouseMoveForm(object sender, EventArgs e)
        {
            bool exclude = false;
            Point mousePosition = Control.MousePosition;

            if (HookKeyboard.CTRPressed == true && HookMouse.MouseOneIsDown==true)
            {
                this.Location = new System.Drawing.Point(mousePosition.X, mousePosition.Y);

            }


            if (exclude==false) {
                Console.WriteLine(HookMouse.MouseTwoIsDown);
                if (HookKeyboard.CTRPressed == true && HookMouse.MouseTwoIsDown == true)
                {
                    start = Control.MousePosition;
                    exclude = true;
                }
            }
            Console.WriteLine(HookMouse.mouseTwoIsUp+"something");
            if (exclude == true)
            {
                if (HookKeyboard.CTRPressed == true && HookMouse.mouseTwoIsUp == true)
                {
                    End = Control.MousePosition;
                    
                    int x = End.X - start.X;
                    int y = End.Y - start.Y;
                    if (x<0)
                    {
                        x = -x;
                    }
                    if (y < 0)
                    {
                        y = -y;
                    }
                    this.Size = new Size(this.Size.Width+x,this.Size.Height+y);
                    exclude = false;
               }
            }

            


        }
       
        private void MousePositionTimer_Tick(object sender, EventArgs e)
        {

            Control temp=this;
            
            // Get the current mouse position
            Point mousePosition = Control.MousePosition;
            Console.WriteLine(IsMouseOverControl(axWindowsMediaPlayer1, mousePosition));
           
                // Check if the mouse is over the axWindowsMediaPlayer control
                if (IsMouseOverControl(axWindowsMediaPlayer1, mousePosition) == true && HookKeyboard.CTRPressed == true)
                {   
                    axWindowsMediaPlayer1.Visible = true;
                    

                    temp = this;
                }
                else if (IsMouseOverControl(axWindowsMediaPlayer1, mousePosition) == true && HookKeyboard.CTRPressed == false)
                {
                    temp = this;
                    axWindowsMediaPlayer1.Visible = false;
                   



                }else if (IsMouseOverControl(axWindowsMediaPlayer1, mousePosition) == false && HookKeyboard.CTRPressed == false)
                {
                    temp = this;
                    axWindowsMediaPlayer1.Visible = true;
                   

                }else if (IsMouseOverControl(axWindowsMediaPlayer1, mousePosition) == false && HookKeyboard.CTRPressed == true)
                {
                    axWindowsMediaPlayer1.Visible = true;
                   

                }
            
        }

        private bool IsMouseOverControl(Control control, Point mousePosition)
        {
            return control.Bounds.Contains(control.PointToClient(mousePosition));
        }
       
     

        
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            Console.WriteLine("Fired");
            if ((WMPLib.WMPPlayState)e.newState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                axWindowsMediaPlayer1.URL = DefaultLocation + "\\" + Files[rand(Files.Length)];
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Size= new System.Drawing.Size(this.Size.Width, this.Size.Height);
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {   
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        //use less
        /*private void axWindowsMediaPlayer1_MouseMoveEvent(object sender, AxWMPLib._WMPOCXEvents_MouseMoveEvent e)
            {
            if (vis==false) {
                // Check if the mouse is within the bounds of the axWindowsMediaPlayer control
                if (e.fX >= axWindowsMediaPlayer1.Left && e.fX <= axWindowsMediaPlayer1.Right &&
                    e.fY >= axWindowsMediaPlayer1.Top && e.fY <= axWindowsMediaPlayer1.Bottom)
                {
                    axWindowsMediaPlayer1.Visible = false;
                    this.Visible = false;
                    Console.WriteLine("on");
                }
            }
            } 
        
         */
    }
}
