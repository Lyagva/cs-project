using System;
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
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
//using HelpTimer;

namespace CMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  <--- Data for func(s) --->

        //история настроек
        public Dictionary<string, string> settings = new Dictionary<string, string>()
        { 
          ["volume"] = "50",
          ["fontSize"] = "18",
          ["timeLock"] = "2",
          ["timerEnabled"] = "false",
          ["musicEnabled"] = "true",
          ["fullscreenEnabled"] = "false",
          ["chapter"] = "0",
          ["id"] = "0"
        };

        //история ввода
        List<string> inputHistory = new List<string> { };
        int inputHistoryPointer = 0;

        int currentMood = 0;
        List<int[]> actions = new List<int[]>();
        List<string> actionsContent = new List<string>();

        private static MediaPlayer _backgroundMusic = new MediaPlayer();

        // секундомер новый
        //OurTimer TotalStopWatcher;
        // секундомер старый
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            loadState();
        }



        // <--- Maincast styles --->
        /// <summary>
        /// Функции, которые как либо меняют объекты, должны писаться в этом блоке 
        /// </summary>
        public SolidColorBrush HexToColor(string col)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
        }
        private void Obj_MouseEnter(object sender, MouseEventArgs e)
        {
            Button obj = (Button)sender;
            obj.Foreground = HexToColor("#2ecc71");
            if (obj == New_gamebutton)
            {
                obj.Foreground = HexToColor("#FFFFFF");
            }
        }
        private void Obj_MouseLeave(object sender, MouseEventArgs e)
        {
            Button obj = (Button)sender;
            obj.Foreground = HexToColor("#229652");
            if (obj == New_gamebutton)
            {
                obj.Foreground = HexToColor("#2ecc71");
            }
        }

        // музыка
        public void BackgroundMusic_Start()
        {
            _backgroundMusic.Open(new Uri("music/Rain and you.mp3", UriKind.Relative));
            _backgroundMusic.MediaEnded += new EventHandler(BackgroundMusic_Ended);
            _backgroundMusic.Play();
            settings["musicEnabled"] = "true";
        }
        private void BackgroundMusic_Ended(object sender, EventArgs e)
        {
            _backgroundMusic.Position = TimeSpan.Zero;
            _backgroundMusic.Play();
        }
        private void BackgroundMusic_Stop()
        {
            _backgroundMusic.Position = TimeSpan.Zero;
            _backgroundMusic.Stop();
            settings["musicEnabled"] = "false";
        }

        // секундомер - старый
        public void Timer_Start()
        {
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //dt.Interval = new TimeSpan(Convert.ToInt64(TimeSpan.Parse(settings["timeLock"])));
            sw.Start();
            dt.Start();
            settings["timerEnabled"] = "true";
        }
        public void Timer_Stop()
        {
            sw.Stop();
            dt.Stop();
            settings["timerEnabled"] = "true";
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (timer_checker.IsChecked == true)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds);
                clocktxtblock.Content = currentTime;
                settings["timeLock"] = currentTime.ToString();
            }
        }

        // чтение переходов
        public void changePlot(int chapter, int id)
        {
            settings["chapter"] =  chapter.ToString();
            settings["id"] = id.ToString();

            actionsContent.Clear();
            actions.Clear();

            List<string> plotFile = File.ReadAllLines("plot/chapter" + chapter.ToString() +
                "/" + id.ToString() + ".txt").ToList<string>();


            for (int i = 0; i < plotFile.Count; i++)
            {
                List<string> line = plotFile[i].Split(' ').ToList<string>();
                if (line[0] == "{MOOD}")
                {
                    currentMood = Convert.ToInt32(line[1]);
                }
                else if (line[0] == "{ACTION}")
                {
                    actions.Add(
                        new int[] {
                            Convert.ToInt32(line[1]), Convert.ToInt32(line[2])});
                    line.RemoveAt(0); line.RemoveAt(0); line.RemoveAt(0);
                    actionsContent.Add(string.Join(" ", line));

                }
                else if (line[0] == "{TEXT}")
                {
                    line.RemoveAt(0);
                    addConsoleOutput(string.Join(" ", line));
                }

            }
            setButtonsCount(actionsContent.Count, actionsContent);
        }

        //  <--- Main tab comands and func(s) --->

        // Main tab - buttons
        /// <summary>
        /// Функции вызываемые кнопками, должны содержать в себе функцию в нижнем регистре, а также содержать в названии функции прямую отсылку на что она делает
        /// Начало начинается с refer__
        /// </summary>
        private void refer__newgame(object sender, RoutedEventArgs e)
        {
            if (game_tab.Visibility == Visibility.Hidden)
            {
                game_tab.Visibility = Visibility.Visible;
                main_game.Visibility = Visibility.Visible;
                New_gamebutton.Content = "CLOSE";
            }
            else
            {
                game_tab.Visibility = Visibility.Hidden;
                main_game.Visibility = Visibility.Hidden;
                New_gamebutton.Content = "CONTINUE";
            }
        }
        private void refer__showcredits(object sender, RoutedEventArgs e)
        {
            CreditsWindow window = new CreditsWindow();

            window.Owner = this;
            window.Show();
        }
        private void refer__exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        //  <--- Settings tab comands and func(s) --->

        // Settings tab - buttons & sliders
        private void refer__fullscreen(object sender, RoutedEventArgs e)
        {
            switch_fullscreen();
        }
        private void refer__fontsizech(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var fontsizeSlider = sender as Slider;
            Application.Current.MainWindow.FontSize = (double)fontsizeSlider.Value;

            settings["fontSize"] = ((double)fontsizeSlider.Value).ToString();
        }
        private void refer__volumech(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var volumeSlider = sender as Slider;
            _backgroundMusic.Volume = 0.01 * (double)volumeSlider.Value;

            settings["volume"] = ((int)volumeSlider.Value).ToString();
            //Volume может работать в диапозоне от 0 до 1 
            //(на самом деле можно и 2389 поставить, но тогда разницы всё равно не получится)
            //однако Slider у нас 100-балльный для удобства
        }
        private void refer__musicplayer(object sender, RoutedEventArgs e)
        {
            CheckBox obj = (CheckBox)sender;
            if (obj.IsChecked == true)
            {
                BackgroundMusic_Start();
                settings["musicEnabled"] = "true";
            }
            else
            {
                BackgroundMusic_Stop();
                settings["musicEnabled"] = "false";
            }
        }
        private void refer__timelocker(object sender, RoutedEventArgs e)
        {
            CheckBox obj = (CheckBox)sender;
            if (obj.IsChecked == true)
            {
                Timer_Start();
                settings["timerEnabled"] = "true";
            }
            else
            {
                Timer_Stop();
                settings["timerEnabled"] = "false";
            }
        }

        // switchers for loading prog with saves
        private void switch_fullscreen()
        {
            if (WindowState == WindowState.Normal)
            {
                settings["fullscreenEnabled"] = "true";
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                fullscreen.Content = "Windowed";
                settings_left_grid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                settings_left_grid.RowDefinitions[4].Height = new GridLength(3, GridUnitType.Star);
                settings_right_grid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                settings_right_grid.RowDefinitions[6].Height = new GridLength(3, GridUnitType.Star);
            }
            else
            {
                settings["fullscreenEnabled"] = "false";
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                fullscreen.Content = "Fullscreen";
                settings_left_grid.RowDefinitions[0].Height = new GridLength(0.4, GridUnitType.Star);
                settings_left_grid.RowDefinitions[4].Height = new GridLength(0.4, GridUnitType.Star);
                settings_right_grid.RowDefinitions[0].Height = new GridLength(0.5, GridUnitType.Star);
                settings_right_grid.RowDefinitions[6].Height = new GridLength(0.5, GridUnitType.Star);
            }
        }
        public void switch_music(CheckBox arg)
        {
            if (arg.IsChecked == true)
            {
                arg.IsChecked = false;
                BackgroundMusic_Stop();
            }
            else
            {
                arg.IsChecked = true;
                BackgroundMusic_Start();
            }
        }
        public void switch_timer(CheckBox arg)
        {
            if (arg.IsChecked == true)
            {
                arg.IsChecked = false;
                Timer_Stop();
            }
            else
            {
                arg.IsChecked = true;
                Timer_Start();
            }
        }

        // loader and saver of prog
        private void saveState()
        {
            using (StreamWriter writer = new StreamWriter("settings.idk"))
            {
                for (int i = 0; i < settings.Count; i++)
                {
                    writer.WriteLine(settings.Keys.ToArray()[i] + " = " + settings[settings.Keys.ToArray()[i]]);
                }
            }

        }
        private void loadState()
        {
            // чтение настроек из settings.idk
            Dictionary<string, string> settings = File.ReadAllLines("settings.idk")
                                       .Select(x => x.Replace(" ", "").Split('='))
                                       .ToDictionary(x => x[0], x => x[1]);

            // установка Slider(s)
            volume_slider.Value = Convert.ToDouble(settings["volume"]);
            fontSize_slider.Value = Convert.ToDouble(settings["fontSize"]);
            clocktxtblock.Content = Convert.ToString(settings["timeLock"]);

            // установка CheckBox(s)
            if ((Convert.ToBoolean(settings["fullscreenEnabled"]) && WindowState != WindowState.Maximized) ||
                (!Convert.ToBoolean(settings["fullscreenEnabled"]) && WindowState != WindowState.Normal))
            {
                switch_fullscreen();
            }
            if (Convert.ToBoolean(settings["musicEnabled"]) != music_checker.IsChecked)
            {
                switch_music(music_checker);
            }
            if (Convert.ToBoolean(settings["timerEnabled"]) != timer_checker.IsChecked)
            {
                switch_timer(timer_checker);
            }

            // установка Plot
            changePlot(Convert.ToInt32(settings["chapter"]), Convert.ToInt32(settings["id"]));
        }



        //  <--- Game tab comands and func(s) --->




        //перезапись кнопок
        public void setButtonsCount(int count, List<string> actionsContent)
        {
            int delta = actionButtonsGrid.ColumnDefinitions.Count / count;
            actionButtonsGrid.Children.Clear();
            for (int i = 0; i < count; i++)
            {
                Button tmpBtn = new Button();
                tmpBtn.Name = "act" + Convert.ToString(i);
                tmpBtn.SetValue(Grid.ColumnProperty, i * delta);
                tmpBtn.SetValue(Grid.ColumnSpanProperty, delta);
                tmpBtn.Click += new RoutedEventHandler(actionButton_MouseClick);
                tmpBtn.Content = new TextBlock
                {
                    Text = actionsContent[i],
                    TextWrapping = TextWrapping.Wrap
                };

                actionButtonsGrid.Children.Add(tmpBtn);
            }
        }



        // фокусировка консоли
        private void consoleInput_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox obj = (TextBox)sender;
            obj.Foreground = HexToColor("#2ecc71");
        }
        private void consoleInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox obj = (TextBox)sender;
            obj.Foreground = HexToColor("#229652");
        }

        // отправка сообщений
        public void addConsoleOutput(string text)
        {
            Console.WriteLine(text);
            consoleOutput.Text += "\n" + text;
        }

        // отправка по кнопкам
        private void actionButton_MouseClick(object sender, RoutedEventArgs e)
        {
            Button obj = (Button)sender;
            int index = actionButtonsGrid.Children.IndexOf(obj);
            changePlot(actions[index][0], actions[index][1]);
        }

        // ограничения для настроек
        public double Clamp(double value, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }
        public float Clamp(float value, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }
        public int Clamp(int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        // работа с командной строкой
        public string operateCommand(string text)
        {
            text = text.ToLower();
            List<string> args = text.Split(' ').ToList();
            string command = args[0];
            args.Remove(command);

            // ======== Help ========
            if (new[] { "help", "-h", "?" }.Contains(command))
            {
                return operateCommandHelp(args);
            }

            // ======== Settings ========
            if (new[] { "sett", "settings" }.Contains(command))
            {
                return operateCommandSettings(args);
            }
            if (new[] { "cls", "clear" }.Contains(command))
            {
                consoleOutput.Text = "";
                changePlot(Convert.ToInt32(settings["chapter"]), Convert.ToInt32(settings["id"]));
                return "";
            }
            if (new[] { "act", "action" }.Contains(command))
            {
                operateCommandAct(args);
                return "";
            }

            return "Command not found";
        }
        public string operateCommandSettings(List<string> args)
        {
            if (args.Count < 1)
            {
                return "No property provided. Use 'sett -h' for more info";
            }

            if (args[0] == "-h")
            {
                return
                    "Usage:" +
                    "\n\tsett <property> <value>" +
                    "\nAvailable setting properties:" +
                    "\n\t[fontsize; size] <int [0; inf)>\tchanges font size" +
                    "\n\t[music; audio] <bool [true; false]>\tenables and disables music" +
                    "\n\t[volume] <int [0;100]>\tchanges music volume" +
                    "\n\t[fullscreen] <bool [true; false]>\tswitches fullscreen and windowed mode" +
                    "\n\t[restart] true\tresets game progress" +
                    "\n\t[default] true\tresets all settings";
            }

            if (args.Count < 2)
            {
                return "No value provided. Use 'sett -h' for more info";
            }

            string property = args[0];
            string value = args[1];

            if (new[] { "fontsize", "size" }.Contains(property))
            {
                // Value check
                if (!int.TryParse(value, out _))
                {
                    return "Invalid value [" + value + "]. Value should be in format of integer [0; inf]";
                }
                if (Convert.ToInt32(value) < 0)
                {
                    return "Invalid value [" + value + "]. Value should be in format of integer [0; inf]";
                }

                fontSize_slider.Value = Convert.ToDouble(value);
                property = "font size";
            }

            else if (new[] { "music", "audio" }.Contains(property))
            {
                // Value check
                if (!new[] { "true", "false" }.Contains(value))
                {
                    return "Invalid value [" + value + "]. Value should be ['true'; 'false']";
                }

                if (Convert.ToBoolean(value) != music_checker.IsChecked)
                {
                    switch_music(music_checker);
                }
                property = "music";
            }

            else if (new[] { "volume" }.Contains(property))
            {
                // Value check
                if (!int.TryParse(value, out _))
                {
                    return "Invalid value [" + value + "]. Value should be in format of integer [0; 100]";
                }
                if (Convert.ToInt32(value) < 0 || Convert.ToInt32(value) > 100)
                {
                    return "Invalid value [" + value + "]. Value should be in format of integer [0; inf]";
                }

                volume_slider.Value = Convert.ToDouble(value);
                property = "volume";
            }

            else if (new[] { "fullscreen" }.Contains(property))
            {
                // Value check
                if (!new[] { "true", "false" }.Contains(value))
                {
                    return "Invalid value [" + value + "]. Value should be ['true'; 'false']";
                }

                if (Convert.ToBoolean(value) && WindowState != WindowState.Maximized)
                {
                    switch_fullscreen();
                }
                else if (!Convert.ToBoolean(value) && WindowState != WindowState.Normal)
                {
                    switch_fullscreen();
                }
                property = "fullscreen";
            }

            else if (new[] { "restart" }.Contains(property))
            {
                changePlot(0, 0);
                property = "restart";
                return "";
            }

            else if (new[] { "default" }.Contains(property))
            {
                string chapter = settings["chapter"];
                string id = settings["id"];
                settings = new Dictionary<string, string>()
                {
                    ["volume"] = "50",
                    ["fontSize"] = "18",
                    ["timeLock"] = "00:00:02",
                    ["timerEnabled"] = "false",
                    ["musicEnabled"] = "true",
                    ["fullscreenEnabled"] = "false",
                    ["chapter"] = chapter,
                    ["id"] = id
                };
                saveState();
                loadState();
                property = "default";
            }

            else
            {
                return "Invalid property [" + property + "]. Use 'sett -h' for more info";
            }

            return "Successfully changed [" + property + "] to [" + value + "]";
        }
        public string operateCommandHelp(List<string> args)
        {
            return
                "Available commands:" +
                "\n\t[help, -h, ?]\tShows this message" +
                "\n\t[settings; sett]\tedits game properties. Use 'sett -h' for more info" +
                "\n\t[act; action]\temulates action button press. Use 'act <number>' to use. Count starts from 0" +
                "\n\t[cls; clear]\tclears console";
        }
        public string operateCommandAct(List<string> args)
        {
            string value = args[0];
            if (!int.TryParse(value, out _))
            {
                return "Invalid action number";
            }
            int num = Convert.ToInt32(value);
            if (num < 0 || num >= actions.Count)
            {
                return "Number out of range. Number shoud be in range [0; " + Convert.ToString(actions.Count) + "]";
            }
            changePlot(actions[num][0], actions[num][1]);

            return "";
        }
        // Обработчик преднажатий
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && consoleInput.IsFocused)
            {
                string inputLine = consoleInput.Text;
                string text = operateCommand(inputLine);
                addConsoleOutput(text);

                inputHistory.Insert(0, inputLine);
                inputHistoryPointer = -1;

                consoleInput.Text = "";
            }
            if (e.Key == Key.Up && inputHistory.Count >= 1)
            {
                inputHistoryPointer = Clamp(inputHistoryPointer + 1, 0, inputHistory.Count - 1);
                consoleInput.Text = inputHistory[inputHistoryPointer];
                consoleInput.Select(consoleInput.Text.Length, 0);
            }
            if (e.Key == Key.Down && inputHistory.Count >= 1)
            {
                inputHistoryPointer = Clamp(inputHistoryPointer - 1, 0, inputHistory.Count - 1);
                consoleInput.Text = inputHistory[inputHistoryPointer];
                consoleInput.Select(consoleInput.Text.Length, 0);
            }
        }

        // Обработчик окна
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveState();
        }
    }
}
