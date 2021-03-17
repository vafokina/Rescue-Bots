using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Rescue_Bots
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public ImageDrawer Drawer;
        static public Map CurrentMap;
        public MainWindow()
        {
            InitializeComponent();
            ThemeChange("lightTheme");
            Drawer = new ImageDrawer();

            //List<string> styles = new List<string> { "light", "dark" };
            //styleBox.SelectionChanged += ThemeChange;
            //styleBox.ItemsSource = styles;
            //styleBox.SelectedItem = "dark";
        }

        private void ThemeChange(string style)//(object sender, SelectionChangedEventArgs e)
        {
            // определяем путь к файлу ресурсов
            var uri = new Uri(style + ".xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GridMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Normal) DragMove();
        }

        private void ButtonMinMax_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            if (DarkTheme.IsChecked != true)
                ThemeChange("lightTheme");
            else
                ThemeChange("darkTheme");
        }

        private void ShowMap()
        {
            if (CurrentMap == null) throw new Exception("No loaded map");

            //MapImage.Children.Add(CurrentMap.MapLayout);
            MapImage.Children.Add(Drawer.BitmapToControl(CurrentMap.MapBitmap));
            MapImage.Height = CurrentMap.MapLayout.Height;
            MapImage.Width = CurrentMap.MapLayout.Width;
                for (int j = 1; j < CurrentMap.MapHeight; j++)
                {
                    Line line = new Line();
                    line.Y1 = j * 32;
                    line.Y2 = line.Y1;
                    line.X1 = 0;
                    line.X2 = CurrentMap.MapLayout.Width;
                    line.Stroke = System.Windows.Media.Brushes.Yellow;
                MapImage.Children.Add(line);
                }
                for (int i = 1; i < CurrentMap.MapWidth; i++)
                {
                    Line line = new Line();
                    line.X1 = i * 32;
                    line.X2 = line.X1;
                    line.Y1 = 0;
                    line.Y2 = CurrentMap.MapLayout.Height;
                    line.Stroke = System.Windows.Media.Brushes.Yellow;
                MapImage.Children.Add(line);
                }
            //    foreach (Object o in CurrentMap.Humans)
            //{
            //    MapImage.Children.Add(o.ImageControl);
            //}
            //foreach (Object o in CurrentMap.Robots)
            //{
            //    MapImage.Children.Add(o.ImageControl);
            //}
        }

        private void ButtonOpenMap_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".tmx"; // Default file extension
            dlg.Filter = "Tiled map file (.tmx)|*.tmx"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                CurrentMap = new Map(filename);
                ShowMap();
            }
        }
    }
}
