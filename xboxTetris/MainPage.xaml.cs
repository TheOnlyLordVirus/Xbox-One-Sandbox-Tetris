
/*
         ▄▀▀▀█▀▀▄  ▄▀▀█▄▄▄▄  ▄▀▀▀█▀▀▄  ▄▀▀▄▀▀▀▄  ▄▀▀█▀▄   ▄▀▀▀▀▄ 
        █    █  ▐ ▐  ▄▀   ▐ █    █  ▐ █   █   █ █   █  █ █ █   ▐ 
        ▐   █       █▄▄▄▄▄  ▐   █     ▐  █▀▀█▀  ▐   █  ▐    ▀▄   
           █        █    ▌     █       ▄▀    █      █    ▀▄   █  
         ▄▀        ▄▀▄▄▄▄    ▄▀       █     █    ▄▀▀▀▀▀▄  █▀▀▀   
        █          █    ▐   █         ▐     ▐   █       █ ▐      
        ▐          ▐        ▐                   ▐       ▐        
    Copyright Disclaimer under section 107 of the Copyright Act of 1976, 
    allowance is made for “fair use” for purposes such as criticism, comment,
    news reporting, teaching, scholarship, education and research.
    Fair use is a use permitted by copyright statute that might otherwise be infringing.
*/
// TO DO: Add hard drop, make things look "Finished."

/// <summary>
/// The main project namespace.
/// </summary>
namespace xboxTetris
{
    using System;
    using System.Threading;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.Gaming.Input;
    using Windows.Media.Core;
    using Windows.UI.Xaml.Media;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.System;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Variables

        #region Cheats

        /// <summary>
        /// Used to check what option we have selected.
        /// </summary>
        private enum CheatScreenOption
        {
            InfinitySpin,
            GhostBlock,
            ChooseBlock,
            MaxScore
        }

        /// <summary>
        /// What cheat option are we on?
        /// </summary>
        private CheatScreenOption selectedCheat = CheatScreenOption.InfinitySpin;

        /// <summary>
        /// Have the cheat options been.
        /// </summary>
        private bool cheatFlag = false;

        /// <summary>
        /// Stores the bools for the cheats
        /// </summary>
        private bool[] cheatBools = new bool[]
        {
            true, // Inf Spin
            true, // Ghost Block
            false, // Choose Block
            false // Max Score
        };

        /// <summary>
        /// Used for choosing your cheat block.
        /// </summary>
        private Block.Tetrimino CheatTetrimino = Block.Tetrimino.Default;

        #endregion

        #region Cached Images
                    
        // Xbox One Buttons.
        BitmapImage buttonMenu = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_Menu.png"));
        BitmapImage buttonWindows = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_Windows.png"));
        BitmapImage buttonA = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_A.png"));
        BitmapImage buttonB = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_B.png"));
        BitmapImage buttonDpadRight = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_Dpad_Right.png"));
        BitmapImage buttonDpadLeft = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_Dpad_Left.png"));
        BitmapImage buttonLB = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_LB.png"));
        BitmapImage buttonRB = new BitmapImage(new Uri("ms-appx:///Images/Xbox One/XboxOne_RB.png"));

        // Keyboard Keys.
        BitmapImage keyEnter = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_Enter.png"));
        BitmapImage keyShift = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_Shift_Alt.png"));
        BitmapImage keyEsc = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_Esc.png"));
        BitmapImage keyD = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_D.png"));
        BitmapImage keyA = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_A.png"));
        BitmapImage keyQ = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_Q.png"));
        BitmapImage keyW = new BitmapImage(new Uri("ms-appx:///Images/Keys/Keyboard_Black_W.png"));

        #endregion

        /// <summary>
        /// Are we using a Xbox One?
        /// </summary>
        private bool isXbox = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox";

        /// <summary>
        /// Is there a Xbox One Controller plugged in?
        /// </summary>
        private bool xboxController = false;

        /// <summary>
        /// Used to to save the last state of the xboxController Boolean Variable in memory.
        /// </summary>
        private bool xboxControllerSaveInMem = false;

        /// <summary>
        /// Used to lock a thread to proccess gamepads.
        /// </summary>
        private readonly object myLock = new object();

        /// <summary>
        /// Used for sizing the matrix for the tetriminos to fall in to.
        /// </summary>
        private int squareSize = 22;

        /// <summary>
        /// The main TetrisGame where all the the squares will be stored.
        /// </summary>
        private TetrisGame mainTetrisGame;

