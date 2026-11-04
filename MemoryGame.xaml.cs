using GameLastOne.BusinessLogic;

namespace GameLastOne;

/// <summary>
/// Presentation layer for Planet Memory Match. <see cref="MemoryLogic"/> decides what each
/// flip means; this page builds the board and redraws the buttons from the game state.
/// </summary>
public partial class MemoryGame : ContentPage
{
    private const int Columns = 4;
    private const string CardBack = "❔";

    private readonly MemoryLogic _game = new MemoryLogic();
    private readonly List<Button> _cardButtons = new List<Button>();

    public MemoryGame()
    {
        InitializeComponent();
        BuildBoard();
        Redraw();
    }

    /// <summary>Creates one button per card and places it in the 4 x 3 grid.</summary>
    private void BuildBoard()
    {
        for (int i = 0; i < _game.Cards.Count; i++)
        {
            var button = new Button
            {
                FontSize = 36,
                WidthRequest = 80,
                HeightRequest = 80,
                CornerRadius = 12,
                BackgroundColor = Color.FromArgb("#2C3E50"),
                TextColor = Colors.White
            };

            int index = i; // capture a copy for the click handler
            button.Clicked += async (sender, e) => await OnCardClicked(index);

            _board.Add(button, i % Columns, i / Columns);
            _cardButtons.Add(button);
        }
    }

    private async Task OnCardClicked(int index)
    {
        FlipResult result = _game.Flip(index);
        if (result == FlipResult.Ignored)
            return;

        Redraw();

        if (result == FlipResult.NoMatch)
        {
            // give the player a moment to see both cards before hiding them
            await Task.Delay(800);
            _game.HideUnmatched();
            Redraw();
        }
        else if (_game.IsComplete)
        {
            _resultLabel.Text = $"🎉 You found all {_game.TotalPairs} pairs in {_game.Moves} moves!";
            _resultFrame.IsVisible = true;
        }
    }

    /// <summary>Shows each card face up or face down and dims the matched ones.</summary>
    private void Redraw()
    {
        for (int i = 0; i < _cardButtons.Count; i++)
        {
            MemoryCard card = _game.Cards[i];
            Button button = _cardButtons[i];

            button.Text = card.IsFaceUp || card.IsMatched ? card.Symbol : CardBack;
            button.BackgroundColor = card.IsMatched ? Color.FromArgb("#27AE60") : Color.FromArgb("#2C3E50");
        }

        _movesLabel.Text = $"Moves: {_game.Moves}";
        _pairsLabel.Text = $"Pairs: {_game.PairsFound} / {_game.TotalPairs}";
    }

    private void OnNewGameClicked(object? sender, EventArgs e)
    {
        _game.NewGame();
        _resultFrame.IsVisible = false;
        Redraw();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}
