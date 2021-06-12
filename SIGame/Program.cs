using SIGameLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SIGame
{
    class Program
    {
        private static PressedKeys keys;
        private const int WindowWidth = 45;
        private const int WindowHeight = 25;

        [STAThread]
        static void Main()
        {
            ConsoleExtension.DisableCursor();  // Отключить отображение курсора
            ConsoleExtension.DisableResizing();  // Отключить изменение размера консоли
            ConsoleExtension.ResizeWindow(WindowWidth, WindowHeight);  // Изменить размер консоли

            var levels = new List<LevelSettings>  // Настройки уровней
            {
                new LevelSettings{InvaderInRow = 6, InvaderInColumn = 4,
                    InvaderSpeed = 1.2f, InvaderFireSpeed = 2.0f, TrolleyFireSpeed = 0.5f,
                    InvaderAcc = 1, BombSpeed = 1, RocketSpeed = 3f, TrolleyStrength = 3},
                new LevelSettings{InvaderInRow = 7, InvaderInColumn = 4, InvaderSpeed = 2.5f,
                    InvaderFireSpeed = 1.6f, TrolleyFireSpeed = 0.4f,
                    InvaderAcc = 1.025f, BombSpeed = 1.2f, RocketSpeed = 3f, TrolleyStrength = 2},
                new LevelSettings{InvaderInRow = 8, InvaderInColumn = 4, InvaderSpeed = 2.0f, 
                    InvaderFireSpeed = 2.0f, TrolleyFireSpeed = 0.3f,  
                    InvaderAcc = 1.05f, BombSpeed = 1.4f, RocketSpeed = 2f, TrolleyStrength = 1}
            };
            var screen = new DBCharScreen(WindowWidth, WindowHeight);  // Создание виртуального экрана для отображения
            dynamic dispatcher = ObjectCreator.Create("SIGame.Components.Dispatcher", levels, screen);  // Диспетчер
            dispatcher.Start();  // Запуск диспетчера
            while (((keys = Input.GetPressedKeys()) & PressedKeys.Escape) != PressedKeys.Escape)  // Получить нажатые клавиши
            {
                dispatcher.Send(keys);  // Передать диспетчеру нажатые клавиши
                Thread.Sleep(25);  // Задержка
            }
        }
    }
}
