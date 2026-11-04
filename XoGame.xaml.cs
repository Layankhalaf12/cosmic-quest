using GameLastOne.BusinessLogic;

namespace GameLastOne;

public partial class XoGame : ContentPage
{// add list of ImageButtons for the game board
    private List<ImageButton> _board = new List<ImageButton>();
    //called GameLogic to handle game logic
    private GameLogic? _game;
    private string _currentPlayer = "X";
    private string _playerName = "Player";
    private string _playerSymbol = "X";
    private string _computerSymbol = "O";
    private Random _random = new Random();


    // ? Timer for delayed computer move
    private IDispatcherTimer? _computerMoveTimer;
    /// <summary>
    /// Initializes the XoGame page, sets up the game board and timer.
    /// </summary>
    // hope its working
        public XoGame()
	{
		InitializeComponent();
        InitializeGame();
        // Initialize timer properly
        _computerMoveTimer = Dispatcher.CreateTimer();
        _computerMoveTimer.Interval = TimeSpan.FromSeconds(2);
        _computerMoveTimer.Tick += OnComputerMoveTimerTick;


    }
   

    //  Start Game Button clicked
    private async void OnStartGameClicked(object sender, EventArgs e)
    {
        // Read player name
        string enteredName = _playerNameEntry.Text;
        // Validate player name
        if (string.IsNullOrWhiteSpace(enteredName))
        {
            // Default to "Player" if no name entered
            _playerName = "Player";
        }
        else
        { // Set player name to entered value
            _playerName = enteredName;
        }

        // Read player symbol choice
        if (_chooseXRadio.IsChecked)
        {
            _playerSymbol = "X";
            _computerSymbol = "O";
        }
        else
        {
            _playerSymbol = "O";
            _computerSymbol = "X";
        }

        // Show welcome message
        await DisplayAlert("Welcome", "Welcome, " + _playerName + "! You will play as " + _playerSymbol + ".", "OK");

        // Reset board for new game
        ResetBoard();

        // If player chose O, computer starts first
        if (_playerSymbol == "O")
        {
            ComputerMove();
        }
    }

    // ? Initialize the game board
    private void InitializeGame()
    {
        _board = new List<ImageButton>
        {
            Cell00, Cell01, Cell02,
            Cell10, Cell11, Cell12,
            Cell20, Cell21, Cell22
        };

        _game = new GameLogic(_board, TurnLabel, ScoreXLabel, ScoreOLabel);
        ResetBoard();
    }

    // ? Reset board and stop any pending timers
    private void ResetBoard()
    {
        _computerMoveTimer?.Stop();

        foreach (ImageButton button in _board)
        {
            button.Source = null;
            button.IsEnabled = true;
        }

        _currentPlayer = "X";

        if (_playerSymbol == "X")
        {
            TurnLabel.Text = _playerName + " (X) Turn";
        }
        else
        {
            TurnLabel.Text = "Computer (X) Turn";
        }

        ResultFrame.IsVisible = false;
    }

    // ? Switch turn between X and O
    private void SwitchPlayer()
    {
        if (_currentPlayer == "X")
        {
            _currentPlayer = "O";

            if (_currentPlayer == _playerSymbol)
            {
                TurnLabel.Text = _playerName + " (O) Turn";
            }
            else
            {
                TurnLabel.Text = "Computer (O) Turn";
            }
        }
        else
        {
            _currentPlayer = "X";

            if (_currentPlayer == _playerSymbol)
            {
                TurnLabel.Text = _playerName + " (X) Turn";
            }
            else
            {
                TurnLabel.Text = "Computer (X) Turn";
            }
        }
    }

    // ? Set the correct image for the clicked cell
    private void SetCellImage(ImageButton button)
    {
        if (_currentPlayer == "X")
        {
            button.Source = "x.png";
        }
        else
        {
            button.Source = "o.jpg";
        }
    }

    // ? Display the game result
    private void HandleGameEnd(string message)
    {
        _computerMoveTimer?.Stop(); // Stop timer if game is over
        ResultLabel.Text = message;
        ResultFrame.IsVisible = true;
    }

    // ? When player clicks a cell
    private void OnImageButtonClicked(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;

        // Don�t allow clicking already filled cells
        if (button.Source != null)
        {
            return;
        }

        // Place image (X or O)
        SetCellImage(button);

        // Check if the move won the game
        if (_game.CheckWinner(_currentPlayer))
        {
            string winnerName = (_currentPlayer == _playerSymbol) ? _playerName : "Computer";
            HandleGameEnd(winnerName + " Wins!");
            _game.UpdateScore(_currentPlayer);
            _game.DisableBoard();
            return;
        }

        // Check if draw
        if (_game.IsDraw())
        {
            HandleGameEnd("It's a Draw!");
            return;
        }

        // Switch turn
        SwitchPlayer();

        // ? If it�s now computer�s turn, start the 10-second delay
        if (_currentPlayer == _computerSymbol)
        {
            _computerMoveTimer.Stop();
            _computerMoveTimer.Start();
        }
    }

    // ? Computer Move Timer Tick � triggers after 10 seconds
    private void OnComputerMoveTimerTick(object sender, EventArgs e)
    {
        _computerMoveTimer.Stop(); // Only trigger once
        ComputerMove();
    }

    // ? When "Play Again" is clicked
    private void OnPlayAgainClicked(object sender, EventArgs e)
    {
        ResetBoard();

        if (_playerSymbol == "O")
        {
            ComputerMove();
        }
    }

    // ? When "Reset Scores" is clicked
    private void OnResetClicked(object sender, EventArgs e)
    {
        _game.ResetScores();
        ResetBoard();
    }

    // ? Computer makes a move
    private void ComputerMove()
    {
        List<ImageButton> emptyCells = new List<ImageButton>();

        // Find all empty cells
        foreach (ImageButton cell in _board)
        {
            if (cell.Source == null)
            {
                emptyCells.Add(cell);
            }
        }

        // If no empty cells, stop
        if (emptyCells.Count == 0)
        {
            return;
        }

        // Choose a random empty cell
        int index = _random.Next(emptyCells.Count);
        ImageButton chosenCell = emptyCells[index];

        // Place O or X for computer
        SetCellImage(chosenCell);

        // Check if computer won
        if (_game.CheckWinner(_currentPlayer))
        {
            HandleGameEnd("Computer Wins!");
            _game.UpdateScore(_currentPlayer);
            _game.DisableBoard();
            return;
        }

        // Check if draw
        if (_game.IsDraw())
        {
            HandleGameEnd("It's a Draw!");
            return;
        }

        // Switch back to player
        SwitchPlayer();
    }
}