using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PongGame
{
    /// <summary>
    /// Logika interakcji dla klasy MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private static DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);

        public MenuPage()
        {
            InitializeComponent();
            dispatcherTimer.Tick += InitializeMenu;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(150);
            dispatcherTimer.Start();
        }

        private void InitializeMenu(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down))
            {
                if (startLabel.Foreground == Brushes.Red)
                {
                    startLabel.Foreground = Brushes.White;
                    exitLabel.Foreground = Brushes.Red;
                }
                else
                {
                    startLabel.Foreground = Brushes.Red;
                    exitLabel.Foreground = Brushes.White;
                }
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                if (exitLabel.Foreground == Brushes.Red)
                {
                    Application.Current.Shutdown();
                }
                else if (startLabel.Foreground == Brushes.Red)
                {
                    App.Current.MainWindow.Content = new GamePage();
                }
            }
        }
    }
}
