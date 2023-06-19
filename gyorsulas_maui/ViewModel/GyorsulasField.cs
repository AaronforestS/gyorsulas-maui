using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace gyorsulas_wpf.ViewModel
{
    public class GyorsulasField : ViewModelBase
    {
        private int x;
        public int X { get { return x; } private set { x = value; } }
        private int y;
        public int Y { get { return y; } private set { y = value; } }
        private int color;
        public int BGColor { get { return color; } set { color = value; OnPropertyChanged(); } }
        public GyorsulasField(int x, int y)
        {
            X = x;
            Y = y;
            BGColor = 0;
        }        
    }
}
