using SIGameLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Dispatcher
{
    [ProgId("SIGame.Components.Dispatcher"), Guid("813D7AE9-87FF-4E39-82A9-B13933AF7A6E"),
       ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDispatcher))]
    [ComVisible(true)]
    public class Dispatcher : IDispatcher  // Диспетчер 
    {
        private int _synCount;  // Счетчик синхронизации
        private int _levelIndex;  // Индекс текущего уровня
        private bool _alive;  // Жив ли поток
        private bool _win;  // Выиграна ли игра
        private bool _lost;  // Проиграна ли игра
        private object _synLocker;  // Объект синхронизации кораблей
        private dynamic _trolley;  // Тачанка
        private List<LevelSettings> _levels;  // Настройки уровней
        private Thread _thread;  // Поток
        private List<dynamic> _rockets;  // Ракеты
        private List<dynamic> _bombs;  // Бомбы
        private List<dynamic> _invaders;  // Захватчики
        private Semaphore _semaphore;  // Семафор
        private DBCharScreen _screen;  // Виртуальный экран
        private Point _leftInvaderPosition;  // Самый левый захватчик
        private Point _rightInvaderPosition;  // Самый правый захватчик
        private int _score;  // Счет

        public Semaphore Semaphore => _semaphore;  // Семафор
        public List<LevelSettings> Levels => _levels;  // Настройки уровней
        public List<dynamic> Rockets => _rockets;  // Ракеты
        public List<dynamic> Bombs => _bombs;  // Бомбы
        public int Width => _screen.Width;  // Ширина
        public int Height => _screen.Height;  // Высота
        public Point LeftInvaderPosition => _leftInvaderPosition;  // Самый левый захватчик
        public Point RightInvaderPosition => _rightInvaderPosition;  // Самый правый захватчик

        public dynamic Assign(params object[] parameters)  // Инициализация переменных
        {
            if (parameters.Length != 2)
                throw new ArgumentException("Ivalid argument number!");
            _screen = (DBCharScreen)parameters[1];
            _levels = (List<LevelSettings>)parameters[0];  
            _synLocker = new object();  
            _synCount = 1;  
            _trolley = ObjectCreator.Create("SIGame.Component.Trolley", this, _screen.Width / 2 - 1, _screen.Height - 4, 
                _levels[_levelIndex].TrolleyFireSpeed, _levels[_levelIndex].RocketSpeed, _levels[_levelIndex].TrolleyStrength);  
            _rockets = new List<dynamic>();   
            _bombs = new List<dynamic>();
            _invaders = new List<dynamic>();  
            _levelIndex = 0;  
            _win = false;  
            _lost = false;  
            _trolley.OnDeath += new EventHandler(OnDeathTrolley);  
            _score = 0;
            if (_levelIndex <= _levels.Count)  
                NextLevel(_levels[_levelIndex++]);  
            return this;
        }

        private void OnDeathTrolley(object sender, EventArgs e)  // Обработка собятия смерти тачанки
        {
            lock (_rockets)  
            {
                foreach (var rocket in _rockets)  
                    rocket.Destroy();  
            }
            lock (_invaders)  
            {
                foreach (var invader in _invaders)  
                    invader.Kill();  
            }

            _lost = true;  
        }

        private void OnDeathInvader(object sender, EventArgs args)  // Обработка события смерти захватчика
        {
            lock (_invaders)  
            {
                _invaders.Remove(sender);  
                _score += _levelIndex;
            }
        }

        private void Show()  // Отображение игры в консоль
        {
            _screen.Clear();
            if (_lost)
            {
                var lost = $"GAME LOST!";
                _screen.Draw(_screen.Width / 2 - lost.Length / 2, _screen.Height / 2 - 1, lost);
            }
            else if (_win)
            {
                var win = $"GAME WIN!";
                _screen.Draw(_screen.Width / 2 - win.Length / 2, _screen.Height  / 2 - 1, win);
            }
            else
            {
                for (int i = 0; i < _bombs.Count; i++)  
                    _bombs[i].Draw(_screen);  
                for (int i = 0; i < _rockets.Count; i++)  
                    _rockets[i].Draw(_screen);  
                _trolley.Draw(_screen);  
                for (int i = 0; i < _invaders.Count; i++)  
                    _invaders[i].Draw(_screen);  
                _screen.Draw(0, _screen.Height - 2, new string('-', _screen.Width));
                var strenght = new string('@', _trolley.Strength);
                _screen.Draw(0, _screen.Height - 1, strenght, ConsoleColor.Red);
                var level = $"LEVEL {_levelIndex}";
                _screen.Draw(_screen.Width - level.Length - 1, _screen.Height - 1, level);
                var score = $"SCORE: {_score}";
                _screen.Draw(_screen.Width / 2 - score.Length / 2, _screen.Height - 1, score);
            }
            _screen.Show();  
        }

        public void Send(PressedKeys keys)  // Отправить нажатые клавишы диспетчеру
        {
            _trolley.Send(keys);  
        }

        private void ThreadLoop()  // Цикл потока
        {
            while (_alive)  
            {
                if (_invaders.Count == 0)
                {
                    if (_levelIndex >= _levels.Count)
                        _win = true;
                    else
                        NextLevel(_levels[_levelIndex++]);
                }
                if (!_win && !_lost)  
                {
                    lock (_rockets)  
                    {
                        for (int i = _rockets.Count - 1; i >= 0; i--)  
                        {
                            if (_rockets[i].Position.Y < 2 || _rockets[i].Position.Y > _screen.Height)  
                            {
                                _rockets[i].Destroy();  
                                _rockets.RemoveAt(i);  
                            }
                        }
                    }               
                }
                Show();  
                Thread.Sleep(50);  
            }
        }

        public void Start()  // Запуск диспетчера
        {
            _alive = true;  
            _trolley.Start();  
            _thread = new Thread(ThreadLoop);  
            _thread.Start();  
        }

        public void Kill()  // Убить поток диспетчера
        {
            _alive = false;  
            lock (_invaders)  
            {
                foreach (var invader in _invaders)  
                    invader.Kill();  
            }
            lock (_rockets)  
            {
                foreach (var rocket in _rockets)  
                    rocket.Destroy();  
            }
            _trolley.Kill();  
        }

        private void NextLevel(LevelSettings level)  // Перейти к уровню
        {
            lock (_invaders)  
            {
                foreach (var invader in _invaders)  
                    invader.Kill();  
            }
            lock (_rockets)  
            {
                foreach (var rocket in _rockets)  
                    rocket.Destroy();  
            }
            _invaders.Clear();  
            _rockets.Clear();  

            _lost = false;  
            _trolley.Kill();  
            _trolley.Assign(this, _screen.Width / 2 - 1, _screen.Height - 4,
                level.TrolleyFireSpeed, level.RocketSpeed, level.TrolleyStrength);

            _trolley.Start();  
            var shipNumber = level.InvaderInRow * level.InvaderInColumn;  
            _semaphore = new Semaphore(shipNumber, shipNumber);  
            for (int i = 0; i < level.InvaderInRow; i++)  
            {
                for (int j = 0; j < level.InvaderInColumn; j++)  
                {
                    dynamic invader = ObjectCreator.Create("SIGame.Component.Invader", this, i * 3, 1 + j * 2, 
                        level.InvaderFireSpeed, level.InvaderSpeed, level.InvaderAcc, level.BombSpeed);  
                    invader.OnDeath += new EventHandler(OnDeathInvader);  
                    invader.OnDescent += new EventHandler(OnDescentInvader);  
                    _invaders.Add(invader);  
                }
            }
            var left = _invaders.OrderBy(item => item.Position.X).FirstOrDefault();
            if (left != null)
                _leftInvaderPosition = left.Position;

            var right = _invaders.OrderByDescending(item => item.Position.X).FirstOrDefault();
            if (right != null)
                _rightInvaderPosition = right.Position;

            foreach (dynamic invader in _invaders)
                invader.Start();
        }

        private void OnDescentInvader(object sender, EventArgs e)  // Обработка события "захватчик коснулся земли"
        {
            lock (_invaders)  
            {
                foreach (var invader in _invaders)  
                    invader.Kill();  
            }
            lock (_rockets)  
            {
                foreach (var rocket in _rockets)  
                    rocket.Destroy();  
            }
            _lost = true;  
        }

        public void Synchronize()  // Синхронизация захватчиков
        {
            lock (_synLocker)  
            {
                _synCount++;  
                lock (_invaders)  
                {
                    if (_synCount == _invaders.Count + 1)  
                        _synCount = 1;  
                }
            }
            while (true)  
            {
                lock (_synLocker) 
                {
                    if (_synCount == 1)  
                        break;  
                }
                Thread.Sleep(1);  
            }          
        }

        public void DestroyBomb(dynamic bomb)  // Уничтожить бомбу
        {
            lock (_bombs)
            {
                bomb.Destroy();
                _bombs.Remove(bomb);
            }
        }

        public void DestroyRocket(dynamic rocket)  // Уничтожить ракету
        {
            lock (_rockets)
            {
                rocket.Destroy();
                _rockets.Remove(rocket);
            }
        }
        public void LaunchRocket(int x, int y)  // Запустить ракету
        {
            var rocket = ObjectCreator.Create("SIGame.Component.Rocket", this, x, y, _levels[_levelIndex].RocketSpeed);
            rocket.Launch();
            lock (_rockets)
            {
                _rockets.Add(rocket);
            }
        }

        public void DropBomb(int x, int y)  // Сбросить бомбу
        {
            var bomb = ObjectCreator.Create("SIGame.Component.Bomb", this, x, y, _levels[_levelIndex].BombSpeed);
            bomb.Drop();
            lock (_bombs)
            {
                _bombs.Add(bomb);
            }
        }
    }
}
