﻿using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected List<PlayedGame> testPlayedGames = new List<PlayedGame>();
        protected GameDefinition testGameDefinition;
        protected GameDefinition testGameDefinitionWithOtherGamingGroupId;
        protected Player testPlayer1;
        protected string testPlayer1Name = "testPlayer1";
        protected Player testPlayer2;
        protected string testPlayer2Name = "testPlayer2";
        protected Player testPlayer3;
        protected string testPlayer3Name = "testPlayer3";
        protected Player testPlayer4;
        protected string testPlayer4Name = "testPlayer4";
        protected Player testPlayer5;
        protected string testPlayer5Name = "testPlayer5";
        protected Player testPlayer6;
        protected string testPlayer6Name = "testPlayer6";
        protected Player testPlayer7WithOtherGamingGroupId;
        protected string testPlayer7Name = "testPlayer7";
        protected string testGameName = "this is test game definition name";
        protected string testGameNameForGameWithOtherGamingGroupId = "this is test game definition name for game with other GamingGroupId";
        protected string testGameDescription = "this is a test game description 123abc";
        protected string testApplicationUserNameForUserWithDefaultGamingGroup = "username with default gaming group";
        protected UserContext testUserContextForUserWithDefaultGamingGroup;
        protected string testApplicationUserNameForUserWithOtherGamingGroup = "username with other gaming group";
        protected UserContext testUserContextForUserWithOtherGamingGroup;
        protected string testGamingGroup1Name = "this is test gaming group 1";
        protected string testGamingGroup2Name = "this is test gaming group 2";

        protected GamingGroup gamingGroup;
        protected GamingGroup otherGamingGroup;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                CleanUpTestData();

                gamingGroup = SaveGamingGroup(dbContext, testGamingGroup1Name);
                otherGamingGroup = SaveGamingGroup(dbContext, testGamingGroup2Name);

                testUserContextForUserWithDefaultGamingGroup = SaveApplicationUser(
                    dbContext, 
                    testApplicationUserNameForUserWithDefaultGamingGroup, 
                    "a@mailinator.com",
                    gamingGroup.Id);
                testUserContextForUserWithOtherGamingGroup = SaveApplicationUser(
                    dbContext,
                    testApplicationUserNameForUserWithOtherGamingGroup,
                    "b@mailinator.com",
                    otherGamingGroup.Id);

                testGameDefinition = SaveGameDefinition(dbContext, gamingGroup.Id, testGameName);
                testGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(dbContext, otherGamingGroup.Id, testGameNameForGameWithOtherGamingGroupId);
                SavePlayers(dbContext, gamingGroup.Id, otherGamingGroup.Id);

                CreatePlayedGames(dbContext);
            }
        }

        protected void CreatePlayedGames(NemeStatsDbContext dbContext)
        {
            PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);

            List<Player> players = new List<Player>() { testPlayer1, testPlayer2 };
            List<int> playerRanks = new List<int>() { 1, 1 };
            PlayedGame playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer3, testPlayer2 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer3, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //make player4 beat player 1 three times
            players = new List<Player>() { testPlayer4, testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3, 4 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //--make the inactive player5 beat player1 3 times
            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //make player 2 be the only one who beat player 5
            players = new List<Player>() { testPlayer2, testPlayer5 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //--create a game that has a different GamingGroupId
            players = new List<Player>() { testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithOtherGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);
        }

        protected void SavePlayers(NemeStatsDbContext dbContext, int primaryGamingGroupId, int otherGamingGroupId)
        {
            testPlayer1 = new Player() { Name = testPlayer1Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player() { Name = testPlayer2Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer2);
            testPlayer3 = new Player() { Name = testPlayer3Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer3);
            testPlayer4 = new Player() { Name = testPlayer4Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer4);
            testPlayer5 = new Player() { Name = testPlayer5Name, Active = false, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer5);
            testPlayer6 = new Player() { Name = testPlayer6Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer6);

            testPlayer7WithOtherGamingGroupId = new Player() { Name = testPlayer7Name, Active = true, GamingGroupId = otherGamingGroupId };
            dbContext.Players.Add(testPlayer7WithOtherGamingGroupId);

            dbContext.SaveChanges();
        }

        protected GameDefinition SaveGameDefinition(NemeStatsDbContext dbContext, int gamingGroupId, string gameDefinitionName)
        {
            GameDefinition gameDefinition = new GameDefinition() { Name = testGameName, Description = testGameDescription, GamingGroupId = gamingGroupId };
            dbContext.GameDefinitions.Add(gameDefinition);
            dbContext.SaveChanges();

            return gameDefinition;
        }

        protected UserContext SaveApplicationUser(NemeStatsDbContext dbContext, string userName, string email, int gamingGroupId)
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = email,
                UserName = userName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CurrentGamingGroupId = gamingGroupId
            };
            dbContext.Users.Add(applicationUser);
            dbContext.SaveChanges();

            UserContext userContext = new UserContext()
            {
                ApplicationUserId = applicationUser.Id,
                GamingGroupId = gamingGroupId
            };

            return userContext;
        }

        protected GamingGroup SaveGamingGroup(NemeStatsDbContext dbContext, string gamingGroupName)
        {
            GamingGroup gamingGroup = new GamingGroup() { Name = gamingGroupName };
            dbContext.GamingGroups.Add(gamingGroup);
            dbContext.SaveChanges();
            return gamingGroup;
        }

        protected PlayedGame CreateTestPlayedGame(
            List<Player> players,
            List<int> correspondingPlayerRanks,
            UserContext userContext,
            PlayedGameLogic playedGameLogic)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();

            for (int i = 0; i < players.Count(); i++)
            {
                playerRanks.Add(new PlayerRank()
                    {
                        PlayerId = players[i].Id,
                        GameRank = correspondingPlayerRanks[i]
                    });
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
                {
                    GameDefinitionId = testGameDefinition.Id,
                    PlayerRanks = playerRanks,
                };

            return playedGameLogic.CreatePlayedGame(newlyCompletedGame, userContext);
        }

        protected void CleanUpTestData()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                CleanUpPlayerGameResults(dbContext);
                CleanUpPlayedGames(dbContext);
                CleanUpGameDefinitions(dbContext, testGameName);
                CleanUpGameDefinitions(dbContext, testGameNameForGameWithOtherGamingGroupId);
                CleanUpPlayers(dbContext);
                CleanUpGamingGroup(testGamingGroup1Name, dbContext);
                CleanUpGamingGroup(testGamingGroup2Name, dbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroup, dbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithOtherGamingGroup, dbContext);

                dbContext.SaveChanges();
            }
        }

        protected void CleanUpPlayers(NemeStatsDbContext dbContext)
        {
            CleanUpPlayerByPlayerName(testPlayer1Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer2Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer3Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer4Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer5Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer6Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer7Name, dbContext);
        }

        protected void CleanUpApplicationUser(string testApplicationUserName, NemeStatsDbContext dbContext)
        {
            ApplicationUser applicationUserToDelete = (from applicationUser in dbContext.Users
                                                       where applicationUser.UserName == testApplicationUserName
                                                       select applicationUser).FirstOrDefault();

            if (applicationUserToDelete != null)
            {
                try
                {
                    dbContext.Users.Remove(applicationUserToDelete);
                }
                catch (Exception) { }
            }
        }

        protected void CleanUpGamingGroup(string testGamingGroupName, NemeStatsDbContext dbContext)
        {
            GamingGroup gamingGroupToDelete = (from gamingGroup in dbContext.GamingGroups
                                               where gamingGroup.Name == testGamingGroupName
                                               select gamingGroup).FirstOrDefault();

            if (gamingGroupToDelete != null)
            {
                try
                {
                    dbContext.GamingGroups.Remove(gamingGroupToDelete);
                }
                catch (Exception) { }
            }
        }

        protected void CleanUpPlayedGames(NemeStatsDbContext dbContext)
        {
            List<PlayedGame> playedGamesToDelete = (from playedGame in dbContext.PlayedGames
                                                    where playedGame.GameDefinition.Name == testGameName
                                                    select playedGame).ToList();

            foreach (PlayedGame playedGame in playedGamesToDelete)
            {
                try
                {
                    dbContext.PlayedGames.Remove(playedGame);
                }
                catch (Exception) { }
            }
        }

        protected void CleanUpPlayerGameResults(NemeStatsDbContext dbContext)
        {
            List<PlayerGameResult> playerGameResultsToDelete = (from playerGameResult in dbContext.PlayerGameResults
                                                                where playerGameResult.PlayedGame.GameDefinition.Name == testGameName
                                                                select playerGameResult).ToList();

            foreach (PlayerGameResult playerGameResult in playerGameResultsToDelete)
            {
                try
                {
                    dbContext.PlayerGameResults.Remove(playerGameResult);
                }
                catch (Exception) { }
            }
        }

        protected void CleanUpGameDefinitions(NemeStatsDbContext dbContext, string gameDefinitionName)
        {
            List<GameDefinition> gameDefinitionsToDelete = (from game in dbContext.GameDefinitions
                                                            where game.Name == gameDefinitionName
                                                            select game).ToList();

            foreach (GameDefinition game in gameDefinitionsToDelete)
            {
                try
                {
                    dbContext.GameDefinitions.Remove(game);
                }
                catch (Exception) { }
            }
        }

        protected static void CleanUpPlayerByPlayerName(string playerName, NemeStatsDbContext nemeStatsDbContext)
        {
            Player playerToDelete = nemeStatsDbContext.Players.FirstOrDefault(player => player.Name == playerName);

            if (playerToDelete != null)
            {
                try
                {
                    nemeStatsDbContext.Players.Remove(playerToDelete);
                }
                catch (Exception) { }
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            CleanUpTestData();
        }
    }
}
