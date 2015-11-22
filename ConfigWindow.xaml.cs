using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace KC12VU_CGoL
{
    /// <summary>
    /// Interaction logic for WindowConfig.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        static string strWidth = "Width:";
        static string strHeight = "Height:";
        static string strSize = "Size:";
        static string strThickness = "Border thickness:";
        static string strLifespan = "Lifespan:";
        static string strSleep = "Sleep in ms:";
        static string strAlive = "Cell color:";
        static string strDead = "Dead cell color:";
        static string strBorder = "Border color:";

        int width;
        int height;
        int size;
        int thickness;
        int lifespan;
        int sleep;

        Brush alive;
        Brush dead;
        Brush border;

        public ConfigWindow(int width, int height, int size, int thickness, int lifespan, int sleep,
            Brush alive, Brush dead, Brush border)
        {
            this.width = width;
            this.height = height;
            this.size = size;
            this.thickness = thickness;
            this.lifespan = lifespan;
            this.sleep = sleep;

            this.alive = alive;
            this.dead = dead;
            this.border = border;

            InitializeComponent();
            InitializeAppearance();
        }

        private void InitializeAppearance()
        {
            lblWidth.Content = strWidth;
            tbWidth.Text = width.ToString();

            lblHeight.Content = strHeight;
            tbHeight.Text = height.ToString();

            lblSize.Content = strSize;
            tbSize.Text = size.ToString();

            lblThickness.Content = strThickness;
            tbThickness.Text = thickness.ToString();

            lblLifespan.Content = strLifespan;
            tbLifespan.Text = lifespan.ToString();

            lblSleep.Content = strSleep;
            tbSleep.Text = sleep.ToString();

            lblAlive.Content = strAlive;
            tbAlive.Text = alive.ToString().Substring(3);

            lblDead.Content = strDead;
            tbDead.Text = dead.ToString().Substring(3);

            lblBorder.Content = strBorder;
            tbBorder.Text = border.ToString().Substring(3);

        }

        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(new Regex("[0-9]").IsMatch(e.Text));
        }

        private void tbHex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(new Regex("[0-9A-Fa-f]").IsMatch(e.Text));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void config_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOK_Click(sender, e);
        }

        private void btnRnd_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            Random rnd = new Random();
            string color = String.Format("{0:X6}", rnd.Next(0x1000000));

            switch (btn.Name.Substring(6))
            {
                case "Alive":
                    tbAlive.Text = color;
                    break;
                case "Dead":
                    tbDead.Text = color;
                    break;
                default:
                    tbBorder.Text = color;
                    break;
            }
        }

        // pasting naughty input is disabled
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }
    }
}
