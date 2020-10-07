using System.Windows;
using System.Windows.Controls;

namespace PongGame
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();

            Main.Content = new MenuPage();
        }
    }
}