using System;
using System.Collections.Generic;

namespace Game2048.Resources
{
    internal sealed class Game
    {
        private const int DEFAULT_FIELD_SIZE = 0x00000004;

        private readonly byte endpoint;
        private byte[,] field;
        private long score;
        private State gameState;

        public byte FieldSize
        {
            get
            {
                return (byte)field.GetLength(0);
            }
        }

        public List<Point> EmptyCells
        {
            get
            {
                var points = new List<Point>();

                for (byte i = 0; i < FieldSize; i++)
                {
                    for (byte j = 0; j < FieldSize; j++)
                    {
                        if (field[i, j] == 0)
                        {
                            points.Add(new Point(i, j));
                        }
                    }
                }

                return points;
            }
        }

        public byte[,] Field
        {
            get
            {
                return field;
            }
        }

        public long Score
        {
            get
            {
                return score;
            }
        }

        internal Game(byte fieldSize = DEFAULT_FIELD_SIZE, byte endpoint = 11)
        {
            score = 0;
            field = CreateEmptyField(fieldSize);
            CreateNewCells(1);
            this.endpoint = endpoint;
            gameState = State.Running;
        }

        internal static int GetCellValue(byte power)
        {
            return (int)Math.Pow(2, power);
        }

        private static byte[,] CreateEmptyField(byte size)
        {
            var field = new byte[size, size];

            for (byte i = 0; i < size; i++)
            {
                for (byte j = 0; j < size; j++)
                {
                    field[i, j] = 0;
                }
            }

            return field;
        }

        private static byte RandomizeValueOfNewCell()
        {
            var rand = new Random();

            return (byte)rand.Next(1, 101) < 81 ? (byte)1 : (byte)2;
        }
        
        private static bool AreFieldsEqual(byte[,] a, byte[,] b, byte fieldSize)
        {
            for (byte i = 0; i < fieldSize; i++)
            {
                for (byte j = 0; j < fieldSize; j++)
                {
                    if (a[i, j] == b[i, j])
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        private bool IsMoveAvailable()
        {
            int pairCount = 0;

            for (byte i = 0; i < FieldSize; i++)
            {
                for (byte j = 0; j < FieldSize; j++)
                {
                    if (i < FieldSize - 1 && field[i, j] == field[i + 1, j])
                    {
                        pairCount++;
                    }    

                    if (j < FieldSize - 1 && field[i, j] == field[i, j + 1])
                    {
                        pairCount++;
                    }
                }
            }

            return pairCount == 0;
        }

        private byte[] GetColumnElements(byte column)
        {
            var elements = new List<byte>();

            for (byte row = 0; row < FieldSize; row++)
            {
                if (field[row, column] > 0)
                {
                    elements.Add(field[row, column]);
                }
            }

            return elements.ToArray();
        }

        private byte[] GetRowElements(byte row)
        {
            var elements = new List<byte>();

            for (byte column = 0; column < FieldSize; column++)
            {
                if (field[row, column] > 0)
                {
                    elements.Add(field[row, column]);
                }
            }

            return elements.ToArray();
        }
        
        private static byte[] GetElementSequenceAfter(byte[] oldSeq, out int gainedScore)
        {
            gainedScore = 0;
            var newSeq = new List<byte>();

            for (int i = 0; i < oldSeq.Length; i++)
            {
                if (i < oldSeq.Length - 1 && oldSeq[i] == oldSeq[i + 1])
                {
                    gainedScore += GetCellValue((byte)(oldSeq[i] + 1));
                    newSeq.Add((byte)(oldSeq[i] + 1));

                    i++;
                }
                else
                {
                    newSeq.Add(oldSeq[i]);
                }
            }

            return newSeq.ToArray();
        }

        private void CreateNewCells(int count)
        {
            var rand = new Random();
            var emptyCells = EmptyCells;

            if (count > emptyCells.Count)
            {
                count = emptyCells.Count - 1;
            }

            var cellsToAssign = new List<int>();
            int pointIndex;

            for (int i = 0; i < count; i++)
            {
                do
                {
                    pointIndex = rand.Next(emptyCells.Count);
                }
                while (cellsToAssign.Contains(pointIndex));

                cellsToAssign.Add(pointIndex);
            }

            foreach (var cell in cellsToAssign)
            {
                field[emptyCells[cell].x, emptyCells[cell].y] = RandomizeValueOfNewCell();
            }
        }

        internal void LoseGame()
        {
            gameState = State.Losed;
        }

        internal State HandleSwipe(Direction dir)
        {
            if (gameState != State.Running)
            {
                return gameState;
            }
            else
            {
                var newField = CreateEmptyField(FieldSize);
                byte[] elements;
                int gainedScore;

                if (dir == Direction.Up)
                {
                    for (byte column = 0; column < FieldSize; column++)
                    {
                        elements = GetElementSequenceAfter(GetColumnElements(column), out gainedScore);
                        score += gainedScore;

                        for (byte row = 0; row < elements.Length; row++)
                        {
                            newField[row, column] = elements[row];
                        }
                    }
                }

                if (dir == Direction.Down)
                {
                    for (byte column = 0; column < FieldSize; column++)
                    {
                        elements = GetElementSequenceAfter(GetColumnElements(column), out gainedScore);
                        score += gainedScore;

                        for (byte row = 0; row < elements.Length; row++)
                        {
                            newField[row + FieldSize - elements.Length, column] = elements[row];
                        }
                    }
                }

                if (dir == Direction.Left)
                {
                    for (byte row = 0; row < FieldSize; row++)
                    {
                        elements = GetElementSequenceAfter(GetRowElements(row), out gainedScore);
                        score += gainedScore;

                        for (byte column = 0; column < elements.Length; column++)
                        {
                            newField[row, column] = elements[column];
                        }
                    }
                }

                if (dir == Direction.Right)
                {
                    for (byte row = 0; row < FieldSize; row++)
                    {
                        elements = GetElementSequenceAfter(GetRowElements(row), out gainedScore);
                        score += gainedScore;

                        for (byte column = 0; column < elements.Length; column++)
                        {
                            newField[row, column + FieldSize - elements.Length] = elements[column];
                        }
                    }
                }

                var oldField = field;
                field = newField;

                if (!AreFieldsEqual(oldField, newField, FieldSize))
                {
                    CreateNewCells(1);
                }

                for (byte i = 0; i < FieldSize; i++)
                {
                    for (byte j = 0; j < FieldSize; j++)
                    {
                        if (field[i, j] == endpoint)
                        {
                            gameState = State.Won;
                            return State.Won;
                        }
                    }
                }

                if (EmptyCells.Count == 0 && AreFieldsEqual(oldField, newField, FieldSize) && IsMoveAvailable())
                {
                    gameState = State.Losed;
                    return State.Losed;
                }

                gameState = State.Running;
                return State.Running;
            }
        }

        internal enum Direction
        {
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4
        }

        internal enum State
        {
            Running = 0,
            Won = 1,
            Losed = 2
        }
    }

    internal readonly struct Point
    {
        public readonly int x;
        public readonly int y;

        internal Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
