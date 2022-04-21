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
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for GameGrid.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public int borderMargin;
        public int tileLength;
        private Grid grid;
        private Rectangle tileHighlight;
        private Image[][] gameGridTiles;
        private CroppedBitmap[] tiles;
        private BitmapImage[] borders;

        public GameView()
        {
            this.Focusable = true;
            this.Background = Brushes.Gray;

            MouseMove += GameGrid_MouseMove;
            MouseEnter += (object sender, MouseEventArgs e) => tileHighlight.Visibility = Visibility.Visible;
            MouseLeave += (object sender, MouseEventArgs e) => tileHighlight.Visibility = Visibility.Hidden;

            grid = new Grid();
            this.AddChild(grid);

            tiles = new CroppedBitmap[15];

            LoadImages();

            InitializeComponent();
        }

        private void GameGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseCoords = e.GetPosition(this);
            if (mouseCoords.X > this.Width - borderMargin || 
                mouseCoords.Y > this.Height - borderMargin ||
                mouseCoords.X < borderMargin ||
                mouseCoords.Y < borderMargin)
            {
                tileHighlight.Visibility = Visibility.Hidden;
                return;
            }

            tileHighlight.Visibility = Visibility.Visible;

            int highlightX = (int)((mouseCoords.X - borderMargin) / tileLength) + 1;
            int highlightY = (int)((mouseCoords.Y - borderMargin) / tileLength) + 1;

            Grid.SetColumn(tileHighlight, highlightX);
            Grid.SetRow(tileHighlight, highlightY);
        }

        public void UpdateGrid(TileState[][] tileStates)
        {
            for (int i = 0; i < tileStates.Length; i++)
            {
                for (int j = 0; j < tileStates[i].Length; j++)
                {
                    gameGridTiles[i + 1][j + 1].Source = tiles[(int)tileStates[i][j]];

                    if (gameGridTiles[i + 1][j + 1].Source != tiles[0])
                    {
                        Grid.SetZIndex(gameGridTiles[i + 1][j + 1], 255);
                    }
                    else
                    {
                        Grid.SetZIndex(gameGridTiles[i + 1][j + 1], -1);
                    }
                }
            }
        }

        public void InitializeTiles(int maxWidth, int maxHeight, int tilesX, int tilesY)
        {
            tileLength = (maxWidth - borderMargin * 2) / tilesX;
            this.Width = tilesX * tileLength + borderMargin * 2;
            this.Height = tilesY * tileLength + borderMargin * 2;

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            //Load Images
            BitmapImage tilesImage = new BitmapImage();
            tilesImage.BeginInit();
            tilesImage.UriSource = new Uri("images/TestImages.bmp", UriKind.Relative);
            tilesImage.DecodePixelHeight = tileLength;
            tilesImage.DecodePixelWidth = tileLength * 15;
            tilesImage.EndInit();

            for (int i = 0; i < 15; i++)
            {
                CroppedBitmap croppedTile = new CroppedBitmap(tilesImage, new Int32Rect(tileLength * i, 0, tileLength, tileLength));
                tiles[i] = croppedTile;
            }

            tileHighlight = new Rectangle();
            tileHighlight.Width = tileLength;
            tileHighlight.Height = tileLength;
            tileHighlight.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
            tileHighlight.Visibility = Visibility.Hidden;
            Grid.SetColumn(tileHighlight, 1);
            Grid.SetRow(tileHighlight, 1);

            Grid.SetZIndex(tileHighlight, 16);
            grid.Children.Add(tileHighlight);

            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(borderMargin);
            grid.ColumnDefinitions.Add(cd);

            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(borderMargin);
            grid.RowDefinitions.Add(rd);

            for (int i = 0; i < tilesX; i++)
            {
                cd = new ColumnDefinition();
                cd.Width = new GridLength(tileLength);
                grid.ColumnDefinitions.Add(cd);
            }

            for (int j = 0; j < tilesY; j++)
            {
                rd = new RowDefinition();
                rd.Height = new GridLength(tileLength);
                grid.RowDefinitions.Add(rd);
            }

            cd = new ColumnDefinition();
            cd.Width = new GridLength(borderMargin);
            grid.ColumnDefinitions.Add(cd);

            rd = new RowDefinition();
            rd.Height = new GridLength(borderMargin);
            grid.RowDefinitions.Add(rd);

            gameGridTiles = new Image[tilesX + 2][];

            for (int i = 1; i < tilesX + 1; i++)
            {
                gameGridTiles[i] = new Image[tilesY + 2];

                for (int j = 1; j < tilesY + 1; j++)
                {
                    gameGridTiles[i][j] = SetGridTile(tiles[0], i, j);
                }
            }

            for (int i = 1; i < gameGridTiles.Length - 1; i++)
            {
                gameGridTiles[i][0] = SetGridTile(borders[1], i, 0);
                gameGridTiles[i][tilesY + 1] = SetGridTile(borders[1], i, tilesY + 1);
            }

            gameGridTiles[0] = new Image[tilesY + 2];
            gameGridTiles[tilesX + 1] = new Image[tilesY + 2];

            for (int i = 1; i < gameGridTiles[0].Length - 1; i++)
            {
                gameGridTiles[0][i] = SetGridTile(borders[0], 0, i);
                gameGridTiles[tilesX + 1][i] = SetGridTile(borders[0], tilesX + 1, i);
            }

            gameGridTiles[0][0] = SetGridTile(borders[2], 0, 0);
            gameGridTiles[tilesX + 1][0] = SetGridTile(borders[3], tilesX + 1, 0);
            gameGridTiles[0][tilesY + 1] = SetGridTile(borders[6], 0, tilesY + 1);
            gameGridTiles[tilesX + 1][tilesY + 1] = SetGridTile(borders[7], tilesX + 1, tilesY + 1);

        }

        private Image SetGridTile(ImageSource source, int x, int y)
        {
            Image image = new Image();
            image.Source = source;
            image.Stretch = Stretch.Fill;
            Grid.SetColumn(image, x);
            Grid.SetRow(image, y);
            grid.Children.Add(image);
            return image;
        }

        private void LoadImages()
        {
            borders = new BitmapImage[8];
            string[] fileNames = Directory.GetFiles("images/Border");

            BitmapImage horzImage = new BitmapImage();
            horzImage.BeginInit();
            horzImage.UriSource = new Uri($"./images/Border/{System.IO.Path.GetFileName(fileNames[0])}", UriKind.Relative);
            horzImage.DecodePixelWidth = tileLength;
            horzImage.EndInit();

            borderMargin = horzImage.PixelHeight;
            borders[0] = horzImage;

            BitmapImage vertImage = new BitmapImage();
            vertImage.BeginInit();
            vertImage.UriSource = new Uri($"./images/Border/{System.IO.Path.GetFileName(fileNames[1])}", UriKind.Relative);
            vertImage.DecodePixelHeight = tileLength;
            vertImage.EndInit();

            borders[1] = vertImage;

            for (int i = 2; i < fileNames.Length; i++)
            {
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri($"./images/Border/{System.IO.Path.GetFileName(fileNames[i])}", UriKind.Relative);
                source.DecodePixelWidth = vertImage.PixelWidth;
                source.EndInit();

                borders[i] = source;
            }
        }
    }
}
