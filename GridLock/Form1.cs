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
using System.Security.AccessControl;
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

            convertMapTo2DArray();
            //constructor to execute main functions

        }

        private void Form1_Load1(object sender, EventArgs e)
        {

            // activates code once all controls have loaded

            this.KeyPreview = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            initiateLevelsComboBox();



        }

        private void initiateLevelsComboBox()
        {
            DirectoryInfo d = new DirectoryInfo(@"csvLevels/");

            FileInfo[] Files = d.GetFiles("*.csv"); //Getting csv filesnames
            string str = "";

            foreach (FileInfo file in Files)
            {
                comboBox1.Items.AddRange(new object[] { $"{file.Name}" });

            }

            comboBox1.SelectedIndex = 0;
            // gets csv filenames and inserts them into combobox
        }

        private void loadLevel()
        {
            int selectedIndex = comboBox1.SelectedIndex;
            Object selectedItem = comboBox1.SelectedItem;

            clearBackgroundBlocks();
            Global.currentLevelFilePath = @"csvLevels/"+ selectedItem.ToString();
            ReadFile();
            InitiateMap();
            Global.selectedBlock = null;
            // Get selected level from combobox and store it in the file path, then reset all variables and play all main file reading and level setup functions
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Global.selectedBlock != null)
            {
                if (e.KeyCode == Keys.Right && Global.selectedBlock.movement != "V")
                {
                    Global.selectedBlock.Move(0, 1);
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
            Global.gameBoard.Clear();
            int x = 0;
            int y = 0;
            string value = "";
            bool endRead = false;
            string path = Global.currentLevelFilePath;
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
                }
            }
        }

        private void convertMapTo2DArray()
        {
            for (int y = 0; y < Constants.blocksDown; y++) // Create dummy map for list insertions
            {
                Global.live2DGameBoard.Add(new List<string>());

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Global.live2DGameBoard[y].Add(" ");

                }
            }

            for (int y = 0; y < Constants.blocksDown; y++) // Insert blocks now
            {

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if (Global.gameBoard[y][x] != " " && $"{Global.gameBoard[y][x][0]}{Global.gameBoard[y][x][1]}" != "FF")
                    {
                        int yL = Convert.ToInt32(Global.gameBoard[y][x][2].ToString());
                        int xL = Convert.ToInt32(Global.gameBoard[y][x][3].ToString());
                        for (int yCount = 0; yCount < yL; yCount++)
                        {
                            for (int xCount = 0; xCount < xL; xCount++)
                            {
                                Global.live2DGameBoard[y+yCount][x+xCount] = $"{Global.gameBoard[y][x][0]}{Global.gameBoard[y][x][1]}";
                            }
                        }
                    }
                }
            }


        }

        private void InitiateMap()
        {
            Global.blocks.Clear();
            for (int y = 0; y < Global.gameBoard.Count; y++)
            {
                for (int x = 0; x < Global.gameBoard[y].Count; x++)
                {
                    var item = Global.gameBoard[y][x];

                    Console.WriteLine(item);

                    if (item != " " && item != null)
                    {

                        string colourCode = $"{item[0]}{item[1]}";
                        Color colour = Color.LightGray;
                        string dimensions = $"{item[2]}{item[3]}";
                        string typeOfBlock = $"{item[4]}";


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
                            Global.blocks.Add(new Block(colour, colourCode, y, x, (int)Char.GetNumericValue(dimensions[0]), (int)Char.GetNumericValue(dimensions[1]), typeOfBlock));
                            Global.blocks[Global.blocks.Count - 1].drawBlock();
                        }
                        else
                        {

                            Global.finishblock = new FinishBlock(y, x, (int)Char.GetNumericValue(dimensions[0]), (int)Char.GetNumericValue(dimensions[1]));
                        }



                    }
                }
            }
        }

        private void generateGridPictureBoxes()
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
                    item.Click += new EventHandler(NewPictureBox_Click);
                    this.Controls.Add(item);
                }
            }

        }

        private void NewPictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Global.selectedBlock = findBlockWithCords(pictureBox.Location.Y / Constants.blockPixelLength, pictureBox.Location.X / Constants.blockPixelLength);
            if(Global.selectedBlock != null)
            {
                Global.form1Ref.pictureBox1.BackColor = Global.selectedBlock.colour;

            }

            //Gets picturebox cordinates and finds the corresponding block with the mouse cordinates, then change the selected block icon colour
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
            for (int y = 0; y < Constants.blocksDown; y++)
            {
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    var item = Global.pictureBoxes[y][x];
                    item.BackColor = Constants.gridBackColor;
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

            for (int y = 0; y < Constants.blocksDown; y++)
            {
                Console.WriteLine("");
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Console.Write(Global.live2DGameBoard[y][x]);
                    if(Global.live2DGameBoard[y][x] == " ")
                    {
                        Console.Write(" ");

                    }
                }
            }

            
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void levelSolver()
        {
            /* AI method
             1. Create all board combinations from moving the green block to every possible square.
             2. Create all possible board combinations from moving other blocks
             3. Check if one of those boards has a solved combination
             4. If not, create all possible board combinations from the pool of existing boards
             5. Repeat until one of the boards has a solved combination
            */

            

            // 1st step - create all board combinations from moving the green block to every possible square
            Global.AIboards.Add(new List<List<string>>());
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {
                Global.AIboards[0].Add(new List<string>());
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Global.AIboards[0][y].Add(Global.live2DGameBoard[y][x]);
                }
            }

            if(checkAIMoveValid(Global.AIboards[0], "g_", 0, -1))
            {
                moveAIgameBoardPieces(0, "g_", -1, -1);
            }

            // explore all possible positions for the green block to move

        }

        private bool checkAIMoveValid(List<List<string>> board, string colourCode, int directionVertical, int directionHorizontal)
        {
            // locate block on board
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if (board[y][x] == colourCode)
                    {
                        // check if position about to move to is in bounds
                        if ((y+directionVertical < 0 || y+directionVertical > Constants.blocksDown-1) || (x + directionHorizontal < 0 || x + directionHorizontal > Constants.blocksAcross - 1))
                        {
                            return false;
                            
                        } else if (board[y + directionVertical][x + directionHorizontal] != " " && board[y + directionVertical][x + directionHorizontal] != colourCode)
                        {
                            return false;
                        }

                    }
                }
            }

            return true;
        }

        private void moveAIgameBoardPieces(int boardGeneration, string colourCode, int directionVertical, int directionHorizontal)
        {
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if (Global.AIboards[boardGeneration][y][x] == colourCode)
                    {
                        Global.AIboards[boardGeneration][y][x] = " ";
                        Global.AIboards[boardGeneration][y + directionVertical][x + directionHorizontal] = colourCode;
                    }
                }
            }
        }
       

        class Global
        {
            

            public static List<List<string>> gameBoard = new List<List<string>>();
            public static List<List<string>> live2DGameBoard = new List<List<string>>();
            public static List<Block> blocks = new List<Block>();
            public static FinishBlock finishblock = null;
            public static List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();
            public static List<List<List<string>>> AIboards = new List<List<List<string>>>(); // the first section of this variable is the generation the board exists in


            public static Block selectedBlock = null;

            public static Form1 form1Ref = null;
            public static string currentLevelFilePath = @"csvLevels/aiTester.csv";
        }
        public class Block
        {
            //private Form1 Form1 = new Form1();

            private int y;
            private int x;

            public Color colour { get; set; }
            public string colourCode { get; set; }
            public List<List<int>> cords { get; set; }
            public int yLength { get; set; }
            public int xLength { get; set; }

            public string movement { get; set; }

            public Block(Color colour, string colourCode, int yCord, int xCord, int yLength, int xLength, string movement)
            {

                this.colour = colour;
                this.cords = new List<List<int>>();
                this.colourCode = colourCode;
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
                        this.cords[this.cords.Count - 1].Add(yInitialCord + y);
                        this.cords[this.cords.Count - 1].Add(xInitialCord + x);
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
                            Global.live2DGameBoard[this.cords[i][0]][this.cords[i][1]] = " ";

                        }  // erase blocks on the live gameboard

                        for (int i = 0; i < this.cords.Count; i++)
                        {
                            this.cords[i][0] += yDirection;

                            Global.live2DGameBoard[this.cords[i][0]][this.cords[i][1]] = this.colourCode;

                        } // loop through x-cords and add x direction
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
                            Global.live2DGameBoard[this.cords[i][0]][this.cords[i][1]] = " ";

                        }  // erase blocks on the live gameboard

                        for (int i = 0; i < this.cords.Count; i++)
                        {
                            this.cords[i][1] += xDirection;

                            Global.live2DGameBoard[this.cords[i][0]][this.cords[i][1]] = this.colourCode;

                        } // loop through x-cords and add x direction

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
                this.x = x - length + 1;
                drawFinish();

            }

            public void drawFinish()
            {

                PictureBox item = new PictureBox();
                item.Width = Constants.blockPixelLength * this.length + Constants.pictureBoxGap;
                item.Height = Constants.blockPixelLength * this.height + Constants.pictureBoxGap;
                item.BackColor = Constants.finishColor;
                item.Location = new Point(this.x * Constants.blockPixelLength - Constants.pictureBoxGap, this.y * Constants.blockPixelLength - Constants.pictureBoxGap);
                Global.form1Ref.Controls.Add(item);



            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Graphics gr = Global.pictureBoxes[0][0].CreateGraphics();
            Pen mypen = new Pen(Brushes.Black, 3);
            gr.DrawRectangle(mypen, 0, 0, 50, 50);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
                if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && e.Modifiers != Keys.Shift)
                {
                    e.SuppressKeyPress = true;
                }
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            loadLevel();

            //loads a new level in loadLevel function
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var levelDesignerWindow = new levelDesigner();
            levelDesignerWindow.Show();

            // creates a new level designer window and shows it
        }

        private void button3_Click(object sender, EventArgs e)
        {
            levelSolver();
        }
    }
}


