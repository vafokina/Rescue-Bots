using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Bots
{
   public class Object
    {
        private static int count = 0;

        public System.Windows.Controls.Image ImageControl { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        static int Count
        {
            get { return count; }
        }

        public Object(Bitmap tile, int x, int y)
        {
            if (MainWindow.Drawer == null) throw new Exception("ImageDrawer is null");
           ImageControl = MainWindow.Drawer.BitmapToControl(tile);
            X = x;
            Y = y;
            count++;
        }
        public Object(Bitmap tile, int x, int y, int id)
        {
            if (MainWindow.Drawer == null) throw new Exception("ImageDrawer is null");
            ImageControl = MainWindow.Drawer.BitmapToControl(tile);
            X = x;
            Y = y;
            count++;
            ImageControl.ToolTip = "Robot " + id;
        }
    }
}
