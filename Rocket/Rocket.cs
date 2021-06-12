using SIGameLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Rocket
{
    [ProgId("SIGame.Component.Rocket"), Guid("792D8170-D68B-4CC0-8100-7CF280BC0789"),
        ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IRocket))]
    [ComVisible(true)]
    public class Rocket : IRocket  // Ракета
    {
        private int _x;  // X координата
        private int _y;  // Y координата
        private bool _alive;  // Жив ли поток
        private int _delay;  // Задержка между движением ракеты
        private Thread _thread;  // Поток
        private dynamic _dispatcher;  // Диспетчер

        public Point Position { get => new Point(_x, _y); }  // Позиция

        public dynamic Assign(params object[] parameters)  // Инициализация объекта снаряда
        {
            if (parameters.Length != 4)
                throw new ArgumentException("Ivalid argument number!");
            _dispatcher = (dynamic)parameters[0];
            _x = (int)parameters[1];  
            _y = (int)parameters[2];  
            _delay = (int)(150 / (float)parameters[3]);  
            return this;
        }

        private void ThreadLoop()  // Цикл потока
        {
            while (_alive)  
            {
                if (_y >= 0)  
                    _y--;  
                else
                    _dispatcher.DestroyRocket(this);
                Thread.Sleep(_delay);  
            }
        }

        public void Launch()  // Запуск ракеты
        {
            _alive = true;  
            _thread = new Thread(ThreadLoop);  
            _thread.Start();  
        }

        public void Destroy()  // Уничтожить ракету
        {
            _alive = false;
        }

        public void Draw(DBCharScreen screen)  // Нарисовать ракету
        {
            screen.Draw(_x, _y, "*", ConsoleColor.Green);
        }
    }
}
