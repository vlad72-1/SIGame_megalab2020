using SIGameLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Rocket
{
    [Guid("6C5B9A9F-B340-4415-ACAB-0E1B52A622A4")]
    public interface IRocket  // Интерфейс ракета
    {
        Point Position { get; }  // Позиция

        dynamic Assign(params object[] parameters);  // Инициализация переменных
        void Launch();  // Запуск ракеты
        void Destroy();  // Уничтожить ракету
        void Draw(DBCharScreen screen);  // Нарисовать ракету
    }
}
