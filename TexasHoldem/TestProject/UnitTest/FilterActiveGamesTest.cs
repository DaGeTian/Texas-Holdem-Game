﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SL;
using Backend.Game;
using System.Collections.Generic;
using DAL;
using Backend.User;
using Moq;
using Backend.Game.DecoratorPreferences;
using static Backend.Game.DecoratorPreferences.GamePolicyDecPref;
using ApplicationFacade;

namespace TestProject
{
    [TestClass]
    public class FilterActiveGamesTest
    {
        SLInterface sl;
        GameCenter center;
        [TestInitialize]
        public void SetUp()
        {
            var userList = new List<SystemUser>
            {
                new SystemUser("Hadas", "Aa123456", "email0", "image0", 1000),
                new SystemUser("Gili", "123123", "email1", "image1", 0),
                new SystemUser("Or", "111111", "email2", "image2", 700),
                new SystemUser("Aviv", "Aa123456", "email3", "image3", 1500)
            };

            center = GameCenter.getGameCenter();

            //set users ranks.
            userList[0].rank = 10;
            userList[1].rank = 15;
            userList[2].rank = 20;
            userList[3].rank = 25;

            for (int i = 0; i < 4; i++)
            {
                userList[i].id = i;
                //center.login(userList[i].name, userList[i].password);
            }

            //set the leagues
            center.maintainLeagues(userList);

            //get the league of user 3
            League l = center.getUserLeague(userList[3]);

            //setting the games
            //pref order: mustpref(spectate,league)->game type , buy in policy, starting chips, minimal bet, minimum players, maximum players.
            var gamesList = new List<TexasHoldemGame>
            {
                //regular games
                new TexasHoldemGame(userList[0],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (9,null) ))))),true)),
                new TexasHoldemGame(userList[0],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (9,null) ))))),false)),
                new TexasHoldemGame(userList[1],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),true)),
                new TexasHoldemGame(userList[1],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),false)),
                new TexasHoldemGame(userList[2],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),false)),
                new TexasHoldemGame(userList[2],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),false)),
                //league games
                new TexasHoldemGame(userList[3],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),false,l.minRank,l.maxRank)),
                new TexasHoldemGame(userList[3],new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit,0,
                                                                    new BuyInPolicyDecPref(100,new StartingAmountChipsCedPref(500,
                                                                    new MinBetDecPref(20,new MinPlayersDecPref(2,
                                                                    new MaxPlayersDecPref (2,null) ))))),false,l.minRank,l.maxRank))
            //new TexasHoldemGame(userList[0], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 9, true)),
            //    new TexasHoldemGame(userList[0], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 9, false)),
            //    new TexasHoldemGame(userList[1], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, true)),
            //    new TexasHoldemGame(userList[1], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false)),
            //    new TexasHoldemGame(userList[2], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false)),
            //    new TexasHoldemGame(userList[2], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false)),
            //    new TexasHoldemGame(userList[3], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false, 0, 1000)),
            //    new TexasHoldemGame(userList[3], new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false, 1000, 2000))
            };

            for (int i=0; i<gamesList.Count; i++)
            {
                gamesList[i].gameId = i;
                center.texasHoldemGames.Add(gamesList[i]);
            }

            Mock<DALInterface> dalMock = new Mock<DALInterface>();
            dalMock.Setup(x => x.getAllGames()).Returns(gamesList);
            dalMock.Setup(x => x.getUserById(It.IsAny<int>())).Returns((int i) => userList[i]);
            dalMock.Setup(x => x.getGameById(It.IsAny<int>())).Returns((int i) => gamesList[i]);
            sl = new SLImpl();
        }

        [TestMethod]
        public void filterActiveGamesByPlayerNameSuccessTest()
        {
            var user2 = center.getUserById(2);
            var m = sl.joinActiveGame(user2.id, 3);
            
            CollectionAssert.AreNotEqual(center.filterActiveGamesByPlayerName("Hadas"),new List<TexasHoldemGame>());
        }

        [TestMethod]
        public void filterActiveGamesByPlayerNameTwoGamesTest()
        {
            var user2 = center.getUserById(2);
            var m = sl.joinActiveGame(user2.id, 3);

            var m2 = sl.joinActiveGame(user2.id, 0);

            Assert.AreEqual(center.filterActiveGamesByPlayerName("Hadas").Count, 2);
        }

        [TestMethod]
        public void filterActiveGamesByPlayerNameTwoGamesFailsTest()
        {
            var user2 = center.getUserById(0);
            sl.joinActiveGame(user2.id, 3);

            sl.joinActiveGame(user2.id, 0);

            Assert.AreEqual(center.filterActiveGamesByPlayerName("Shaked").Count, 0);
        }

        [TestMethod]
        public void filterActiveGamesByPotSizeTest()
        {
            var user2 = center.getUserById(0);
            sl.joinActiveGame(user2.id, 3);

            sl.joinActiveGame(user2.id, 0);

            Assert.AreEqual(center.filterActiveGamesByPotSize(0).Count, 8);
        }

        [TestMethod]
        public void filterActiveGamesByPotSizeFailsTest()
        {
            var user2 = center.getUserById(0);
            sl.joinActiveGame(user2.id, 3);

            sl.joinActiveGame(user2.id, 0);

            Assert.AreEqual(center.filterActiveGamesByPotSize(100).Count, 8);
        }

        [TestMethod]
        public void filterActiveGamesByPreferencesTest()
        {
            MustPreferences mustPref = new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit, 0,
                                                           new BuyInPolicyDecPref(100, new StartingAmountChipsCedPref(500,
                                                           new MinBetDecPref(20, new MinPlayersDecPref(2,
                                                           new MaxPlayersDecPref(9, null)))))), true);
            //GamePreferences pref = new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 9, true);
            
            Assert.AreEqual(center.filterActiveGamesByGamePreferences(mustPref).Count, 1);
        }

        [TestMethod]
        public void filterActiveGamesByFewPreferencesTest()
        {
            MustPreferences mustPref = new MustPreferences(new BuyInPolicyDecPref(100,null), true);

            Assert.AreEqual(center.filterActiveGamesByGamePreferences(mustPref).Count, 2);
        }

        [TestMethod]
        public void filterActiveGamesBySomePreferencesTest()
        {
            Assert.AreEqual(center.filterActiveGamesByGamePreferences(null, null, 100, null,null,null,null,true,false).Count, 2);
        }


        [TestMethod]
        public void filterActiveGamesByPreferencesThreeGamesTest()
        {
            MustPreferences mustPref = new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit, 0,
                                                           new BuyInPolicyDecPref(100, new StartingAmountChipsCedPref(500,
                                                           new MinBetDecPref(20, new MinPlayersDecPref(2,
                                                           new MaxPlayersDecPref(2, null)))))), false);

            //GamePreferences pref = new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 100, 500, 20, 2, 2, false);

            Assert.AreEqual(center.filterActiveGamesByGamePreferences(mustPref).Count, 3);
        }

        [TestMethod]
        public void filterActiveGamesByPreferencesFailsTest()
        {
            MustPreferences mustPref = new MustPreferences(new GamePolicyDecPref(GameTypePolicy.No_Limit, 0,
                                                           new BuyInPolicyDecPref(1000000, new StartingAmountChipsCedPref(500,
                                                           new MinBetDecPref(20, new MinPlayersDecPref(2,
                                                           new MaxPlayersDecPref(2, null)))))), false);

            //GamePreferences pref = new GamePreferences(GamePreferences.GameTypePolicy.no_limit, 1000000, 500, 20, 2, 2, false);

            Assert.AreEqual(center.filterActiveGamesByGamePreferences(mustPref).Count, 0);
        }

        [TestCleanup]
        public void TearDown()
        {
            center.shutDown();
        }
    }
}
