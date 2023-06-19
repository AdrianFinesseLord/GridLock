using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GridLock.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace GridLock
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            Global.form1Ref = this;
            InitializeComponent();
            ReadFile();
            generateGridPictureBoxes();
            InitiateMap();

            Load += Form1_Load1;
            KeyDown += new KeyEventHandler(Form1_KeyDown);

            //constructor to execute main functions
            
        }

        private void Form1_Load1(object sender, EventArgs e)
        {


            // activates code once all controls have loaded
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(Global.selectedBlock != null)
            {
                if (e.KeyCode == Keys.Right && Global.selectedBlock.movement != "V")
                {
                    Global.selectedBlock.Move(0,1);
                }
                else if (e.KeyCode == Keys.Left && Global.selectedBlock.movement != "V")
                {
                    Global.selectedBlock.Move(0, -1);
                }
                else if (e.KeyCode == Keys.Up && Global.selectedBlock.movement != "H")
                {
                    Global.selectedBlock.Move(-1, 0);
                }
                else if (e.KeyCode == Keys.Down && Global.selectedBlock.movement != "H")
                {
                    Global.selectedBlock.Move(1, 0);
                }
            }

        }
        void ReadFile()
        {
            /* code for file reading:
             first two digits are the colour code
                if the colour code is FF, it is the finish line
             3rd digit is height
             4th digit is length
             5th digit is movement of block type
                V = vertical
                H = horizontal
                A = all

             */
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
                while (value != "\r\n" && endRead == false)
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
                    Console.WriteLine(Global.gameBoard[i][ii]);
                }
            }
        }

        void InitiateMap()
        {
            
            for (int y = 0; y < Global.gameBoard.Count - 1; y++)
            {
                for (int x = 0; x <= Global.gameBoard[y].Count-1; x++)
                {
                    var item = Global.gameBoard[y][x];

                    if (x == 5 && y == 2)
                    {
                        Console.WriteLine(Global.gameBoard[y]);
                    }

                    if (item != " " && item != null)
                    {
                        
                        string colourCode = $"{item[0]}{item[1]}";
                        Color colour = Color.LightGray;
                        string dimensions = $"{item[2]}{item[3]}";
                        string typeOfBlock = $"{item[4]}";
                        Console.WriteLine(item);
                        if (colourCode != "FF")
                        {
                            if (colourCode == "r_") colour = Color.Red;
                            else if (colourCode == "y_") colour = Color.Yellow;
                            else if (colourCode == "g_") colour = Color.Green;
                            else if (colourCode == "gy") colour = Color.Gray;
                            else if (colourCode == "b_") colour = Color.Black;
                            else if (colourCode == "p_") colour = Color.Pink;
                            else if (colourCode == "o_") colour = Color.Orange;
                            else if (colourCode == "pu") colour = Color.Purple;
                            else if (colourCode == "br") colour = Color.Brown;
                            Global.blocks.Add(new Block(colour, y, x, (int)Char.GetNumericValue(dimensions[0]), (int)Char.GetNumericValue(dimensions[1]), typeOfBlock));
                            Global.blocks[Global.blocks.Count - 1].drawBlock();
                        } else
                        {
                            
                            Global.finishblock = new FinishBlock(y, x, (int)Char.GetNumericValue(dimensions[0]), (int)Char.GetNumericValue(dimensions[1]));
                        }

                        

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
                    item.Width = Global.blockPixelLength - Global.pictureBoxGap;
                    item.Height = Global.blockPixelLength - Global.pictureBoxGap;
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
            public static FinishBlock finishblock = null;
            public static List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();

            public static Block selectedBlock = null;
            public static Color gridBackColor = Color.LightGray;
            public static Color finishColor = Color.LimeGreen;
            public static int pictureBoxGap = 2;

            public static Form1 form1Ref = null;
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

            public string movement { get; set; }

            public Block(Color colour, int yCord, int xCord, int yLength, int xLength, string movement)
            {

                this.colour = colour;
                this.cords = new List<List<int>>();
                this.InitialiseCords(yCord, xCord, yLength, xLength);
                this.movement = movement;
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

        public class FinishBlock
        {
            public int y { get; set; }
            public int x { get; set; }

            public int height { get; set; }
            public int length { get; set; }


            public FinishBlock(int y, int x, int height, int length)
            {
                this.height = height;
                this.length = length;
                this.y = y;
                this.x = x-length+1;
                drawFinish();

            }

            public void drawFinish()
            {
                
                    PictureBox item = new PictureBox();
                    item.Width = Global.blockPixelLength*this.length + Global.pictureBoxGap;
                    item.Height = Global.blockPixelLength*this.height + Global.pictureBoxGap;
                    item.BackColor = Global.finishColor;
                    item.Location = new Point(this.x * Global.blockPixelLength - Global.pictureBoxGap, this.y * Global.blockPixelLength - Global.pictureBoxGap);
                    Global.form1Ref.Controls.Add(item);



            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Graphics gr = Global.pictureBoxes[0][0].CreateGraphics();
            Pen mypen = new Pen(Brushes.Black, 3);
            gr.DrawRectangle(mypen, 0, 0, 50, 50);
        }

    }


}


