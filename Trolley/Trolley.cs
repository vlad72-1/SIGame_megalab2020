using SIGameLibrary;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Trolley
{
    [ProgId("SIGame.Component.Trolley"), Guid("72F447C4-0185-42C8-AADF-CB4E9797D611"),
        ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(ITrolley))]
    [ComVisible(true)]
    public class Trolley : ITrolley  // Класс реализующий тачанку
    {
        private int _x;  // X координата
        private int _y;  // Y координата
        private bool _alive;  // Жив ли поток
        private int _delay;  // Задержка между действиями
        private int _strength;  // Прочность
        private int _fullStrength;  // Максимальная прочность
        private float _rocketSpeed;  // Скорость снарядов
        private dynamic _dispatcher;  // Диспетчер COM объектов
        private Stopwatch _timerShots;  // Тамер для частоты выстрелов
        private Stopwatch _timesStrength;   // Часы для восстановления прочности
        private Thread _thread;  // Поток

        public int Strength => _strength;  // Запас прочности

        public event EventHandler OnDeath;  // Событие смерти
         
        public dynamic Assign(params object[] parameters)  // Инициализация переменных
        {
            if (parameters.Length != 6)
                throw new ArgumentException("Ivalid argument number!");
            _strength = (int)parameters[5];  
            _dispatcher = parameters[0];  
            _rocketSpeed = (float)parameters[4];  
            _x = (int)parameters[1];  
            _y = (int)parameters[2];  
            _fullStrength = (int)parameters[5];  
            _timerShots = new Stopwatch();  
            _timesStrength = new Stopwatch();  
            _delay = (int)(500 / (float)parameters[3]);  
            return this;
        }

        private void ThreadLoop()  // Цикл потока
        {
            while (_alive)  
            {
                if (_timesStrength.ElapsedMilliseconds > 8000)  
                {
                    if (_strength < _fullStrength)  
                        _strength++;  
                    _timesStrength.Restart();  
                }
                lock (_dispatcher.Bombs)  
                {
                    for (int i = _dispatcher.Bombs.Count - 1; i >= 0; i--)  
                    {
                        lock (_dispatcher.Bombs[i])  
                        {
                            if (_dispatcher.Bombs[i].Position.X >= _x &&   
                                _dispatcher.Bombs[i].Position.X < _x + 2 &&
                                _dispatcher.Bombs[i].Position.Y == _y)
                            {
                                _dispatcher.DestroyBomb(_dispatcher.Bombs[i]);
                                _strength--;  
                                if (_strength == 0)  
                                {
                                    _alive = false;  
                                    OnDeath?.Invoke(this, EventArgs.Empty);  
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Start()  // Запуск тачанки
        {
            _alive = true;  
            _timerShots.Start();  
            _timesStrength.Start();  
            _thread = new Thread(ThreadLoop);
            _thread.Start();  
        }

        public void Kill()  // Убить поток тачанки
        {
            _alive = false; 
        }

        public void Send(PressedKeys keys)  // Отправка нажатых клавиш
        {
            if ((keys & PressedKeys.Space) == PressedKeys.Space && _timerShots.ElapsedMilliseconds >= _delay)  
            {
                dynamic rocket = ObjectCreator.Create("SIGame.Component.Rocket", _dispatcher, _x + 1, _y - 1, _rocketSpeed);
                rocket.Launch();  
                lock (_dispatcher.Rockets)  
                {
                    _dispatcher.Rockets.Add(rocket);  
                }
                _timerShots.Restart();  
            }
            if ((keys & PressedKeys.LeftArrow) == PressedKeys.LeftArrow && _x > 0)  
                _x--;  
            if ((keys & PressedKeys.RightArrow) == PressedKeys.RightArrow && _x < _dispatcher.Width - 4)  
                _x++;  
        }

        public void Draw(DBCharScreen screen)  // Нарисовать тачанку
        {
            screen.Draw(_x, _y, @"/\");
        }
    }
}
