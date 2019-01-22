using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.CSharp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using VX_Library;
using System.Text.RegularExpressions;
using System.IO;

namespace VoltDev
{
    public partial class Form1 : Form
    {
        private string filePath = "";

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

        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        //styles
        public readonly Style CommentStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Regular);
        public readonly Style LawnGreenStyle = new TextStyle(Brushes.ForestGreen, null, FontStyle.Regular);
        public readonly Style LightBlueStyle = new TextStyle(Brushes.DodgerBlue, null, FontStyle.Regular);
        public readonly Style HardBlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public readonly Style AquaStyle = new TextStyle(Brushes.MediumAquamarine, null, FontStyle.Regular);
        public readonly Style NameStyle = new TextStyle(null, null, FontStyle.Underline);
        public readonly Style ClosedStyle = new TextStyle(null, null, FontStyle.Strikeout);
        public readonly Style YellowStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Regular);
        public readonly Style BlackBGStyle = new TextStyle(null, Brushes.Black, FontStyle.Regular);
        public readonly Style RedStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
        public readonly Style MaroonStyle = new TextStyle(Brushes.Chocolate, null, FontStyle.Regular); //string

        public Form1()
        {
            InitializeComponent();
            SwitchStripsTheme.DarkTheme(panel2.Controls,Color.FromArgb(20,20,20),Color.FromArgb(28,28,28));
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public static RegexOptions RegexCompiledOption
        {
            get
            {
                if (platformType == Platform.X86)
                    return RegexOptions.Compiled;
                else
                    return RegexOptions.None;
            }
        }

        private void InitVoltExSyntax()
        {
            fastColoredTextBox1.RightBracket2 = '[';
            fastColoredTextBox1.LeftBracket2 = ']';
            fastColoredTextBox1.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            fastColoredTextBox1.AutoIndentCharsPatterns = @"
             ^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);
             ^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //clear
            e.ChangedRange.ClearStyle(CommentStyle, LightBlueStyle, AquaStyle, HardBlueStyle,
                LawnGreenStyle, YellowStyle, NameStyle);
            e.ChangedRange.ClearFoldingMarkers();
            //HIGHLIGHT------------------------------------------------
            e.ChangedRange.SetStyle(CommentStyle, @"#.*$");
            e.ChangedRange.SetStyle(YellowStyle, @"\b(plugin|event|on_start|config)\b");
            e.ChangedRange.SetStyle(YellowStyle, @"\b(change|print|use|storage|message|save|load|
                                                      run|render|console)\b");
            //e.ChangedRange.SetStyle(AquaStyle, @"\b()\s+(?<range>[\w_]+?)\b");
            e.ChangedRange.SetStyle(LightBlueStyle, @"\b(new|object|label|lb|button|bt|progressbar|pbar|
                                                         richtextbox|rtb|textbox|tb|panel|pn|treeview|tview
                                                         webbrowser|browser|function|end)\b");
            e.ChangedRange.SetStyle(LawnGreenStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            e.ChangedRange.SetStyle(MaroonStyle, @"""""|"".*?[^\\]""");
            //e.ChangedRange.SetStyle(NameStyle, @"\b()\s+(?<range>[\w_]+?)\b");
            //block of code
            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers(@"#block\b", @"#endblock\b");
        }

        private void RefreshSyntax()
        {
            string data = fastColoredTextBox1.Text;
            fastColoredTextBox1.Clear();
            fastColoredTextBox1.Text = data;
        }

        private void Save()
        {
            if(filePath!="")
            {
                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(fastColoredTextBox1.Text);
                sw.Close();
            }
            else
            {
                SaveDlg();
            }
        }
        private void SaveDlg()
        {
            SaveFileDialog sfg = new SaveFileDialog();
            sfg.Filter = "VX File|*.vx|Text File|*.txt|Any File|*.*";
            if (sfg.ShowDialog()==DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfg.FileName);
                sw.Write(fastColoredTextBox1.Text);
                sw.Close();

                filePath = sfg.FileName;
            }
        }
        private void OpenDlg()
        {
            OpenFileDialog sfg = new OpenFileDialog();
            sfg.Filter = "VX File|*.vx|Text File|*.txt|Any File|*.*";
            if(sfg.ShowDialog()==DialogResult.OK)
            {
                StreamReader sr = new StreamReader(sfg.FileName);
                fastColoredTextBox1.Text = sr.ReadToEnd();
                sr.Close();

                filePath = sfg.FileName;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(fastColoredTextBox1.Text.Replace(" ","")=="")
            {
                Application.Exit();
            }
            else
            {
                DialogResult mb = MessageBox.Show("Do you want to save file?","Saving", MessageBoxButtons.YesNoCancel);
                if(mb==DialogResult.Yes)
                {
                    Save();
                    Application.Exit();
                }
                else if(mb==DialogResult.Cancel)
                {

                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowFindDialog();
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowGoToDialog();
        }

        private void endToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.GoEnd();
        }

        private void fastColoredTextBox1_AutoIndentNeeded(object sender, AutoIndentEventArgs e)
        {

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDlg();
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDlg();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("Are you sure?", "Deleting code", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
                fastColoredTextBox1.Clear();
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveFileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDlg();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("Are you sure?", "Deleting code", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
                fastColoredTextBox1.Clear();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ShowReplaceDialog();
        }

        private void cApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Executable File|*.exe|Library|*.dll";
            if(sfd.ShowDialog()==DialogResult.OK)
            {
                path = sfd.FileName;
            }
            if(path!="")
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                ICodeCompiler icc = codeProvider.CreateCompiler();
                string Output = path.GetAfter(@"\");
                CompilerParameters parameters = new CompilerParameters();
                if(path.Contains(".exe"))
                    parameters.GenerateExecutable = true;
                else
                    parameters.GenerateExecutable = false;

                parameters.OutputAssembly = path;
                CompilerResults results = icc.CompileAssemblyFromSource(parameters, fastColoredTextBox1.Text);

                if (results.Errors.Count > 0)
                {
                    ErrorsList el = new ErrorsList(results.Errors);
                    el.Show();
                }
                else
                {
                    //Successful Compile
                    MessageBox.Show("Compiled");
                    //Process.Start(Output);
                }
            }
        }

        private void buildRunToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string buildFolder = Application.StartupPath + @"\build\builded";
            foreach(string f in Directory.GetFiles(buildFolder))
            {
                File.Delete(f);
            }

            foreach(string f in Directory.GetFiles(Application.StartupPath+@"\config\build\temp"))
            {
                string name = f.GetAfter(@"\");
                File.Copy(f,buildFolder+@"\"+name);
                if(name=="config.vxc")
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(buildFolder + @"\" + name);
                        sw.Write(fastColoredTextBox1.Text);
                        //sw.Write(Translate.Encrypt(fastColoredTextBox1.Text, "vx"));
                        sw.Close();
                    }
                    catch { MessageBox.Show("Builded app has errors"); }
                }
            }
            Process.Start(buildFolder);
        }

        private void testRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string buildFolder = Application.StartupPath + @"\build\builded";
            foreach (string f in Directory.GetFiles(buildFolder))
            {
                try
                {
                    File.Delete(f);
                }
                catch { MessageBox.Show("failed"); }
            }

            foreach (string f in Directory.GetFiles(Application.StartupPath + @"\config\build\temp"))
            {
                string name = f.GetAfter(@"\");
                File.Copy(f, buildFolder + @"\" + name);
                if (name == "config.vxc")
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(buildFolder + @"\" + name);
                        sw.Write(fastColoredTextBox1.Text);
                        //sw.Write(Translate.Encrypt(fastColoredTextBox1.Text, "vx"));
                        sw.Close();
                    }
                    catch { MessageBox.Show("Builded app has errors"); }
                }
            }
            try
            {
                Process.Start(buildFolder + @"\App.exe");
            }
            catch { MessageBox.Show("Failed running app"); }
        }

        private void codeMarketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CodeMarket cm = new CodeMarket();
            cm.Show();
        }
    }
}
