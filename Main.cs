using Microsoft.VisualBasic.Devices;
using System.IO;
using System.Windows.Forms;
using ZenScribe.Helpers;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TextEdit
{
    public partial class Main : Form
    {
        const int updateInterval = 1000;
        const int autosaveInterval = 300000;

        Color black = System.Drawing.ColorTranslator.FromHtml("#000000");
        Color black1 = System.Drawing.ColorTranslator.FromHtml("#222323");
        Color black2 = System.Drawing.ColorTranslator.FromHtml("#382b26");
        Color black3 = System.Drawing.ColorTranslator.FromHtml("#3e232c");
        Color white = System.Drawing.ColorTranslator.FromHtml("#ffffff");
        Color white1 = System.Drawing.ColorTranslator.FromHtml("#f0f6f0");
        Color white2 = System.Drawing.ColorTranslator.FromHtml("#b8c2b9");
        Color white3 = System.Drawing.ColorTranslator.FromHtml("#edf6d6");
        public Color[] backgroundColors = new Color[3];
        public Color[] foregroundColors = new Color[3];

        public string[] fonts = new string[3] { "Courier New", "Verdana", "Trebouchet MS" };
        int currentColorSchemeIndex = 0;
        int currentFontIndex = 0;
        int currentFontSize = 18;
        public string currentFontName;
        public string currentFile = "";
        public int wordCount = 0;
        public int charactersCount = 0;
        public int charactersCountWithOutSpaces = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autoSaveTimer = new System.Windows.Forms.Timer();
        KeyboardHelper kh;
        FileHelper fh;

        public Main()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CustomInitialize();
        }
        private void MainText_TextChanged(object sender, EventArgs e)
        {
            if (MainText.Text.Length > 3) {
                if (MainText.Text.Substring(MainText.TextLength - 3, 3) == "___")
                {
                    kh.AddSeparator();
                }
            }
            UpdateCounters();
        }
        private void MainText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                kh.AddTitle();
            }

            if (e.KeyCode == Keys.Tab){
                e.SuppressKeyPress = true;
                kh.AddTab();
            }

            if (e.Alt && e.KeyCode == Keys.F) {
                currentFontIndex++;
                if (currentFontIndex > fonts.Length - 1) currentFontIndex = 0;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Alt && e.KeyCode == Keys.C) { 
                currentColorSchemeIndex++;
                if (currentColorSchemeIndex > backgroundColors.Length - 1) currentColorSchemeIndex = 0;
                UpdateColors();
            }

            if (e.Alt && e.KeyCode == Keys.Oemplus){
                currentFontSize++;
                if (currentFontSize > 64) currentFontSize = 64;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Alt && e.KeyCode == Keys.OemMinus){
                currentFontSize--;
                if(currentFontSize < 8) currentFontSize = 8;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Control && e.KeyCode == Keys.S){
                if (!string.IsNullOrEmpty(currentFile))
                {
                    fh.SaveFile(currentFile, MainText);
                    ShowSavedFile();
                }
                else {
                    fh.SaveNewFile(currentFile, MainText);
                }
            }

            if (e.Control && e.KeyCode == Keys.F){
                Search s = new Search();
                s.ShowDialog();
                MainText.Find(s.ReturnValue);
            } 

            if (e.Control && e.KeyCode == Keys.O) {
                Tuple<string,string> openFileResult = fh.OpenFile();
                MainText.Text = openFileResult.Item2;
                MainText.SelectionStart = MainText.Text.Length;
                MainText.SelectionLength = 0;
                currentFile = openFileResult.Item1;
                UpdateCounters();
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }
        private void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFile))
            {
                fh.SaveFile(currentFile, MainText);
                ShowSavedFile();
            }
        }
        private void ShowSavedFile() {
            Notification.Text = "File saved!";
            Notification.Refresh();
            System.Threading.Thread.Sleep(1000);
            Notification.Text = "";
            Notification.Refresh();
        }
        private void UpdateFont() {
            System.Drawing.Font currentFont = new System.Drawing.Font(fonts[currentFontIndex], currentFontSize);
            MainText.Font = currentFont;
            currentFontName = currentFont.Name;
            UpdateCounters();
        }
        private void UpdateColors() {
            MainText.BackColor = backgroundColors[currentColorSchemeIndex];
            MainText.ForeColor = foregroundColors[currentColorSchemeIndex];
        }
        private void CustomInitialize() {
            kh = new KeyboardHelper(MainText);
            fh = new FileHelper();
            System.Drawing.Font currentFont = new System.Drawing.Font("Courier New", 18);
            MainText.Font = currentFont;
            backgroundColors[0] = black;
            backgroundColors[1] = black2;
            backgroundColors[2] = black3;
            foregroundColors[0] = white;
            foregroundColors[1] = white2;
            foregroundColors[2] = white3;
            currentFontName = currentFont.Name;
            MainText.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Counter.Anchor = Notification.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Clock.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            MainText.KeyDown += new KeyEventHandler(MainText_KeyDown);
            timer.Tick += new EventHandler(timer_Tick);
            autoSaveTimer.Tick += new EventHandler(autoSaveTimer_Tick);
            MainText.AcceptsTab = true;
            Clock.TextAlign = ContentAlignment.MiddleRight;
            timer.Interval = updateInterval;
            autoSaveTimer.Interval = autosaveInterval;
            timer.Start();
            autoSaveTimer.Start();
            UpdateClock();
            UpdateCounters();
            UpdateColors();
        }
        private void UpdateCounters() {
            charactersCount = MainText.Text.Replace("#","").Replace("-","").Length;
            charactersCountWithOutSpaces = MainText.Text.Replace("#", "").Replace("-", "").Replace(" ", "").Length;
            wordCount = GenericHelper.CountWords(MainText.Text);
            Counter.Text = 
                $"WORDS: {wordCount} | CHARACTERS: {charactersCount} | WITHOUT SPACES: {charactersCountWithOutSpaces} | " +
                $"FONT: {currentFontName} at {currentFontSize}pt";
        }
        private void UpdateClock() { 
            Clock.Text = DateTime.Now.DayOfWeek + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }
    }
}
