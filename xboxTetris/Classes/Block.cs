/// <summary>
/// The main namespace.
/// </summary>
namespace xboxTetris
{
    using System;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The tetrimino object.
    /// </summary>
    class Block
    {
        #region Variables

        /// <summary>
        /// A Enum for all of the block types.
        /// </summary>
        public enum Tetrimino
        {
            // Default 
            Default,

            // Line
            I,

            // J Shape 
            J,

            // L shape
            L,

            // Square
            O,

            // S shape
            S,

            // Z shape
            Z,

            // T cross
            T
        }

        /// <summary>
        /// A Enum for the directions the Block is facing.
        /// </summary>
        public enum BlockState
        {
            // Faceing up
            Up,

            // Faceing Right
            Right,

            // Faceing Down
            Down,

            // Faceing Left
            Left
        }

        /// <summary>
        /// X Coordinate
        /// </summary>
        private int x = 3;

        /// <summary>
        /// Y Coordinate
        /// </summary>
        private int y = 0;

        /// <summary>
        /// Boolean value to indicate that the block is moving.
        /// </summary>
        private bool isMoving = true;

        /// <summary>
        /// Stores the type of block this object is.
        /// </summary>
        private Tetrimino blockType = Tetrimino.Default;

        /// <summary>
        /// Stores the current block state.
        /// </summary>
        private BlockState currentBlockState = BlockState.Up;

        #region Cached Images

        static BitmapImage debugBlock = new BitmapImage(new Uri("ms-appx:///Images/debugBlock.png"));
        static BitmapImage debugGhost = new BitmapImage(new Uri("ms-appx:///Images/debugGhost.png"));
        static BitmapImage cyanBlock = new BitmapImage(new Uri("ms-appx:///Images/cyanBlock.png"/*Cyan*/));
        static BitmapImage cyanGhost = new BitmapImage(new Uri("ms-appx:///Images/cyanGhost.png"));
        static BitmapImage tetriminoI = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_I.png"));
        static BitmapImage blueBlock = new BitmapImage(new Uri("ms-appx:///Images/blueBlock.png"/*Blue*/));
        static BitmapImage blueGhost = new BitmapImage(new Uri("ms-appx:///Images/blueGhost.png"));
        static BitmapImage tetriminoJ = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_J.png"));
        static BitmapImage orangeBlock = new BitmapImage(new Uri("ms-appx:///Images/orangeBlock.png"/*Orange*/));
        static BitmapImage orangeGhost = new BitmapImage(new Uri("ms-appx:///Images/orangeGhost.png"));
        static BitmapImage tetriminoL = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_L.png"));
        static BitmapImage yellowBlock = new BitmapImage(new Uri("ms-appx:///Images/yellowBlock.png"/*Yellow*/));
        static BitmapImage yellowGhost = new BitmapImage(new Uri("ms-appx:///Images/yellowGhost.png"));
        static BitmapImage tetriminoO = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_O.png"));
        static BitmapImage greenBlock = new BitmapImage(new Uri("ms-appx:///Images/greenBlock.png"/*Green*/));
        static BitmapImage greenGhost = new BitmapImage(new Uri("ms-appx:///Images/greenGhost.png"));
        static BitmapImage tetriminoZ = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_Z.png"));
        static BitmapImage redBlock = new BitmapImage(new Uri("ms-appx:///Images/redBlock.png"/*Red*/));
        static BitmapImage redGhost = new BitmapImage(new Uri("ms-appx:///Images/redGhost.png"));
        static BitmapImage tetriminoS = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_S.png"));
        static BitmapImage pinkBlock = new BitmapImage(new Uri("ms-appx:///Images/pinkBlock.png"/*pink*/));
        static BitmapImage pinkGhost = new BitmapImage(new Uri("ms-appx:///Images/pinkGhost.png"));
        static BitmapImage tetriminoT = new BitmapImage(new Uri("ms-appx:///Images/Tetrimino_T.png"));

        #endregion

        /// <summary>
        /// Stores the block image.
        /// </summary>
        private BitmapImage blockImage = debugBlock;

