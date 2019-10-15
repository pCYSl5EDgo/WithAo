using System;
using NUnit.Framework;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    public class CellTests
    {
        [Test]
        public void CellTestsSimplePasses()
        {
            var cell = new Cell();
            Assert.IsTrue(@"{""ChipType"": ""None"", ""Territories"": []}" == cell.ToString());
        }

        [Test]
        public void DefaultNoTerritory()
        {
            var cell = new Cell();
            for (var i = 0; i < 20; i++)
            {
                Assert.IsFalse(cell.IsTerritoryOf(i));
                Assert.IsTrue(0 == cell[i]);
            }
        }

        [Test]
        public void SimpleSetTest()
        {
            for (var i = 0; i < 20; i++)
            {
                var cell = new Cell();
                cell[i] = 0;
                Assert.IsFalse(cell.IsTerritoryOf(i));
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    Assert.IsFalse(cell.IsTerritoryOf(j));
                }
                for (var j = 1; j <= 256; j++)
                {
                    cell[i] = j;
                    Assert.IsTrue(cell.IsTerritoryOf(i));
                    Assert.IsTrue(j == cell[i]);
                }
            }
        }

        [Test]
        public void SimpleSetOutOfRangeTest()
        {
            for (var i = 0; i < 20; i++)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var cell = new Cell();
                    cell[i] = -1;
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var cell = new Cell();
                    cell[i] = 257;
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var cell = new Cell();
                    cell[i] = int.MinValue;
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var cell = new Cell();
                    cell[i] = int.MaxValue;
                });
            }
        }

        [Test]
        public void SimpleAddTest()
        {
            var cell = new Cell();
            for (var i = 0; i < 20; i++)
            {
                cell.AddPaint(i, 1);
                Assert.IsTrue(cell.IsTerritoryOf(i));
                Assert.AreEqual(1, cell[i]);
            }
            Assert.AreEqual(20, cell.Sum());
        }

        [Test]
        public void Two256Test()
        {
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    Cell cell = default;
                    cell.AddPaint(i, 256);
                    Assert.AreEqual(256 , cell[i]);
                    cell.AddPaint(j, 256);
                    Assert.AreEqual(0, cell[i]);
                    Assert.AreEqual(256, cell[j]);
                }
            }
        }

        [Test]
        public void Two1024Test()
        {
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    Cell cell = default;
                    cell.AddPaint(i, 1024);
                    Assert.AreEqual(256, cell[i]);
                    cell.AddPaint(j, 1024);
                    Assert.AreEqual(0, cell[i]);
                    Assert.AreEqual(256, cell[j]);
                }
            }
        }

        [Test]
        public void Three128Test()
        {
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    for (var k = 0; k < 20; k++)
                    {
                        if (i == k || j == k) continue;
                        Cell cell = default;
                        cell.AddPaint(i, 128);
                        Assert.IsTrue(128 == cell[i]);
                        cell.AddPaint(j, 128);
                        Assert.AreEqual(128, cell[i]);
                        Assert.AreEqual(128, cell[j]);
                        Assert.AreEqual(256, cell.Sum());
                        cell.AddPaint(k, 128);
                        Assert.IsTrue(64 == cell[i]);
                        Assert.IsTrue(64 == cell[j]);
                        Assert.IsTrue(128 == cell[k]);
                    }
                }
            }
        }

        [Test]
        public void Three256Test()
        {
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    for (var k = 0; k < 20; k++)
                    {
                        if (i == k || j == k) continue;
                        Cell cell = default;
                        cell.AddPaint(i, 256);
                        Assert.IsTrue(256 == cell[i]);
                        cell.AddPaint(j, 256);
                        Assert.IsTrue(0 == cell[i]);
                        Assert.IsTrue(256 == cell[j]);
                        cell.AddPaint(k, 256);
                        Assert.IsTrue(0 == cell[i]);
                        Assert.IsTrue(0 == cell[j]);
                        Assert.IsTrue(256 == cell[k]);
                    }
                }
            }
        }

        [Test]
        public void Three1024Test()
        {
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    if (i == j) continue;
                    for (var k = 0; k < 20; k++)
                    {
                        if (i == k || j == k) continue;
                        Cell cell = default;
                        cell.AddPaint(i, 1024);
                        Assert.IsTrue(256 == cell[i]);
                        cell.AddPaint(j, 1024);
                        Assert.IsTrue(0 == cell[i]);
                        Assert.IsTrue(256 == cell[j]);
                        cell.AddPaint(k, 1024);
                        Assert.IsTrue(0 == cell[i]);
                        Assert.IsTrue(0 == cell[j]);
                        Assert.IsTrue(256 == cell[k]);
                    }
                }
            }
        }

        [Test]
        public void Twenty13Test()
        {
            Cell cell = default;
            for (var i = 0; i < 20; i++)
            {
                cell.AddPaint(i, 13);
                Assert.AreEqual(13, cell[i]);
            }
            for (var i = 0; i < 4; i++)
            {
                Assert.AreEqual(12, cell[i]);
            }
            for (var i = 4; i < 20; i++)
            {
                Assert.AreEqual(13, cell[i]);
            }
        }

        [Test]
        public void ComplexTest1()
        {
            Cell cell = default;
            cell.AddPaint(0, 76);
            Assert.AreEqual(76, cell[0]);
            cell.AddPaint(1, 68);
            Assert.AreEqual(144, cell.Sum());
            Assert.AreEqual(68, cell[1]);
            cell.AddPaint(2, 53);
            Assert.AreEqual(197, cell.Sum());
            Assert.AreEqual(53, cell[2]);
            cell.AddPaint(3, 100); // 297 - 256 = 41
            // 41 = 13 * 3 + 2
            Assert.AreEqual(256, cell.Sum());
            Assert.AreEqual(100, cell[3]);
            Assert.AreEqual(76 - 14, cell[0]);
            Assert.AreEqual(68 - 14, cell[1]);
            Assert.AreEqual(53 - 13, cell[2]);
        }

        [Test]
        public void ComplexTest2()
        {
            Cell cell = default;
            cell.AddPaint(0, 76);
            cell.AddPaint(1, 68);
            cell.AddPaint(2, 53);
            cell.AddPaint(3, 10);
            cell.AddPaint(4, 190); // 397 - 256 = 141 141 = 4 * 35 + 1
            
            Assert.AreEqual(256, cell.Sum());
            Assert.AreEqual(190, cell[4]);
            // 141 - 36 - 35*2 - 10 = 61 - 36 = 25 = 8 * 3 + 1
            Assert.AreEqual(76 - 36 - 9, cell[0]);
            Assert.AreEqual(68 - 35 - 8, cell[1]);
            Assert.AreEqual(53 - 35 - 8, cell[2]);
            Assert.IsFalse(cell.IsTerritoryOf(3));
            Assert.AreEqual(190, cell[4]);
        }
    }
}