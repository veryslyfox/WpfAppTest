// static class Game
// {
//     public static bool _gameOver;
// }
// class Player
// {
//     public Player(Options options)
//     {
//         Options = options;
//     }
//     void Tick()
//     {
//         // test!
//         Position.X += Options.Velocity;
//         Position.Y += Move;
//     }
//     void Click()
//     {
//         switch (Mode)
//         {
//             case PlayerMode.Cube:
//                 Move = Options.JumpY;
//                 break;
//             case PlayerMode.Spaceship:
//                 Move += Options.EnginePower;
//                 break;
//             case PlayerMode.Wheel:
//                 Options.Gravitation = -Options.Gravitation;
//                 break;
//             case PlayerMode.UFO:
//                 Move = 1.5;
//                 break;
//             case PlayerMode.Wave:
//                 Move = -Move;
//                 return;
//         }
//     }
//     public Point Position;
//     public double Move { get; set; }
//     public PlayerMode Mode { get; set; }
//     public Options Options { get; private set; }
// }
// enum PlayerMode
// {
//     Cube,
//     Spaceship,
//     Wheel,
//     UFO,
//     Wave
// }
// class Level
// {
//     public Level(LevelElement[] space)
//     {
//         Space = space;
//     }
//     public void GetCallback(int x, int y)
//     {

//     }
//     public LevelElement[] Space { get; }
// }
// enum LevelElementType : byte
// {
//     Block,
//     Spike,
//     Trampolin
// }
// class LevelElement
// {
//     public LevelElement(LevelElementType type, int x, int y)
//     {
//         Type = type;
//         X = x;
//         Y = y;
//     }

//     public LevelElementType Type { get; }
//     public int X { get; }
//     public int Y { get; }
//     public void CollideWithBlock(Player player)
//     {
//         switch (Type)
//         {
//             case LevelElementType.Block:
//                 player.Move = 0;
//                 break;
//             case LevelElementType.Spike:
//                 Game.GameOver();
//                 break;
//             case LevelElementType.Trampolin:
//                 player.Move = 3;
//                 break;
//         }
//     }
// }
// record Callback(bool isBlockCollide, bool isGameOver, PlayerMode newMode);
// class Options
// {
//     public Options(
//     double velocity, // X component of move vector
//     double gravitation, // move of Y component 
//     double jumpY, // Y component of move vector in start jump
//     double enginePower // move of Y component
// )
//     {
//         Velocity = velocity;
//         Gravitation = gravitation;
//         JumpY = jumpY;
//         EnginePower = enginePower;
//     }

//     public double Velocity { get; set; }
//     public double Gravitation { get; set; }
//     public double JumpY { get; set; }
//     public double EnginePower { get; set; }
// }
