using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Maze
    {
        private readonly int width;
        private readonly int height;
        private readonly Cell[,] cells;
        private readonly Random random = new Random();

        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
            GenerateMaze();
        }

        private void GenerateMaze()
        {
            Stack<Cell> stack = new Stack<Cell>();
            Cell current = cells[0, 0];
            current.IsVisited = true;

            do
            {
                List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(current);

                if (unvisitedNeighbors.Count > 0)
                {
                    Cell chosen = unvisitedNeighbors[random.Next(unvisitedNeighbors.Count)];
                    RemoveWalls(current, chosen);
                    stack.Push(current);
                    current = chosen;
                    current.IsVisited = true;
                }
                else if (stack.Count > 0)
                {
                    current = stack.Pop();
                }

            } while (stack.Count > 0);
        }

        private List<Cell> GetUnvisitedNeighbors(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();

            if (cell.X > 0 && !cells[cell.X - 1, cell.Y].IsVisited) neighbors.Add(cells[cell.X - 1, cell.Y]);
            if (cell.X < width - 1 && !cells[cell.X + 1, cell.Y].IsVisited) neighbors.Add(cells[cell.X + 1, cell.Y]);
            if (cell.Y > 0 && !cells[cell.X, cell.Y - 1].IsVisited) neighbors.Add(cells[cell.X, cell.Y - 1]);
            if (cell.Y < height - 1 && !cells[cell.X, cell.Y + 1].IsVisited) neighbors.Add(cells[cell.X, cell.Y + 1]);

            return neighbors;
        }

        private void RemoveWalls(Cell current, Cell chosen)
        {
            if (current.X == chosen.X)
            {
                if (current.Y > chosen.Y)
                {
                    current.HasTopWall = false;
                    chosen.HasBottomWall = false;
                }
                else
                {
                    current.HasBottomWall = false;
                    chosen.HasTopWall = false;
                }
            }
            else
            {
                if (current.X > chosen.X)
                {
                    current.HasLeftWall = false;
                    chosen.HasRightWall = false;
                }
                else
                {
                    current.HasRightWall = false;
                    chosen.HasLeftWall = false;
                }
            }
        }

        public Cell[,] GetCells()
        {
            return cells;
        }
    }
}
