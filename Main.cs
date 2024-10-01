using Microsoft.VisualBasic.Devices;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TextEdit
{
    public partial class Main : Form
    {
        public string[] fonts = new string[3] { "Courier New", "Verdana", "Trebouchet MS" };
        int currentFontIndex = 0;
        int currentFontSize = 18;
        public string currentFontName;
        public string currentFile = "";
        public int wordCount = 0;
        public int charactersCount = 0;
        public int charactersCountWithOutSpaces = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
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
                    AddSeparator();
                }
            }

            UpdateCounters();
        }
        private void MainText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                AddTitle();
            }

            if (e.KeyCode == Keys.Tab)
            {
                AddTab(e);
            }

            if (e.Alt && e.KeyCode == Keys.F) {
                currentFontIndex++;
                if (currentFontIndex > fonts.Length - 1) currentFontIndex = 0;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Alt && e.KeyCode == Keys.Oemplus)
            {
                currentFontSize++;
                if (currentFontSize > 64) currentFontSize = 64;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Alt && e.KeyCode == Keys.OemMinus)
            {
                currentFontSize--;
                if(currentFontSize < 8) currentFontSize = 8;
                UpdateFont();
                UpdateCounters();
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                if (!string.IsNullOrEmpty(currentFile))
                {
                    SaveFile();
                }
                else {
                    SaveNewFile();
                }
            }

            if (e.Control && e.KeyCode == Keys.F)
            {
                Search s = new Search();
                s.ShowDialog();
                MainText.Find(s.ReturnValue);
            }

            //if (e.Control && e.KeyCode == Keys.D1)
            //{
            //    string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            //    System.Drawing.Image bg = System.Drawing.Image.FromFile(path += "\\Backgrounds\\1.jpg");
            //    richTextBox1.BackColor = new Color()
            //}   

            if (e.Control && e.KeyCode == Keys.O) {
                OpenFile();
            }
        }

        private void UpdateFont() {
            System.Drawing.Font currentFont = new System.Drawing.Font(fonts[currentFontIndex], currentFontSize);
            MainText.Font = currentFont;
            currentFontName = currentFont.Name;
            UpdateCounters();
        }
        private void CustomInitialize() {
            System.Drawing.Font currentFont = new System.Drawing.Font("Courier New", 18);
            MainText.Font = currentFont;
            currentFontName = currentFont.Name;
            MainText.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Counter.Anchor  = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Clock.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            MainText.KeyDown += new KeyEventHandler(MainText_KeyDown);
            timer.Tick += new EventHandler(timer_Tick);
            MainText.AcceptsTab = true;
            Clock.TextAlign = ContentAlignment.MiddleRight;
            timer.Interval = 1000;
            timer.Start();
            UpdateClock();
            UpdateCounters();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }
        private void AddTab(KeyEventArgs e) {
            e.SuppressKeyPress = true;
            int selectionStart = MainText.SelectionStart;
            MainText.Text = MainText.Text.Insert(selectionStart, "\t");
            MainText.SelectionStart = selectionStart + 1;
        }
        private void AddTitle() {
            int currentLineIndex = MainText.GetLineFromCharIndex(MainText.SelectionStart);
            string lastLine = MainText.Lines[currentLineIndex];

            if (lastLine.StartsWith("#"))
            {
                MainText.Text = MainText.Text.Replace(lastLine, lastLine.Replace("#", "###") + "###");
                MainText.SelectionStart = MainText.Text.Length;
                MainText.SelectionLength = 0;
            }
        }
        private void UpdateCounters() {
            charactersCount = MainText.Text.Replace("#","").Replace("-","").Length;
            charactersCountWithOutSpaces = MainText.Text.Replace("#", "").Replace("-", "").Replace(" ", "").Length;
            wordCount = CountWords(MainText.Text);
            Counter.Text = 
                $"WORDS: {wordCount} | CHARACTERS: {charactersCount} | WITHOUT SPACES: {charactersCountWithOutSpaces} | " +
                $"FONT: {currentFontName} at {currentFontSize}pt";
        }
        private void UpdateClock() { 
            Clock.Text = DateTime.Now.DayOfWeek + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }
        private void AddSeparator() {
            try
            {
                int textWidth = CalculateMaxDescriptionLineLength();
                string separator = "-";
                for (int i = 0; i <= textWidth; i++)
                {
                    separator += "-";
                }
                MainText.Text = MainText.Text.Replace("___", separator + Environment.NewLine);
                MainText.SelectionStart = MainText.Text.Length;
                MainText.SelectionLength = 0;
            }
            catch (Exception)
            {
                //
            }
        }
        private void SaveFile()
        {
            MainText.SaveFile(currentFile, RichTextBoxStreamType.PlainText);
            MessageBox.Show("File Saved.");
        }
        private void SaveNewFile() {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.FileName = currentFile;
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                MainText.SaveFile(fs, RichTextBoxStreamType.PlainText);

                fs.Close();
            }
        }
        private void OpenFile() {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text file|*.txt";
            openFileDialog1.Title = "Open";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)openFileDialog1.OpenFile();
                string fileContents;
                using (StreamReader reader = new StreamReader(fs))
                {
                    fileContents = reader.ReadToEnd();
                }
                MainText.Text = fileContents;
                MainText.SelectionStart = MainText.Text.Length;
                MainText.SelectionLength = 0;
                currentFile = openFileDialog1.FileName;
                UpdateCounters();
            }
        }
        private int CountWords(string text) {
            int _wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                _wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }
            return _wordCount;
        }
        private int CalculateMaxDescriptionLineLength()
        {
            Graphics g = MainText.CreateGraphics();
            float twoCharW = g.MeasureString("aa", MainText.Font).Width;
            float oneCharW = g.MeasureString("a", MainText.Font).Width;
            return (int)((float)MainText.Width / (twoCharW - oneCharW));
        }
    }
}
