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
using System.IO;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for SegmentedDisplay.xaml
    /// </summary>
    public partial class SegmentedDisplay : UserControl
    {
        private Grid grid;
        private Image[] display;
        private BitmapImage[] digits;
        private bool isNegative;
        private bool showZeroes;
        private int maxDigits;
        private int digitWidth;
        private int value;
        private int maxValue;
        private int minValue;

        public SegmentedDisplay()
        {
            grid = new Grid();
            this.AddChild(grid);

            InitializeComponent();

            LoadImages();
        }

        public void InitializeDisplay(int maxWidth, int maxDigits, bool showZeroes = true)
        {
            digitWidth = maxWidth / maxDigits;
            this.Width = digitWidth * maxDigits;
            this.maxDigits = maxDigits;
            this.showZeroes = showZeroes;
            this.isNegative = false;

            minValue = int.Parse("-" + new string('9', maxDigits - 1));
            maxValue = int.Parse(new string('9', maxDigits));

            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(digits[0].PixelHeight);
            grid.RowDefinitions.Add(rd);

            display = new Image[maxDigits];
            for (int i = 0; i < maxDigits; i++)
            {                
                display[i] = new Image();
                display[i].Source = showZeroes ? digits[0] : digits[11];

                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(digits[0].PixelWidth);
                grid.ColumnDefinitions.Add(cd);

                Grid.SetColumn(display[i], i);
                Grid.SetRow(display[i], 0);
                grid.Children.Add(display[i]);
            }
        }

        public void UpdateDisplay(int value)
        {
            if (value < minValue)
            {
                this.value = minValue;
            }
            else if (value > maxValue)
            {
                this.value = maxValue;
            }
            else
            {
                this.value = value;
            }

            string displayDigits = this.value.ToString();
            if (displayDigits[0] == '-')
            {
                isNegative = true;
                display[0].Source = digits[10];
            }

            for (int i = 0; i < maxDigits; i++)
            {
                display[i].Source = showZeroes ? digits[0] : digits[11];
            }

            int index = 0;
            while (maxDigits - index > displayDigits.Length) index++;

            for (int i = index; i < maxDigits; i++)
            {
                char digit = displayDigits[i - index];
                if (digit == '-')
                {
                    display[0].Source = digits[10];
                    continue;
                }
                display[i].Source = digits[int.Parse(digit.ToString())];
            }
        }

        private void LoadImages()
        {
            digits = new BitmapImage[12];
            string[] fileNames = Directory.GetFiles("images/Digit_Display");

            for (int i = 0; i < fileNames.Length; i++)
            {
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri($"./images/Digit_Display/{System.IO.Path.GetFileName(fileNames[i])}", UriKind.Relative);
                source.DecodePixelWidth = digitWidth;
                source.EndInit();

                digits[i] = source;
            }
        }
    }
}
