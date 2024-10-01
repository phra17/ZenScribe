using Microsoft.VisualBasic.Devices;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TextEdit
{
    public partial class Main : Form
    {
        public string currentFile = "";
        public int wordCount = 0;
        public int charactersCount = 0;
        public int charactersCountWithOutSpaces = 0;
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
        private void CustomInitialize() {
            MainText.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Counter.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            MainText.KeyDown += new KeyEventHandler(MainText_KeyDown);
            MainText.AcceptsTab = true;
            UpdateCounters();
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
            Counter.Text = $"WORDS: {wordCount} | CHARACTERS: {charactersCount} | WITHOUT SPACES: {charactersCountWithOutSpaces}";
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
