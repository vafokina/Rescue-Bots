using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Bots
{
   public class GameObject
    {
        public System.Windows.Controls.Image ImageControl { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameObject(Bitmap tile, int x, int y)
        {
            ImageControl = ImageDrawer.BitmapToControl(tile);
            X = x;
            Y = y;
        }
    }
}
