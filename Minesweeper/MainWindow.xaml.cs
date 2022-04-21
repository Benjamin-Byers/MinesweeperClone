using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            InitializeComponent();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!MinesweeperGame.IsFocused) MinesweeperGame.Focus();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selection = (MenuItem) sender;

            bool SetDifficulty(GameLevel level)
            {
                Difficulty0.IsChecked = false;
                Difficulty1.IsChecked = false;
                Difficulty2.IsChecked = false;
                Difficulty3.IsChecked = false;
                ChangeLevel(level);
                return true;
            }

            switch (selection.Header)
            {
                case "Beginner":
                    Difficulty0.IsChecked = SetDifficulty(GameLevel.Beginner);
                    break;
                case "Intermediate":
                    Difficulty1.IsChecked = SetDifficulty(GameLevel.Intermediate);
                    break;
                case "Advanced":
                    Difficulty2.IsChecked = SetDifficulty(GameLevel.Advanced);
                    break;
                case "Custom":
                    Difficulty3.IsChecked = SetDifficulty(GameLevel.Custom);
                    break;
                case "New":
                    MinesweeperGame.NewGame();
                    break;
                case "Marks (?)":
                    MinesweeperGame.ToggleQuestionMarks();
                    break;
                default:
                    break;
            }
        }

        private void ChangeLevel(GameLevel level)
        {
            MinesweeperGame.ChangeLevel(level);
            MinesweeperGame.NewGame();
        }
    }
}
