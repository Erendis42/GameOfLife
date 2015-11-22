using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace KC12VU_CGoL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: config file
        static int rows;
        static int cols;

        static int width = 800;
        static int height = 600;
        static int padX = 18;
        static int padY = 81;
        static int size = 10;
        static int thickness = 1;
        static int lifespan = 100;
        static int sleep = 100;

        static string strLifespan = "Lifespan:";
        static string strSleep = "Sleep (ms):";
        static string strStart = "Start";
        static string strPause = "Pause";
        static string strResume = "Resume";
        static string strStop = "Stop";
        static string strClear = "Clear";

        static Cell[,] cells;

        static BrushConverter bc = new BrushConverter();

        static Brush border = (Brush)bc.ConvertFrom("#111111");
        static Brush alive = Brushes.Lime;
        static Brush dead = Brushes.Black;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAppearance();
        }

        private void InitializeConfiguration()
        {
            ConfigWindow cw = new ConfigWindow(width, height, size, thickness, lifespan, sleep, alive, dead, border);
            if (cw.ShowDialog() == true)
            {
                if (cw.tbWidth.Text != string.Empty)
                {
                    width = int.Parse(cw.tbWidth.Text);
                }

                if (cw.tbHeight.Text != string.Empty)
                {
                    height = int.Parse(cw.tbHeight.Text);
                }

                if (cw.tbThickness.Text != string.Empty)
                {
                    thickness = int.Parse(cw.tbThickness.Text);
                }

                if (cw.tbLifespan.Text != string.Empty)
                {
                    lifespan = int.Parse(cw.tbLifespan.Text);
                }

                if (cw.tbSleep.Text != string.Empty)
                {
                    sleep = int.Parse(cw.tbSleep.Text);
                }

                if (cw.tbAlive.Text != string.Empty)
                {
                    alive = (Brush)bc.ConvertFrom("#" + cw.tbAlive.Text);
                }

                if (cw.tbDead.Text != string.Empty)
                {
                    dead = (Brush)bc.ConvertFrom("#" + cw.tbDead.Text);
                }

                if (cw.tbBorder.Text != string.Empty)
                {
                    border = (Brush)bc.ConvertFrom("#" + cw.tbBorder.Text);
                }
            }
        }

        private void InitializeAppearance()
        {
            InitializeConfiguration();

            Width = width+padX;
            Height = height+padY;

            cols = width / size;
            rows = height / size;

            cells = new Cell[rows, cols];

            lblLifespan.Content = strLifespan;
            tbLifespan.Text = lifespan.ToString();

            lblSleep.Content = strSleep;
            tbSleep.Text = sleep.ToString();

            btnStartPause.Content = strStart;
            btnStop.Content = strStop;
            btnClear.Content = strClear;

            pbar.Foreground = alive;
            pbar.Background = dead;

            for (int row = 0; row < rows; row++)
            {
                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                for (int col = 0; col < cols; col++)
                {
                    Cell c = new Cell(size, thickness, row, col, alive, dead, border);
                    c.Click += c_Click;
                    cells[row,col] = c;
                    sp.Children.Add(c);
                }
                stackpanel.Children.Add(sp);
            }
        }

        void c_Click(object sender, RoutedEventArgs e)
        {
            Cell c = (Cell)sender;
            Flip(c);
        }

        private void File_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                StreamReader reader = new StreamReader(filename);

                Clear();

                int padY = (rows - File.ReadLines(@filename).Count()) / 2;
                int padX = 0;

                int row = padY;
                int col = 0;

                bool first = true;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (first)
                    {
                        padX = (cols - line.Length) / 2;
                        first = false;
                    }
                    col = padX;
                    foreach (char c in line)
                    {
                        if (c == '*')
                        {
                            Cell cell = cells[row,col];
                            Flip(cell);
                        }
                        col++;
                    }
                    row++;
                }
                reader.Close();
            }
            
        }

        // typing anything but numbers is disabled
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(new Regex("[0-9]").IsMatch(e.Text));
        }

        // pasting naughty input is disabled
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }

        int counter = 0;

        bool pause = false;

        async private void btnStartPause_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartPause.Content.Equals(strStart))
            {
                counter = 0;
                btnStartPause.Content = strPause;
                lifespan = int.Parse(tbLifespan.Text);
            }
            else if (btnStartPause.Content.Equals(strPause))
            {
                pause = true;
                btnStartPause.Content = strResume;
                btnStop.IsEnabled = false;
                btnClear.IsEnabled = false;
            }
            else
            {
                pause = false;
                btnStartPause.Content = strPause;
                btnStop.IsEnabled = true;
                btnClear.IsEnabled = true;
            }

            /* rules:                                  
                Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                Any live cell with two or three live neighbours lives on to the next generation.
                Any live cell with more than three live neighbours dies, as if by overcrowding.
                Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            */

            while (!pause && counter < lifespan)
            {
                pbar.Value = ++counter;
                await Task.Delay(sleep);

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        Cell c = cells[row, col];
                        if ((c.Living && c.Nbrs == 2) || c.Nbrs == 3)
                            c.Next = true;
                        else
                            c.Next = false;
                        c.Nbrs = 0;
                        c.Living = false;
                    }
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        Cell c = cells[row, col];

                        if (c.Next)
                        {
                            Flip(c);
                            c.Next = false;
                        }
                    }
                }
            }

            if (!pause)
            {
                pbar.Value = pbar.Minimum;
                counter = 0;
                btnStartPause.Content = strStart;
            }
        }

        static int[][] nbrs =
        {
            new int[]{-1,-1}, new int[]{-1, 0}, new int[]{-1, 1},
            new int[]{ 0,-1}, /*     me      */ new int[]{ 0, 1},
            new int[]{ 1,-1}, new int[]{ 1, 0}, new int[]{ 1, 1}
        };

        int nrow;
        int ncol;

        private void Flip(Cell c)
        {
            int diff = c.Living ? -1 : 1;

            if (c.Living)
                c.Living = false;
            else
                c.Living = true;

            foreach (int[] nbr in nbrs)
            {
                nrow = c.Row + nbr[0];
                ncol = c.Col + nbr[1];

                if(nrow >= 0 && nrow < rows && ncol >= 0 && ncol < cols)
                {
                    Cell nc = cells[nrow,ncol];
                    if (nc != null)
                    {
                        nc.Nbrs += diff;
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            Stop();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Cell c = cells[row, col];
                    c.Living = false;
                    c.Nbrs = 0;
                    c.Next = false;
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            counter = lifespan;
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Conway's Game of Life", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void tbLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string str = tb.Text;
            if (str.Equals(string.Empty))
            {
                switch (tb.Name.Substring(2))
                {
                    case "Sleep":
                        tb.Text = sleep.ToString();
                        break;
                    default:
                        tb.Text = lifespan.ToString();
                        break;
                }
            }
            else
            {
                switch (tb.Name.Substring(2))
                {
                    case "Sleep":
                        sleep = int.Parse(tb.Text);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
