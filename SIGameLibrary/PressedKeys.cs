namespace SIGameLibrary
{
    public enum PressedKeys  // Нажатые клавиши
    {   
        Space = 0b00001,  // Пробер
        Enter = 0b00010,  // Enter
        RightArrow = 0b00100,  // Правая стрелка
        LeftArrow = 0b01000,    // Левая стрелка
        Escape = 0b10000,  // Escape
        None = 0b00000   // Не нажата ни одна клавиша
    }
}
