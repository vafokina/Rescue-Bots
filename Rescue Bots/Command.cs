using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Rescue_Bots
{
    public class SwitcherCommand
    {
        public static ICommand Switch(string command, int tractorId, Map map)
        {
            switch (command)
            {
                case "Вперед":
                    return new CommandGoForward(tractorId, map);
                case "Стоп":
                    return null;
                case "Налево":
                    return new CommandGoForward(tractorId, map);
                case "Направо":
                    return new CommandGoForward(tractorId, map);
                case "Опустить плуг":
                    return new CommandGoForward(tractorId, map);
                case "Поднять плуг":
                    return new CommandGoForward(tractorId, map);
            }
            return null;
        }
    }

    public class CommandGoForward : ICommand
    {
        private Tractor tractor;
        private Map map;
        private int x, y;
        public CommandGoForward(int tractorId, Map map)
        {
            this.map = map;
            this.tractor = map.Tractors.Where(a => a.Id == tractorId).First();
            x = tractor.X;
            y = tractor.Y;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            Tractor.Direction direction = tractor.CurrentDirection;

            if (direction == Tractor.Direction.Up) y--;
            if (direction == Tractor.Direction.Down) y++;
            if (direction == Tractor.Direction.Left) x--;
            if (direction == Tractor.Direction.Right) x++;
            if (x < 0 || x > map.MapWidth - 1 || y < 0 || y > map.MapHeight - 1)
                return false;
            if (map.MapString[x, y] == "B" || map.MapString[x, y] == "X")
                return false;
            return true;
        }

        public void Execute(object parameter = null)
        {
            Tractor.Direction direction = tractor.CurrentDirection;
            x = tractor.X;
            y = tractor.Y;
            int speed = 0;
            if (tractor.IsToolDown) speed = 1;
            else speed = 2;
            if (direction == Tractor.Direction.Up) y -= speed;
            if (direction == Tractor.Direction.Down) y += speed;
            if (direction == Tractor.Direction.Left) x -= speed;
            if (direction == Tractor.Direction.Right) x += speed;

            //Storyboard storyboard = new Storyboard();
            //storyboard.Duration = new Duration(TimeSpan.FromSeconds(3.0));
            //DoubleAnimationUsingPath rotateAnimation = new DoubleAnimationUsingPath()
            //{
            //    // From = 0,
            //    Source = PathAnimationSource.X,
            //    PathGeometry 
            //    Duration = storyboard.Duration
            //};
            //Storyboard.SetTarget(rotateAnimation, o.ImageControl);
            //Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            //storyboard.Children.Add(rotateAnimation);
            //Storyboard.SetTarget(rotateAnimation, o.ImageControl);
            //Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            //storyboard.Children.Add(rotateAnimation);
            //storyboard.Begin();
        }
    }
    public class CommandRoundToLeft : ICommand
    {
        private Tractor tractor;
        private Map map;
        public CommandRoundToLeft(int tractorId, Map map)
        {
            this.map = map;
            this.tractor = map.Tractors.Where(a => a.Id == tractorId).First();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            return true;
        }

        public void Execute(object parameter = null)
        {
            // Storyboard storyboard = new Storyboard();
            //storyboard.Duration = new Duration(TimeSpan.FromSeconds(3.0));
            //DoubleAnimation rotateAnimation = new DoubleAnimation()
            //{
            //    // From = 0,
            //    To = x*map.TileWidth,
            //    Duration = storyboard.Duration
            //};
            //Storyboard.SetTarget(rotateAnimation, o.ImageControl);
            //Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            //storyboard.Children.Add(rotateAnimation);
            //Storyboard.SetTarget(rotateAnimation, o.ImageControl);
            //Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            //storyboard.Children.Add(rotateAnimation);
            //storyboard.Begin();
        }
    }
}