        /// <summary>
        /// The controller we are using.
        /// </summary>
        private Gamepad mainGamepad;

        /// <summary>
        /// Used for reading gamepad inputs.
        /// </summary>
        private GamepadReading reading;

        /// <summary>
        /// Used for reading and updating the game every 10th of a second.
        /// </summary>
        private DispatcherTimer UpdateGame = new DispatcherTimer();

        /// <summary>
        /// Used for message boxes.
        /// </summary>
        private UWP_MessageBox NOTIFY = new UWP_MessageBox();

        /// <summary>
        /// The current game state.
        /// </summary>
        private GameState currentGameState = GameState.TitleScreen;

        /// <summary>
        /// Used to check what the games state is.
        /// </summary>
        private enum GameState
        {
            TitleScreen,
            CheatScreen,
            InGame,
            Paused,
            GameOver
        }

        #endregion

        #region Methods

        /// <summary>
        /// The contructor.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
            mainPage.RequiresPointer = RequiresPointer.Never;

            // Sound player.
            SoundPlayer.AutoPlay = true;
            SoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/Tetris.mp3"));
            SoundPlayer.MediaPlayer.IsLoopingEnabled = true;
            SoundPlayer.MediaPlayer.Volume = 0.1;

            InitiateGamepad();
            UpdateGame.Tick += UpdateGame_Tick;
            UpdateGame.Interval = new System.TimeSpan(TimeSpan.TicksPerSecond / 10);
            UpdateGame.Start();

