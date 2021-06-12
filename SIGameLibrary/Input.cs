using System.Diagnostics;
using System.Windows.Input;

namespace SIGameLibrary
{
    public class Input  // Ввод
    {
        public static PressedKeys GetPressedKeys()  // Проверка нажатых клавиш
        {
            PressedKeys pressedKeys = PressedKeys.None;
            if (Keyboard.IsKeyDown(Key.Escape))
                pressedKeys |= PressedKeys.Escape;
            if (Keyboard.IsKeyDown(Key.Enter))
                pressedKeys |= PressedKeys.Enter;
            if (Keyboard.IsKeyDown(Key.Right))
                pressedKeys |= PressedKeys.RightArrow;
            if (Keyboard.IsKeyDown(Key.Left))
                pressedKeys |= PressedKeys.LeftArrow;
            if (Keyboard.IsKeyDown(Key.Space))
                pressedKeys |= PressedKeys.Space;
            return pressedKeys;
        }
    }
}
