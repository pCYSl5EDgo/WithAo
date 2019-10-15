using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    public sealed class BoardTests
    {
        [Test]
        public void SimpleBoardTest()
        {
            var board = new Board(new int2(2, 2));
            board.Clear();
            foreach (ref var cell in board)
            {
                Assert.AreEqual(default(Cell), cell);
            }
        }
    }
}