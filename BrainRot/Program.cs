using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BrainRot
{
  
    static class HookMouse {
        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D
            // Add more mouse messages as needed
        }
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        public static bool MouseOneIsDown = false;
        public static bool mouseOneIsUp = true;
        public static bool MouseTwoIsDown=false;
        public static bool mouseTwoIsUp=true;
        public static int replacement=0;
        //
        private static IntPtr _hookID;
        public static MouseMessages mouseMessage;
        public static IntPtr HookID { get { return _hookID; } set { _hookID = HookID; } }
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        public static LowLevelMouseProc passer=HookCallback;
        
        

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // Set the hook using SetWindowsHookEx
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
            
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Check if the hook code is valid and it's a left mouse button down event
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MouseMessages mouseMessage = (MouseMessages)wParam.ToInt32();
                MouseOneIsDown = true;
                mouseOneIsUp = false;

            }
            else if(nCode >= 0 && wParam == (IntPtr)MouseMessages.WM_LBUTTONUP)
            {
                MouseOneIsDown = false;
                mouseOneIsUp = true;
            }
            if (nCode >= 0 && wParam == (IntPtr)MouseMessages.WM_RBUTTONDOWN)
            {
                MouseMessages mouseMessage = (MouseMessages)wParam.ToInt32();
                MouseTwoIsDown = true;
                mouseTwoIsUp = false;

            }
            else if (nCode >= 0 && wParam == (IntPtr)MouseMessages.WM_RBUTTONUP)
            {
                MouseTwoIsDown = false;
                mouseTwoIsUp = true;
            }
            // Call the next hook in the chain
           
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
         
        public static IntPtr _SetHook(){

           
            return SetHook(passer);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void unhook(IntPtr id)
        {
            UnhookWindowsHookEx(id);
        }


    }

    static class HookKeyboard {

        //constant valueand a delegate that will be pased unto a Function that will add to a another delegate 
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        
        public static LowLevelKeyboardProc _proc = HookCallback;
        public static IntPtr _hookID = IntPtr.Zero;
        //
        public static String KeyPressed = "";
        public static bool CTRPressed = false;
        public static bool CTRReleased = true;
        public static int ArrowKey =0;
        //

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {

                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                KeysConverter kc = new KeysConverter();
                string keyName = kc.ConvertToString(key);


                KeyPressed = keyName;
                if (keyName == "LControlKey")
                {
                    CTRPressed = true;
                    CTRReleased = false;
                }

            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                KeysConverter kc = new KeysConverter();
                string keyName = kc.ConvertToString(key);
                if (keyName == "LControlKey")
                {
                    CTRPressed = false;
                    CTRReleased = true;
                }
            }
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                // Key Down event
                if (Marshal.ReadInt32(lParam) == 0x27) // Left Arrow
                {
                    ArrowKey = 1;
                }
            }
            else if (wParam == (IntPtr)WM_KEYUP)
            {
                // Key Up event
                if (Marshal.ReadInt32(lParam) == 0x27) // Left Arrow
                {
                    ArrowKey = 0;
                }
            }
            //throw new NotImplementedException();
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                // Key Down event
                if (Marshal.ReadInt32(lParam) == 0x25) // Left Arrow
                {
                    ArrowKey = -1;
                }
            }
            else if (wParam == (IntPtr)WM_KEYUP)
            {
                // Key Up event
                if (Marshal.ReadInt32(lParam) == 0x25) // Left Arrow
                {
                    ArrowKey = 0;
                }
            }
          
            Console.WriteLine(CTRPressed);
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();


    }




    static class Program
    {       
        



        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string Result = Dec();
            string DefaultPath = @"C:\Users\User\OneDrive\Desktop\BrainRotVId";          
            if (Result=="Youtube") {
                HookKeyboard._hookID = HookKeyboard.SetHook(HookKeyboard._proc);
                HookMouse.HookID = HookMouse._SetHook();
                Application.Run(new Form1());
                
            }
            else if (Result=="Video")
            {   
               
                Application.Run(new Form4(Console.ReadLine()));
            }else if(Result=="Rot")
            {
                
                XmlDocument Writer = new XmlDocument();
                string Path = @"C:\Users\User\source\repos\BrainRot\BrainRot\InfoHolder.xml";
                Writer.Load(Path);
                string path = "//YourInfo/FileLocation";
                string FilePath = Writer.SelectSingleNode(path).InnerText;
                if (FilePath == "")
                {
                    Console.WriteLine("Give File Path");
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                     
                      folderBrowserDialog.Description = "Select a Folder";
                     folderBrowserDialog.SelectedPath = @"C:\";
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the selected folder path
                        string selectedFolderPath = folderBrowserDialog.SelectedPath;

                        // Do something with the selected folder path, for example:
                        MessageBox.Show("Selected folder: " + selectedFolderPath);
                        DefaultPath = selectedFolderPath;
                        Writer.SelectSingleNode(path).InnerText = selectedFolderPath;
                        Writer.Save(Path);
                    }
                   

                }
                HookKeyboard._hookID = HookKeyboard.SetHook(HookKeyboard._proc);
                HookMouse.HookID = HookMouse._SetHook();
                Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2(DefaultPath));
                

            }
            HookKeyboard.UnhookWindowsHookEx(HookKeyboard._hookID);
            HookMouse.unhook(HookMouse.HookID);
        }
        
        public static string Dec()
        {
            Console.WriteLine("Console is Open");
            Console.WriteLine("For Youtube type Youtube");
            Console.WriteLine("IF Whant to Play your own Video Type Video");
            Console.WriteLine("If you whant repeating non audio video type Rot");
            string returner = Console.ReadLine();
            if (returner == "Youtube" || returner == "Video" || returner == "Rot")
            {
                return returner;
            }
            returner = " ";
            Dec();
            return "";
        }
    }
}
