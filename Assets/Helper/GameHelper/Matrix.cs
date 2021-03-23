using System.Collections.Generic;
using UnityEngine;

namespace Slime.Helper.GameHelper
{
    /// <summary>
    /// A matrix whose origin is left bottom
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Matrix<T> : IMatrix<T> where T : ACellModel
    {
        private int _width,_height;
        private T[] _items;
        private bool[] _isActives;
        
        public void SetDimensions(int width, int height)
        {
            _width = width;
            _height = height;
            _items = new T[width * height];
            _isActives = new bool[width * height];
        }

        public T GetItem(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return default;
            }
            return _items[x + y * _width];
        }

        public void SetItem(int x, int y, T item)
        {
            _items[x + y * _width] = item;
            _isActives[x + y * _width] = true;
            item.x = x;
            item.y = y;
        }

        public T[] GetRow(int y)
        {
            var row = new T[_width];
            for (int x = 0; x < _width; x++)
            {
                row[x] = _items[x + y * _width];
            }

            return row;
        }

        public T[] GetColumn(int x)
        {
            var column = new T[_height];
            for (int y = 0; y < _height; y++)
            {
                column[y] = _items[y * _width + x];
            }

            return column;
        }

        public void OffsetRow(int y, int offset)
        {
            var row = GetRow(y);
            for (int x = 0; x < _width; x++)
            {
                SetItem((_width + x + offset) % _width,y,row[x]);
            }
        }

        public void OffsetColumn(int x, int offset)
        {
            var column = GetColumn(x);
            for(int y = 0; y < _height; y++)
            {
                SetItem(x,(_height + y + offset) % _height,column[y]);
            }
        }
    
        public (int, int, int,int) GetMaxRectangleBox(int x, int y)
        {
            //first create maximum width row
            if (GetItem(x, y).isOccupied)
            {
                return default;
            }

            var upperMost = y;
            var lowerMost = y;
            var rightMost = GetRightMostSameOfRow(x, y);
            var leftMost = GetLeftMostSameOfRow(x, y);
            bool canEnlargeToTop = true, canEnlargeToBottom = true;
            while (canEnlargeToTop)
            {
                for (int i = leftMost; i <= rightMost && canEnlargeToTop; i++)
                {
                    canEnlargeToTop = CanEnlargeToTop(i, upperMost);
                }

                if (canEnlargeToTop)
                {
                    upperMost += 1;    
                }
            }

            while (canEnlargeToBottom)
            {
                for (int i = leftMost; i <= rightMost && canEnlargeToBottom; i++)
                {
                    canEnlargeToBottom = CanEnlargeToBottom(i, lowerMost);
                }

                if (canEnlargeToBottom)
                {
                    lowerMost += 1;    
                }
            }

            for (int i = leftMost; i <= rightMost; i++)
            {
                for (int j = lowerMost; j <= upperMost; j++)
                {
                    GetItem(i, j).isOccupied = true;
                }
            }

            return (leftMost, upperMost, rightMost, lowerMost);
        }

        public void Print()
        {
            string s = "";
            
            for (int y = _height - 1; y > -1 ; y--)
            {
                for (int x = 0; x < _width; x++)
                {
                    s += $"{GetItem(x, y).ToString()}\t";
                }

                s += "\n";
            }
            Util.EditorLog(s);
        }

        public List<Vector2> GetSimilarRegion(int x, int y)
        {
            var currentCell = GetItem(x, y);
            currentCell.isOccupied = true;
            List<Vector2> cells = new List<Vector2>();
            cells.Add(new Vector2(x,y));
            //add right cell if it suitable
            if (x != _width - 1)
            {
                var rightCell = GetItem(x + 1, y);
                if (!rightCell.isOccupied && rightCell.Equals(currentCell))
                {
                    foreach (var point in GetSimilarRegion(x + 1, y))
                    {
                        cells.Add(point);
                    }
                }
            }
            //left cell
            if (x != 0)
            {
                var leftCell = GetItem(x - 1, y);
                if (!leftCell.isOccupied && leftCell.Equals(currentCell))
                {
                    foreach (var point in GetSimilarRegion(x - 1, y))
                    {
                        cells.Add(point);
                    }
                }
            }
            //top cell
            if (y != _height - 1)
            {
                var topCell = GetItem(x, y + 1);
                if (!topCell.isOccupied && topCell.Equals(currentCell))
                {
                    foreach (var point in GetSimilarRegion(x, y + 1))
                    {
                        cells.Add(point);
                    }
                }
            }
            //down cell
            if (y != 0)
            {
                var downCell = GetItem(x, y - 1);
                if (!downCell.isOccupied && downCell.Equals(currentCell))
                {
                    foreach (var point in GetSimilarRegion(x, y - 1))
                    {
                        cells.Add(point);
                    }
                }
            }

            return cells;
        }

        public T[] GetPlusShape(int x, int y)
        {
            var leftCell = GetItem(x - 1, y);
            var upperCell = GetItem(x, y + 1);
            var rightCell = GetItem(x + 1, y);
            var downCell = GetItem(x, y - 1);
            return new []{leftCell, upperCell, rightCell, downCell};
        }

        private int GetLeftMostSameOfRow(int x, int y)
        {
            var currentCell = GetItem(x, y);
            
            if (x == 0)
            {
                return x;
            }
            var nextCell = GetItem(x - 1, y);
            if(nextCell.isOccupied || !nextCell.Equals(currentCell))
            {
                return x;
            }
            

            return GetLeftMostSameOfRow(x - 1, y);
        }

        private int GetRightMostSameOfRow(int x, int y)
        {
            var currentCell = GetItem(x, y);
            
            if (x == _width - 1)
            {
                return x;
            }
            var nextCell = GetItem(x + 1, y);
            if(nextCell.isOccupied || !nextCell.Equals(currentCell))
            {
                return x;
            }

            return GetRightMostSameOfRow(x + 1, y);
        }

        private bool CanEnlargeToBottom(int x,int y)
        {
            if (y == 0)
            {
                return false;
            }

            var nextCell = GetItem(x, y - 1);
            var currentCell = GetItem(x, y);
            return !nextCell.isOccupied && nextCell.Equals(currentCell);
        }

        private bool CanEnlargeToTop(int x, int y)
        {
            if (y == _height - 1)
            {
                return false;
            }
            var nextCell = GetItem(x, y + 1);
            var currentCell = GetItem(x, y);
            return !nextCell.isOccupied && nextCell.Equals(currentCell);
        }
    }
}