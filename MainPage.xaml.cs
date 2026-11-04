using Microsoft.Maui.Layouts;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GameLastOne
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// we use a timer to animate the planets orbiting around the center of the screen.
        /// rand is used to generate a random number for the game selection in the OnGuessClicked method.
        /// </summary>
        private IDispatcherTimer _orbitTimer;
        private Random _rand = new Random();
        private string _currentCorrectAnswer = string.Empty;

        // Inner ring angles for  green and Colorful
        private double _angleGreen = 0;
        private double _angleColorful = 180;

        private double _angleBlue = 0;
        private double _angleYellow = 90;
        private double _angleOrange = 180;
        private double _angleRed = 300;

        // orbit radius inner and outer 
        private double _outerRadius = 170;   // outer ring radius
        private double _radius = 110; // inner ring  radius in pixels
        private bool _timerStarted = false;

        // jagged array for the quiz questions and answers
        private string[][] _planetQuiz =
{
    new string[] { "Which planet is known as the Red Planet?", "Mars", "Mars", "Jupiter", "Venus" },
    new string[] { "Which planet has rings?", "Saturn", "Saturn", "Earth", "Mars" },
    new string[] { "Which planet is closest to the Sun?", "Mercury", "Mercury", "Venus", "Earth" }
};


        public MainPage()
        {
            InitializeComponent();

            // Create and configure the orbit animation timer
            _orbitTimer = Dispatcher.CreateTimer();
            _orbitTimer.Interval = TimeSpan.FromMilliseconds(30);
            _orbitTimer.Tick += OnOrbitTimerTick;

            // Start timer only after OrbitLayout is measured
            _orbitLayout.SizeChanged += OrbitLayout_SizeChanged;

            // Wire up button click events
            _greenPlanet.Clicked += OnGuessClicked;
            _colorfulPlanet.Clicked += OnGuessClicked;
            _bluePlanet.Clicked += OnGuessClicked;

            //  Wire up button click events for outer planets
            _yellowPlanet.Clicked += OnGuessClicked;
            _orangePlanet.Clicked += OnGuessClicked;
            _redPlanet.Clicked += OnGuessClicked;

            // when the on Page is appearing, reset the quiz frame and buttons
            this.Appearing += OnPageAppearing;
        }

        private void OnPageAppearing(object? sender, EventArgs e)
        {
            // Hide quiz frame and reset when returning to this page
            _quizFrame.IsVisible = false;
            _questionsLabel.IsVisible = false;
            _gameChoiceLayout.IsVisible = false;
            _gameCardsGrid.IsVisible = false;
            _featureGame.IsVisible = false;
            _showUpMessage.IsVisible = false;

            // Hide answer buttons and clear their text
            _answerButton1.IsVisible = false;
            _answerButton2.IsVisible = false;
            _answerButton3.IsVisible = false;
            _answerButton1.Text = string.Empty;
            _answerButton2.Text = string.Empty;
            _answerButton3.Text = string.Empty;
        }

        /// <summary>
        ///  orbitLayout_SizeChanged is triggered when the OrbitLayout size changes.
        ///  also starts the orbitTimer when the layout is ready.
        ///  then it checks if the timer has already started to avoid multiple starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrbitLayout_SizeChanged(object? sender, EventArgs e)
        {
            if (!_timerStarted && _orbitLayout.Width > 0 && _orbitLayout.Height > 0)
            {
                _orbitTimer.Start();
                _timerStarted = true;
            }
        }

        /// <summary>
        /// onOrbitTimerTick is called on each tick of the orbitTimer.
        /// it updates the angles of the planets based on the speed from the SpeedSlider.
        /// also calls AnimateOrbit for each planet to update their positions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOrbitTimerTick(object? sender, EventArgs e)
        {
            try
            {
                //  Speed comes from the slider value
                double speed = _speedSlider.Value;
                Debug.Assert(speed >= 0, "Speed should be positive");

                //add to the animateOrbit method for each planet speed, angle , radius and size
                AnimateOrbit(_greenPlanet, ref _angleGreen, speed, _radius, 40);
                AnimateOrbit(_colorfulPlanet, ref _angleColorful, speed, _radius, 40);

                //  OUTER RING (4 planets)
                AnimateOrbit(_bluePlanet, ref _angleBlue, speed * 0.8, _outerRadius, 50);
                AnimateOrbit(_yellowPlanet, ref _angleYellow, speed * 0.8, _outerRadius, 50);
                AnimateOrbit(_orangePlanet, ref _angleOrange, speed * 0.8, _outerRadius, 50);
                AnimateOrbit(_redPlanet, ref _angleRed, speed * 0.8, _outerRadius, 50);
                // hello workd
            }

            catch (Exception)
            {
                _orbitTimer?.Stop();
                throw new InvalidOperationException("Unexpected error during animation");
            }
        }

        /// <summary>
        /// This method animates the orbit of a planet.
        /// Most of the logic is based on the angle, speed and orbit radius.
        /// Calculates the new position of the planet based on the angle and speed.
        /// Calculate the radians from the angle to use trigonometric functions.
        /// We use Math.Cos and Math.Sin to calculate the new position based on the angle.
        /// And find the offset from the center of the layout.
        /// Adjusts the position of the planet using AbsoluteLayout.SetLayoutBounds.
        /// <param name="planet">Representing the orbiting object.</param>
        /// <param name="angle">A reference to the current angle of the orbit.</param>
        /// <param name="speed">The speed of the orbit, in degrees per frame. Must be a positive value.</param>
        /// <param name="orbitRadius">The radius of the orbit, relative to the layout dimensions.</param>
        private void AnimateOrbit(View planet, ref double angle, double speed, double orbitRadius, double planetSize)
        {


            //  Make sure we have a valid radius
            Debug.Assert(orbitRadius > 0, "Orbit radius should be positive");

            //  Increase the angle by the speed (like moving a clock hand)
            angle += speed;
            // if angle exceeds 360 degrees, reset it to 0 
            if (angle >= 360)
                angle = 0;

            //  Convert angle to radians  to use Math.Cos and Math.Sin
            double radians = angle * Math.PI / 180;

            //  Center of the layout is (0.5, 0.5) when using PositionProportional
            const double centerX = 0.5;
            const double centerY = 0.5;

            //  How far the planet should move from the center
            double offsetX = Math.Cos(radians) * orbitRadius / _orbitLayout.Width;
            double offsetY = Math.Sin(radians) * orbitRadius / _orbitLayout.Height;

            //  Final position = center + offset
            double x = centerX + offsetX;
            double y = centerY + offsetY;

            //  Place the planet in the layout using AbsoluteLayout
            AbsoluteLayout.SetLayoutBounds(planet, new Rect(x, y, planetSize, planetSize));
            AbsoluteLayout.SetLayoutFlags(planet, AbsoluteLayoutFlags.PositionProportional);
        }

        /// <summary>
        /// This method is called when the Tic Tac Toe button is clicked. 
        /// async because it navigates to another page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnXOGameButtonClicked(object? sender, EventArgs e)
        {
            try
            {

                // navigate to XoGame page 
                await Shell.Current.GoToAsync("//XoGame");
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Unable to navigate to Tic Tac Toe game");
            }

        }
        /// <summary>
        /// this method is called when the Asteroid Dodge button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAsteroidGameButtonClicked(object? sender, EventArgs e)
        {
            Debug.Assert(sender != null, "Sender should not be null");

            // navigate to AsteroidGame page
            await Shell.Current.GoToAsync("//AsteroidGame");

        }

        private async void OnMemoryGameButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                // navigate to MemoryGame page
                await Shell.Current.GoToAsync("//MemoryGame");
            }

            catch (Exception)
            {
                await DisplayAlert("Error", "An unexpected error occurred while loading Memory Match.", "OK");
            }
        }



        private void OnGuessClicked(object? sender, EventArgs e)
        {
            // check if the quiz data is not empty
            Debug.Assert(_planetQuiz.Length > 0, "Quiz data should not be empty");

            try
            {
                //show the message that the quiz is starting
                _showUpMessage.IsVisible = true;

                // Show the quiz frame and question label
                _quizFrame.IsVisible = true;
                _questionsLabel.IsVisible = true;

                // Hide the permanent game choice buttons (only show if correct later)
                _gameChoiceLayout.IsVisible = false;

                // Pick a random  question from the quiz
                var quizItem = _planetQuiz[_rand.Next(_planetQuiz.Length)];

                // add the question to the label that store in the array on the first index
                _questionsLabel.Text = quizItem[0];

                // add correct answer  from the array on the second index
                _currentCorrectAnswer = quizItem[1];

                //  Set up the predefined answer buttons
                _answerButton1.Text = quizItem[2];
                _answerButton2.Text = quizItem[3];
                _answerButton3.Text = quizItem[4];

                // Show the answer buttons
                _answerButton1.IsVisible = true;
                _answerButton2.IsVisible = true;
                _answerButton3.IsVisible = true;
            }

            catch (Exception)
            {
                throw new InvalidOperationException("An unexpected error occurred while loading the quiz");
            }
        }
        /// <summary>
        /// this method is called when the user clicks one of the answer buttons.
        /// also checks if the answer is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private async void OnOptionButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                // Check if the sender is a Button and have an answer .
                if (sender is Button btn)
                {
                    //store the answer of the clicked button as the chosen answer
                    string chosenAnswer = btn.Text;

                    // If the user clicked the correct answer
                    if (chosenAnswer == _currentCorrectAnswer)
                    {
                        // Hide the question and answer buttons
                        _questionsLabel.IsVisible = false;
                        _answerButton1.IsVisible = false;
                        _answerButton2.IsVisible = false;
                        _answerButton3.IsVisible = false;
                        _showUpMessage.IsVisible = false;

                        // Show the game choice buttons
                        _gameChoiceLayout.IsVisible = true;
                        _gameCardsGrid.IsVisible = true;
                        _featureGame.IsVisible = true;
                    }
                    else
                    {
                        // If the answer is incorrect, show an alert and redirect to a random game
                        await DisplayAlert(" Wrong!", $"{chosenAnswer} is NOT correct! You'll get a random game.", "OK");
                        // Hide the quiz frame and question label
                        _gameChoiceLayout.IsVisible = false;
                        _quizFrame.IsVisible = false;

                        //  Send them to a RANDOM GAME
                        int choice = _rand.Next(1, 4);
                        switch (choice)
                        {
                            case 1:
                                await Shell.Current.GoToAsync("//XoGame");
                                break;
                            case 2:
                                await Shell.Current.GoToAsync("//AsteroidGame");
                                break;
                            case 3:
                                await Shell.Current.GoToAsync("//MemoryGame");
                                break;
                        }
                    }
                }
            }

            catch (Exception)
            {
                throw new InvalidOperationException(" Error!! An unexpected error occurred while processing your answer");
            }
        }
    }
}






















