using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class WhatTheChess
    {
        private readonly struct Position
            : IEquatable<Position>
        {
            public readonly int X;
            public readonly int Y;

            public Position(int x, int y)
            {
                if (x < 1 || x > 8) throw new ArgumentOutOfRangeException(nameof(x));
                if (y < 1 || y > 8) throw new ArgumentOutOfRangeException(nameof(y));

                X = x;
                Y = y;
            }

            public override string ToString()
            {
                var x = (char)(X + 96);
                return $"{x}{Y}";
            }

            public bool Equals(Position other)
            {
                return X == other.X && Y == other.Y;
            }

            public override bool Equals(object? obj)
            {
                return obj is Position other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }

            public static bool operator ==(Position left, Position right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Position left, Position right)
            {
                return !left.Equals(right);
            }
        }

        private enum Piece
        {
            Rook,
            Knight,
            Bishop
        }

        [TestMethod]
        public void GenerateWhatTheChess()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Position start, Position end)
            {
                var move = ChoosePiece(start, end);
                if (!move.HasValue)
                    return;

                var move2 = ChoosePiece2(start, end);
                Assert.AreEqual(move, move2);

                input.Add(new Dictionary<string, Value> {
                    { "a", start.ToString() },
                    { "b", end.ToString() },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", move.ToString()! },
                });
            }

            SingleCase(new Position(1, 1), new Position(1, 3));
            SingleCase(new Position(3, 3), new Position(7, 7));
            SingleCase(new Position(2, 2), new Position(6, 6));
            SingleCase(new Position(1, 1), new Position(2, 3));
            SingleCase(new Position(4, 3), new Position(2, 4));

            var r = new Random();
            while (input.Count < 100000)
            {
                var s = new Position(r.Next(1, 9), r.Next(1, 9));
                var e = new Position(r.Next(1, 9), r.Next(1, 9));
                SingleCase(s, e);
            }

            Generator.YololLadderGenerator(input, output);
        }

        private static Piece? ChoosePiece(Position start, Position end)
        {
            if (start == end)
                return null;

            if (start.X == end.X || start.Y == end.Y)
                return Piece.Rook;

            var dx = Math.Abs(end.X - start.X);
            var dy = Math.Abs(end.Y - start.Y);
            if (dx == dy)
                return Piece.Bishop;

            if ((dx == 1 && dy == 2) || (dx == 2 && dy == 1))
                return Piece.Knight;

            return null;
        }

        private static Piece? ChoosePiece2(Position start, Position end)
        {
            if (start == end)
                return null;

            if (start.X == end.X || start.Y == end.Y)
                return Piece.Rook;

            var xOddDelta = IsOdd(start.X) != IsOdd(end.X);
            var yOddDelta = IsOdd(start.Y) != IsOdd(end.Y);
            if (xOddDelta != yOddDelta)
                return Piece.Knight;

            return Piece.Bishop;
        }

        private static bool IsOdd(int x)
        {
            return x % 2 == 1;
        }
    }
}