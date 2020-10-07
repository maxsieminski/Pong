using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PongGame
{
    /// <summary>
    /// Logika interakcji dla klasy GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        DispatcherTimer gameTimer = new DispatcherTimer(DispatcherPriority.Render);

        double? moveVertical, moveHorizontal;
        readonly static int playerSpeed = 4;
        readonly static int ballSpeed = 6;
        readonly static int maxScore = 10;
        bool ballStartMove = true;
        bool gameIsRunning = false;

        readonly Rectangle player1 = new Rectangle
        {
            Width = 10,
            Height = 50,
            Fill = System.Windows.Media.Brushes.White
        };
        readonly Rectangle player2 = new Rectangle
        {
            Width = 10,
            Height = 50,
            Fill = System.Windows.Media.Brushes.White
        };
        readonly Rectangle ball = new Rectangle
        {
            Width = 10,
            Height = 10,
            Fill = System.Windows.Media.Brushes.White
        };

        public GamePage()
        {
            InitializeComponent();
            InitializeGameElements();

            Thread.Sleep(1000);

            gameTimer.Tick += StartGame;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Start();
        }

        public static IEnumerable<Key> KeysDown()
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (key != Key.None && key != Key.LineFeed && key != Key.DeadCharProcessed && key != Key.System && Keyboard.IsKeyDown(key))
                    yield return key;
            }
        }

        // Puts pong and both players on screen
        // TODO : Make it adjustable to screen size
        private void InitializeGameElements()
        {
            gameCanvas.Children.Add(player1);
            gameCanvas.Children.Add(player2);
            gameCanvas.Children.Add(ball);

            Canvas.SetLeft(player1, 50 - player1.Width);
            Canvas.SetTop(player1, (450 - player1.Height) / 2);

            Canvas.SetLeft(player2, 750 - player2.Width);
            Canvas.SetTop(player2, (450 - player2.Height) / 2);

            Canvas.SetLeft(ball, (800 - ball.Width) / 2);
            Canvas.SetTop(ball, (450 - ball.Height) / 2);
        }

        private void PlayerMovement()
        {
            // Player 1
            if (Keyboard.IsKeyDown(Key.W) && Canvas.GetTop(player1) >= 0)
            {
                Canvas.SetTop(player1, Canvas.GetTop(player1) - playerSpeed);
            }
            else if (Keyboard.IsKeyDown(Key.S) && Canvas.GetTop(player1) + player1.Height <= 410)
            {
                Canvas.SetTop(player1, Canvas.GetTop(player1) + playerSpeed);
            }

            // Player 2
            if (Keyboard.IsKeyDown(Key.Up) && Canvas.GetTop(player2) >= 0)
            {
                Canvas.SetTop(player2, Canvas.GetTop(player2) - playerSpeed);
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Canvas.GetTop(player2) + player2.Height <= 410)
            {
                Canvas.SetTop(player2, Canvas.GetTop(player2) + playerSpeed);
            }
        }

        private void PlayerCollision()
        {
            if (Canvas.GetLeft(ball) < Canvas.GetLeft(player1) + player1.Width &&
                Canvas.GetLeft(ball) + ball.Width > Canvas.GetLeft(player1) &&
                Canvas.GetTop(ball) < Canvas.GetTop(player1) + player1.Height &&
                Canvas.GetTop(ball) + ball.Height > Canvas.GetTop(player1))
            {
                double relativeIntersectY = Canvas.GetTop(player1) + player1.Height / 2 - Canvas.GetTop(ball);
                double normalizedIntesectY = relativeIntersectY / (player1.Height / 2);
                double bounceAngle = normalizedIntesectY * (5 * Math.PI / 12);
                moveHorizontal = ballSpeed - Math.Sin(bounceAngle);
                moveVertical = ballSpeed * Math.Cos(bounceAngle);
            }
            else if (Canvas.GetLeft(ball) + ball.Width > Canvas.GetLeft(player2) &&
                     Canvas.GetLeft(ball) < Canvas.GetLeft(player2) + player2.Width &&
                     Canvas.GetTop(ball) < Canvas.GetTop(player2) + player2.Height &&
                     Canvas.GetTop(ball) + ball.Height > Canvas.GetTop(player2))
            {
                double relativeIntersectY = Canvas.GetTop(player2) + player2.Height / 2 - Canvas.GetTop(ball);
                double normalizedIntesectY = relativeIntersectY / (player2.Height / 2);
                double bounceAngle = normalizedIntesectY * (5 * Math.PI / 12);
                moveHorizontal = (ballSpeed - Math.Sin(bounceAngle)) * -1;
                moveVertical = ballSpeed * Math.Cos(bounceAngle);
            }
        }

        private void BallMovement()
        {
            Random random = new Random();

            // When the game starts or it's next turn
            if (ballStartMove)
            {
                // Put ball in the middle of the screen

                Canvas.SetLeft(ball, (800 - ball.Width) / 2);
                Canvas.SetTop(ball, (450 - ball.Height) / 2);

                // If it's the first turn in the game, generate random direction
                if (moveVertical == null)
                {
                    moveVertical = random.NextDouble() * (2.4 + 2.4) - 2.4;
                    moveHorizontal = ballSpeed - moveVertical;
                }
                // If the game has already begun, switch horizontal direction
                else
                {
                    moveVertical = random.NextDouble() * (2.4 + 2.4) - 2.4;
                    moveHorizontal *= -1;
                }

                ballStartMove = false;
            }
            else
            {
                // Wall collisions 
                if (Canvas.GetTop(ball) <= 0 || Canvas.GetTop(ball) >= gameCanvas.ActualHeight - ball.Height * 5)
                {
                    moveVertical *= -1;
                }
                else if (Canvas.GetLeft(ball) <= 0)
                {
                    player2Score.Content = int.Parse(player2Score.Content.ToString()) + 1;
                    ballStartMove = true;
                }
                else if (Canvas.GetLeft(ball) >= gameCanvas.ActualWidth)
                {
                    player1Score.Content = int.Parse(player1Score.Content.ToString()) + 1;
                    ballStartMove = true;
                }

                if (Canvas.GetLeft(ball) <= Canvas.GetLeft(player1) + player1.Width * 2 || Canvas.GetLeft(ball) >= Canvas.GetLeft(player2) - player2.Width * 2)
                {
                    PlayerCollision();
                }
            }

            Canvas.SetTop(ball, Canvas.GetTop(ball) + (double)moveVertical);
            Canvas.SetLeft(ball, Canvas.GetLeft(ball) + (double)moveHorizontal);
        }

        private void StartGame(object sender, EventArgs e)
        {
            if (!gameIsRunning)
            {
                gameCanvas.Focus();

                if (KeysDown().Any() && gameCanvas.Children.Contains(player1))
                {
                    gameIsRunning = true;
                }
                // If game has finished, reinitialize players and the ball
                else if (Keyboard.IsKeyDown(Key.Enter) && !gameCanvas.Children.Contains(player1) && !gameCanvas.Children.Contains(player2) && !gameCanvas.Children.Contains(ball))
                {
                    InitializeGameElements();
                    pressEnterLabel.Content = "";
                    playerWonLabel.Content = "";
                    gameOverLabel.Content = "";
                    gameIsRunning = true;
                }
                else if (Keyboard.IsKeyDown(Key.Escape))
                {
                    App.Current.MainWindow.Content = new MenuPage();
                }
            }

            else
            {
                gameCanvas.Focus();
                PlayerMovement();
                BallMovement();

                if (int.Parse(player1Score.Content.ToString()) == maxScore || int.Parse(player2Score.Content.ToString()) == maxScore)
                {

                    gameCanvas.Children.Remove(player1);
                    gameCanvas.Children.Remove(player2);
                    gameCanvas.Children.Remove(ball);

                    gameOverLabel.Content = "GAME OVER";

                    if (int.Parse(player1Score.Content.ToString()) == maxScore)
                    {
                        playerWonLabel.Content = "Left won!";
                    }
                    else if (int.Parse(player2Score.Content.ToString()) == maxScore)
                    {
                        playerWonLabel.Content = "Right won!";
                    }

                    pressEnterLabel.Content = "Press enter or escape!";

                    player1Score.Content = 0;
                    player2Score.Content = 0;

                    gameIsRunning = false;
                }
            }
        }
    }
}