            // What system are we running on?
            if (!isXbox)
            {
                squareSize = 30;
                txtQuitApplication.Text = "To close Tetris.";
                updateButtonImages();
            }
        }

        /// <summary>
        /// Get the first controller, Add the gamepad added and disconnected events.
        /// </summary>
        private void InitiateGamepad()
        {
            Gamepad.GamepadAdded += (object sender, Gamepad e) =>
            {
                lock (myLock)
                {
                    mainGamepad = e;
                    xboxController = true;
                }
            };

            Gamepad.GamepadRemoved += (object sender, Gamepad e) =>
            {
                lock (myLock)
                {
                    mainGamepad = null;
                    xboxController = false;
                }
            };

            lock (myLock)
            {
                // Find the first gamepad.
                foreach (var gamepad in Gamepad.Gamepads)
                {
                    if (!mainGamepad.Equals(gamepad))
                    {
                        mainGamepad = gamepad;
                        xboxController = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Changes the button images to display the xbox buttons or the keys on a keyboard.
        /// </summary>
        private void updateButtonImages()
        {
            if (!isXbox)
            {
                if (xboxController)
                {
                    startGameImage.Source = buttonMenu;
                    cheatMenuImage.Source = buttonWindows;
                    quitGameImage.Source = buttonB;
                    gameMoveRight.Source = buttonDpadRight;
                    gameMoveLeft.Source = buttonDpadLeft;
                    gameRotateLeft.Source = buttonLB;
                    gameRotateRight.Source = buttonRB;
                    gamePause.Source = buttonMenu;
                    gameBackToTitle.Source = buttonWindows;
                    gameUnPause.Source = buttonWindows;
                    selectCheatButton.Source = buttonA;
                    dropCheatBlock.Source = buttonA;
                }

                else if (!xboxController)
                {
                    startGameImage.Source = keyEnter;
                    cheatMenuImage.Source = keyShift;
                    quitGameImage.Source = keyEsc;
                    gameMoveRight.Source = keyD;
                    gameMoveLeft.Source = keyA;
                    gameRotateRight.Source = keyW;
                    gameRotateLeft.Source = keyQ;
                    gamePause.Source = keyShift;
                    gameBackToTitle.Source = keyEsc;
                    gameUnPause.Source = keyShift;
                    selectCheatButton.Source = keyEnter;
                    dropCheatBlock.Source = keyEnter;
                }
            }
        }

        #region Game Input

        /// <summary>
        /// Updates the games data every 10th of a second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateGame_Tick(object sender, Object e)
        {
            // Retrive game data.
            if (currentGameState == GameState.InGame)
            {
                ScoreTextBlock.Text = mainTetrisGame.GameScore.ToString("D10");

                // Next Block image.
                if (!cheatBools[2])
                {
                    nextBlockImg.Source = mainTetrisGame.NextBlock;
                }

                else
                {
                    nextBlockImg.Source = Block.tetriminoFullImage(CheatTetrimino);
                    
                    // Drop block button.
                    if (!mainTetrisGame.CheatBlockMoving && dropCheatBlock.Visibility == Visibility.Collapsed)
                    {
                        dropCheatBlock.Visibility = Visibility.Visible;
                    }

                    else if (mainTetrisGame.CheatBlockMoving && dropCheatBlock.Visibility == Visibility.Visible)
                    {
                        dropCheatBlock.Visibility = Visibility.Collapsed;
                    }
                }

                // TO DO: Add System.IO and keep track of scores when the game ends.
                if (mainTetrisGame.GameOver)
                {
                    currentGameState = GameState.GameOver;
                    NOTIFY.popupMessage("Your final score was: " + mainTetrisGame.GameScore + " \nWould you like to try again?", "GAME OVER!",

                    // Yes.
                    () =>
                    {
                        gameGrid.Children.Clear();
                        toggleGame(true);
                    },

                    // No.
                    () =>
                    {
                        toggleGame(false);
                    });

                    return;
                }
            }

            #region Controller Input

            // Updates the images for the buttons if a controller gets disconnected or connected on PC.
            if (!isXbox && xboxControllerSaveInMem != xboxController)
            {
                updateButtonImages();
                xboxControllerSaveInMem = xboxController;

                // Will also pause the game on controller state change.
                if (currentGameState == GameState.InGame)
                {
                    togglePause(true);
                }
            }

            // If the controller disconnects on xbox.
            else if (isXbox && currentGameState == GameState.InGame && xboxController == false)
            {
                togglePause(true);
            }

            // Send controller input.
            if (xboxController)
            {
                reading = mainGamepad.GetCurrentReading();

                #region Permanent Binds

                // Toggle sound.
                if (reading.Buttons == GamepadButtons.Y)
                {
                    ToggleSound();
                }

                // Close Game.
                else if (reading.Buttons == GamepadButtons.B)
                {
                    Application.Current.Exit();
                }

                #endregion

                #region Pause Screen

                // Pause Menu.
                else if (currentGameState == GameState.Paused && reading.Buttons == GamepadButtons.Menu)
                {
                    togglePause(false);
                    Thread.Sleep(200);
                }

                #endregion

                #region Title Screen

                // Title Screen.
                else if (currentGameState == GameState.TitleScreen)
                {
                    // Start Game.
                    if (reading.Buttons == GamepadButtons.Menu)
                    {
                        toggleGame(true);
                    }

                    // Enter cheat menu.
                    else if (reading.Buttons == GamepadButtons.View)
                    {
                        toggleCheatMenu(true);
                    }
                }

                #endregion

                #region Cheats Screen

                // Cheat screen.
                else if (currentGameState == GameState.CheatScreen)
                {
                    // Enter title screen.
                    if (reading.Buttons == GamepadButtons.View)
                    {
                        toggleCheatMenu(false);
                        Thread.Sleep(200);
                    }

                    // Scroll Up.
                    else if (reading.Buttons == GamepadButtons.DPadUp && selectedCheat != CheatScreenOption.InfinitySpin)
                    {
                        selectedCheat--;
                        selectCheatButton.Margin = new Thickness(0, selectCheatButton.Margin.Top - 80, 140, 0);
                        Thread.Sleep(200);
                    }

                    // Scroll Down.
                    else if (reading.Buttons == GamepadButtons.DPadDown && selectedCheat != CheatScreenOption.MaxScore)
                    {
                        selectedCheat++;
                        selectCheatButton.Margin = new Thickness(0, selectCheatButton.Margin.Top + 80, 140, 0);
                        Thread.Sleep(200);
                    }

                    // Toggle Cheat.
                    else if (reading.Buttons == GamepadButtons.A)
                    {
                        ToggleCheat(selectedCheat);
                        Thread.Sleep(200);
                    }
                }

                #endregion

                #region In Game

                // Game movement.
                else if (currentGameState == GameState.InGame)
                {
                    #region One button press

                    // Soft Drop.
                    if (reading.Buttons == GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.SoftDrop(true);
                        return;
                    }

                    else if (reading.Buttons != GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.SoftDrop(false);
                    }

                    // Quit Game.
                    if (reading.Buttons == GamepadButtons.View)
                    {
                        toggleGame(false);
                        Thread.Sleep(300);
                    }

                    // Pause.
                    else if (reading.Buttons == GamepadButtons.Menu)
                    {
                        togglePause(true);
                        Thread.Sleep(200);
                    }

                    // Choose block cheat.
                    if (cheatBools[2] && !mainTetrisGame.CheatBlockMoving)
                    {
                        if (reading.Buttons == GamepadButtons.LeftShoulder && CheatTetrimino != Block.Tetrimino.Default)
                        {
                            CheatTetrimino--;
                            Thread.Sleep(200);
                        }

                        else if (reading.Buttons == GamepadButtons.RightShoulder && CheatTetrimino != Block.Tetrimino.T)
                        {
                            CheatTetrimino++;
                            Thread.Sleep(200);
                        }

                        else if (reading.Buttons == GamepadButtons.A)
                        {
                            mainTetrisGame.DropBlock(new Block(CheatTetrimino));
                        }
                    }

                    // Block Movement Left / Right.
                    else if (reading.Buttons == GamepadButtons.DPadLeft)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        Vibrate(150, .5, 0, 0, 0);
                    }

                    else if (reading.Buttons == GamepadButtons.DPadRight)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        Vibrate(150, 0, 0, .5, 0);
                    }

                    // Rotation Left / Right.
                    else if (reading.Buttons == GamepadButtons.LeftShoulder)
                    {
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        Vibrate(150, 0, .2, 0, 0);
                    }

                    else if (reading.Buttons == GamepadButtons.RightShoulder)
                    {
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        Vibrate(150, 0, 0, 0, .2);
                    }

                    #endregion

                    #region Two button presses

                    // Rotation while moving.
                    else if ((int)reading.Buttons == (int)GamepadButtons.LeftShoulder + (int)GamepadButtons.DPadLeft)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.RightShoulder + (int)GamepadButtons.DPadLeft)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.LeftShoulder + (int)GamepadButtons.DPadRight)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        Vibrate(150, 0, .2, .5, 0);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.RightShoulder + (int)GamepadButtons.DPadRight)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        Vibrate(150, 0, 0, .5, .2);
                    }

                    // Soft Drop while moving.
                    else if ((int)reading.Buttons == (int)GamepadButtons.DPadLeft + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.DPadRight + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    // Soft Drop while rotating.
                    else if ((int)reading.Buttons == (int)GamepadButtons.LeftShoulder + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.RightShoulder + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    #endregion

                    #region Three button presses

                    // Rotation & Soft Drop while moving.
                    else if ((int)reading.Buttons == (int)GamepadButtons.LeftShoulder + (int)GamepadButtons.DPadLeft + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.RightShoulder + (int)GamepadButtons.DPadLeft + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, .5, 0, 0, .2);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.LeftShoulder + (int)GamepadButtons.DPadRight + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, 0, .2, .5, 0);
                    }

                    else if ((int)reading.Buttons == (int)GamepadButtons.RightShoulder + (int)GamepadButtons.DPadRight + (int)GamepadButtons.DPadDown)
                    {
                        mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                        mainTetrisGame.SoftDrop(true);
                        Vibrate(150, 0, 0, .5, .2);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }


        #region Keyboard Input

        /// <summary>
        /// Handles Key inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Toggle Sound.
            if (args.VirtualKey == VirtualKey.Tab)
            {
                ToggleSound();
            }

            // UnPause.
            else if (currentGameState == GameState.Paused && args.VirtualKey == VirtualKey.Shift)
            {
                togglePause(false);
            }

            #region Title Screen

            // Title Screen.
            else if (currentGameState == GameState.TitleScreen)
            {
                if (args.VirtualKey == VirtualKey.Enter)
                {
                    toggleGame(true);
                }

                else if (args.VirtualKey == VirtualKey.Shift)
                {
                    toggleCheatMenu(true);
                }

                else if (args.VirtualKey == VirtualKey.Escape)
                {
                    Application.Current.Exit();
                }
            }

            #endregion

            #region Cheat Screen

            // Cheat screen.
            else if (currentGameState == GameState.CheatScreen)
            {
                // Enter title screen.
                if (args.VirtualKey == VirtualKey.Shift || args.VirtualKey == VirtualKey.Escape)
                {
                    toggleCheatMenu(false);
                }

                // Scroll Up.
                else if ((args.VirtualKey == VirtualKey.W || args.VirtualKey == VirtualKey.Up) && selectedCheat != CheatScreenOption.InfinitySpin)
                {
                    selectedCheat--;
                    selectCheatButton.Margin = new Thickness(0, selectCheatButton.Margin.Top - 80, 140, 0);
                }

                // Scroll Down.
                else if ((args.VirtualKey == VirtualKey.S || args.VirtualKey == VirtualKey.Down) && selectedCheat != CheatScreenOption.MaxScore)
                {
                    selectedCheat++;
                    selectCheatButton.Margin = new Thickness(0, selectCheatButton.Margin.Top + 80, 140, 0);
                }

                // Toggle Cheat.
                else if (args.VirtualKey == VirtualKey.Enter)
                {
                    ToggleCheat(selectedCheat);
                }
            }

            #endregion

            #region In Game.

            // In Game.
            else if (currentGameState == GameState.InGame)
            {
                // Quit Game.
                if (args.VirtualKey == VirtualKey.Escape)
                {
                    toggleGame(false);
                }

                // Pause.
                else if (args.VirtualKey == VirtualKey.Shift)
                {
                    togglePause(true);
                }

                // Choose block cheat.
                if (cheatBools[2] && !mainTetrisGame.CheatBlockMoving)
                {
                    if ((args.VirtualKey == VirtualKey.Left || args.VirtualKey == VirtualKey.A) && CheatTetrimino != Block.Tetrimino.Default)
                    {
                        CheatTetrimino--;
                    }

                    else if ((args.VirtualKey == VirtualKey.Right || args.VirtualKey == VirtualKey.D) && CheatTetrimino != Block.Tetrimino.T)
                    {
                        CheatTetrimino++;
                    }

                    else if (args.VirtualKey == VirtualKey.Enter)
                    {
                        mainTetrisGame.DropBlock(new Block(CheatTetrimino));
                    }
                }

                // Block Movement Left / Right.
                else if (args.VirtualKey == VirtualKey.Left || args.VirtualKey == VirtualKey.A)
                {
                    mainTetrisGame.MoveBlock(TetrisGame.Direction.Left);
                    Vibrate(150, .5, 0, 0, 0);
                }

                else if (args.VirtualKey == VirtualKey.Right || args.VirtualKey == VirtualKey.D)
                {
                    mainTetrisGame.MoveBlock(TetrisGame.Direction.Right);
                    Vibrate(150, 0, 0, .5, 0);
                }

                // Soft Drop.
                else if (args.VirtualKey == VirtualKey.Down || args.VirtualKey == VirtualKey.S)
                {
                    mainTetrisGame.SoftDrop(true);
                }

                // Rotation Left / Right.
                else if (args.VirtualKey == VirtualKey.Q || args.VirtualKey == VirtualKey.Up)
                {
                    mainTetrisGame.RotateBlock(TetrisGame.Direction.Left);
                    Vibrate(150, 0, .2, 0, 0);
                }

                else if (args.VirtualKey == VirtualKey.W)
                {
                    mainTetrisGame.RotateBlock(TetrisGame.Direction.Right);
                    Vibrate(150, 0, 0, 0, .2);
                }
            }

            #endregion
        }

        /// <summary>
        /// Handles Key released inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (currentGameState == GameState.InGame && (args.VirtualKey != VirtualKey.Down || args.VirtualKey != VirtualKey.S))
            {
                mainTetrisGame.SoftDrop(false);
            }
        }

        #endregion

        #endregion

        #region Core Game Methods

        /// <summary>
        /// Toggles a game.
        /// </summary>
        /// <param name="startGame"></param>
        private void toggleGame(bool Flag)
        {
            if(Flag)
            {
                // Swich from the title screen to the game screen. 
                titleScreen.Visibility = Visibility.Collapsed;
                gameScreen.Visibility = Visibility.Visible;
                currentGameState = GameState.InGame;

                // Create new TetrisGame.
                if (!cheatFlag)
                {
                    mainTetrisGame = new TetrisGame(squareSize);
                }

                // Create now TetrisGame with the cheats.
                else
                {
                    mainTetrisGame = new TetrisGame(squareSize, cheatBools[0], cheatBools[1], cheatBools[2], cheatBools[3]);
                }

                mainTetrisGame.StartNewGame();

                // Add Squares to the game grid.
                foreach (Image I in mainTetrisGame.FeildSquares)
                {
                    gameGrid.Children.Add(I);
                }
            }

            else
            {
                // Switch from the game screen to the title screen.
                titleScreen.Visibility = Visibility.Visible;
                gameScreen.Visibility = Visibility.Collapsed;
                currentGameState = GameState.TitleScreen;
                dropCheatBlock.Visibility = Visibility.Collapsed;

                // Remove the old grid.
                gameGrid.Children.Clear();
            }
        }

        /// <summary>
        /// Pauses or unpauses the game.
        /// </summary>
        /// <param name="shouldPause"></param>
        private void togglePause(bool Flag)
        {
            if (Flag)
            {
                currentGameState = GameState.Paused;
                pauseScreen.Visibility = Visibility.Visible;
                mainTetrisGame.IsPaused = true;
            }

            else
            {
                currentGameState = GameState.InGame;
                pauseScreen.Visibility = Visibility.Collapsed;
                mainTetrisGame.IsPaused = false;
            }
        }

        /// <summary>
        /// Open or Close cheats menu.
        /// </summary>
        private void toggleCheatMenu(bool Flag)
        {
            if (Flag)
            {
                selectCheatButton.Margin = new Thickness(0, 126, 140, 0);
                selectedCheat = 0;
                titleScreen.Visibility = Visibility.Collapsed;
                cheatsScreen.Visibility = Visibility.Visible;
                currentGameState = GameState.CheatScreen;
            }

            else
            {
                cheatsScreen.Visibility = Visibility.Collapsed;
                titleScreen.Visibility = Visibility.Visible;
                currentGameState = GameState.TitleScreen;
            }
        }

        /// <summary>
        /// Toggles the currently selected cheat.
        /// </summary>
        /// <param name="cheat"></param>
        private void ToggleCheat(CheatScreenOption cheat)
        {
            cheatBools[(int)cheat] = !cheatBools[(int)cheat];
            cheatFlag = true;

            switch ((int)cheat)
            {
                // Inf Spin.
                case 0:
                    cheat0.Text = cheatBools[0].ToString();
                    if (cheatBools[0])
                    {
                        cheat0.Foreground = new SolidColorBrush(Colors.Lime);
                    }

                    else
                    {
                        cheat0.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    break;

                // Ghost Block.
                case 1:
                    cheat1.Text = cheatBools[1].ToString();
                    if (cheatBools[1])
                    {
                        cheat1.Foreground = new SolidColorBrush(Colors.Lime);
                    }

                    else
                    {
                        cheat1.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    break;

                // Choose Block.
                case 2:
                    cheat2.Text = cheatBools[2].ToString();
                    if (cheatBools[2])
                    {
                        cheat2.Foreground = new SolidColorBrush(Colors.Lime);
                    }

                    else
                    {
                        cheat2.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    break;

                // Max Score.
                case 3:
                    cheat3.Text = cheatBools[3].ToString();
                    if (cheatBools[3])
                    {
                        cheat3.Foreground = new SolidColorBrush(Colors.Lime);
                    }

                    else
                    {
                        cheat3.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    break;

                // Break.
                default:
                    break;
            }
        }

        /// <summary>
        /// Toggles Sound.
        /// </summary>
        private void ToggleSound()
        {
            if (SoundPlayer.MediaPlayer.CurrentState == Windows.Media.Playback.MediaPlayerState.Playing)
            {
                SoundPlayer.MediaPlayer.Pause();
                soundToggle1.Source = new BitmapImage(new Uri("ms-appx:///Images/disabledSounds.png"));
                soundToggle2.Source = new BitmapImage(new Uri("ms-appx:///Images/disabledSounds.png"));
            }

            else
            {
                SoundPlayer.MediaPlayer.Position = System.TimeSpan.MinValue;
                SoundPlayer.MediaPlayer.Play();
                soundToggle1.Source = new BitmapImage(new Uri("ms-appx:///Images/enabledSounds.png"));
                soundToggle2.Source = new BitmapImage(new Uri("ms-appx:///Images/enabledSounds.png"));
            }
        }

        /// <summary>
        /// Used to vibrate the controller for a select ammount of time.
        /// </summary>
        /// <param name="vibrationMSTime"></param>
        /// <param name="vibrationLeftPower"></param>
        /// <param name="vibrationLeftTriggerPower"></param>
        /// <param name="vibrationRightPower"></param>
        /// <param name="vibrationRightTriggerPower"></param>
        private void Vibrate(int vibrationMSTime, double vibrationLeftPower, double vibrationLeftTriggerPower, double vibrationRightPower, double vibrationRightTriggerPower)
        {
            if (mainGamepad != null)
            {
                mainGamepad.Vibration = new GamepadVibration() { LeftMotor = vibrationLeftPower, LeftTrigger = vibrationLeftTriggerPower, RightMotor = vibrationRightPower, RightTrigger = vibrationRightTriggerPower };
                Thread.Sleep(vibrationMSTime);
                mainGamepad.Vibration = new GamepadVibration();
            }
        }

        #endregion

        #endregion
    }
}