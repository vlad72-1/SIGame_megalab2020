using SIGameLibrary;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Invader
{
    [ProgId("SIGame.Component.Invader"), Guid("4A8B61B3-6A21-43D0-AE9B-64410B23BFBE"),
        ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IInvader))]
    [ComVisible(true)]
    public class Invader : IInvader  // Захватчик
    {
        private int _x;  // X координата
        private int _y;  // Y координата
        private int _sx;  // Начальная X координата
        private int _sy;  // Начальная Y координата
        private int _bombDelay;  // Задержка между движениеми бомбы
        private int _actionDelay;  // Задержка между действиями захватчика
        private int _frame;  // Кадр анимации (0, 1)
        private bool _alive;  // Жив ли поток
        private float _bombSpeed;  // Скорость бомб
        private float _acc;  // Ускорение снижения
        private dynamic _dispatcher;  // Диспетчер
        private Thread _thread;   // Поток      
        private Stopwatch _timerMove;  // Тимер для движения корабля
        private Random _random;  // Рандом
        private int _state;  // Состояние

        public Point Position => new Point(_x, _y);  // Позиция

        public event EventHandler OnDeath;  // Событие при смерти
        public event EventHandler OnDescent;  // Событие при снижении до земли

        public dynamic Assign(params object[] parameters)  // Инициализация переменных
        {
            if (parameters.Length != 7)
                throw new ArgumentException("Ivalid argument number!");
            _dispatcher = parameters[0];
            _x = (int)parameters[1];
            _y = (int)parameters[2];
            _sx = (int)parameters[1];
            _sy = (int)parameters[2];
            _bombDelay = (int)(1000 / (float)parameters[3]);
            _random = new Random(_x * _y * 10000);
            _timerMove = new Stopwatch();
            _state = 2;
            _bombSpeed = (float)parameters[6];
            _frame = 0;
            _acc = (float)parameters[5];
            _actionDelay = (int)(500 / (float)parameters[4]);
            return this;
        }

        private void ThreadLoop()  // Цикл потока
        {
            Thread.Sleep(1000);
            while (_alive)  
            {
                _dispatcher.Semaphore.WaitOne();  
                if (_y >= _dispatcher.Height - 4)  
                {
                    OnDescent?.Invoke(this, EventArgs.Empty);  
                    _alive = false;  
                }
                lock (_dispatcher.Rockets)  
                {
                    for (int i = _dispatcher.Rockets.Count - 1; i >= 0; i--)  
                    {
                        lock (_dispatcher.Rockets[i])  
                        {
                            if (_dispatcher.Rockets[i].Position.X >= _x && 
                                _dispatcher.Rockets[i].Position.X < _x + 2 &&
                                _dispatcher.Rockets[i].Position.Y == _y)
                            {
                                _alive = false;  
                                _dispatcher.DestroyRocket(_dispatcher.Rockets[i]);
                                OnDeath?.Invoke(this, EventArgs.Empty);  
                                break;
                            }

                        }
                    }
                }
                if (_timerMove.ElapsedMilliseconds >= _actionDelay)  
                {
                    var rightPosition = (Point)_dispatcher.RightInvaderPosition;
                    var leftPosition = (Point)_dispatcher.LeftInvaderPosition;
                    if (_alive)  
                    {
                        switch (_state)  
                        {
                            case 0:  
                                if (_x - _sx > 0)  
                                {
                                    _x--;   
                                    _frame++;   
                                    _dispatcher.Synchronize();  
                                }
                                else
                                    _state = 1;  
                                break;
                            case 1:  
                                _y++;
                                _frame++;  
                                _dispatcher.Synchronize();  
                                _actionDelay = (int)(_actionDelay / _acc);  
                                _state = 2;  
                                break;
                            case 2:  
                                if (_x + rightPosition.X - _sx + 2 < _dispatcher.Width)
                                {
                                    _x++;
                                    _frame++;  
                                    _dispatcher.Synchronize();  
                                }
                                else
                                    _state = 3;  
                                break;
                            case 3:  
                                _y++;
                                _frame++;  
                                _dispatcher.Synchronize();  
                                _actionDelay = (int)(_actionDelay / _acc);  
                                _state = 0;  
                                break;
                        }
                    }
                    _timerMove.Restart();  
                }
                if (_random.Next(1000000) < _bombDelay)  
                {
                    _dispatcher.DropBomb(_x + 1, _y + 1);
                }
                if (_alive)  
                {
                    Thread.Sleep(50);  
                    _dispatcher.Semaphore.Release();  
                }
            }
        }

        public void Start()  // Запуск захватчика
        {
            _alive = true;  // Утсновка флага
            _timerMove.Start();  // Запуск часов
            _thread = new Thread(ThreadLoop);  // Создание потока
            _thread.Start();
        }

        public void Kill()  // Убить поток завхватчка
        {
            _alive = false; 
        }

        public void Draw(DBCharScreen screen)  // Нарисовать захватчика
        {
            screen.Draw(_x, _y, _frame % 2 == 0 ? "><" : "][");
        }
    }
}
