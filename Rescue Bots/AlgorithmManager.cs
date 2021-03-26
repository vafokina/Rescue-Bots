using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Rescue_Bots
{
    public class AlgorithmManager
    {
        Map map;
        public DataGrid DataView { get; set; }
        public Grid Tools { get; set; }
        public Grid ObjectsTools { get; set; }
        public Grid CommandTools { get; set; }
        public Button ButtonOpenAlgorithm { get; set; }
        public Button ButtonSaveAlgorithm { get; set; }
        public Button ButtonExecute { get; set; }
        public Button ButtonExecuteStep { get; set; }
        public Button ButtonExecuteByPosition { get; set; }
        public Map CurrentMap
        {
            get { return map; }
            set
            {
                map = value;
                Init();
            }
        }
        public DataTable Data { get; set; }
        public readonly string[] Commands = new string[] { "Вперед", "Стоп", "Налево", "Направо", "Опустить плуг", "Поднять плуг" };

        public int CurrentRow { get; set; }
        public int CurrentCol { get; set; }

        public void Init()
        {
            DataView.Columns.Clear();
            ObjectsTools.Children.Clear();
            ObjectsTools.RowDefinitions.Clear();

            Data = new DataTable();
            Data.Columns.Add("Tact", typeof(Int32));
            for (int i = 0; i < map.Tractors.Count - 1; i++)
            {
                Data.Columns.Add("Object" + i);
                Data.Columns.Add("Command" + i);
            }
            for (int i = 1; i < 21; i++)
                Data.Rows.Add(new object[] { i });
            DataView.ItemsSource = Data.DefaultView;

            DataView.Columns[0].Header = "Такт";
            for (int i = 1; i < DataView.Columns.Count; i = i + 2)
            {
                DataView.Columns[i].Header = "Адресат ";
                DataView.Columns[i + 1].Header = "Команда ";
            }

            RowDefinition rowDef0 = new RowDefinition();
            rowDef0.Height = new GridLength(35);
            ObjectsTools.RowDefinitions.Add(rowDef0);
            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = new GridLength(5);
            ObjectsTools.RowDefinitions.Add(rowDef1);

            Button buttonAll = new Button();
            buttonAll.Content = "Все";
            buttonAll.Click += ClickAll;
            buttonAll.Margin = new Thickness(2, 0, 2, 0);
            buttonAll.Focusable = false;
            Style styleAll = Application.Current.FindResource("MaterialDesignOutlinedButton") as Style;
            buttonAll.Style = styleAll;
            Grid.SetColumn(buttonAll, 0);
            Grid.SetRow(buttonAll, 0);
            ObjectsTools.Children.Add(buttonAll);

            int row = 1; int col = 1;
            foreach (Tractor o in CurrentMap.Tractors)
            {
                Button button = new Button();
                button.Content = o.Name + " " + o.Id;
                button.Click += o.Click;
                button.Margin = new Thickness(2, 0, 2, 0);
                button.Focusable = false;
                Style style = Application.Current.FindResource("MaterialDesignOutlinedButton") as Style;
                button.Style = style;
                col++;
                if (col == 2)
                {
                    col = 0; row++;
                    RowDefinition rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(35);
                    ObjectsTools.RowDefinitions.Add(rowDef);
                }
                Grid.SetColumn(button, col);
                Grid.SetRow(button, row);
                ObjectsTools.Children.Add(button);
            }

            Tools.IsEnabled = true;
            ButtonExecute.IsEnabled = false;
            ButtonExecuteStep.IsEnabled = false;
            ButtonExecuteByPosition.IsEnabled = false;
            ButtonSaveAlgorithm.IsEnabled = false;

            foreach (Button b in CommandTools.Children.OfType<Button>())
            {
                b.Click -= ClickCommand;
                b.Click += ClickCommand;
            }
            DataView.SelectedCellsChanged -= AlgorithmEditor_SelectedCellsChanged;
            DataView.SelectedCellsChanged += AlgorithmEditor_SelectedCellsChanged;
            DataView.PreviewKeyDown -= AnyKeyDown;
            DataView.PreviewKeyDown += AnyKeyDown;

            ButtonExecute.Click -= ClickExecute;
            ButtonExecute.Click += ClickExecute;
        }


        #region Обработчики кликов и нажатий
        private void AnyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                ClearCell();
        }
        public void ClickAll(object sender, RoutedEventArgs e)
        {
            if (IsCellSelected())
            {
                Data.Rows[CurrentRow].ItemArray = InsertObject("Все", Data.Rows[CurrentRow].ItemArray);
            }
        }
        public void ClickTractor(object sender, RoutedEventArgs e)
        {
            string t = ((Tractor)sender).Id.ToString();
            if (IsCellSelected())
            {
                Data.Rows[CurrentRow].ItemArray = InsertObject("Трактор " + t, Data.Rows[CurrentRow].ItemArray);
            }
        }
        public void ClickCommand(object sender, RoutedEventArgs e)
        {
            string nameButton = ((Button)sender).Name;

            if (IsCellSelected())
            {
                switch (nameButton)
                {
                    case "ButtonStart":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[0], Data.Rows[CurrentRow].ItemArray);
                        return;
                    case "ButtonStop":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[1], Data.Rows[CurrentRow].ItemArray);
                        return;
                    case "ButtonToLeft":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[2], Data.Rows[CurrentRow].ItemArray);
                        return;
                    case "ButtonToRight":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[3], Data.Rows[CurrentRow].ItemArray);
                        return;
                    case "ButtonDownTool":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[4], Data.Rows[CurrentRow].ItemArray);
                        return;
                    case "ButtonUpTool":
                        Data.Rows[CurrentRow].ItemArray = InsertCommand(Commands[5], Data.Rows[CurrentRow].ItemArray);
                        return;
                }
            }
        }
        public void ClickExecute(object sender, RoutedEventArgs e)
        {
            List<ICommand>[] commands = new List<ICommand>[map.Tractors.Count];
            ICommand[] prevCommands = new ICommand[map.Tractors.Count];
            for (int i = 0; i < prevCommands.Length; i++)
                prevCommands[i] = null;

            object[] row0 = Data.Rows[0].ItemArray;
            for (int i = 1; i < row0.Length; i = i + 2)
            {
                if ((row0[i].ToString() != "" && row0[i + 1].ToString() == "") || (row0[i+1].ToString() != "" && row0[i].ToString() == ""))
                {
                    // exception
                }
                int id = Convert.ToInt32(row0[i].ToString().Replace("Трактор ", ""));
                commands[id].Add(SwitcherCommand.Switch(row0[i + 1].ToString(), id, CurrentMap));
            }

            for (int i = 1; i < Data.Rows.Count; i++)
            {
                object[] row = Data.Rows[i].ItemArray;

            }
        }
        private void AlgorithmEditor_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var currentItem = DataView.SelectedCells.First().Item;
            CurrentRow = DataView.Items.IndexOf(currentItem);
            CurrentCol = DataView.CurrentColumn.DisplayIndex;
            if (DataView.CurrentColumn.DisplayIndex == 0) MessageBox.Show("Hello");
        }
        #endregion

        #region Методы для заполнения таблицы: вставить, удалить, фокус
        public void ClearCell()
        {
            if (IsCellSelected())
            {
                object[] row = Data.Rows[CurrentRow].ItemArray;
                row[CurrentCol] = "";
                Data.Rows[CurrentRow].ItemArray = row;
                if (IsDataEmpty()) ExecuteButtonsOff();
            }
        }
        public object[] InsertCommand(string command, object[] row)
        {
            if (CurrentCol % 2 == 1) return row;
            if (CurrentCol == 0) return row;
            object[] newRow = row;
            newRow[CurrentCol] = command;
            for (int i = newRow.Length - 1; i > 1; i = i - 2)
            {
                if (newRow[i].ToString() == null || newRow[i].ToString() == "") continue;
                else if (i - 2 > 0 && (newRow[i - 2].ToString() == null || newRow[i - 2].ToString() == ""))
                {
                    newRow[i - 2] = newRow[i];
                    newRow[i] = "";
                    CurrentCol = i - 2;
                }
            }
            SetFocusOnCell();
            ExecuteButtonsOn();
            return newRow;
        }
        public object[] InsertObject(string obj, object[] row)
        {
            if (CurrentCol % 2 == 0) return row;
            if (CurrentCol == 0) return row;
            object[] newRow = row;
            if (newRow.Where(a => a.ToString() == obj).Count() > 0)
            {
                newRow[CurrentCol] = "";
                for (int i = 1; i < newRow.Length; i++)
                    if (newRow[i].ToString() == obj)
                        CurrentCol = i;
                SetFocusOnCell();
                return newRow;
            }
            newRow[CurrentCol] = obj;
            if (obj == "Все")
            {
                for (int i = 3; i < newRow.Length; i++)
                    newRow[i] = "";
                newRow[1] = obj;
                CurrentCol = 1;
            }
            else
            {
                if (newRow[1].ToString() == "Все")
                {
                    newRow[1] = "";
                }
                for (int i = newRow.Length - 2; i > 0; i = i - 2)
                {
                    if (newRow[i].ToString() == null || newRow[i].ToString() == "") continue;
                    if (i - 2 > 0 && (newRow[i - 2].ToString() == null || newRow[i - 2].ToString() == ""))
                    {
                        newRow[i - 2] = newRow[i];
                        newRow[i] = "";
                        CurrentCol = i - 2;
                    }
                }
            }
            SetFocusOnCell();
            ExecuteButtonsOn();
            return newRow;
        }
        public void SetFocusOnCell()
        {
            // DataView.CurrentCell = new DataGridCellInfo(DataView.Items[CurrentRow], DataView.Columns[CurrentCol]);
            //DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(DataView.Items[CurrentRow], DataView.Columns[CurrentCol]);
            //DataView.SelectedCells.Clear();
            //DataView.SelectedCells.Add(dataGridCellInfo);
            //In case of more columns
            DataView.Focus();
            DataView.CurrentCell = new DataGridCellInfo(DataView.Items[CurrentRow], DataView.Columns[CurrentCol]);
        }
        #endregion

        #region Булевые методы проверки
        private bool IsCellSelected()
        {
            if (DataView.SelectedCells.Count == 1 && DataView.CurrentColumn != null
                && CurrentCol > -1 && CurrentCol < Data.Columns.Count
                && CurrentRow > -1 && CurrentRow < Data.Rows.Count)
                return true;
            else
                return false;
        }
        private bool IsDataEmpty()
        {
            for (int i = 0; i < Data.Rows.Count; i++)
            {
                object[] row = Data.Rows[i].ItemArray;
                for (int j = 1; j < row.Length; j++)
                {
                    if (row[j] != null && row[j].ToString() != "")
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region Блокировка кнопок
        private void ExecuteButtonsOn()
        {
            ButtonExecute.IsEnabled = true;
            ButtonExecuteByPosition.IsEnabled = true;
            ButtonExecuteStep.IsEnabled = true;
            ButtonSaveAlgorithm.IsEnabled = true;
        }
        private void ExecuteButtonsOff()
        {
            ButtonExecute.IsEnabled = false;
            ButtonExecuteByPosition.IsEnabled = false;
            ButtonExecuteStep.IsEnabled = false;
            ButtonSaveAlgorithm.IsEnabled = false;
        }
        #endregion
    }
}
