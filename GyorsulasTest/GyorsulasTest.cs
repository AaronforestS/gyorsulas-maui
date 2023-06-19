using Moq;
using gyorsulas.Model;
using gyorsulas.Persistence;

namespace GyorsulasTest
{
        [TestClass]
        public class GyorsulasTest
        {
            private GyorsulasModel model = null!;
            private GyorsulasData persistence = null!;
            private Mock<GyorsulasFileAccessInterface> mock = null!;

            [TestInitialize]
            public void InitializeGyorsulasTest()
            {
                int y = 9;
                int x = 5;
                int gameTime = 5;
                int fuel = 15;
                int playerPosition = 3;
                int TickRate = 800;
                int[,] grid = new int[y, x];
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        grid[i, j] = 0;
                    }
                }
                grid[0, 0] = 2;
                grid[y - 2, playerPosition] = 2;
                grid[y - 1, playerPosition + 1] = 2;

                persistence = new GyorsulasData(gameTime, fuel, playerPosition, TickRate, grid);

                mock = new Mock<GyorsulasFileAccessInterface>();
                mock.Setup(_mock => _mock.GyorsulasLoad(It.IsAny<string>())).Returns(() => Task.FromResult(persistence));
                model = new GyorsulasModel(mock.Object);
            }
            [TestMethod]
            public void GyorsulasNewGameTest()
            {
                model.NewGame();
                Assert.IsFalse(DoWeHaveFuel(model));
                Assert.AreEqual(model.Persistence._fuel, 100);
                Assert.AreEqual(model.Persistence._tickRate, 1000);
                Assert.AreEqual(model.Persistence._gameTime, 0);
                Assert.AreEqual(model.Persistence._playerPosition, 2);
                Assert.AreEqual(model.Persistence[8, 2], 1);
            }
            [TestMethod]
            public async Task GyorsulasLoadTest()
            {
                model.NewGame();

                await model.LoadGyorsulasGame(string.Empty);

                for (int i = 0; i < model.Persistence._y; i++)
                {
                    for (int j = 0; j < model.Persistence._x; j++)
                    {
                        Assert.AreEqual(persistence[i, j], model.Persistence[i, j]);
                    }
                }
                Assert.AreEqual(persistence._fuel, model.Persistence._fuel);
                Assert.AreEqual(persistence._tickRate, model.Persistence._tickRate);
                Assert.AreEqual(persistence._gameTime, model.Persistence._gameTime);
                Assert.AreEqual(persistence._playerPosition, model.Persistence._playerPosition);

                mock.Verify(_mock => _mock.GyorsulasLoad(string.Empty), Times.Once());
            }
            [TestMethod]
            public async Task GyorsulasNewGameAfterLoadTest()
            {
                await model.LoadGyorsulasGame(string.Empty);

                model.NewGame();

                model.NewGame();
                Assert.IsFalse(DoWeHaveFuel(model));
                Assert.AreEqual(model.Persistence._fuel, 100);
                Assert.AreEqual(model.Persistence._tickRate, 1000);
                Assert.AreEqual(model.Persistence._gameTime, 0);
                Assert.AreEqual(model.Persistence._playerPosition, 2);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);
            }
            [TestMethod]
            public async Task GyorsulasTimerTickTest()
            {
                model.NewGame();
                await model.LoadGyorsulasGame(string.Empty);

                model.TimerTick(4);

                Assert.AreEqual(model.Persistence._gameTime, 6);
                Assert.AreEqual(model.Persistence[0, 4], 2);
                Assert.AreEqual(model.Persistence._tickRate, 800);
                Assert.AreEqual(model.Persistence._fuel, 14);
                for (int i = 0; i < 4; i++)
                {
                    model.TimerTick(4);
                }
                Assert.AreEqual(model.Persistence._gameTime, 10);
                Assert.AreEqual(model.Persistence[0, 4], 2);
                Assert.AreEqual(model.Persistence._tickRate, 770);
                Assert.AreEqual(model.Persistence._fuel, 10);
            }
            [TestMethod]
            public async Task GyorsulasProgressGameTest()
            {
                model.NewGame();
                await model.LoadGyorsulasGame(string.Empty);

                model.ProgressGame();

                Assert.AreEqual(model.Persistence[1, 0], 2);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 2, model.Persistence._playerPosition], 0);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition + 1], 0);
                Assert.AreEqual(model.Persistence._fuel, 30);

                model.Persistence.SetGrid(1, 0, 0);
                Assert.IsFalse(DoWeHaveFuel(model));
            }
            [TestMethod]
            public void GyorsulasMoveTest_Left()
            {
                model.NewGame();
                int position;

                for (int i = 1; i <= 2; i++)
                {
                    position = model.Persistence._playerPosition;
                    model.Move(true);
                    Assert.AreEqual(position - 1, model.Persistence._playerPosition);
                    Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);
                }

                model.Move(true);
                Assert.AreEqual(0, model.Persistence._playerPosition);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);
            }
            [TestMethod]
            public void GyorsulasMoveTest_Right()
            {
                model.NewGame();
                int position;

                for (int i = 1; i <= 2; i++)
                {
                    position = model.Persistence._playerPosition;
                    model.Move(false);
                    Assert.AreEqual(position + 1, model.Persistence._playerPosition);
                    Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);
                }

                model.Move(false);
                Assert.AreEqual(model.Persistence._x - 1, model.Persistence._playerPosition);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);


            }
            [TestMethod]
            public void GyorsulasGameEndTest()
            {
                model.NewGame();

                for (int i = 1; i < 100; i++)
                {
                    model.TimerTick(0);
                    Assert.IsFalse(model.GameEnd);
                    Assert.AreEqual(model.Persistence._fuel, 100 - i);
                }
                model.TimerTick(0);
                Assert.AreEqual(model.Persistence._fuel, 0);
                Assert.IsTrue(model.GameEnd);
            }
            [TestMethod]
            public async Task GyorsulasPauseTest()
            {
                model.NewGame();
                await model.LoadGyorsulasGame(string.Empty);
                model.Pause();

                model.TimerTick(3);
                model.ProgressGame();
                model.Move(true);

                for (int i = 0; i < model.Persistence._y; i++)
                {
                    for (int j = 0; j < model.Persistence._x; j++)
                    {
                        Assert.AreEqual(persistence[i, j], model.Persistence[i, j]);
                    }
                }
                Assert.AreEqual(persistence._fuel, model.Persistence._fuel);
                Assert.AreEqual(persistence._tickRate, model.Persistence._tickRate);
                Assert.AreEqual(persistence._gameTime, model.Persistence._gameTime);
                Assert.AreEqual(persistence._playerPosition, model.Persistence._playerPosition);
            }
            [TestMethod]
            public void GyorsulasMoveOnTopOfAFuelTest_Left()
            {
                model.NewGame();

                for (int i = 0; i < 30; i++)
                {
                    model.TimerTick(0);
                }

                model.Persistence.SetGrid(model.Persistence._y - 1, model.Persistence._playerPosition - 1, 2);

                model.Move(true);

                Assert.AreEqual(model.Persistence._fuel, 85);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);

                model.Persistence.SetGrid(0, 0, 0);
                Assert.IsFalse(DoWeHaveFuel(model));
            }
            [TestMethod]
            public void GyorsulasMoveOnTopOfAFuelTest_Right()
            {
                model.NewGame();

                for (int i = 0; i < 30; i++)
                {
                    model.TimerTick(0);
                }

                model.Persistence.SetGrid(model.Persistence._y - 1, model.Persistence._playerPosition + 1, 2);

                model.Move(false);

                Assert.AreEqual(model.Persistence._fuel, 85);
                Assert.AreEqual(model.Persistence[model.Persistence._y - 1, model.Persistence._playerPosition], 1);

                model.Persistence.SetGrid(0, 0, 0);
                Assert.IsFalse(DoWeHaveFuel(model));
            }
            public bool DoWeHaveFuel(GyorsulasModel model)
            {
                for (int i = 0; i < model.Persistence._y; i++)
                {
                    for (int j = 0; j < model.Persistence._x; j++)
                    {
                        if (model.Persistence[i, j] == 2)
                            return true;
                    }
                }
                return false;
            }
        }
    }