using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace KeyboardLanguageTooltip
{
    public partial class Form1 : Form
    {
        UserActivityHook actHook;
        readonly int document = 50030;
        readonly int pane = 50033;
        readonly int edit = 50004;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            actHook = new UserActivityHook();
            actHook.Start();
            actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
            
        }


        public static void Main(string[] args)
        {
            Application.Run(new Form1());
        }


        private void MouseMoved(object sender, MouseEventArgs e)
        {
            this.Location = new Point(e.X + 1, e.Y + 1);

            if (e.Clicks > 0)
            {
                if (GetControl(new System.Windows.Point(e.X, e.Y)))
                {
                    textBox1.Text = GetCurrentKeyboardLayout().Name;
                    textBox1.Visible = true;
                }
                else
                    textBox1.Visible = false;
            }
        }

        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
        [DllImport("user32.dll")] static extern IntPtr GetKeyboardLayout(uint thread);
        public CultureInfo GetCurrentKeyboardLayout()
        {
            try
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
                int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
                return new CultureInfo(keyboardLayout);
            }
            catch (Exception _)
            {
                return new CultureInfo(1033); // Assume English if something went wrong.
            }
        }

        private bool GetControl(System.Windows.Point point)
        {
            AutomationElement el = null;
            Task.Run(()=> {
                el = AutomationElement.FromPoint(point);
            }).Wait();

            if (el.Current.ControlType.Id == document || el.Current.ControlType.Id == pane || el.Current.ControlType.Id == edit)
                return true;
            else
                return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Visible = false;
        }
    }
}