        /// <summary>
        /// Stores the ghost Image.
        /// </summary>
        private BitmapImage ghostImage = debugGhost;

        /// <summary>
        /// Stores the full tetrimino image.
        /// </summary>
        private BitmapImage tetriminoImage = debugBlock;

        #endregion

        #region Propertys

        /// <summary>
        /// Boolean property to indicate that the block is moving.
        /// </summary>
        public bool IsMoving
        {
            set { isMoving = value; }
            get { return isMoving; }
        }

        /// <summary>
        /// X Property for the top-left most square of the moving block area.
        /// </summary>
        public int X
        {
            set { x = value; }
            get { return x; }
        }

        /// <summary>
        /// Y Property for the top-left most square of the moving block area.
        /// </summary>
        public int Y
        {
            set { y = value; }
            get { return y; }
        }
        
        /// <summary>
        /// Block type property.
        /// </summary>
        public BlockState CurrentBlockState
        {
            set { currentBlockState = value; }
            get { return currentBlockState; }
        }

        /// <summary>
        /// Block image property.
        /// </summary>
        public BitmapImage BlockImage
        {
            get { return blockImage; }
        }

        /// <summary>
        /// Block image property.
        /// </summary>
        public BitmapImage GhostImage
        {
            get { return ghostImage; }
        }

