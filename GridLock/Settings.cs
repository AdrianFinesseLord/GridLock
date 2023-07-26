using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridLock
{
    public partial class Settings : Form
    {
        loadScreen loadScreen;
        public Settings(loadScreen loadScre)
        {
            InitializeComponent();
            loadScreen = loadScre;
            loadScreen.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            applySettings();
        }

        private void applySettings()
        {
            // background colour
            if (comboBox1.Text == "Ivory")
            {
                SettingsVar.backgroundColour = Color.Ivory;
                this.BackColor = Color.Ivory;
            }
            else if (comboBox1.Text == "LightSkyBlue")
            {
                SettingsVar.backgroundColour = Color.LightSkyBlue;
                this.BackColor = Color.LightSkyBlue;
            }
            else if (comboBox1.Text == "Pink")
            {
                SettingsVar.backgroundColour = Color.Pink;
                this.BackColor = Color.Pink;
            }
            else if (comboBox1.Text == "LightGreen")
            {
                SettingsVar.backgroundColour = Color.LightGreen;
                this.BackColor = Color.LightGreen;
            }

            // change music
            if(comboBox2.Text != SettingsVar.music)
            {
                if (comboBox2.Text == "music1.wav")
                {
                    SettingsVar.music = comboBox2.Text;
                }
                else if (comboBox2.Text == "music2.wav")
                {
                    SettingsVar.music = comboBox2.Text;
                }
                else if (comboBox2.Text == "music3.wav")
                {
                    SettingsVar.music = comboBox2.Text;
                }
                loadScreen.loadMusic(SettingsVar.music);
            }


            // close settings
            loadScreen.Show();
            loadScreen.BackColor = SettingsVar.backgroundColour;
            this.Close();

            // applies variables and sets them to global settings class
        }


        private void Settings_Load(object sender, EventArgs e)
        {
            this.BackColor = SettingsVar.backgroundColour;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    internal class SettingsVar
    {
        public static Color backgroundColour = Color.Ivory;
        public static string music = "music1.wav";

        // global variable for settings
    }
}
