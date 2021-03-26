using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rescue_Bots
{
    public class Tractor : GameObject
    {
        private bool isToolDown;

        public enum Direction { Up, Left, Down, Right }

        public int Id { get; set; }
        public string Name { get; set; }
        // ImageControl { get; set; }
        // X { get; set; }
        // Y { get; set; }
        public Direction CurrentDirection { get; set; }

        public int StartX { get; set; }
        public int StartY { get; set; }
        public Direction StartDirection { get; set; }

        public Rectangle Backlight { get; set; }
        public OutlinedTextBlock Label { get; set; }
        public bool IsToolDown
        {
            get { return isToolDown; }
            set
            {
                isToolDown = value;
                if (isToolDown == false) Backlight.Visibility = Visibility.Hidden;
                else Backlight.Visibility = Visibility.Visible;
            }
        }

        public Tractor(int id, System.Drawing.Bitmap tile, int x, int y, string name = "") : base(tile, x, y)
        {
            Id = id;
            Name = name;
            StartX = x;
            StartY = y;
            StartDirection = Direction.Up;
            CurrentDirection = Direction.Up;
            ImageControl.ToolTip = name + " " + id;
            Backlight = new Rectangle()
            {
                Width = (int)ImageControl.Width,
                Height = (int)ImageControl.Height,
                Fill = Brushes.Orange,
                Opacity = 0.5,
            };
            Label = new OutlinedTextBlock()
            {
                Width = (int)ImageControl.Width,
                Height = (int)ImageControl.Height,
                Text = id.ToString(),
                Style = Application.Current.FindResource("TractorLabel") as Style,
                ToolTip = name + " " + id,
            };
            Label.Click += Click;
            ImageControl.MouseLeftButtonDown += Click;
            IsToolDown = false;
        }

        public void Init(int tileWidth, int tileHeight, int mapWidth, int mapHeight)
        {
            int x = StartX * tileWidth;
            int y = StartY * tileHeight;

            Canvas.SetLeft(Backlight, x);
            Canvas.SetTop(Backlight, y);

            Canvas.SetLeft(ImageControl, x);
            Canvas.SetTop(ImageControl, y);

            Canvas.SetLeft(Label, x);
            Canvas.SetTop(Label, y);

            // калибровка направления в зависимости от местоположения объекта на карте
            StartDirection = Direction.Up;
            if (X >= 0 && X <= mapWidth / 6 && X <= 3) StartDirection = Direction.Right;
            else if (X <= mapWidth && X >= mapWidth - mapWidth / 6 && X >= mapWidth - 3) StartDirection = Direction.Left;
            else if (Y >= 0 && Y <= mapHeight / 6 && Y <= 3) StartDirection = Direction.Down;
            CurrentDirection = StartDirection;
            RoundObject();
        }
        /// <summary>
        /// Получить числовое значение угла поворота для направления
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int GetRoundValue(int toRight = 0)
        {
            int value = 0;
            if (CurrentDirection == Direction.Right)
                value = 90;
            else if (CurrentDirection == Direction.Down)
                value = 180;
            else if (CurrentDirection == Direction.Left)
                value = -90;
            if (toRight == 1) value += 90;
            if (toRight == -1) value -= 90;
            return value;
        }
        public void SetRoundValue(int toRight)
        {
            int value = GetRoundValue(toRight);
            value = value % 180;
            if (value == 90)
                CurrentDirection = Direction.Right;
            else if (value == 180)
                CurrentDirection = Direction.Down;
            else if (value == -90)
                CurrentDirection = Direction.Left;
            else CurrentDirection = Direction.Up;
        }
        /// <summary>
        /// Поворот объекта при инициализации (калибровке) 
        /// </summary>
        /// <param name="array"></param>
        public void RoundObject()
        {
            RotateTransform rotateTransform = new RotateTransform(GetRoundValue());
            ImageControl.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            ImageControl.RenderTransform = rotateTransform;
        }
        public void Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Manager.ClickTractor(this, e);
        }
    }
}
