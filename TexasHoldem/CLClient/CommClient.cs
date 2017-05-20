﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CLClient.Entities;

namespace CLClient
{
    public static class CommClient
    {
        private static TcpClient client = new TcpClient("127.0.0.1", 2345);

        #region Static functionality

        public static JObject sendMessage(object obj)
        {

            var jsonObj             = JObject.FromObject(obj);
            var serializedJsonObj   = JsonConvert.SerializeObject(jsonObj);

            var networkStream = client.GetStream();

            if (networkStream.CanWrite)
            {
                var jsonObjArray = Encoding.ASCII.GetBytes(serializedJsonObj);

                networkStream.Write(jsonObjArray, 0, jsonObjArray.Length);
            }
                return getJsonObjectFromStream(client);
        }

        private static JObject getJsonObjectFromStream(TcpClient client)
        {
            var message = new byte[1024];

            var bytesRead = client.GetStream().Read(message, 0, 1024);

            string myObject = Encoding.ASCII.GetString(message);

            Object deserializedProduct = JsonConvert.DeserializeObject(myObject);

            Console.WriteLine(deserializedProduct);

            return JObject.FromObject(deserializedProduct);
        }

        #endregion

        #region PL Functions

        public static SystemUser Login(string username, string password)
        {
            var message     = new { action = "Login", username, password };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<SystemUser>();

            return response;
        }

        public static Boolean Logout(int userId)
        {
            var message     = new { action = "Logout", userId };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<Boolean>();

            return response;
        }

        public static TexasHoldemGame CreateGame(int gameCreatorId, string gamePolicy, int gamePolicyLimit, int buyInPolicy, int startingChips, int minimalBet, int minimalPlayers, int maximalPlayers, bool? spectateAllowed, bool? isLeague)
        {
            var message = new
            {
                action = "CreateGame",
                gameCreatorId,
                gamePolicy,
                gamePolicyLimit,
                buyInPolicy,
                startingChips,
                minimalBet,
                minimalPlayers,
                maximalPlayers,
                spectateAllowed,
                isLeague
            };

            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<TexasHoldemGame>();

            return response;
        }

        public static TexasHoldemGame getGame(int gameId)
        {
            var message     = new { action = "GetGame", gameId };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<TexasHoldemGame>();

            return response;
        }

        public static TexasHoldemGame joinActiveGame(int userId, int gameId)
        {
            var message     = new { action = "JoinActiveGame", userId, gameId };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<TexasHoldemGame>();

            return response;
        }

        public static TexasHoldemGame spectateActiveGame(int userId, int gameId)
        {
            var message     = new { action = "SpectateActiveGame", userId, gameId };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<TexasHoldemGame>();

            return response;
        }

        public static List<TexasHoldemGame> findAllActiveAvailableGames()
        {
            var message     = new { action = "FindAllActiveAvailableGames" };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<List<TexasHoldemGame>>();

            return response;
        }

        public static List<TexasHoldemGame> filterActiveGamesByGamePreferences(string gamePolicy, int? buyInPolicy, int? startingChips, int? minimalBet, int? minimalPlayers, int? maximalPlayers, bool? spectateAllowed, bool? isLeague)
        {
            var message = new
            {
                action = "FilterActiveGamesByGamePreferences",
                gamePolicy,
                buyInPolicy,
                startingChips,
                minimalBet,
                minimalPlayers,
                maximalPlayers,
                spectateAllowed,
                isLeague
            };

            var jsonMessage = sendMessage(message);
            var response = jsonMessage.ToObject<List<TexasHoldemGame>>();

            return response;
        }

        public static List<TexasHoldemGame> filterActiveGamesByPotSize(int potSize)
        {
            var message = new { action = "FilterActiveGamesByPotSize", potSize };

            var jsonMessage = sendMessage(message);
            var response = jsonMessage.ToObject<List<TexasHoldemGame>>();

            return response;
        }

        public static List<TexasHoldemGame> filterActiveGamesByPlayerName(string playerName)
        {
            var message = new { action = "FilterActiveGamesByPlayerName", playerName };

            var jsonMessage = sendMessage(message);
            var response = jsonMessage.ToObject<List<TexasHoldemGame>>();

            return response;
        }

        public static Boolean raiseBet(int gameId, int playerIndex, int coins)
        {
            var message     = new { action = "Raise", gameId, playerIndex, coins };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<Boolean>();

            return response;
        }

        public static SystemUser Register(string username, string password, string email, string userImage)
        {
            var message     = new { action = "Register", username, password, email, userImage };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<SystemUser>();

            return response;
        }

        public static Boolean editUserProfile(int userId, string name, string password, string email, string avatar, int amount)
        {
            var message     = new { action = "EditUserProfile", userId, name, password, email, avatar, amount };
            var jsonMessage = sendMessage(message);
            var response    = jsonMessage.ToObject<Boolean>();

            return response;
        }

        #endregion
    }
}
