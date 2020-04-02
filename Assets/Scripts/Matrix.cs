using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class Matrix<T>
    {
        private List<List<T>> _content;

        public int Cols { get; private set; }
        public int Rows { get; private set; }

        public Matrix(int cols, int rows)
        {
            _content = new List<List<T>>();
            this.IntRange(1, cols).ForEach(x =>
            {
                var list = new List<T>();

                this.IntRange(1, rows).ForEach(y => { list.Add(default(T)); });

                _content.Add(list);
            });

            Cols = cols;
            Rows = rows;
        }

        public void SetValue(T value, int col, int row)
        {
            _content[col][row] = value;
        }

        public T GetValue(int col, int row)
        {
            return _content[col][row];
        }

        public T TryGetValue(int col, int row)
        {
            if (col < 0 || col >= Cols || row < 0 || row >= Rows) return default;
            return GetValue(col, row);
        }

        public void DoTransposed()
        {
            var copy = _content;
            _content = new List<List<T>>();

            this.IntRange(1, Rows).ForEach(x => _content.Add(new List<T>()));

            for (var i = 0; i < Cols; i++)
            {
                var currentList = copy[i];
                for (var j = 0; j < Rows; j++)
                {
                    _content[j].Add(currentList[j]);
                }
            }


            var cols = Cols;
            Cols = Rows;
            Rows = cols;
        }

        public List<T> GetColumn(int col)
        {
            return _content.GetOrDefault(col);
        }

        public List<T> GetRow(int row)
        {
            var result = new List<T>();
            for (var i = 0; i < Cols; i++)
            {
                result.Add(GetValue(i, row));
            }

            return result;
        }

        public static Matrix<float> GenerateRandomMatrix(int x, int y)
        {
            var matrix = new Matrix<float>(x, y);
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    matrix.SetValue(0.GetRandomInt(0, 100), i, j);
                }
            }

            return matrix;
        }

        public delegate T FillMatrixDelegate(int col, int row);

        public void FillMatrix(FillMatrixDelegate value)
        {
            for (var col = 0; col < Cols; col++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    SetValue(value(col, row), col, row);
                }
            }
        }

        public override string ToString()
        {
            var cloneMatrix = CloneMatrix();
            cloneMatrix.DoTransposed();
            return cloneMatrix._content.Stringify(x => x.Stringify(y => y?.ToString() ?? "") + "\n");
        }

        public Matrix<T> CloneMatrix()
        {
            var result = new Matrix<T>(Cols, Rows);
            ForEach((col, row, item) => { result.SetValue(item, col, row); });
            return result;
        }

        public delegate void ForEachDelegate(int col, int row, T item);

        public void ForEach(ForEachDelegate iteration)
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    iteration(j, i, GetValue(j, i));
                }
            }
        }

        public delegate TS SelectDelegate<out TS>(T item, int col, int row);

        public IEnumerable<TS> Select<TS>(SelectDelegate<TS> selector)
        {
            var result = new List<TS>();
            ForEach((col, row, item) => { result.Add(selector(item, col, row)); });
            return result;
        }

        public delegate T ClearDelegate(T item);

        public void Clear(ClearDelegate mapping)
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    SetValue(mapping(GetValue(j, i)), j, i);
                }
            }
        }

        public Matrix<T> GetSubMatrix(int initialCol, int initialRow, int colLength, int rowLength)
        {
            var resultingMatrix = new Matrix<T>(colLength, rowLength);
            for (var col = 0; col < colLength; col++)
            {
                for (var row = 0; row < rowLength; row++)
                {
                    resultingMatrix.SetValue(GetValue(col + initialCol, row + initialRow), col, row);
                }
            }

            return resultingMatrix;
        }

        public void SetSubMatrix(Matrix<T> subMatrix, int initialCol, int initialRow)
        {
            for (var col = 0; col < subMatrix.Cols; col++)
            {
                for (var row = 0; row < subMatrix.Rows; row++)
                {
                    var realCol = col + initialCol;
                    var realRow = row + initialRow;
                    if (IsIndexBetweenLimits(new Vector2Int(realCol, realRow)))
                    {
                        SetValue(subMatrix.GetValue(col, row), realCol, realRow);
                    }
                }
            }
        }

        public int FirstRowIndex(int column, Func<T, bool> selector)
        {
            for (var row = 0; row < Rows; row++)
            {
                if (selector(GetValue(column, row)))
                {
                    return row;
                }
            }

            return -1;
        }

        public delegate bool GetChunkDelegate(T searchingItem);

        public Matrix<T> GetChunk(GetChunkDelegate selector)
        {
            var firstCol = -1;
            var firstRow = -1;
            for (var col = 0; col < Cols; col++)
            {
                if (firstCol != -1 || firstRow != -1) break;
                for (var row = 0; row < Rows; row++)
                {
                    var iteration = GetValue(col, row);
                    if (selector(iteration))
                    {
                        firstCol = col;
                        firstRow = row;
                        break;
                    }
                }
            }

            var chunkList = new List<Vector2Int> {new Vector2Int(firstCol, firstRow)};

            RecursiveChunkFinder(selector, firstCol, firstRow, chunkList);

            var minX = chunkList.Min(x => x.x).x;
            var minY = chunkList.Min(x => x.y).y;

            chunkList = chunkList.Select(x => x - new Vector2Int(minX, minY)).ToList();

            var maxX = chunkList.Max(x => x.x).x;
            var maxY = chunkList.Max(x => x.y).y;

            var result = new Matrix<T>(maxX + 1, maxY + 1);

            for (var col = 0; col < result.Cols; col++)
            {
                for (var row = 0; row < result.Rows; row++)
                {
                    var currentValue = TryGetValue(minX + col, minY + row);
                    if (selector(currentValue))
                    {
                        result.SetValue(currentValue, col, row);
                    }
                }
            }

            return result;
        }

        public Vector2Int GetMidPoint()
        {
            var maxCol = Cols / 2;
            var maxRow = Rows / 2;
            return new Vector2Int(maxCol, maxRow);
        }

        public Vector2 GetFloatedMidPoint()
        {
            return new Vector2(Cols / 2f, Rows / 2f);
        }

        private void RecursiveChunkFinder(GetChunkDelegate selector, int currentCol, int currentRow,
            List<Vector2Int> foundPositions)
        {
            var left = TryGetValue(currentCol - 1, currentRow);
            if (!foundPositions.Contains(new Vector2Int(currentCol - 1, currentRow)) && selector(left))
            {
                foundPositions.Add(new Vector2Int(currentCol - 1, currentRow));
                RecursiveChunkFinder(selector, currentCol - 1, currentRow, foundPositions);
            }

            var right = TryGetValue(currentCol + 1, currentRow);
            if (!foundPositions.Contains(new Vector2Int(currentCol + 1, currentRow)) && selector(right))
            {
                foundPositions.Add(new Vector2Int(currentCol + 1, currentRow));
                RecursiveChunkFinder(selector, currentCol + 1, currentRow, foundPositions);
            }

            var up = TryGetValue(currentCol, currentRow - 1);
            if (!foundPositions.Contains(new Vector2Int(currentCol, currentRow - 1)) && selector(up))
            {
                foundPositions.Add(new Vector2Int(currentCol, currentRow - 1));
                RecursiveChunkFinder(selector, currentCol, currentRow - 1, foundPositions);
            }

            var down = TryGetValue(currentCol, currentRow + 1);
            if (!foundPositions.Contains(new Vector2Int(currentCol, currentRow + 1)) && selector(down))
            {
                foundPositions.Add(new Vector2Int(currentCol, currentRow + 1));
                RecursiveChunkFinder(selector, currentCol, currentRow + 1, foundPositions);
            }
        }

        public Matrix<T> RotateRight()
        {
            var midCol = Cols / 2;
            if (Cols % 2 == 0) midCol--;
            var midRow = Rows / 2;
            if (Rows % 2 == 0) midRow--;

            var midPoint = new Vector2(midCol, -midRow);

            var result = new Matrix<T>(Rows, Cols);
            var dictionary = new Dictionary<Vector2, T>();
            ForEach((col, row, n) =>
            {
                var directorVector = midPoint - new Vector2(col, row);
                var normalVector = new Vector2(directorVector.y, -directorVector.x);
                var newPosition = normalVector + midPoint;
                dictionary.Add(new Vector2(-newPosition.x, -newPosition.y), n);
            });

            // Move matrix in order to remove negative items
            var minX = dictionary.Keys.Min(x => x.x).x;
            var minY = dictionary.Keys.Min(x => x.y).y;

            dictionary.ForEach(pair =>
            {
                result.SetValue(pair.Value, (int) (pair.Key.x - minX), (int) (pair.Key.y - minY));
            });

            return result;
        }

        public Vector2Int LocateSubMatrix(Matrix<T> subMatrix, List<GetChunkDelegate> ignoreData = null)
        {
            bool CheckEncounter(int col, int row)
            {
                for (var c = 0; c < subMatrix.Cols; c++)
                {
                    for (var r = 0; r < subMatrix.Rows; r++)
                    {
                        var v1 = TryGetValue(col + c, row + r);
                        var v2 = subMatrix.TryGetValue(c, r);
                        if (ignoreData != null)
                        {
                            var skipStep = false;
                            foreach (var data in ignoreData)
                            {
                                if (data(v2))
                                {
                                    skipStep = true;
                                }
                            }

                            if (skipStep)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if ((v1 == null && v2 != null))
                            {
                                return false;
                            }
                        }

                        if (v1 == null && v2 == null)
                        {
                            continue;
                        }

                        if (v1 == null) return false;

                        if (!v1.Equals(v2))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            for (var col = 0; col < Cols; col++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    var result = CheckEncounter(col, row);
                    if (result)
                    {
                        return new Vector2Int(col, row);
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }

        public Matrix<TS> Convert<TS>(Func<T, TS> selector)
        {
            var result = new Matrix<TS>(Cols, Rows);
            ForEach((col, row, value) => { result.SetValue(selector(value), col, row); });
            return result;
        }

        // First rows, then columns
        public List<T> ToList()
        {
            var result = new List<T>();
            ForEach((col, row, value) => result.Add(value));
            return result;
        }

        public List<Vector2Int> ToKeyList()
        {
            var result = new List<Vector2Int>();
            ForEach((col, row, value) => { result.Add(new Vector2Int(col, row)); });
            return result;
        }

        public bool IsIndexBetweenLimits(Vector2Int index)
        {
            if (index.x >= Cols) return false;
            if (index.x < 0) return false;
            if (index.y >= Rows) return false;
            if (index.y < 0) return false;
            return true;
        }

        public delegate bool AllDelegate(T item, int col, int row);

        public bool All(AllDelegate selector)
        {
            var result = true;
            ForEach((col, row, item) =>
            {
                if (!selector(item, col, row))
                {
                    result = false;
                }
            });
            return result;
        }

        public void GetAvailableSubMatrix(int cols, int rows)
        {
            
        }
    }
}