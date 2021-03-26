using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Rescue_Bots
{
    public class ImageDrawer
    {
        private Map map;
        public Canvas View { get; set; }
        public Map CurrentMap
        {
            get { return map; }
            set
            {
                if (View.Children != null && View.Children.Count > 0) View.Children.Clear();
                map = value;
                View.Children.Add(map.MapBackground);
                View.Height = map.MapBackground.Height;
                View.Width = map.MapBackground.Width;
                InitObjects();
                DrawLines();
            }
        }
        public static System.Windows.Controls.Image BitmapToControl(Bitmap bmp)
        {
            System.Windows.Controls.Image ImageControl = new System.Windows.Controls.Image();
            ImageControl.Width = bmp.Width;
            ImageControl.Height = bmp.Height;

            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); // Image is an image source
            ImageControl.Source = imageSource;

            return ImageControl;
        }
        public void DrawLines()
        {
            for (int j = 1; j < CurrentMap.MapHeight; j++)
            {
                Line line = new Line();
                line.Y1 = j * 32;
                line.Y2 = line.Y1;
                line.X1 = 0;
                line.X2 = View.Width;
                line.Stroke = System.Windows.Media.Brushes.LightYellow;
                line.Opacity = 0.5;
                View.Children.Add(line);
            }
            for (int i = 1; i < CurrentMap.MapWidth; i++)
            {
                Line line = new Line();
                line.X1 = i * 32;
                line.X2 = line.X1;
                line.Y1 = 0;
                line.Y2 = View.Height;
                line.Stroke = System.Windows.Media.Brushes.LightYellow;
                line.Opacity = 0.5;
                View.Children.Add(line);
            }
        }
        public void InitObjects()
        {
            for (int j = 0; j < CurrentMap.Targets.Count; j++)
            {
                GameObject target = CurrentMap.Targets[j];
                View.Children.Add(target.ImageControl);
                Canvas.SetLeft(target.ImageControl, target.X * CurrentMap.TileWidth);
                Canvas.SetTop(target.ImageControl, target.Y * CurrentMap.TileHeight);
            }
            for (int j = 0; j < CurrentMap.Tractors.Count; j++)
            {
                Tractor tractor = CurrentMap.Tractors[j];

                int x = tractor.X * CurrentMap.TileWidth;
                int y = tractor.Y * CurrentMap.TileHeight;

                View.Children.Add(tractor.Backlight);
                View.Children.Add(tractor.ImageControl);
                View.Children.Add(tractor.Label);
                tractor.Init(CurrentMap.TileWidth, CurrentMap.TileHeight, CurrentMap.MapWidth, CurrentMap.MapHeight);
            }
        }
        
        //public void RoundObjects(Tractor[] arrayToLeft, Tractor[] arrayToRight)
        //{

        //    //int value = 45;
        //    //int speed = value / 5;
        //    Storyboard storyboard = new Storyboard();
        //    storyboard.Duration = new Duration(TimeSpan.FromSeconds(10.0));
        //    foreach (Tractor o in arrayToLeft)
        //    {
        //        DoubleAnimation rotateAnimation = new DoubleAnimation()
        //        {
        //            // From = 0,
        //            To = GetRoundValue(o, -1),
        //            Duration = storyboard.Duration
        //        };
        //        Storyboard.SetTarget(rotateAnimation, o.ImageControl);
        //        Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
        //        storyboard.Children.Add(rotateAnimation);

        //        SetRoundValue(o, -1);
        //    }
        //    foreach (Tractor o in arrayToRight)
        //    {
        //        DoubleAnimation rotateAnimation = new DoubleAnimation()
        //        {
        //            // From = 0,
        //            To = GetRoundValue(o, 1),
        //            Duration = storyboard.Duration
        //        };
        //        Storyboard.SetTarget(rotateAnimation, o.ImageControl);
        //        Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
        //        storyboard.Children.Add(rotateAnimation);

        //        SetRoundValue(o, 1);
        //    }
        //    storyboard.Begin();


        //    //for (int i = 0; i <= value; i = i + speed)
        //    //{
        //    //    foreach (GameObject o in arrayToRight)
        //    //    {
        //    //        o.ImageControl.RenderTransform.
        //    //        RotateTransform rotateTransform = new RotateTransform(value);
        //    //        o.ImageControl.RenderTransform = rotateTransform;
        //    //    }
        //    //    Thread.Sleep(50);
        //    //}
        //    //value = -45;
        //    //speed = value / 5;
        //    //for (int i = 0; i <= value; i = i + speed)
        //    //{
        //    //    foreach (GameObject o in arrayToLeft)
        //    //    {
        //    //        RotateTransform rotateTransform = new RotateTransform(value);
        //    //        o.ImageControl.RenderTransform = rotateTransform;
        //    //    }
        //    //    Thread.Sleep(50);
        //    //}
        //}
    }
}
