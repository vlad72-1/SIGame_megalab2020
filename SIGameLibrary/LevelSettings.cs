namespace SIGameLibrary
{
    public class LevelSettings  // Настройки уровня
    {
        public float BombSpeed { get; set; }  // Скорость бомб
        public float RocketSpeed { get; set; }  // Скорость снарядов
        public int InvaderInColumn { get; set; }  // Количество кораблей в столбце
        public float InvaderSpeed { get; set; }  // Скорость кораблей
        public float TrolleyFireSpeed { get; set; }  // Скорость перезарядки тачанки
        public float InvaderAcc { get; set; }  // Ускорение кораблей
        public int InvaderInRow { get; set; }  // Количество кораблей в строке
        public int TrolleyStrength { set; get; }  // Прочность тачанки
        public float InvaderFireSpeed { get; set; }  // Скорость перезарядки кораблей
 
    }
}
