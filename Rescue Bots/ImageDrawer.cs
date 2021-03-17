using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rescue_Bots
{
   public class ImageDrawer
    {
        private MemoryStream Ms;
        public ImageDrawer()
        {
            Ms = new MemoryStream();
        }
        public System.Windows.Controls.Image BitmapToControl(Bitmap bmp)
        {
            System.Windows.Controls.Image ImageControl = new System.Windows.Controls.Image();
            System.Drawing.Bitmap ObjBitmap = bmp;
            ObjBitmap.Save(Ms, System.Drawing.Imaging.ImageFormat.Png);
            Ms.Position = 0;
            BitmapImage ObjBitmapImage = new BitmapImage();
            ObjBitmapImage.BeginInit();
            ObjBitmapImage.StreamSource = Ms;
            ObjBitmapImage.EndInit();
            ImageControl.Width = ObjBitmap.Width;
            ImageControl.Height = ObjBitmap.Height;
            ImageControl.Source = ObjBitmapImage;
            return ImageControl;
        }
    }
}
