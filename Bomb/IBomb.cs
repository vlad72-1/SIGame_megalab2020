using SIGameLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Bomb
{
    [Guid("2B818030-4980-45BE-A434-766B149851D7")]  
    public interface IBomb  // Интерфейс бомбы
    {
        Point Position { get; }  // Позиция

        dynamic Assign(params object[] parameters);  // Инициализация переменных
        void Drop();  // Сбросить бомбу
        void Destroy();  // Уничтожить бомбу
        void Draw(DBCharScreen screen);  // Нарисовать бомбу
    }
}
