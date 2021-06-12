using SIGameLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Invader
{
    [Guid("1389EEEA-9C4F-41E6-938E-602077FCA4F0")]
    public interface IInvader  // Интерфейс захватчика
    {
        event EventHandler OnDeath;  // Событие при смерти захватчика

        event EventHandler OnDescent;  // Событие при снижении до земли

        Point Position { get; }  // Позиция

        dynamic Assign(params object[] parameters);  // Инициализация переменных
        void Start();  // Запуск захватчика
        void Kill();  // Убить поток завхватчка
        void Draw(DBCharScreen screen);  // Нарисовать захватчика
    }
}
