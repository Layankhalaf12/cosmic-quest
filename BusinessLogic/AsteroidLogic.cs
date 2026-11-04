namespace GameLastOne.BusinessLogic
{
    /// <summary>
    /// One falling asteroid. Position and size are in device-independent units
    /// relative to the top-left corner of the play area.
    /// </summary>
    public class Asteroid
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public double Speed { get; set; }
    }

    /// <summary>
    /// Game rules for Asteroid Dodge, kept separate from the UI so the page only draws.
    /// The rocket moves left and right along the bottom; asteroids fall from the top.
    /// Each asteroid that passes the rocket scores a point; touching one ends the game.
    /// </summary>
    public class AsteroidLogic
    {
        private readonly Random _random = new Random();
        private readonly List<Asteroid> _asteroids = new List<Asteroid>();

        public const double RocketSize = 44;
        private const double MoveStep = 30;
        private const double SpawnChance = 0.04;   // chance of a new asteroid each tick
        private const double MaxSpawnChance = 0.12;

        public double Width { get; }
        public double Height { get; }
        public double RocketX { get; private set; }
        public double RocketY => Height - RocketSize - 10;
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        /// <summary>Read-only view of the asteroids currently on screen.</summary>
        public IReadOnlyList<Asteroid> Asteroids => _asteroids;

        public AsteroidLogic(double width, double height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Play area must have a positive size.");

            Width = width;
            Height = height;
            Reset();
        }

        /// <summary>Puts the rocket back in the middle and clears the sky.</summary>
        public void Reset()
        {
            _asteroids.Clear();
            RocketX = (Width - RocketSize) / 2;
            Score = 0;
            IsGameOver = false;
        }

        public void MoveLeft() => RocketX = Math.Max(0, RocketX - MoveStep);

        public void MoveRight() => RocketX = Math.Min(Width - RocketSize, RocketX + MoveStep);

        /// <summary>
        /// Advances the game by one frame: moves every asteroid down, spawns new ones,
        /// removes the ones that left the screen (scoring a point each) and checks for a hit.
        /// </summary>
        public void Tick()
        {
            if (IsGameOver)
                return;

            // the game gets busier as the score goes up
            double chance = Math.Min(MaxSpawnChance, SpawnChance + Score * 0.002);
            if (_random.NextDouble() < chance)
                Spawn();

            // loop backwards so removing an item does not skip the next one
            for (int i = _asteroids.Count - 1; i >= 0; i--)
            {
                Asteroid rock = _asteroids[i];
                rock.Y += rock.Speed;

                if (Collides(rock))
                {
                    IsGameOver = true;
                    return;
                }

                if (rock.Y > Height)
                {
                    _asteroids.RemoveAt(i);
                    Score++;
                }
            }
        }

        private void Spawn()
        {
            double size = _random.Next(28, 48);
            _asteroids.Add(new Asteroid
            {
                Size = size,
                X = _random.NextDouble() * (Width - size),
                Y = -size,
                Speed = 3 + _random.NextDouble() * 3 + Score * 0.05
            });
        }

        /// <summary>Circle-to-circle check using the centres of the rocket and the asteroid.</summary>
        private bool Collides(Asteroid rock)
        {
            double rocketCentreX = RocketX + RocketSize / 2;
            double rocketCentreY = RocketY + RocketSize / 2;
            double rockCentreX = rock.X + rock.Size / 2;
            double rockCentreY = rock.Y + rock.Size / 2;

            double dx = rocketCentreX - rockCentreX;
            double dy = rocketCentreY - rockCentreY;
            double hitDistance = (RocketSize + rock.Size) / 2 * 0.8; // a little forgiving

            return dx * dx + dy * dy < hitDistance * hitDistance;
        }
    }
}
