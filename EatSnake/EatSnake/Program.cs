using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace EatSnake
{
    class Program
    {
        /// <summary>
        /// 游戏开始
        /// 游戏主处理
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Title = "贪吃蛇";

            //分数
            int Number = 0;

            //获取窗口长度，高度
            int X = Console.WindowWidth;
            int Y = Console.WindowHeight;

            //米粒的位置
            Point MIP = Point.Empty;
            Snake snake = new();
            //有没有米粒
            bool HasMI = false;

            ConsoleKeyInfo consoleKeyInfo;
            while (true)
            {
                //蛇的移动
                if (Console.KeyAvailable)
                {
                    consoleKeyInfo = Console.ReadKey();
                    if (consoleKeyInfo.Key == ConsoleKey.UpArrow)
                    {
                        snake.FangXiang = FangXiang.Shang;
                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.DownArrow)
                    {
                        snake.FangXiang = FangXiang.Xia;
                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        snake.FangXiang = FangXiang.Zuo;
                    }
                    else if (consoleKeyInfo.Key == ConsoleKey.RightArrow)
                    {
                        snake.FangXiang = FangXiang.You;
                    }
                }
                //蛇的移动,如果撞到边界,则重新游戏
                if (!snake.Move())
                {
                    Main(null);
                }
                //米粒的放置
                if (!HasMI)
                {
                    Random random = new(Guid.NewGuid().GetHashCode());
                    int NewX = random.Next(0, X);
                    int NewY = random.Next(0, Y);
                    MIP = new Point(NewX, NewY);
                    Console.SetCursorPosition(NewX, NewY);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("o");
                    Console.ResetColor();
                    HasMI = true;
                    Console.SetCursorPosition(0, 0);
                }
                //检测蛇有没有吃到豆子
                if (snake.IsHit(MIP))
                {
                    HasMI = false;
                    snake.AddOne();
                    Number += 100;
                    Console.Title = $"贪吃蛇  得分：{Number}";
                }
                Thread.Sleep(100);
            }
        }
    }

    /// <summary>
    /// 贪吃蛇
    /// </summary>
    internal class Snake
    {
        public List<BaseBody> SnakeBodies = new();
        public FangXiang FangXiang { get; set; } = FangXiang.You;
        public Snake()
        {
            int WindowsH = Console.WindowHeight;
            int NewY = WindowsH / 2;
            SnakeBodies.Add(new SnakeHead(0, NewY));
            SnakeBodies.Add(new SnakeBody(0, NewY));
            SnakeBodies.Add(new SnakeBody(0, NewY));
        }

        /// <summary>
        /// 添加一节身体
        /// </summary>
        public void AddOne()
        {
            var body = SnakeBodies.Last();
            switch (body.FangXiang)
            {
                case FangXiang.Shang:
                    SnakeBodies.Add(new SnakeBody(body.BodyXY.X, body.BodyXY.Y + 1));
                    break;
                case FangXiang.Xia:
                    SnakeBodies.Add(new SnakeBody(body.BodyXY.X, body.BodyXY.Y - 1));
                    break;
                case FangXiang.Zuo:
                    SnakeBodies.Add(new SnakeBody(body.BodyXY.X + 1, body.BodyXY.Y));
                    break;
                case FangXiang.You:
                    SnakeBodies.Add(new SnakeBody(body.BodyXY.X - 1, body.BodyXY.Y));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 蛇头有没有吃到豆子
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsHit(Point point)
        {
            var P = SnakeBodies.First();
            return (point.X == P.BodyXY.X && point.Y == P.BodyXY.Y);
        }

        readonly int H = Console.WindowHeight;
        readonly int W = Console.WindowWidth;
        /// <summary>
        /// 移动
        /// </summary>
        /// <returns></returns>
        public bool Move()
        {
            BaseBody BBody = null;
            for (int i = 0; i < SnakeBodies.Count; i++)
            {
                var Body = SnakeBodies[i];
                if (Body is SnakeHead)
                {
                    switch (FangXiang)
                    {
                        case FangXiang.Shang:
                            if (Body.FangXiang == FangXiang.Xia)
                            {
                                break;
                            }
                            break;
                        case FangXiang.Xia:
                            if (Body.FangXiang == FangXiang.Shang)
                            {
                                break;
                            }
                            break;
                        case FangXiang.Zuo:
                            if (Body.FangXiang == FangXiang.You)
                            {
                                break;
                            }
                            break;
                        case FangXiang.You:
                            if (Body.FangXiang == FangXiang.Zuo)
                            {
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                    Body.FangXiang = FangXiang;
                    BBody = BaseBody.NEW(Body);
                    Body.Move();
                    continue;
                }
                else if (Body is SnakeBody)
                {
                    if (BBody != null)
                    {
                        BaseBody baseBody = BaseBody.NEW(Body);
                        BBody.CopyTo(Body);
                        BBody = baseBody;
                    }
                    continue;
                }
            }
            foreach (var item in SnakeBodies)
            {
                if (item.BodyXY.X < 0 || item.BodyXY.X >= W || item.BodyXY.Y < 0 || item.BodyXY.Y >= H)
                {
                    return false;
                }
                Console.SetCursorPosition(item.BodyXY.X, item.BodyXY.Y);
                Console.Write(item.C);
            }
            Console.SetCursorPosition(BBody.BodyXY.X, BBody.BodyXY.Y);
            Console.Write(" ");
            return true;
        }

        /// <summary>
        /// 蛇头
        /// </summary>
        public class SnakeHead : BaseBody
        {
            public SnakeHead(int X, int Y) : base(X, Y)
            {
                C = 'O';
            }

            public override void Move()
            {
                switch (FangXiang)
                {
                    case FangXiang.Shang:
                        BodyXY = new Point(BodyXY.X, BodyXY.Y - 1);
                        break;
                    case FangXiang.Xia:
                        BodyXY = new Point(BodyXY.X, BodyXY.Y + 1);
                        break;
                    case FangXiang.Zuo:
                        BodyXY = new Point(BodyXY.X - 1, BodyXY.Y);
                        break;
                    case FangXiang.You:
                        BodyXY = new Point(BodyXY.X + 1, BodyXY.Y);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 蛇身子
        /// </summary>
        public class SnakeBody : BaseBody
        {
            public SnakeBody(int X, int Y) : base(X, Y)
            {
                C = 'o';
            }

            public override void Move()
            {

            }
        }

        public abstract class BaseBody
        {
            public static BaseBody NEW<T>(T baseBody) where T : BaseBody
            {
                BaseBody body = null;
                if (baseBody is SnakeBody)
                {
                    body = new SnakeBody(baseBody.BodyXY.X, baseBody.BodyXY.Y)
                    {
                        C = baseBody.C,
                        FangXiang = baseBody.FangXiang
                    };
                }
                else if (baseBody is SnakeHead)
                {
                    body = new SnakeHead(baseBody.BodyXY.X, baseBody.BodyXY.Y)
                    {
                        C = baseBody.C,
                        FangXiang = baseBody.FangXiang
                    };
                }
                return body;
            }
            public BaseBody(int X, int Y)
            {
                BodyXY = new Point(X, Y);
            }
            public char C { get; set; }
            public Point BodyXY { get; set; }
            public FangXiang FangXiang { get; set; }

            public void CopyTo(BaseBody baseBody)
            {
                baseBody.BodyXY = BodyXY;
                baseBody.FangXiang = FangXiang;
            }

            public abstract void Move();
        }

    }

    internal enum FangXiang
    {
        NONE,
        Shang, Xia, Zuo, You
    }
}
