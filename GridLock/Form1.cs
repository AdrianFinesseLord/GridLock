﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GridLock.Form1;

namespace GridLock
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            ReadFile();
            generateGridPictureBoxes();
            InitiateMap();
            
            KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(Global.selectedBlock != null)
            {
                if (e.KeyCode == Keys.Right)
                {
                    Global.selectedBlock.Move(0,1);
                }
                else if (e.KeyCode == Keys.Left)
                {
                    Global.selectedBlock.Move(0, -1);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    Global.selectedBlock.Move(-1, 0);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    Global.selectedBlock.Move(1, 0);
                }
            }

        }
        void ReadFile()
        {

            int x = 0;
            int y = 0;
            string value = "";
            bool endRead = false;
            string path = @"readthis.csv";
            StreamReader reader = new StreamReader(path);
            int readCharacter = 0;

            while (endRead == false)
            {
                Global.gameBoard.Add(new List<string>());
                while (value != "\n" && endRead == false)
                {
                    readCharacter = reader.Read();

                    if ((char)readCharacter == 'Z')
                    {
                        endRead = true;
                    }
                    else if ((char)readCharacter == ',')
                    {

                        Global.gameBoard[y].Add(value);
                        x++;
                        value = null;
                    }
                    else
                    {
                        value += (char)readCharacter;
                    }
                }

                y++;
                x = 0;
                value = null;

            }
            reader.Close();
            for (int i = 0; i < Global.gameBoard.Count; i++)
            {
                for (int ii = 0; ii < Global.gameBoard[i].Count; ii++)
                {
                }
            }
        }

        void InitiateMap()
        {
            for (int y = 0; y < Global.gameBoard.Count - 1; y++)
            {
                for (int x = 0; x < Global.gameBoard[x].Count - 1; x++)
                {
                    var item = Global.gameBoard[y][x];

                    if (item != " " && item != null)
                    {
                        string colourCode = $"{item[0]}{item[1]}";
                        Color colour = Color.LightGray;
                        string dimensions = $"{item[2]}{item[3]}";

                        if (colourCode == "r_") colour = Color.Red;
                        else if (colourCode == "y_") colour = Color.Yellow;
                        else if (colourCode == "g_") colour = Color.Green;
                        else if (colourCode == "gy") colour = Color.Gray;
                        else if (colourCode == "b_") colour = Color.Black;
                        else if (colourCode == "p_") colour = Color.Pink;
                        else if (colourCode == "o_") colour = Color.Orange;
                        else if (colourCode == "pu") colour = Color.Purple;
                        else if (colourCode == "br") colour = Color.Brown;
                        Global.blocks.Add(new Block(colour, y, x, (int)Char.GetNumericValue(dimensions[0]), (int)Char.GetNumericValue(dimensions[1])));
                        Global.blocks[Global.blocks.Count-1].drawBlock();

                    }
                }
            }
        }

        void generateGridPictureBoxes()
        {
            for (int y = 0; y < Global.blocksDown; y++)
            {
                Global.pictureBoxes.Add(new List<PictureBox>());
                for (int x = 0; x < Global.blocksAcross; x++)
                {
                    Global.pictureBoxes[y].Add(new PictureBox());
                    var item = Global.pictureBoxes[y][x];
                    item.Width = Global.blockPixelLength -1;
                    item.Height = Global.blockPixelLength -1;
                    item.BackColor = Global.gridBackColor;
                    item.Location = new Point(x * Global.blockPixelLength,y * Global.blockPixelLength);
                    item.Click += new EventHandler(NewPictureBox_Click);
                    this.Controls.Add(item);
                }
            }
            

                
        }

        private void NewPictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox) sender;
            Global.selectedBlock = findBlockWithCords(pictureBox.Location.Y / Global.blockPixelLength, pictureBox.Location.X / Global.blockPixelLength);
            Console.WriteLine(Global.selectedBlock);
            Console.WriteLine("hi");
        }

        private Block findBlockWithCords(int y, int x)
        {
            foreach (var thisBlock in Global.blocks)
            {
                foreach (var thisCord in thisBlock.cords)
                {
                    if (thisCord[0] == y && thisCord[1] == x)
                    {
                        return thisBlock;
                    }
                }
            }
            return null;
        }

        static void clearBackgroundBlocks()
        {
            for (int y = 0; y < Global.blocksDown; y++)
            {
                for (int x = 0; x < Global.blocksAcross; x++)
                {
                    var item = Global.pictureBoxes[y][x];
                    item.BackColor = Global.gridBackColor;
                }
            }
        }
        
        private static void renderTick()
        {
            clearBackgroundBlocks();
            foreach (var item in Global.blocks)
            {
                item.drawBlock();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        class Global
        {
            public static int blocksAcross = 7;
            public static int blocksDown = 7;
            public static int blockPixelLength = 50;

            public static List<List<string>> gameBoard = new List<List<string>>();
            public static List<Block> blocks = new List<Block>();
            public static List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();

            public static Block selectedBlock = null;
            public static Color gridBackColor = Color.LightGray;
        }
        public class Block
        {
            //private Form1 Form1 = new Form1();

            private int y;
            private int x;

            public Color colour { get; set; }
            public List<List<int>> cords { get; set; }
            public int yLength { get; set; }
            public int xLength { get; set; }


            public Block(Color colour, int yCord, int xCord, int yLength, int xLength)
            {

                this.colour = colour;
                this.cords = new List<List<int>>();
                this.InitialiseCords(yCord, xCord, yLength, xLength);

            }


            public void InitialiseCords(int yInitialCord, int xInitialCord, int yL, int xL)
            {
                this.yLength = yL;
                this.xLength = xL;

                for (int y = 0; y < yL; y++)
                {
                    for (int x = 0; x < xL; x++)
                    {
                        this.cords.Add(new List<int>());
                        this.cords[this.cords.Count-1].Add(yInitialCord + y);
                        this.cords[this.cords.Count-1].Add(xInitialCord + x);
                    }
                }

            }

            public void Move(int yDirection, int xDirection)
            {
                bool validMove = true;

                // y ints collision detection
                if (yDirection != 0)
                {
                    for (int i = 0; i < this.cords.Count; i++)
                    {
                        foreach (var item in Global.blocks)
                        {
                            if (item != this)
                            {
                                for (int y = 0; y < item.cords.Count; y++)
                                {
                                    if ((item.cords[y][0] == this.cords[i][0] + yDirection && item.cords[y][1] == this.cords[i][1]) || this.cords[i][0] + yDirection < 0 || this.cords[i][0] + yDirection > 6)
                                    {
                                        validMove = false;
                                    }
                                }
                            }
                        }
                    }

                    if (validMove)
                    {
                        for (int i = 0; i < this.cords.Count; i++)
                        {
                            this.cords[i][0] += yDirection;
                        }
                    }
                }

                // x ints collision detection
                if (xDirection != 0 || !validMove)
                {
                    for (int i = 0; i < this.cords.Count; i++)
                    {
                        foreach (var item in Global.blocks)
                        {
                            if (item != this)
                            {
                                for (int x = 0; x < item.cords.Count; x++)
                                {
                                    if ((item.cords[x][1] == this.cords[i][1] + xDirection && item.cords[x][0] == this.cords[i][0]) || this.cords[i][1] + xDirection < 0 || this.cords[i][1] + xDirection > 6)
                                    {
                                        validMove = false;

                                    }
                                }
                            }
                        }
                    }

                    if (validMove)
                    {
                        for (int i = 0; i < this.cords.Count; i++)
                        {
                            this.cords[i][1] += xDirection;
                        }
                    }
                }
                renderTick();

            }

            public void drawBlock()
            {
                int i = 0;
                foreach (var item in this.cords)
                {
                    Global.pictureBoxes[this.cords[i][0]][this.cords[i][1]].BackColor = this.colour;
                    i++;
                }
            }
        }
    }


}