        /// <summary>
        /// Block type property.
        /// </summary>
        public Tetrimino BlockType
        {
            get { return blockType; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The block constuctor.
        /// </summary>
        public Block()
        {
            // Randomize block type
            Random r = new Random();
            int randomInt = r.Next(0, 7);

            // Change the block to the randomized type.
            switch (randomInt)
            {
                case 0:
                    blockType = Tetrimino.I;
                    y = -1;
                    blockImage = cyanBlock;
                    ghostImage = cyanGhost;
                    tetriminoImage = tetriminoI;
                    break;

                case 1:
                    blockType = Tetrimino.J;
                    blockImage = blueBlock;
                    ghostImage = blueGhost;
                    tetriminoImage = tetriminoJ;
                    break;

                case 2:
                    blockType = Tetrimino.L;
                    blockImage = orangeBlock;
                    ghostImage = orangeGhost;
                    tetriminoImage = tetriminoL;
                    break;

                case 3:
                    blockType = Tetrimino.O;
                    x = 4;
                    blockImage = yellowBlock;
                    ghostImage = yellowGhost;
                    tetriminoImage = tetriminoO;
                    break;

                case 4:
                    blockType = Tetrimino.S;
                    blockImage = greenBlock;
                    ghostImage = greenGhost;
                    tetriminoImage = tetriminoS;
                    break;

                case 5:
                    blockType = Tetrimino.Z;
                    blockImage = redBlock;
                    ghostImage = redGhost;
                    tetriminoImage = tetriminoZ;
                    break;

                case 6:
                    blockType = Tetrimino.T;
                    blockImage = pinkBlock;
                    ghostImage = pinkGhost;
                    tetriminoImage = tetriminoT;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Debug constructor for block types.
        /// </summary>
        /// <param name="tetrimino"></param>
        public Block(Tetrimino tetrimino)
        {
            blockType = tetrimino;
            
            // Change the block to the randomized type.
            switch ((int)blockType)
            {
                case 0:
                    // Default block
                    break;

                case 1:
                    blockType = Tetrimino.I;
                    y = -1;
                    blockImage = cyanBlock;
                    ghostImage = cyanGhost;
                    tetriminoImage = tetriminoI;
                    break;

                case 2:
                    blockType = Tetrimino.J;
                    blockImage = blueBlock;
                    ghostImage = blueGhost;
                    tetriminoImage = tetriminoJ;
                    break;

                case 3:
                    blockType = Tetrimino.L;
                    blockImage = orangeBlock;
                    ghostImage = orangeGhost;
                    tetriminoImage = tetriminoL;
                    break;

                case 4:
                    blockType = Tetrimino.O;
                    x = 4;
                    blockImage = yellowBlock;
                    ghostImage = yellowGhost;
                    tetriminoImage = tetriminoO;
                    break;

                case 5:
                    blockType = Tetrimino.S;
                    blockImage = greenBlock;
                    ghostImage = greenGhost;
                    tetriminoImage = tetriminoS;
                    break;

                case 6:
                    blockType = Tetrimino.Z;
                    blockImage = redBlock;
                    ghostImage = redGhost;
                    tetriminoImage = tetriminoZ;
                    break;

                case 7:
                    blockType = Tetrimino.T;
                    blockImage = pinkBlock;
                    ghostImage = pinkGhost;
                    tetriminoImage = tetriminoT;
                    break;

                default:
                    break;
            }

        }

        /// <summary>
        /// Used for the math pertaining to the shape and falling of every block type.
        /// </summary>
        public bool[,] BlockArea(BlockState blockState)
        {
            // Return variable
            bool[,] blockChunk = new bool[,]
            {
                { false, false, false, false },
                { false, false, false, false },
                { false, false, false, false },
                { false, false, false, false }
            };

            // Default
            if (blockType == Tetrimino.Default)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { true, false, true, false },
                        { false, true, false, true },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, true },
                        { false, false, true, false },
                        { false, false, false, true },
                        { false, false, true, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { true, false, false, false },
                        { false, true, false, false },
                        { true, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { false, false, false, false },
                        { true, false, true, false },
                        { false, true, false, true }
                    };
                }

                return blockChunk;
            }

            // I
            else if (blockType == Tetrimino.I)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, true, true },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, true, false },
                        { false, false, true, false },
                        { false, false, true, false },
                        { false, false, true, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, true, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { false, false, false, false },
                        { true, true, true, true },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // J
            else if (blockType == Tetrimino.J)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { true, false, false, false },
                        { true, true, true, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, true, false },
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, false, false },
                        { true, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, true, false },
                        { false, false, true, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // L
            else if (blockType == Tetrimino.L)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, true, false },
                        { true, true, true, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, true, true, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, true, false },
                        { true, false, false, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // O
            else if (blockType == Tetrimino.O)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { true, true, false, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { true, true, false, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { true, true, false, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { true, true, false, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // S
            else if (blockType == Tetrimino.S)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, true, false },
                        { true, true, false, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, true, false },
                        { false, false, true, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { true, false, false, false },
                        { true, true, false, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { false, true, true, false },
                        { true, true, false, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // Z
            else if (blockType == Tetrimino.Z)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { true, true, false, false },
                        { false, true, true, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, true, false },
                        { false, true, true, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { true, true, false, false },
                        { true, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, false, false },
                        { false, true, true, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // T
            else if (blockType == Tetrimino.T)
            {
                if (blockState == BlockState.Up)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { true, true, true, false },
                        { false, false, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Right)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, true, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Left)
                {
                    blockChunk = new bool[,]
                    {
                        { false, true, false, false },
                        { true, true, false, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                else if (blockState == BlockState.Down)
                {
                    blockChunk = new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, true, false },
                        { false, true, false, false },
                        { false, false, false, false }
                    };
                }

                return blockChunk;
            }

            // Nothing
            else
            {
                return blockChunk;
            }
        }

        /// <summary>
        /// Convert the current block type to a full tetrimino image.
        /// </summary>
        /// <param name="blockType"></param>
        public static BitmapImage tetriminoFullImage(Tetrimino blockType)
        {
            switch ((int)blockType)
            {
                case 0:
                    return debugBlock;

                case 1:
                    return tetriminoI;

                case 2:
                    return tetriminoJ;

                case 3:
                    return tetriminoL;

                case 4:
                    return tetriminoO;

                case 5:
                    return tetriminoS;

                case 6:
                    return tetriminoZ;

                case 7:
                    return tetriminoT;

                default:
                    return debugBlock;
            }
        }

        /// <summary>
        /// Cast the current blocks image.
        /// </summary>
        /// <param name="currentBlock"></param>
        public static explicit operator BitmapImage(Block currentBlock)
        {
            return currentBlock.tetriminoImage;
        }

        #endregion
    }
}
