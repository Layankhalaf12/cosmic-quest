using GameLastOne.BusinessLogic;

namespace GameLastOne;

/// <summary>
/// Presentation layer for Asteroid Dodge. All rules live in <see cref="AsteroidLogic"/>;
/// this page only reads the game state on every timer tick and draws it.
/// </summary>
public partial class AsteroidGame : ContentPage
{
    private const double PlayWidth = 360;
    private const double PlayHeight = 480;

    private readonly AsteroidLogic _game = new AsteroidLogic(PlayWidth, PlayHeight);
    private readonly List<Label> _asteroidLabels = new List<Label>();
    private readonly IDispatcherTimer _timer;
    private int _bestScore;

    public AsteroidGame()
    {
        InitializeComponent();

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(30);
        _timer.Tick += OnTimerTick;

        DrawRocket();
    }

    /// <summary>Stops the game when the player leaves the page.</summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer.Stop();
        _startButton.Text = "Start";
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        if (_timer.IsRunning)
        {
            _timer.Stop();
            _startButton.Text = "Resume";
            return;
        }

        if (_game.IsGameOver || _startButton.Text == "Start")
        {
            _game.Reset();
            ClearAsteroids();
            _resultFrame.IsVisible = false;
        }

        _startButton.Text = "Pause";
        _timer.Start();
    }

    private void OnLeftClicked(object? sender, EventArgs e)
    {
        _game.MoveLeft();
        DrawRocket();
    }

    private void OnRightClicked(object? sender, EventArgs e)
    {
        _game.MoveRight();
        DrawRocket();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _game.Tick();
        DrawAsteroids();
        _scoreLabel.Text = $"Score: {_game.Score}";

        if (_game.IsGameOver)
            EndGame();
    }

    private void EndGame()
    {
        _timer.Stop();
        _startButton.Text = "Start";

        if (_game.Score > _bestScore)
        {
            _bestScore = _game.Score;
            _bestLabel.Text = $"Best: {_bestScore}";
            _resultLabel.Text = $"💥 Hit! New best score: {_game.Score}";
        }
        else
        {
            _resultLabel.Text = $"💥 Hit! You dodged {_game.Score} asteroids.";
        }

        _resultFrame.IsVisible = true;
    }

    private void DrawRocket()
    {
        AbsoluteLayout.SetLayoutBounds(_rocket,
            new Rect(_game.RocketX, _game.RocketY, AsteroidLogic.RocketSize, AsteroidLogic.RocketSize));
    }

    /// <summary>
    /// Keeps one label per asteroid: adds labels when new asteroids appear,
    /// removes extras when asteroids leave, then moves every label into place.
    /// </summary>
    private void DrawAsteroids()
    {
        IReadOnlyList<Asteroid> rocks = _game.Asteroids;

        while (_asteroidLabels.Count < rocks.Count)
        {
            var label = new Label
            {
                Text = "🪨",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            _asteroidLabels.Add(label);
            _playArea.Children.Add(label);
        }

        while (_asteroidLabels.Count > rocks.Count)
        {
            Label extra = _asteroidLabels[^1];
            _asteroidLabels.RemoveAt(_asteroidLabels.Count - 1);
            _playArea.Children.Remove(extra);
        }

        for (int i = 0; i < rocks.Count; i++)
        {
            Asteroid rock = rocks[i];
            Label label = _asteroidLabels[i];
            label.FontSize = rock.Size * 0.8;
            AbsoluteLayout.SetLayoutBounds(label, new Rect(rock.X, rock.Y, rock.Size, rock.Size));
        }
    }

    private void ClearAsteroids()
    {
        foreach (Label label in _asteroidLabels)
            _playArea.Children.Remove(label);
        _asteroidLabels.Clear();
        DrawRocket();
    }
}
