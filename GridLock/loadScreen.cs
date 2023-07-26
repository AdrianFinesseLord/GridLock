using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace GridLock
{
    public partial class loadScreen : Form
    {
        public loadScreen()
        {
            InitializeComponent();
        }

        int xLoadBarWidth = 0;
        int buttonPadding = 0;

        private void loadScreen_Load(object sender, EventArgs e)
        {
            button1.Hide();
            comboBox1.Hide();
            initiateLevelsComboBox();
            setTimer();
            loadMusic(SettingsVar.music);

        }

        private void setTimer()
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 10; // 0.01 second
            timer1.Start();
            // sets game timer interval with function timer1_tick 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (xLoadBarWidth < Constants.xloadBarEndWidth) 
            {
                xLoadBarWidth += 10;
                pictureBox2.Size = new System.Drawing.Size(xLoadBarWidth, pictureBox2.Size.Height);
            } else
            {
                pictureBox2.Hide();
                pictureBox3.Hide();
                button1.Show();
                comboBox1.Show();
                
            }
            
        }

        public void loadMusic(string music)
        {
            SoundPlayer simpleSound = new SoundPlayer(@"sounds/" + music);
            simpleSound.Stop();
            simpleSound.Play();
        }

        private void initiateLevelsComboBox()
        {
            DirectoryInfo d = new DirectoryInfo(@"csvLevels/");

            FileInfo[] Files = d.GetFiles("*.csv"); //Getting csv filesnames

            foreach (FileInfo file in Files)
            {
                comboBox1.Items.AddRange(new object[] { $"{file.Name}" });

            }

            comboBox1.SelectedIndex = 0;
            // gets csv filenames and inserts them into combobox
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            Object selectedItem = comboBox1.SelectedItem;

            // prevents invalid level from being input
            try
            {
                string currentLevelFilePath = @"csvLevels/" + selectedItem.ToString();
                var Form1 = new Form1(currentLevelFilePath, this);
                Form1.Show();

                this.Hide();
            }
            catch (Exception error)
            {
                MessageBox.Show("Error: Level not valid");
            } finally
            {
                
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings(this);
            settings.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var help = new Help();
            help.Show();
        }
    }
}
