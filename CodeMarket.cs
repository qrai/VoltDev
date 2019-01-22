using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoltDev
{
    public partial class CodeMarket : Form
    {
        private string VePluginPage =
            "<html> \n" +

            "<head> \n" +
            "<style> \n" +
            "body { \n" + //body
            "background-color: RGB(30,30,30); \n" +
            "} \n" +
            "h1 { \n" + //h1
            "color: RGB(240,240,240); \n" +
            "font-family: " + '"'.ToString() + "Ubuntu" + '"'.ToString() + "\n" +
            "} \n" +
            "h3 { \n" + //h3
            "color: RGB(240,240,240); \n" +
            "font-family: " + '"'.ToString() + "Ubuntu" + '"'.ToString() + "\n" +
            "} \n" +
            "</style> \n" +
            "</head> \n" +

            "<body> \n" +
            "<h1>VE Plugin</h1> \n" + //title(name)
            "<h3></h3> \n" + //space
            "<h3>Price: Free</h3>" + //price
            "<h3>Publisher: Volt Official</h3>" + //publisher
            "<h3>Description: Basic plugin code for Volt Engine 2.0</h3>" +
            "</body> \n" +

            "</html>";

        private string VdPluginPage =
            "<html> \n" +

            "<head> \n" +
            "<style> \n" +
            "body { \n" + //body
            "background-color: RGB(30,30,30); \n" +
            "} \n" +
            "h1 { \n" + //h1
            "color: RGB(240,240,240); \n" +
            "font-family: " + '"'.ToString() + "Ubuntu" + '"'.ToString() + "\n" +
            "} \n" +
            "h3 { \n" + //h3
            "color: RGB(240,240,240); \n" +
            "font-family: " + '"'.ToString() + "Ubuntu" + '"'.ToString() + "\n" +
            "} \n" +
            "</style> \n" +
            "</head> \n" +

            "<body> \n" +
            "<h1>VD Plugin</h1> \n" + //title(name)
            "<h3></h3> \n" + //space
            "<h3>Price: Free</h3>" + //price
            "<h3>Publisher: Volt Official</h3>" + //publisher
            "<h3>Description: Basic plugin code for VoltDev</h3>" +
            "</body> \n" +

            "</html>";
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public CodeMarket()
        {
            InitializeComponent();
            webBrowser1.DocumentText = VePluginPage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void CodeMarket_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
