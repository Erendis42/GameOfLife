using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace GameOfLife
{
    class Cell : Button
    {
        public Cell(int size, int thickness, int row, int col, Brush alive, Brush dead, Brush border)
        {
            Width = size;
            Height = size;
            BorderBrush = border;
            BorderThickness = new Thickness(thickness);
            nbrs = 0;
            next = false;
            this.row = row;
            this.col = col;
            this.alive = alive;
            this.dead = dead;
            this.border = border;
            Living = false;
        }

        Brush alive;
        Brush dead;
        Brush border;

        bool living;

        public bool Living
        {
            get { return living; }
            set
            {
                living = value;
                Background = living ? alive : dead;
            }
        }

        int nbrs;

        public int Nbrs
        {
            get { return nbrs; }
            set { nbrs = value; }
        }

        bool next;

        public bool Next
        {
            get { return next; }
            set { next = value; }
        }

        int row;

        public int Row
        {
            get { return row; }
        }

        int col;

        public int Col
        {
            get { return col; }
        }
    }
}
