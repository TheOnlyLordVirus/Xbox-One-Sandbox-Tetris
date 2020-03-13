/// <summary>
/// The main namespace.
/// </summary>
namespace xboxTetris
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    
    /// <summary>
    /// The tetris matrix object.
    /// </summary>
    class TetrisGame
    {
        #region Variables

        #region Cheats

        /// <summary>
        /// Stores the bools for the cheats
        /// </summary>
        private bool[] cheatBools = new bool[]
        {
            true, // Inf Spin
            true, // Ghost Block
            false, // Choose Block
        };

        /// <summary>
        /// Is their a block moving?
        /// </summary>
        private bool cheatBlockMoving = false;

        #endregion

        /// <summary>
        /// Used to indicate a direction.
        /// </summary>
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        /// <summary>
        /// The current score for this game.
        /// </summary>
        private long gameScore = 0;

        /// <summary>
        /// The speed int milliseconds for the background worker timer.
        /// </summary>
        private int movementTick = 40;

        /// <summary>
        /// Determins if whether or not we are to move downwards this tick.
        /// </summary>
        private int tickInterval = 0;

        /// <summary>
        /// Used to save the last X position of moving block.
        /// </summary>
        private int lastMovingBlockX;

        /// <summary>
        /// Used to save the last Y position of moving block.
        /// </summary>
        private int lastMovingBlockY;

        /// <summary>
        /// boolean value to determine if the game is over.
        /// </summary>
        private bool gameOver = false;

        /// <summary>
        /// boolean value to determine if the game is paused.
        /// </summary>
        private bool isPaused = false;

        /// <summary>
        /// Used to detect if we are soft dropping the block.
        /// </summary>
        private bool softDrop = false;

        /// <summary>
        /// Are these squares blocks?
        /// </summary>
        private volatile bool[,] feildData = new bool[20, 10];

        /// <summary>
        /// Used to save the last position of the moving block.
        /// </summary>
        private bool[,] lastBlockAreaPos = new bool[4, 4];

        /// <summary>
        /// Used to save the last block state of the moving block.
        /// </summary>
        private Block.BlockState lastBlockState;

        /// <summary>
        /// Every block on the feild of squares.
        /// </summary>
        private Image[,] feildSquares = new Image[20, 10];

        /// <summary>
        /// Default image.
        /// </summary>
        private BitmapImage blankSquare = new BitmapImage(new Uri("ms-appx:///Images/feildBackdrop.png", UriKind.Absolute));

        /// <summary>
        /// A list of the blocks on the feild.
        /// </summary>
        private List<Block> blocks = new List<Block>();

        /// <summary>
        /// The current block that is moving.
        /// </summary>
        private Block movingBlock = new Block(Block.Tetrimino.Default);


        /// <summary>
        /// The next block that will be dropped.
        /// </summary>
        private Block nextBlock = new Block(Block.Tetrimino.Default);

        /// <summary>
        /// Used to move the block downwards and rotate it.
        /// </summary>
        private BackgroundWorker blockMover = new BackgroundWorker();

        #endregion

        #region Propertys

        /// <summary>
        /// The current score of the game.
        /// </summary>
        public long GameScore
        {
            get { return gameScore; }
        }

        /// <summary>
        /// Did we loose this game?
        /// </summary>
        public bool GameOver
        {
            get { return gameOver; }
        }

        /// <summary>
        /// Used to check to see if the game is paused or not.
        /// </summary>
        public bool IsPaused
        {
            set { isPaused = value; }
            get { return isPaused; }
        }

        /// <summary>
        /// Is there a block moving? (Used only for the choose block cheat)
        /// </summary>
        public bool CheatBlockMoving
        {
            set { cheatBlockMoving = value; }
            get { return cheatBlockMoving; }
        }

        /// <summary>
        /// Used to grab the list of feildSquare Images, to be stored on the main grid.
        /// </summary>
        public Image[,] FeildSquares
        {
            get { return feildSquares; }
        }

        /// <summary>
        /// The image for the next block.
        /// </summary>
        public BitmapImage NextBlock
        {
            get { return (BitmapImage)nextBlock; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The main constuctor for the game.
        /// </summary>
        /// <param name="squareSize"></param>
        public TetrisGame(int squareSize)
        {
            blockMover.DoWork += blockMover_DoWork;
            blockMover.ProgressChanged += blockMover_ProgressChanged;
            blockMover.RunWorkerCompleted += blockMover_RunWorkerCompleted;
            blockMover.WorkerReportsProgress = true;
            blockMover.WorkerSupportsCancellation = true;
            DrawMatrix(squareSize);
        }

        /// <summary>
        /// The constuctor for the game with cheats.
        /// </summary>
        /// <param name="squareSize"></param>
        public TetrisGame(int squareSize, bool infSpin, bool ghstBlk, bool choseBlk, bool mxScr)
        {
            blockMover.DoWork += blockMover_DoWork;
            blockMover.ProgressChanged += blockMover_ProgressChanged;
            blockMover.RunWorkerCompleted += blockMover_RunWorkerCompleted;
            blockMover.WorkerReportsProgress = true;
            blockMover.WorkerSupportsCancellation = true;
            cheatBools[0] = infSpin;
            cheatBools[1] = ghstBlk;
            cheatBools[2] = choseBlk;
            if(mxScr)
            {
                gameScore = 9999999999;
            }
            DrawMatrix(squareSize);
        }

        #region Game core

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void StartNewGame()
        {
            if(!cheatBools[2])
            {
                nextBlock = new Block();
                DropBlock();
            }
        }

        #region Block Movement Input

        /// <summary>
        /// Moves the block in a direction.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveBlock(Direction direction)
        {
            // If we are moving.
            if (movingBlock.IsMoving)
            {
                // Right.
                if (direction == Direction.Right)
                {
                    if (CheckForIntersectingBlocks(movingBlock.Y, movingBlock.X + 1, movingBlock.CurrentBlockState))
                    {
                        movingBlock.X++;
                    }
                }

                // Left.
                else if (direction == Direction.Left)
                {
                    if (CheckForIntersectingBlocks(movingBlock.Y, movingBlock.X - 1, movingBlock.CurrentBlockState))
                    {
                        movingBlock.X--;
                    }
                }
            }
        }

        /// <summary>
        /// Rotates the block left or right.
        /// </summary>
        public void RotateBlock(Direction direction)
        {
            // If we are moving.
            if (movingBlock.IsMoving)
            {
                // Rotate right.
                if (direction == Direction.Right)
                {
                    if (movingBlock.CurrentBlockState != Block.BlockState.Left && WallKick(movingBlock.CurrentBlockState + 1))
                    {
                        movingBlock.CurrentBlockState++;
                    }

                    else if (movingBlock.CurrentBlockState == Block.BlockState.Left && WallKick(Block.BlockState.Up))
                    {
                        movingBlock.CurrentBlockState = Block.BlockState.Up;
                    }

                    // Infinite rotate.
                    if(cheatBools[0])
                    {
                        tickInterval = 0;
                    }
                }

                // Rotate left.
                else if (direction == Direction.Left)
                {
                    if (movingBlock.CurrentBlockState != Block.BlockState.Up && WallKick(movingBlock.CurrentBlockState - 1))
                    {
                        movingBlock.CurrentBlockState--;
                    }

                    else if (movingBlock.CurrentBlockState == Block.BlockState.Up && WallKick(Block.BlockState.Left))
                    {
                        movingBlock.CurrentBlockState = Block.BlockState.Left;
                    }

                    // Infinite rotate.
                    if(cheatBools[0])
                    {
                        tickInterval = 0;
                    }
                }
            }
        }

        /// <summary>
        /// While we are holding the down button.
        /// </summary>
        public void SoftDrop(bool holdingDpadDown)
        {
            if (holdingDpadDown)
            {
                movementTick = 3;
                softDrop = true;
            }

            else
            {
                movementTick = 20;
                softDrop = false;
            }
        }

        #endregion

        #endregion

        #region Block Updates and Completion

        /// <summary>
        /// Runs async to alow for real time block updates instead of all at once.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockMover_DoWork(object sender, DoWorkEventArgs e)
        {
            while (movingBlock.IsMoving)
            {
                blockMover.ReportProgress(0);
                Thread.Sleep(movementTick);
            }
        }

        /// <summary>
        /// Actually updates the blocks image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockMover_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!isPaused)
            {
                // If we are moving and the block isn't going down this tick.
                if (movingBlock.IsMoving && tickInterval != 25)
                {
                    // Update tick.
                    tickInterval++;

                    // Clear old pos.
                    editFeild(blankSquare, lastBlockAreaPos, false, lastMovingBlockX, lastMovingBlockY);

                    // Draw ghost block.
                    if(cheatBools[1])
                    {
                        // Remove old ghost block.
                        if (movingBlock.X != lastMovingBlockX || movingBlock.CurrentBlockState != lastBlockState)
                        {
                            for (int y = lastMovingBlockY; CheckForIntersectingBlocks(y, lastMovingBlockX, lastBlockState); y++)
                            {
                                if (!CheckForIntersectingBlocks(y + 1, lastMovingBlockX, lastBlockState))
                                {
                                    editFeild(blankSquare, movingBlock.BlockArea(lastBlockState), false, lastMovingBlockX, y);
                                }
                            }
                        }

                        // Draw ghost block.
                        for (int y = movingBlock.Y; CheckForIntersectingBlocks(y, movingBlock.X, movingBlock.CurrentBlockState); y++)
                        {
                            if (!CheckForIntersectingBlocks(y + 1, movingBlock.X, movingBlock.CurrentBlockState))
                            {
                                editFeild(movingBlock.GhostImage, movingBlock.BlockArea(movingBlock.CurrentBlockState), false, movingBlock.X, y);
                            }
                        }
                    }

                    // Update new pos.
                    editFeild(movingBlock.BlockImage, movingBlock.BlockArea(movingBlock.CurrentBlockState), true, movingBlock.X, movingBlock.Y);


                    // Save last block position in memory.
                    lastBlockAreaPos = movingBlock.BlockArea(movingBlock.CurrentBlockState);
                    lastBlockState = movingBlock.CurrentBlockState;
                    lastMovingBlockX = movingBlock.X;
                    lastMovingBlockY = movingBlock.Y;
                }

                // Can we move down again?
                else if (!CheckForIntersectingBlocks(movingBlock.Y + 1, movingBlock.X, movingBlock.CurrentBlockState))
                {
                    // Clear old pos.
                    editFeild(blankSquare, lastBlockAreaPos, false, lastMovingBlockX, lastMovingBlockY);

                    // Update new pos.
                    editFeild(movingBlock.BlockImage, movingBlock.BlockArea(movingBlock.CurrentBlockState), true, movingBlock.X, movingBlock.Y);

                    movingBlock.IsMoving = false;
                    tickInterval = 0;

                    if (softDrop && gameScore != 9999999999)
                    {
                        gameScore++;
                    }
                }

                // If we can move down do it.
                else
                {
                    movingBlock.Y++;
                    tickInterval = 0;

                    if (softDrop && gameScore != 9999999999)
                    {
                        gameScore++;
                    }
                }
            }
        }

        /// <summary>
        /// Background worker is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockMover_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckForCompleteRow();

            if(!cheatBools[2])
            {
                DropBlock();
            }

            else
            {
                cheatBlockMoving = false;
            }
        }

        #endregion

        #region General Logic

        /// <summary>
        /// Changes the margin of each square in the matrix accourding to the size of a your squares.
        /// </summary>
        /// <param name="squareSize"></param>
        private void DrawMatrix(int squareSize)
        {
            // Draw the matrix.
            int Left = 0 - squareSize;
            int Right = squareSize * 10 + squareSize;
            int Top = 0;
            int Bottom = squareSize * 20;

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (x != 0 || y == 0)
                    {
                        Left += squareSize;
                        Right -= squareSize;
                        Image Square = new Image() { Margin = new Thickness(Left, Top, Right, Bottom), Source = blankSquare };
                        Square.Width = squareSize;
                        Square.Height = squareSize;
                        feildSquares[y, x] = Square;
                    }

                    else
                    {
                        Left = 0;
                        Right = squareSize * 10;
                        Top += squareSize;
                        Bottom -= squareSize;
                        Image Square = new Image() { Margin = new Thickness(Left, Top, Right, Bottom), Source = blankSquare };
                        Square.Width = squareSize;
                        Square.Height = squareSize;
                        feildSquares[y, x] = Square;
                    }

                    feildData[y, x] = false;
                }
            }
        }

        /// <summary>
        /// Drops a block.
        /// </summary>
        private void DropBlock()
        {
            blockMover.CancelAsync();
            ResetMemory();

            if (!LooseGame(nextBlock) && !blockMover.IsBusy)
            {
                movingBlock = nextBlock;
                nextBlock = new Block();
                blockMover.RunWorkerAsync();
            }

            else
            {
                gameOver = true;
            }
        }

        /// <summary>
        /// Drops a custom block.
        /// </summary>
        public void DropBlock(Block block)
        {
            blockMover.CancelAsync();
            ResetMemory();

            if (!LooseGame(nextBlock) && !blockMover.IsBusy)
            {
                movingBlock = block;
                cheatBlockMoving = true;
                blockMover.RunWorkerAsync();
            }

            else
            {
                gameOver = true;
            }
        }

        /// <summary>
        /// This resets the memory of the last movent locations at the end of a blocks life cycle.
        /// </summary>
        private void ResetMemory()
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    lastBlockAreaPos[y, x] = false;
                }
            }

            tickInterval = 0;
            lastMovingBlockX = 3;
            lastMovingBlockY = 0;
        }

        /// <summary>
        /// Takes a newly spawned block and checks to see if there is any blocks there.
        /// </summary>
        /// <returns></returns>
        private bool LooseGame(Block newBlock)
        {
            bool[,] BlockArea = newBlock.BlockArea(newBlock.CurrentBlockState);

            for(int y = 0; y < 4; y++)
            {
                for(int x = 0; x < 4; x++)
                {
                    if(BlockArea[y,x] && feildData[newBlock.Y + y, newBlock.X + x])
                    {
                        // Loose.
                        return true;
                    }
                }
            }

            // Game on.
            return false;
        }

        /// <summary>
        /// When a block on the side of the feild atempts to rotate, this method will then 'Kick' said block away from the wall.
        /// </summary>
        /// <param name="blockState"></param>
        /// <returns></returns>
        private bool WallKick(Block.BlockState blockState)
        {
            if (!CheckForIntersectingBlocks(movingBlock.Y, movingBlock.X, blockState))
            {
                for (int i = 1; i < 3; i++)
                {
                    if (CheckForIntersectingBlocks(movingBlock.Y, movingBlock.X + i, blockState))
                    {
                        movingBlock.X += i;
                        return true;
                    }

                    else if (CheckForIntersectingBlocks(movingBlock.Y, movingBlock.X - i, blockState))
                    {
                        movingBlock.X -= i;
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if we completed a row.
        /// </summary>
        private void CheckForCompleteRow()
        {
            int rowsNum = 0;

            // find and remove full rows.
            for (int y = 19; y > 0; y--)
            {
                bool[] row = new bool[10];
                for (int x = 0; x < 10; x++)
                {
                    if (feildData[y, x])
                    {
                        row[x] = true;
                    }
                }

                // If all of the squares are part of blocks.
                if (row.All(x => x))
                {
                    // Remove complete line.
                    for (int x = 0; x < 10; x++)
                    {
                        if (feildData[y, x])
                        {
                            feildData[y, x] = false;
                            feildSquares[y, x].Source = blankSquare;
                        }
                    }

                    for (int y2 = y - 1; y2 > 0; y2--)
                    {
                        for (int x2 = 0; x2 < 10; x2++)
                        {
                            // If this square is part of a block, move it down.
                            if (feildData[y2, x2])
                            {
                                // Set new data.
                                feildSquares[y2 + 1, x2].Source = feildSquares[y2, x2].Source;
                                feildData[y2 + 1, x2] = true;

                                // Remove old data.
                                feildData[y2, x2] = false;
                                feildSquares[y2, x2].Source = blankSquare;
                            }
                        }
                    }

                    rowsNum++;
                    row = new bool[10];
                    y = 20;
                }
            }

            // Give points.
            if(gameScore != 9999999999)
            {
                switch (rowsNum)
                {
                    case 1:
                        gameScore += 40;
                        break;

                    case 2:
                        gameScore += 100;
                        break;

                    case 3:
                        gameScore += 300;
                        break;

                    case 4:
                        gameScore += 1200;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Edit the feilds images & data where ever we want.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="blockArea"></param>
        /// <param name="flag"></param>
        /// <param name="passedX"></param>
        /// <param name="passedY"></param>
        private void editFeild(BitmapImage img, bool[,] blockArea, bool flag, int passedX, int passedY)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (blockArea[y, x])
                    {
                        feildSquares[(passedY + y), passedX + x].Source = img;
                        feildData[(passedY + y), passedX + x] = flag;
                    }
                }
            }
        }

        /// <summary>
        /// Edit the feilds data where ever we want.
        /// </summary>
        /// <param name="blockArea"></param>
        /// <param name="flag"></param>
        private void editFeild(bool[,] blockArea, bool flag, int passedX, int passedY)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (blockArea[y, x])
                    {
                        feildData[passedY + y, passedX + x] = flag;
                    }
                }
            }
        }

        /// <summary>
        /// Used to make sure that we don't intersect any blocks while moving. 
        /// </summary>
        /// <param name="passedY"></param>
        /// <param name="passedX"></param>
        /// <param name="blockState"></param>
        /// <returns> </returns>
        private bool CheckForIntersectingBlocks(int passedY, int passedX, Block.BlockState blockState)
        {
            // Block area shit.
            bool[,] currentBlockArea = movingBlock.BlockArea(movingBlock.CurrentBlockState);
            bool[,] newBlockArea = movingBlock.BlockArea(blockState);
            editFeild(currentBlockArea, false, movingBlock.X, movingBlock.Y);

            // Are we rotating in to any blocks?
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    // Does this square exist?
                    if (newBlockArea[y, x])
                    {
                        if (passedY + y <= 19 && passedY + y >= 0 && passedX + x >= 0 && passedX + x <= 9)
                        {
                            // is it intersecting any blocks?
                            if (feildData[passedY + y, passedX + x])
                            {
                                editFeild(currentBlockArea, true, movingBlock.X, movingBlock.Y);
                                return false;
                            }
                        }

                        else
                        {
                            editFeild(currentBlockArea, true, movingBlock.X, movingBlock.Y);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #endregion
    }
}
