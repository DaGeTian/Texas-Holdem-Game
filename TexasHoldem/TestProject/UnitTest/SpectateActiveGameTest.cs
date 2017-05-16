﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SL;
using Backend.User;
using Backend;
using Moq;
using DAL;
using System.Collections.Generic;
using Backend.Game;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class SpectateActiveGameTest
    {

        SLInterface sl;

        [TestInitialize]
        public void SetUp()
        {
            var usersList = new List<SystemUser>
            {
                new SystemUser("Hadas", "Aa123456", "email0", "image0", 1000),
                new SystemUser("Gili", "123123", "email1", "image1", 0),
                new SystemUser("Or", "111111", "email2", "image2", 700),
                new SystemUser("Aviv", "Aa123456", "email3", "image3", 1500)
            };

            //var leagues = new List<League>
            //{
            //    new League(0, 1000, "Starter League"),
            //    new League(1000, 2000, "Experienced League")
            //};

            var gamesList = new List<TexasHoldemGame>
            {
                new TexasHoldemGame(usersList[0], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 9, true)),
                new TexasHoldemGame(usersList[0], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 9, false)),
                new TexasHoldemGame(usersList[1], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, true)),
                new TexasHoldemGame(usersList[1], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false)),
                new TexasHoldemGame(usersList[3], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false, 0, 1000)),
                new TexasHoldemGame(usersList[3], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false, 1000, 2000))
            };

            Mock<DALInterface> dalMock = new Mock<DALInterface>();
            dalMock.Setup(x => x.registerUser(It.IsAny<SystemUser>())).Returns(new ReturnMessage(true, null));
            dalMock.Setup(x => x.logUser(It.IsAny<string>())).Returns(new ReturnMessage(true, null));
            dalMock.Setup(x => x.logOutUser(It.IsAny<string>())).Returns(new ReturnMessage(true, null));
            dalMock.Setup(x => x.getAllGames()).Returns(gamesList);
            dalMock.Setup(x => x.getUserByName(It.IsAny<string>())).Returns((string name) => usersList.Find(u => u.name == name));
            this.sl = new SLImpl(dalMock.Object);
        }

        [TestMethod]
        public void spectateSuccessTest()
        {
            SystemUser user = sl.getUserById(2);
            var spectateMessage = sl.spectateActiveGame(user, sl.getAllGames()[0].gameId);
         //   Assert.IsTrue(spectateMessage.success);
        }

        [TestMethod]
        public void spectateFailesPreferencesTest()
        {
            SystemUser user = sl.getUserById(0);
            Assert.IsFalse(sl.spectateActiveGame(user, 1).success);
        }

        [TestMethod]
        public void spectateFailesAlreadySpectateTest()
        {
            SystemUser user = sl.getUserById(3);
            sl.spectateActiveGame(user, 0);
            Assert.IsFalse(sl.spectateActiveGame(user, 0).success);
        }

        [TestMethod]
        public void spectateFailesAlreadyPlayTest()
        {
            SystemUser user = sl.getUserById(3);
            sl.joinActiveGame(user, 0);
            Assert.IsFalse(sl.spectateActiveGame(user, 0).success);
        }

        [TestMethod]
        public void spectateFailsGameNoExistsTest()
        {
            SystemUser user = sl.getUserById(0);
            Assert.IsFalse(sl.spectateActiveGame(user, 1000).success);
        }
    }
}
