using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Bus_Sign
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int ledCount = 0;
        private Dictionary<Control, string> LEDs = new Dictionary<Control, string>();
        private Dictionary<string, int> LEDstatus = new Dictionary<string, int>();
        private void Form1_Load(object sender, EventArgs e)
        {
            int x = 1,
                y = 25,
                row = 1;

            ledCount = 1600;
            int ledsPerRow = 100,

                ledHeight = 20,
                ledWidth = 20,
                seperator = 21;


            for (int i = 1; i <= ledCount; i++)
            {
                Button LED = new Button
                {
                    Parent = this,
                    Location = new Point(x, y),
                    Width = ledWidth,
                    Height = ledHeight,
                    BackColor = SystemColors.Control,
                    Name = $"{i}:{row}"
                };

                LED.Click += LED_ColorChange;
                LED.Show();

                LEDs.Add(LED, LED.Name);

                Width = (ledsPerRow * seperator) + (ledWidth - 4);
                Text = $"Active LEDs: {LEDs.Count}";

                x += seperator;

                if ((i % ledsPerRow) == 0)
                {
                    x = 1;
                    y += seperator;
                    row++;
                }
                
                LEDstatus.Add(LED.Name, 0);
            }

            resetButton.Location = new Point((this.Width - (resetButton.Width + 25)), (this.Height - (resetButton.Height + 50)));
        }

        private static bool activeLEDs = false;
        private void button1_Click(object sender, EventArgs e)
        {

            if (!activeLEDs)
            {
                MessageBox.Show("There is nothing to reset on the active canvas", "No active LEDs", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (KeyValuePair<Control, string> LED in LEDs)
            {
                LED.Key.BackColor = SystemColors.Control;
            }

            foreach (var key in LEDstatus.Keys.ToList())
            {
                LEDstatus[key] = 0;
            }

            activeLEDs = false;
        }
        private void LED_ColorChange(object sender, EventArgs e)
        {
            (sender as Button).BackColor = ((sender as Button).BackColor == SystemColors.Control) ? Color.Red : SystemColors.Control;
            activeLEDs = true;

            LEDstatus[(sender as Button).Name] = (LEDstatus[(sender as Button).Name] == 0) ? 1 : 0;
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lines = 0;
            OpenFileDialog importDialog = new OpenFileDialog
            {
                Filter = "BSLED files (*.bsled)|*.bsled"
            };

            DialogResult importResult = importDialog.ShowDialog();

            if (importResult == DialogResult.OK)
            {
                string LED;
                string[] LEDData;

                StreamReader importStream = new StreamReader(importDialog.FileName);

                while ((LED = importStream.ReadLine()) != null)
                {
                    lines++;

                    LEDData = LED.Split('-');
                    string LEDName = LEDData[0].ToString().Replace("[", "");
                    int LEDStatus = int.Parse(LEDData[1].Replace("]", ""));

                    if (LEDStatus == 1)
                        Controls.Find(LEDName, true)[0].BackColor = Color.Red;
                    else
                        Controls.Find(LEDName, true)[0].BackColor = SystemColors.Control;

                    LEDstatus[LEDName] = LEDStatus;
                }
                
                importStream.Dispose();
                importStream.Close();

                activeLEDs = true;
            }

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string bsLEDData = null;

            foreach (KeyValuePair<string, int> LED in LEDstatus)
            {
                bsLEDData += $"[{LED.Key}-{LED.Value}]\n";
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "BSLED files (*.bsled)|*.bsled"
            };

            DialogResult saveResult = saveFileDialog.ShowDialog();

            if (saveResult == DialogResult.OK)
            {
                StreamWriter exportStream = new StreamWriter(saveFileDialog.FileName);
                exportStream.Write(bsLEDData);
                exportStream.Dispose();
                exportStream.Close();
            }
        }
    }
}
