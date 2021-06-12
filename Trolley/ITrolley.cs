using SIGameLibrary;
using System;
using System.Runtime.InteropServices;

namespace Trolley
{
    [Guid("D6699EB1-BE88-4A16-9B65-F479C5DCAD9D")]  // GUID Компонента
    public interface ITrolley  // Интерфейс тачанки
    {
        event EventHandler OnDeath;  // Событие при смерти
        int Strength { get; }  // Прочность

        dynamic Assign(params object[] parameters);  // Инициализация переменных
        void Start();  // Запуск тачанки
        void Kill();  // Убить поток тачанки
        void Send(PressedKeys keys);  // Отправить нажатые клавиши
        void Draw(DBCharScreen screen);  // Нарисовать тачанку
    }
}
