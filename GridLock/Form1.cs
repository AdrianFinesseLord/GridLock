using System;
using System.Collections;
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
using System.Security.Permissions;
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

        public Form1(string lvlName, loadScreen loadScre)
        {
            Global.loadScre = loadScre;
            Global.currentLevelFilePath = lvlName;
            Global.form1Ref = this;
            InitializeComponent();
            ReadFile();
            generateGridPictureBoxes();
            InitiateMap();
            convertMapTo2DArray();

            Load += Form1_Load1;
            KeyDown += new KeyEventHandler(Form1_KeyDown);


            // reset all variables
            Global.selectedBlock = null;
            Global.gameTimer = -1; // restart at -1 so that timer starts ticking at 0s
            Global.AIboards.Clear();
            Global.AIsolveBoardIndexes.Clear();
            Global.AIsolveStep = 0;
            Global.solvedBoard.Clear();
            Global.AIactivated = false;


        //constructor to execute main functions
    }

        

        private void Form1_Load1(object sender, EventArgs e)
        {

            // activates code once all controls have loaded

            this.KeyPreview = true;

            
            
            Global.timer1 = new System.Windows.Forms.Timer();
            Global.timer1.Tick += new EventHandler(timer1_Tick);
            Global.timer1.Interval = 1000; // 1 second
            Global.timer1.Start();
            Global.timerStarted = true;

            
            // sets game timer interval with function timer1_tick 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Global.gameTimer++;
            label3.Text = $"Time: {Global.gameTimer.ToString()}s";

            // updates timer text on screen
        }



        private void loadLevel()
        {


            clearBackgroundBlocks();
            ReadFile();
            InitiateMap();
            Global.live2DGameBoard.Clear();
            convertMapTo2DArray();
            Global.selectedBlock = null;
            Global.gameTimer = -1; // restart at -1 so that timer starts ticking at 0s
            
            // Get selected level from combobox and store it in the file path, then reset all variables and play all main file reading and level setup functions
        }

        private void onWin()
        {
            MessageBox.Show($"You've Escaped. \n \n Your time was {Global.gameTimer}s");

            Global.timer1.Stop();
            Global.timer1.Dispose();
            Global.loadScre.Show();
            this.Hide();

            
            // loads main screen again
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Global.selectedBlock != null && Global.AIsolveStep == 0)
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

            horizontal colours:
            r_ = red
            br = brown
            p_ = pink
            b_ = black

            vertical colours
            pu = purple
            o_ = orange
            gy = gray
            y_ = yellow
            

            
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
            Global.live2DGameBoard.Clear();
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
                    item.Location = new Point(x * Constants.blockPixelLength + Constants.boardOffset, y * Constants.blockPixelLength+ Constants.boardOffset);
                    item.Click += new EventHandler(NewPictureBox_Click);
                    this.Controls.Add(item);
                }
            }

        }

        private void NewPictureBox_Click(object sender, EventArgs e)
        {
            if (!(Global.AIsolveStep > 0)) // makes sure can't click on blocks in the middle of AI
            {
                PictureBox pictureBox = (PictureBox)sender;
                Global.selectedBlock = findBlockWithCords((pictureBox.Location.Y - Constants.boardOffset) / Constants.blockPixelLength, (pictureBox.Location.X - Constants.boardOffset) / Constants.blockPixelLength);
                if (Global.selectedBlock != null)
                {
                    Global.form1Ref.pictureBox1.BackColor = Global.selectedBlock.colour;

                }
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
            Global.AIboards.Add(new List<List<List<string>>>());
            Global.AIboards[0].Add(new List<List<string>>());
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {
                Global.AIboards[0][0].Add(new List<string>());
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Global.AIboards[0][0][y].Add(Global.live2DGameBoard[y][x]);
                }
            }
            


            int depth = 10;
            int i = 1;
            bool foundExit = false;
            int exitBoardIndex = 0;
            while (i < depth + 1 && !foundExit)
            {
                label2.Text = "AI in progress..."; // give user feedback about ai

                Global.AIboards.Add(new List<List<List<string>>>());

                int gameBoardIndex = 0;
                while (gameBoardIndex < Global.AIboards[i-1].Count && !foundExit) 
                {
                    int indexOfPreviousGenerationboard = gameBoardIndex;
                    // duplicate board as reference ...
                    Global.AIboards[i].Insert(0,new List<List<string>>());
                    for (int y = 0; y < Constants.blocksDown; y++) 
                    {
                        Global.AIboards[i][0].Add(new List<string>());
                        for (int x = 0; x < Constants.blocksAcross; x++)
                        {
                            Global.AIboards[i][0][y].Add(Global.AIboards[i-1][gameBoardIndex][y][x]);
                        }
                    }
                    Global.AIboards[i][0][Constants.blocksDown - 1].Add("0");


                    // explore all possible positions for the green block to move

                    List<List<int>> prevGreenCords = new List<List<int>>(); // y first then x


                    int ygreenCord = findAIgameBoardPieces(0, "g_", i).ElementAt(0).Key;
                    int xgreenCord = findAIgameBoardPieces(0, "g_", i).ElementAt(0).Value;

                    prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });
                    prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });


                    List<int> greenBlockMoves = new List<int>(); // a list of moves the green block has taken, 0 = up, 1 = right, 2 = down, 3 = left
                    List<int> identicalGameBoardIndexes = new List<int>(); // the board index matching with the greenblock moves

                    greenBlockMoves.Add(5); // 5 is a dummy number
                    identicalGameBoardIndexes.Add(0);


                    do // steps: keep going up until can't no more, then go right as far as possible, then down etc. when it can't go further in all directions, then it moves back a step and repeats the process
                    {

                        if (checkAIMoveValid(Global.AIboards[i][identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]], "g_", -1, 0) && !(checkIfIntListContainsList(prevGreenCords, new List<int>() { ygreenCord - 1, xgreenCord }))) // up
                        {
                            moveAIgameBoardPieces(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1, "g_", -1, 0, i, indexOfPreviousGenerationboard);
                            ygreenCord += -1;
                            prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });
                            greenBlockMoves.Add(0);
                            identicalGameBoardIndexes.Add(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1);
                        }
                        else if (checkAIMoveValid(Global.AIboards[i][identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]], "g_", 0, 1) && !(checkIfIntListContainsList(prevGreenCords, new List<int>() { ygreenCord, xgreenCord + 1 }))) // right
                        {
                            moveAIgameBoardPieces(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1, "g_", 0, 1, i, indexOfPreviousGenerationboard);
                            xgreenCord += 1;
                            prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });
                            greenBlockMoves.Add(1);
                            identicalGameBoardIndexes.Add(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1);


                        }
                        else if (checkAIMoveValid(Global.AIboards[i][identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]], "g_", 1, 0) && !(checkIfIntListContainsList(prevGreenCords, new List<int>() { ygreenCord + 1, xgreenCord }))) // down
                        {
                            moveAIgameBoardPieces(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1, "g_", 1, 0, i, indexOfPreviousGenerationboard);
                            ygreenCord += 1;
                            prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });
                            greenBlockMoves.Add(2);
                            identicalGameBoardIndexes.Add(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1);

                        }
                        else if (checkAIMoveValid(Global.AIboards[i][identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]], "g_", 0, -1) && !(checkIfIntListContainsList(prevGreenCords, new List<int>() { ygreenCord, xgreenCord - 1 }))) // left
                        {
                            moveAIgameBoardPieces(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1, "g_", 0, -1, i, indexOfPreviousGenerationboard);
                            xgreenCord += -1;
                            prevGreenCords.Add(new List<int>() { ygreenCord, xgreenCord });
                            greenBlockMoves.Add(3);
                            identicalGameBoardIndexes.Add(identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1] + 1);

                        }
                        else
                        {
                            var item = greenBlockMoves[greenBlockMoves.Count - 1];
                            if (item == 0)
                            {
                                ygreenCord++;
                            }
                            else if (item == 1)
                            {
                                xgreenCord--;
                            }
                            else if (item == 2)
                            {
                                ygreenCord--;
                            }
                            else if (item == 3)
                            {
                                xgreenCord++;
                            }

                            identicalGameBoardIndexes.RemoveAt(identicalGameBoardIndexes.Count - 1);
                            greenBlockMoves.RemoveAt(greenBlockMoves.Count - 1);

                            // reverses back a step
                        }

                        if (xgreenCord == 5 && ygreenCord == 2 && !foundExit)
                        {
                            foundExit = true;
                            Global.solvedBoard = Global.AIboards[i][identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]];
                            //Console.WriteLine($"You escaped at board {identicalGameBoardIndexes[identicalGameBoardIndexes.Count - 1]}");
                            Console.WriteLine($"solved at {gameBoardIndex}");
                        }

                    } while (greenBlockMoves.Count > 0);



                    // 2nd step -- get all the possible combinations of the other blocks
                    List<string> nonPlayerBlocks = uniqueBlocksExcludingGreen(i);
                    foreach (string colourCode in nonPlayerBlocks)
                    {

                        int orientation = getOrientationColourCode(colourCode); // 0 = horizontal, 1 = vertical

                        List<List<int>> prevBlockCords = new List<List<int>>(); // y first then x


                        int yBlockCord = findAIgameBoardPieces(0, colourCode, i).ElementAt(0).Key;
                        int xBlockCord = findAIgameBoardPieces(0, colourCode, i).ElementAt(0).Value;

                        prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });
                        prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });


                        List<int> blockMoves = new List<int>(); // a list of moves the block has taken, 0 = up, 1 = right, 2 = down, 3 = left
                        List<int> identicalBlockGameBoardIndexes = new List<int>(); // the board index matching with the block moves

                        blockMoves.Add(5); // 5 is a dummy number
                        identicalBlockGameBoardIndexes.Add(0);
                        
                        do // steps: keep going up until can't no more, then go right as far as possible, then down etc. when it can't go further in all directions, then it moves back a step and repeats the process
                        {
                            if (orientation == 0)
                            {
                                if (checkAIMoveValid(Global.AIboards[i][identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1]], colourCode, 0, 1) && !(checkIfIntListContainsList(prevBlockCords, new List<int>() { yBlockCord, xBlockCord + 1 }))) // right
                                {
                                    moveAIgameBoardPieces(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1, colourCode, 0, 1, i, indexOfPreviousGenerationboard);
                                    xBlockCord += 1;
                                    prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });
                                    blockMoves.Add(1);
                                    identicalBlockGameBoardIndexes.Add(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1);
                                }
                                else if (checkAIMoveValid(Global.AIboards[i][identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1]], colourCode, 0, -1) && !(checkIfIntListContainsList(prevBlockCords, new List<int>() { yBlockCord, xBlockCord - 1 }))) // left
                                {
                                    moveAIgameBoardPieces(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1, colourCode, 0, -1, i, indexOfPreviousGenerationboard);
                                    xBlockCord += -1;
                                    prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });
                                    blockMoves.Add(3);
                                    identicalBlockGameBoardIndexes.Add(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1);

                                }
                                else
                                {
                                    var item = blockMoves[blockMoves.Count - 1];
                                    if (item == 0)
                                    {
                                        yBlockCord++;
                                    }
                                    else if (item == 1)
                                    {
                                        xBlockCord--;
                                    }
                                    else if (item == 2)
                                    {
                                        yBlockCord--;
                                    }
                                    else if (item == 3)
                                    {
                                        xBlockCord++;
                                    }

                                    identicalBlockGameBoardIndexes.RemoveAt(identicalBlockGameBoardIndexes.Count - 1);
                                    blockMoves.RemoveAt(blockMoves.Count - 1);

                                    // reverses back a step
                                }
                            }
                            else if (orientation == 1)
                            {
                                if (checkAIMoveValid(Global.AIboards[i][identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1]], colourCode, -1, 0) && !(checkIfIntListContainsList(prevBlockCords, new List<int>() { yBlockCord - 1, xBlockCord }))) // up
                                {
                                    moveAIgameBoardPieces(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1, colourCode, -1, 0, i, indexOfPreviousGenerationboard);
                                    yBlockCord += -1;
                                    prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });
                                    blockMoves.Add(0);
                                    identicalBlockGameBoardIndexes.Add(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1);
                                }

                                else if (checkAIMoveValid(Global.AIboards[i][identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1]], colourCode, 1, 0) && !(checkIfIntListContainsList(prevBlockCords, new List<int>() { yBlockCord + 1, xBlockCord }))) // down
                                {
                                    moveAIgameBoardPieces(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1, colourCode, 1, 0, i, indexOfPreviousGenerationboard);
                                    yBlockCord += 1;
                                    prevBlockCords.Add(new List<int>() { yBlockCord, xBlockCord });
                                    blockMoves.Add(2);
                                    identicalBlockGameBoardIndexes.Add(identicalBlockGameBoardIndexes[identicalBlockGameBoardIndexes.Count - 1] + 1);

                                }
                                else
                                {
                                    var item = blockMoves[blockMoves.Count - 1];
                                    if (item == 0)
                                    {
                                        yBlockCord++;
                                    }
                                    else if (item == 1)
                                    {
                                        xBlockCord--;
                                    }
                                    else if (item == 2)
                                    {
                                        yBlockCord--;
                                    }
                                    else if (item == 3)
                                    {
                                        xBlockCord++;
                                    }

                                    identicalBlockGameBoardIndexes.RemoveAt(identicalBlockGameBoardIndexes.Count - 1);
                                    blockMoves.RemoveAt(blockMoves.Count - 1);

                                    // reverses back a step
                                }
                            }


                        } while (blockMoves.Count > 0);
                    }

                    gameBoardIndex++;

                }
                if(foundExit)
                {
                    showAISteps(i-1);
                }
                i++;
            }
                


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

                // loops through the selected block and searches for their colourcode on the gameboard, then checks if the spot they will move to is valid
            }

            return true;
        }

        private void moveAIgameBoardPieces(int boardGeneration, string colourCode, int directionVertical, int directionHorizontal, int i, int previousGenerationIndex)
        {
            List<List<int>> fillInBlocks = new List<List<int>>();


            //  duplicate gameBoard
            Global.AIboards[i].Insert(boardGeneration, new List<List<string>>());
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {
                Global.AIboards[i][boardGeneration].Add(new List<string>());
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Global.AIboards[i][boardGeneration][y].Add(Global.AIboards[i][boardGeneration - 1][y][x]);
                }
            }

            // clear positions first
            for (int y = 0; y < Constants.blocksDown; y++) 
            {

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if (Global.AIboards[i][boardGeneration][y][x] == colourCode)
                    {
                        Global.AIboards[i][boardGeneration][y][x] = " ";
                        fillInBlocks.Add(new List<int>() { y, x });
                    }
                }
            }

            //fill in blocks into new positions
            foreach (var item in fillInBlocks)
            {
                Global.AIboards[i][boardGeneration][item[0] + directionVertical][item[1] + directionHorizontal] = colourCode;
            }

            Global.AIboards[i][boardGeneration][Constants.blocksDown - 1].Add($"{previousGenerationIndex}");

            // creates a new board with new positions
        }

        private Dictionary<int, int> findAIgameBoardPieces(int boardGeneration, string colourCode, int i)
        {
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {

                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if (Global.AIboards[i][boardGeneration][y][x] == colourCode)
                    {
                        Dictionary<int, int> cords = new Dictionary<int, int>();
                        cords.Add(y, x);
                        return cords;
                    }
                }
            }
            return null;

            // finds the coordinates of gameboard pieces from their colourcode
        }
        private bool checkIfIntListContainsList(List<List<int>> listOfListInts, List<int> checkList  )
        {
            foreach (var list in listOfListInts)
            {
                if (list[0] == checkList[0] && list[1] == checkList[1])
                {
                    return true;
                } 
            }
            return false;

            // a function that check if there is an identical list within a list of lists
        }

        private List<string> uniqueBlocksExcludingGreen(int i)
        {
            List<string> uniqueStrings = new List<string>();
            for (int y = 0; y < Constants.blocksDown; y++) // copy across live gameboard onto boards
            {
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    if(!uniqueStrings.Contains(Global.AIboards[i][0][y][x]) && Global.AIboards[i][0][y][x] != " " && Global.AIboards[i][0][y][x] != "g_")
                    {
                        uniqueStrings.Add(Global.AIboards[i][0][y][x]);
                    }
                }
            }
            return uniqueStrings;

            // loops through a list of strings to return the colourcodes of blocks besides the green block
        }

        private int getOrientationColourCode(string colourCode)
        {
            if (colourCode == "r_" || colourCode == "br" || colourCode == "p_" || colourCode == "b_")
            {
                return 0;
            } else
            {
                return 1;
            }
            
           // finds the orientation of the colour of block based on their colourcode, their direction are preset
        }

        private void showAISteps(int i)
        {
            // create a list of the board indexes which lead to escape
            List<int> exitBoardIndexes = new List<int>();
            int nextExitBoardIndex = Convert.ToInt32(Global.solvedBoard[Constants.blocksDown - 1][Global.solvedBoard[Constants.blocksDown - 1].Count - 1]);
            exitBoardIndexes.Add(nextExitBoardIndex);

            for (int index = i; index >0; index--)
            {
                exitBoardIndexes.Insert(0, Convert.ToInt32(Global.AIboards[index][nextExitBoardIndex][Constants.blocksDown - 1][Global.AIboards[index][nextExitBoardIndex][Constants.blocksDown - 1].Count - 1]));
                nextExitBoardIndex = exitBoardIndexes[0];
            }


            Global.AIsolveBoardIndexes = exitBoardIndexes;

            // goes back through board generations to find the moves it took to find exit.
        }

        private void displayAIboard(List<List<string>> board)
        {
            for (int y = 0; y < Constants.blocksDown; y++) // clear pictureboxes
            {
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    Color colour = Constants.gridBackColor;
                }
            }



            for (int y = 0; y < Constants.blocksDown; y++) // paint pictureboxes
            {
                for (int x = 0; x < Constants.blocksAcross; x++)
                {
                    string colourCode = board[y][x];
                    Color colour = Constants.gridBackColor;
                    if (colourCode == "r_") colour = Color.Red;
                    else if (colourCode == "y_") colour = Color.Yellow;
                    else if (colourCode == "g_") colour = Color.Green;
                    else if (colourCode == "gy") colour = Color.Gray;
                    else if (colourCode == "b_") colour = Color.Black;
                    else if (colourCode == "p_") colour = Color.Pink;
                    else if (colourCode == "o_") colour = Color.Orange;
                    else if (colourCode == "pu") colour = Color.Purple;
                    else if (colourCode == "br") colour = Color.Brown;
                    Global.pictureBoxes[y][x].BackColor = colour;
                }
            }

            //Loops through a given board to show their colours on the pictureboxes
        }




        class Global
        {
            

            public static List<List<string>> gameBoard = new List<List<string>>();
            public static List<List<string>> live2DGameBoard = new List<List<string>>();
            public static List<Block> blocks = new List<Block>();
            public static FinishBlock finishblock = null;
            public static List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();

            public static List<List<List<List<string>>>> AIboards = new List<List<List<List<string>>>>(); // the first section of this variable is the generation the board exists in
            public static List<int> AIsolveBoardIndexes = new List<int>();
            public static int AIsolveStep = 0;
            public static List<List<string>> solvedBoard = new List<List<string>>();
            public static bool AIactivated = false;

            public static Block selectedBlock = null;
            public static System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            public static double gameTimer = 0;
            public static bool timerStarted = false;

            public static Form1 form1Ref = null;
            public static string currentLevelFilePath = @"csvLevels/aiTester.csv";
            public static loadScreen loadScre;

            // a class named Global, which contains global variables such as the game timer to be accessed from different classes and functions.
        }
        public class Block
        {
            //this class is the standard template for a block. It is created on the read file function and contains coordinates, colour, length etc. along with its methods like moving around

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

                // gets the specific coordinates from their given length and width.
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

                // Check if game is won
                if(this.colourCode == "g_")
                {
                    if (this.cords[0][0] == Global.finishblock.y && this.cords[0][1] == Global.finishblock.x)
                    {
                        Global.form1Ref.onWin();
                    }
                }
                
                // loops through coordinates to check whether it is a valid move, then also checks if the game is won
            }

            public void drawBlock()
            {
                int i = 0;
                foreach (var item in this.cords)
                {
                    Global.pictureBoxes[this.cords[i][0]][this.cords[i][1]].BackColor = this.colour;
                    i++;
                }
                // loops through coordinates and changes the corresponding picturebox in the new moved coordinate.
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
                // standard template for the finish position class
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
                item.Location = new Point(this.x * Constants.blockPixelLength - Constants.pictureBoxGap + Constants.boardOffset, this.y * Constants.blockPixelLength - Constants.pictureBoxGap + Constants.boardOffset);
                Global.form1Ref.Controls.Add(item);

                // creates a picturebox background which is black to show exit. 

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
            if(!Global.AIactivated)
            {
                Global.AIactivated = true;
                do
                {
                    label2.Text = "AI in progress...";
                } while (label2.Text != "AI in progress...");

                Thread.Sleep(100);
                levelSolver();
                button3.Text = "View Next Step";
                label2.Text = "Board Solved, click to see next step.";
            } else
            {
                if (Global.AIsolveBoardIndexes.Count > 0 && !(Global.AIsolveStep > Global.AIsolveBoardIndexes.Count))
                {

                    if (Global.AIsolveStep == 0)
                    {
                        displayAIboard(Global.AIboards[Global.AIsolveStep][Global.AIsolveBoardIndexes[Global.AIsolveStep]]);
                        Global.AIsolveStep++;

                    }

                    if (Global.AIsolveStep == Global.AIsolveBoardIndexes.Count)
                    {
                        displayAIboard(Global.solvedBoard);
                        onWin(); // show win
                    }
                    else
                    {
                        displayAIboard(Global.AIboards[Global.AIsolveStep][Global.AIsolveBoardIndexes[Global.AIsolveStep]]);


                        Global.AIsolveStep++;
                    }


                }


                // goes to next AI step
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
    }
}


