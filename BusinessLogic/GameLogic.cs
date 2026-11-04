using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLastOne.BusinessLogic
{
  public  class GameLogic
    {
        private readonly List<ImageButton> board;
        private readonly Label turnLabel;
        private readonly Label scoreXLabel;
        private readonly Label scoreOLabel;
        private int scoreX;
        private int scoreO;


       
        public GameLogic(List<ImageButton> board, Label turnLabel, Label scoreXLabel, Label scoreOLabel)
        {
            this.board = board;
            this.turnLabel = turnLabel;
            this.scoreXLabel = scoreXLabel;
            this.scoreOLabel = scoreOLabel;

            InitializeScores();
        }
       
        private void InitializeScores()
        {
            scoreX = 0;
            scoreO = 0;
            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            scoreXLabel.Text = $"X: {scoreX}";
            scoreOLabel.Text = $"O: {scoreO}";
        }
        

        public void ResetBoard()
        {
            // Clear all cells and enable them
            foreach (ImageButton btn in board)
            {
                btn.Source = null;
                btn.IsEnabled = true;
            }

            // Reset turn indicator
            turnLabel.Text = "Player X's Turn";
        }

        public void DisableBoard()
        {
            foreach (var btn in board)
            {
                btn.IsEnabled = false;
            }
        }
      
        public void ResetScores()
        {
            scoreX = 0;
            scoreO = 0;
            UpdateScoreDisplay();
            ResetBoard();
        }

        public void UpdateScore(string player)
        {
            if (player == "X")
            {
                scoreX++;
            }
            else
            {
                scoreO++;
            }

            UpdateScoreDisplay();
        }
      
        public bool CheckWinner(string player)
        {
            // Check all possible winning combinations
            return CheckRows() || CheckColumns() || CheckDiagonals();
        }

        private bool CheckRows()
        {
            // Check each row for a win
            for (int row = 0; row < 3; row++)
            {
                int startIndex = row * 3;
                if (IsSameSymbol(board[startIndex], board[startIndex + 1], board[startIndex + 2]))
                    return true;
            }
            return false;
        }

        private bool CheckColumns()
        {
            // Check each column for a win
            for (int col = 0; col < 3; col++)
            {
                if (IsSameSymbol(board[col], board[col + 3], board[col + 6]))
                    return true;
            }
            return false;
        }

        private bool CheckDiagonals()
        {
            // Check both diagonals for a win
            return IsSameSymbol(board[0], board[4], board[8]) ||  // Top-left to bottom-right
                   IsSameSymbol(board[2], board[4], board[6]);     // Top-right to bottom-left
        }

        private bool IsSameSymbol(ImageButton a, ImageButton b, ImageButton c)
        {
            if (a.Source is FileImageSource sa &&
                b.Source is FileImageSource sb &&
                c.Source is FileImageSource sc)
            {
                // Check if all three have the same symbol (either x.png or o.jpg)
                string fileA = sa.File.ToLower();
                string fileB = sb.File.ToLower();
                string fileC = sc.File.ToLower();

                // Check if all are X's or all are O's
                bool allX = fileA.Contains("x") && fileB.Contains("x") && fileC.Contains("x");
                bool allO = fileA.Contains("o") && fileB.Contains("o") && fileC.Contains("o");

                return allX || allO;
            }
            return false;
        }
      
        public bool IsDraw()
        {
            // Check if all cells are filled
            foreach (var btn in board)
            {
                if (btn.Source == null)
                {
                    return false; // If any cell is empty, it's not a draw
                }
            }
            return true; // All cells are filled, and no winner, it's a draw
        }
        
    }
}

