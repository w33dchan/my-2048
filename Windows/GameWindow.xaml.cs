﻿using Game2048.Resources;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Data.SQLite;
using System.IO;

namespace Game2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly TimeSpan halfSecond = new(0, 0, 0, 0, 500);
        private readonly (byte, byte, byte)[] cellColors = new (byte, byte, byte)[]
        {
            (0xCD, 0xC1, 0xB4), // no value
            (0xEE, 0xE4, 0xDA), // 2
            (0xED, 0xE0, 0xC8), // 4
            (0xF2, 0xB1, 0x79), // 8
            (0xF5, 0x95, 0x63), // 16
            (0xF6, 0x7C, 0x5F), // 32
            (0xF6, 0x5E, 0x3B), // 64
            (0xED, 0xCF, 0x72), // 128
            (0xED, 0xCC, 0x61), // 256
            (0xED, 0xC8, 0x4F), // 512
            (0xED, 0xC5, 0x3F), // 1024
            (0xED, 0xC2, 0x2E)  // 2048
        };
        private readonly (byte, byte, byte)[] textColors = new (byte, byte, byte)[]
        {
            (0x77, 0x6E, 0x65), // null, 2, 4
            (0xF9, 0xF6, 0xF2)  // else
        };
        private Game gameInfo;
        private DateTime gameStart;
        private readonly DispatcherTimer dt;

        public GameWindow()
        {
            InitializeComponent();
            gameInfo = new Game();

            if (!Directory.Exists("C:\\ProgramData\\My2048data"))
            {
                Directory.CreateDirectory("C:\\ProgramData\\My2048data");
            }

            if (!File.Exists("C:\\ProgramData\\My2048data\\ScoreData.sqlite"))
            {
                SQLiteConnection.CreateFile("C:\\ProgramData\\My2048data\\ScoreData.sqlite");
            }
            
            CreateTableScoreData();

            dt = new()
            {
                Interval = new TimeSpan(0, 0, 1),
                IsEnabled = false
            };
            dt.Tick += Timer_Tick;

            tbRecord.Content = $"Record: {GetMaxScore()}";
            SetGameField();
        }

        private static string GetVisualCellValue(byte value)
        {
            if (value == 0)
            {
                return "";
            }
            else
            {
                return Game.GetCellValue(value).ToString();
            }
        }

        private (byte, byte, byte) GetCellBackColor(byte value)
        {
            return cellColors[value];
        }

        private (byte, byte, byte) GetCellTextColor(byte value)
        {
            return textColors[value < 3 ? 0 : 1];
        }

        private void SetGameField()
        {
            SetCellValues();
            SetCellColors();
        }

        private void SetCellValues()
        {
            var gameField = gameInfo.Field;

            tbr0c0.Text = GetVisualCellValue(gameField[0, 0]);
            tbr0c1.Text = GetVisualCellValue(gameField[0, 1]);
            tbr0c2.Text = GetVisualCellValue(gameField[0, 2]);
            tbr0c3.Text = GetVisualCellValue(gameField[0, 3]);
            tbr1c0.Text = GetVisualCellValue(gameField[1, 0]);
            tbr1c1.Text = GetVisualCellValue(gameField[1, 1]);
            tbr1c2.Text = GetVisualCellValue(gameField[1, 2]);
            tbr1c3.Text = GetVisualCellValue(gameField[1, 3]);
            tbr2c0.Text = GetVisualCellValue(gameField[2, 0]);
            tbr2c1.Text = GetVisualCellValue(gameField[2, 1]);
            tbr2c2.Text = GetVisualCellValue(gameField[2, 2]);
            tbr2c3.Text = GetVisualCellValue(gameField[2, 3]);
            tbr3c0.Text = GetVisualCellValue(gameField[3, 0]);
            tbr3c1.Text = GetVisualCellValue(gameField[3, 1]);
            tbr3c2.Text = GetVisualCellValue(gameField[3, 2]);
            tbr3c3.Text = GetVisualCellValue(gameField[3, 3]);
        }

        private void SetCellColors()
        {
            var gameField = gameInfo.Field;

            var backColor = GetCellBackColor(gameField[0, 0]);
            var foreColor = GetCellTextColor(gameField[0, 0]);
            cellr0c0.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr0c0.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[0, 1]);
            foreColor = GetCellTextColor(gameField[0, 1]);
            cellr0c1.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr0c1.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[0, 2]);
            foreColor = GetCellTextColor(gameField[0, 2]);
            cellr0c2.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr0c2.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[0, 3]);
            foreColor = GetCellTextColor(gameField[0, 3]);
            cellr0c3.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr0c3.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[1, 0]);
            foreColor = GetCellTextColor(gameField[1, 0]);
            cellr1c0.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr1c0.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[1, 1]);
            foreColor = GetCellTextColor(gameField[1, 1]);
            cellr1c1.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr1c1.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[1, 2]);
            foreColor = GetCellTextColor(gameField[1, 2]);
            cellr1c2.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr1c2.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[1, 3]);
            foreColor = GetCellTextColor(gameField[1, 3]);
            cellr1c3.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr1c3.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[2, 0]);
            foreColor = GetCellTextColor(gameField[2, 0]);
            cellr2c0.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr2c0.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[2, 1]);
            foreColor = GetCellTextColor(gameField[2, 1]);
            cellr2c1.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr2c1.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[2, 2]);
            foreColor = GetCellTextColor(gameField[2, 2]);
            cellr2c2.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr2c2.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[2, 3]);
            foreColor = GetCellTextColor(gameField[2, 3]);
            cellr2c3.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr2c3.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[3, 0]);
            foreColor = GetCellTextColor(gameField[3, 0]);
            cellr3c0.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr3c0.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[3, 1]);
            foreColor = GetCellTextColor(gameField[3, 1]);
            cellr3c1.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr3c1.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[3, 2]);
            foreColor = GetCellTextColor(gameField[3, 2]);
            cellr3c2.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr3c2.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
            backColor = GetCellBackColor(gameField[3, 3]);
            foreColor = GetCellTextColor(gameField[3, 3]);
            cellr3c3.Background = new SolidColorBrush(Color.FromRgb(backColor.Item1, backColor.Item2, backColor.Item3));
            tbr3c3.Foreground = new SolidColorBrush(Color.FromRgb(foreColor.Item1, foreColor.Item2, foreColor.Item3));
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            int time = (int)(DateTime.Now - gameStart).TotalSeconds; 

            if (time == 7200)
            {
                dt.Stop();
                gameInfo.LoseGame();
                _ = MessageBox.Show("Time's up!", "Game over", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
            }

            tbTimeSpan.Content = $"{time / 60:d2}:{time % 60:d2}";
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            gameOverBack.Visibility = Visibility.Hidden;
            gameOverText.Visibility = Visibility.Hidden;

            if (dt.IsEnabled)
            {
                dt.Stop();
            }

            gameInfo = new Game();
            SetGameField();
            tbRecord.Content = $"Record: {GetMaxScore()}";
            tbScore.Content = gameInfo.Score.ToString();
            tbTimeSpan.Content = "start game...";
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.W || e.Key == System.Windows.Input.Key.Up)
            {
                HandleKeyPress(Game.Direction.Up);
            }

            if (e.Key == System.Windows.Input.Key.S || e.Key == System.Windows.Input.Key.Down)
            {
                HandleKeyPress(Game.Direction.Down);
            }

            if (e.Key == System.Windows.Input.Key.A || e.Key == System.Windows.Input.Key.Left)
            {
                HandleKeyPress(Game.Direction.Left);
            }

            if (e.Key == System.Windows.Input.Key.D || e.Key == System.Windows.Input.Key.Right)
            {
                HandleKeyPress(Game.Direction.Right);
            }
        }

        private void HandleKeyPress(Game.Direction direction)
        {
            var state = gameInfo.HandleSwipe(direction);

            tbScore.Content = gameInfo.Score.ToString();
            SetGameField();

            if (state == Game.State.Losed)
            {
                dt.Stop();

                if (gameInfo.Score > GetMaxScore())
                {
                    MessageBox.Show($"Congrats! You set a new record of {gameInfo.Score}!", "New record!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
                }

                SetNewMaxScore(gameInfo.Score, (int)(DateTime.Now - gameStart).TotalSeconds, false);
                gameOverText.Content = "you lose!";
                gameOverBack.Visibility = Visibility.Visible;
                gameOverText.Visibility = Visibility.Visible;
                MessageBox.Show("You lose!", "Game over", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);

            }

            if (state == Game.State.Won)
            {
                dt.Stop();
                SetNewMaxScore(gameInfo.Score, (int)(DateTime.Now - gameStart).TotalSeconds, true);
                gameOverText.Content = "victory!";
                gameOverBack.Visibility = Visibility.Visible;
                gameOverText.Visibility = Visibility.Visible;
                MessageBox.Show("You won!", "Victory", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.None);
            }

            if (state == Game.State.Running && !dt.IsEnabled)
            {
                gameStart = DateTime.Now - halfSecond;
                dt.IsEnabled = true;
            }
        }
        
        private static void CreateTableScoreData()
        {
            string connStr = @"Data Source=C:\ProgramData\My2048data\ScoreData.sqlite;";
            SQLiteConnection connection = new(connStr);
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand command = new("create table if not exists ScoreHistory (PlayTime text not null primary key, Score int not null, GameDuration text not null, IsVictory int not null)", connection);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        private static void SetNewMaxScore(long score, int seconds, bool isVictory)
        {
            string connStr = @"Data Source=C:\ProgramData\My2048data\ScoreData.sqlite;";
            SQLiteConnection connection = new(connStr);
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand command = new("insert into ScoreHistory values" +
                    $"('{((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()}', $Score, $Duration, $IsVictory)", connection);
                SQLiteParameter paramScore = new("$Score", score);
                SQLiteParameter paramDuration = new("$Duration", $"{seconds / 3600:d2}{seconds / 60 % 60:d2}{seconds % 60:d2}");
                SQLiteParameter paramIsVictory = new("$IsVictory", isVictory ? 1 : 0);
                command.Parameters.AddRange(new SQLiteParameter[] { paramScore, paramDuration, paramIsVictory });

                _ = command.ExecuteNonQuery();
            }

            connection.Close();
        }

        private static long GetMaxScore()
        {
            string connStr = @"Data Source=C:\ProgramData\My2048data\ScoreData.sqlite;";
            SQLiteConnection connection = new(connStr);
            connection.Open();
            long maxScore = 0;

            if (connection.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmdMaxScore = new("select max(Score) from ScoreHistory", connection);
                maxScore = Convert.ToInt64(Convert.IsDBNull(cmdMaxScore.ExecuteScalar()) ? 0 : cmdMaxScore.ExecuteScalar());
            }

            connection.Close();
            return maxScore;
        }
    }
}
