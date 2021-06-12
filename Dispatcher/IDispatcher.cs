using SIGameLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Dispatcher
{
    [Guid("F871A151-4CE4-49A4-914E-86692AC12EE4")]  // GUID Компонента
    public interface IDispatcher  // Интерфейс диспетчера
    {
        Semaphore Semaphore { get; }  // Семафор
        int Width { get; }  // Ширина
        int Height { get; }  // Высота
        List<dynamic> Rockets { get; }  // Ракеты
        List<dynamic> Bombs { get; }  // Бомбы

        Point LeftInvaderPosition { get; }  // Самый левый захватчик
        Point RightInvaderPosition { get; }  // Самый правый захватчик

        dynamic Assign(params object[] parameters);  // Инициализация переменных
        void Start(); // Запуск диспетчера
        void Kill();  // Убить поток диспетчера
        void Send(PressedKeys keys);  // Отправить нажатые клавишы диспетчеру
        void Synchronize();  // Синхронизация захватчиков
        void DestroyBomb(dynamic bomb);  // Уничтожить бомбу
        void DestroyRocket(dynamic rocket);  // Уничтожить ракету
        void LaunchRocket(int x, int y);  // Запустить ракету   
        void DropBomb(int x, int y);  // Сбросить бомбу
    }
}
