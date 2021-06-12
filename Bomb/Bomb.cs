using SIGameLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Bomb
{
    [ProgId("SIGame.Component.Bomb"), Guid("645A0CB1-1E57-4E65-A3D9-99F57BC30F4F"),
       ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IBomb))]
    [ComVisible(true)]
    public class Bomb : IBomb  // Бомба
    {
        private int _x;  // X координата
        private int _y;  // Y координата
        private bool _alive;  // Жив ли поток
        private int _delay;  // Задержка между действиями
        private Thread _thread;  // Поток
        private dynamic _dispatcher;  // Диспетчер

        public Point Position { get => new Point(_x, _y); }  // Позиция

        public dynamic Assign(params object[] parameters)  // Инициализация переменных
        {
            if (parameters.Length != 4)
                throw new ArgumentException("Ivalid argument number!");
            _dispatcher = parameters[0];
            _x = (int)parameters[1];
            _y = (int)parameters[2];
            _delay = (int)(150 / (float)parameters[3]);
            return this;
        }

        private void ThreadLoop()  // Цикл потока
        {
            while (_alive)  
            {
                if (_y <= _dispatcher.Height - 4)  
                    _y++;  
                else
                    _dispatcher.DestroyBomb(this);
                Thread.Sleep(_delay);  
            }
        }

        public void Drop()  // Скинуть бомбу
        {
            _alive = true;  
            _thread = new Thread(ThreadLoop);  
            _thread.Start();  
        }

        public void Destroy()  // Уничтожить бомбу
        {
            _alive = false;  
        }

        public void Draw(DBCharScreen screen)  // Нарисовать бомбу
        {
            screen.Draw(_x, _y, "+", ConsoleColor.Green);  
        }
    }
}
