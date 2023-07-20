using System.Collections.Generic;
using System.Drawing;
using System;
using System.Windows.Forms;
using static GridLock.Form1;

namespace GridLock
{
    public partial class levelDesigner : Form
    {
        public levelDesigner()
        {
            InitializeComponent();
            generateGridPictureBoxes();
        }

        void generateGridPictureBoxes()
        {
            Global.pictureBoxes.Clear();
            for (int y = 0; y < Constants.blocksDown; y++)
            {
                Global.pictureBoxes.Add(new List<PictureBox>());
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Global.pictureBoxes[y].Add(new PictureBox());
                    var item = Global.pictureBoxes[y][x];
                    item.Width = Constants.blockPixelLength - Constants.pictureBoxGap;
                    item.Height = Constants.blockPixelLength - Constants.pictureBoxGap;
                    item.BackColor = Constants.gridBackColor;
                    item.Location = new Point(x * Constants.blockPixelLength, y * Constants.blockPixelLength);
                    //item.Click += new EventHandler(NewPictureBox_Click);
                    this.Controls.Add(item);
                }
            }

        }



    }

    class Global
    {
        public static List<List<string>> gameBoard = new List<List<string>>();
        public static List<Block> blocks = new List<Block>();
        public static FinishBlock finishblock = null;
        public static List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();

        public static Block selectedBlock = null;

        public static Form1 form1Ref = null;
        public static string currentLevelFilePath = @"csvLevels/Easy.csv";
    }
}
