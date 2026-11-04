namespace GameLastOne.BusinessLogic
{
    /// <summary>One tile on the memory board.</summary>
    public class MemoryCard
    {
        public string Symbol { get; }
        public bool IsFaceUp { get; set; }
        public bool IsMatched { get; set; }

        public MemoryCard(string symbol)
        {
            Symbol = symbol;
        }
    }

    /// <summary>What happened after the player flipped a card.</summary>
    public enum FlipResult
    {
        Ignored,     // card was already face up or matched, or two cards are waiting to be hidden
        FirstCard,   // first card of a turn
        Match,       // second card matches the first
        NoMatch      // second card does not match; both must be turned back over
    }

    /// <summary>
    /// Rules for Planet Memory Match: six pairs of space symbols are shuffled onto a 4 x 3 board.
    /// The player flips two cards per turn and wins when every pair is found.
    /// </summary>
    public class MemoryLogic
    {
        private static readonly string[] Symbols = { "🪐", "🌍", "🌕", "☀️", "🌟", "☄️" };

        private readonly Random _random = new Random();
        private readonly List<MemoryCard> _cards = new List<MemoryCard>();
        private int? _firstIndex;
        private int? _secondIndex;

        public IReadOnlyList<MemoryCard> Cards => _cards;
        public int Moves { get; private set; }
        public int PairsFound { get; private set; }
        public int TotalPairs => Symbols.Length;
        public bool IsComplete => PairsFound == TotalPairs;

        /// <summary>True while two unmatched cards are showing and must be hidden first.</summary>
        public bool IsWaitingToHide => _secondIndex.HasValue;

        public MemoryLogic()
        {
            NewGame();
        }

        /// <summary>Builds the pairs and shuffles them with a Fisher-Yates shuffle.</summary>
        public void NewGame()
        {
            _cards.Clear();
            foreach (string symbol in Symbols)
            {
                _cards.Add(new MemoryCard(symbol));
                _cards.Add(new MemoryCard(symbol));
            }

            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }

            _firstIndex = null;
            _secondIndex = null;
            Moves = 0;
            PairsFound = 0;
        }

        public FlipResult Flip(int index)
        {
            if (index < 0 || index >= _cards.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            MemoryCard card = _cards[index];
            if (IsWaitingToHide || card.IsFaceUp || card.IsMatched)
                return FlipResult.Ignored;

            card.IsFaceUp = true;

            if (_firstIndex is null)
            {
                _firstIndex = index;
                return FlipResult.FirstCard;
            }

            Moves++;
            MemoryCard first = _cards[_firstIndex.Value];

            if (first.Symbol == card.Symbol)
            {
                first.IsMatched = true;
                card.IsMatched = true;
                PairsFound++;
                _firstIndex = null;
                return FlipResult.Match;
            }

            _secondIndex = index;
            return FlipResult.NoMatch;
        }

        /// <summary>Turns the two unmatched cards back over so the next turn can start.</summary>
        public void HideUnmatched()
        {
            if (_firstIndex.HasValue) _cards[_firstIndex.Value].IsFaceUp = false;
            if (_secondIndex.HasValue) _cards[_secondIndex.Value].IsFaceUp = false;
            _firstIndex = null;
            _secondIndex = null;
        }
    }
}
