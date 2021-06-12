using System;

namespace SIGameLibrary
{
    public class DBCharScreen  // Виртуальный экран
    {
        private class DBCharPixel  // Виртуальный пиксель
        {
            public class Data  // Символ и цвет
            {
                public char Pixel { set; get; }
                public ConsoleColor Color { set; get; }
            }

            private Data _pixel;  // Символ
            private Data _buffer;  // Буфер

            public bool IsNew => _pixel.Pixel != _buffer.Pixel || _pixel.Color != _pixel.Color;  // Проверка изменение пикселя

            public DBCharPixel(char pixel = ' ', ConsoleColor color = ConsoleColor.White)  // Конструктор
            {
                _pixel = new Data {Pixel = pixel, Color = color};
                _buffer = new Data { Pixel = pixel, Color = color};
            }

            public void Set(char pixel = ' ', ConsoleColor color = ConsoleColor.White)  // Установка пикселя
            {
                _pixel = new Data { Pixel = pixel, Color = color };
            }

            public Data Get()  // Получить данные пикселя
            {
                _buffer = _pixel;
                return _pixel;
            }
        }

        private readonly DBCharPixel[,] _buffer;  // Буфер экрана (для двойной буфферизации)

        public int Width => _buffer.GetLength(0);  // Ширина
        public int Height => _buffer.GetLength(1);  // Высота

        public DBCharScreen(int width, int height)  // Конструктор
        {
            _buffer = new DBCharPixel[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _buffer[x, y] = new DBCharPixel();
        }

        public void Clear()  // Очистить
        {
            foreach (var pixel in _buffer)
                pixel.Set();
        }

        public void Draw(int x, int y, string pixels, ConsoleColor color = ConsoleColor.White)  // Нарисовать что-либо
        {
            var height = _buffer.GetLength(1);
            if (y >= 0 && y < height && x >= 0)
            {
                var width = _buffer.GetLength(0);
                for (int i = x, c = 0; i < width && c < pixels.Length; i++, c++)
                    _buffer[i, y].Set(pixels[c], color);
            }
        }

        public void Show()  // Отобразить в консоль
        {
            var width = _buffer.GetLength(0);
            var height = _buffer.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_buffer[x, y].IsNew)
                    {
                        Console.SetCursorPosition(x, y);
                        var pixel = _buffer[x, y].Get();
                        Console.ForegroundColor = pixel.Color;
                        Console.Write(pixel.Pixel);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
