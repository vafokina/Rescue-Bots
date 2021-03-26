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
        static public AlgorithmManager Manager;
        public MainWindow()
        {
            InitializeComponent();
            ThemeChange("lightTheme");
            Drawer = new ImageDrawer();
            Drawer.View = MapImage;
            Manager = new AlgorithmManager();
            Manager.DataView = AlgorithmEditor;
            Manager.Tools = Tools;
            Manager.ObjectsTools = ObjectsTools;
            Manager.CommandTools = CommandsTools;
            Manager.ButtonSaveAlgorithm = ButtonSaveAlgorithm;
            Manager.ButtonOpenAlgorithm = ButtonOpenAlgorithm;
            Manager.ButtonExecute = ButtonExecute;
            Manager.ButtonExecuteStep = ButtonExecuteStep;
            Manager.ButtonExecuteByPosition = ButtonExecuteByPosition;
            KeyDown += AnyKeyDown;
            KeyUp += AnyKeyUp;
        }

        private void AnyKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X)
                ButtonOpenMap_Click(sender, e);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.O)
                ButtonOpenMap_Click(sender, e);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.E)
                ButtonClose_Click(sender, e);
            if (e.Key == Key.Delete && Manager.CurrentMap != null)
                Manager.ClearCell();
        }
        private void AnyKeyUp(object sender, KeyEventArgs e)
        {
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
                Map map = new Map(filename);
                Drawer.CurrentMap = map;
                Manager.CurrentMap = map;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

    }
}
