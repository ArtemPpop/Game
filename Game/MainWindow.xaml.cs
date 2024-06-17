using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MazeWidth = 20;
        private const int MazeHeight = 20;
        private const int CellSize = 30;
        private const double MoveStep = 2.0; // Шаг перемещения
        private Maze maze;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMaze();
            DrawMaze();
            InitializePlayer();
        }

        private void InitializeMaze()
        {
            maze = new Maze(MazeWidth, MazeHeight);
        }

        private void DrawMaze()
        {
            Cell[,] cells = maze.GetCells();

            for (int x = 0; x < MazeWidth; x++)
            {
                for (int y = 0; y < MazeHeight; y++)
                {
                    Cell cell = cells[x, y];

                    if (cell.HasTopWall)
                    {
                        DrawWall(x * CellSize, y * CellSize, (x + 1) * CellSize, y * CellSize);
                    }
                    if (cell.HasRightWall)
                    {
                        DrawWall((x + 1) * CellSize, y * CellSize, (x + 1) * CellSize, (y + 1) * CellSize);
                    }
                    if (cell.HasBottomWall)
                    {
                        DrawWall(x * CellSize, (y + 1) * CellSize, (x + 1) * CellSize, (y + 1) * CellSize);
                    }
                    if (cell.HasLeftWall)
                    {
                        DrawWall(x * CellSize, y * CellSize, x * CellSize, (y + 1) * CellSize);
                    }
                }
            }

            //  финишняя точка
            Rectangle finish = new Rectangle
            {
                Width = CellSize - 2,
                Height = CellSize - 2,
                Fill = Brushes.Green
            };
            Canvas.SetLeft(finish, (MazeWidth - 1) * CellSize + 1);
            Canvas.SetTop(finish, (MazeHeight - 1) * CellSize + 1);
            GameCanvas.Children.Add(finish);
        }

        private void DrawWall(double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            GameCanvas.Children.Add(line);
        }

        private void InitializePlayer()
        {
            Canvas.SetLeft(Player, 1);
            Canvas.SetTop(Player, 1);
        }

        private void GameCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(GameCanvas);
            double playerX = Canvas.GetLeft(Player);
            double playerY = Canvas.GetTop(Player);

            double deltaX = mousePosition.X - (playerX + Player.Width / 2);
            double deltaY = mousePosition.Y - (playerY + Player.Height / 2);
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > MoveStep)
            {
                double stepX = (deltaX / distance) * MoveStep;
                double stepY = (deltaY / distance) * MoveStep;

                double newLeft = playerX + stepX;
                double newTop = playerY + stepY;

                if (CanMoveTo(newLeft, newTop))
                {
                    Canvas.SetLeft(Player, newLeft);
                    Canvas.SetTop(Player, newTop);
                }
            }

            // Проверка на победу
            if (CheckCollision(Player, GameCanvas.Children[GameCanvas.Children.Count - 1]))
            {
                MessageBox.Show("Вы выиграли!");
                // Сброс позиции игрока
                InitializePlayer();
            }
        }

        private bool CanMoveTo(double x, double y)
        {
            Rect playerRect = new Rect(x, y, Player.Width, Player.Height);

            // Проверка границ лабиринта
            if (x < 0 || x + Player.Width > MazeWidth * CellSize || y < 0 || y + Player.Height > MazeHeight * CellSize)
            {
                return false;
            }

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Line wall)
                {
                    Rect wallRect = new Rect(Math.Min(wall.X1, wall.X2), Math.Min(wall.Y1, wall.Y2), Math.Abs(wall.X2 - wall.X1), Math.Abs(wall.Y2 - wall.Y1));
                    if (playerRect.IntersectsWith(wallRect))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckCollision(UIElement a, UIElement b)
        {
            Rect rectA = new Rect(Canvas.GetLeft(a), Canvas.GetTop(a), a.RenderSize.Width, a.RenderSize.Height);
            Rect rectB = new Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.RenderSize.Width, b.RenderSize.Height);
            return rectA.IntersectsWith(rectB);
        }
    }
}