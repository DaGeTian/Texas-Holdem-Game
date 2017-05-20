﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SL;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace CLServer
{
    public class CLImpl
    {
        private static SLInterface sl = null;

        /// <summary>
        /// Task to proccess the client's requests.
        /// </summary>
        /// <param name="obj">The tcp client.</param>
        private static void ProcessClientRequests(Object obj)
        {
            TcpClient client = (TcpClient)obj;

            try
            {
                while (true)
                {
                    var jsonObject = getJsonObjectFromStream(client);

                    tryExecuteAction(client, jsonObject);
                }
            }
            catch (TargetInvocationException tie)
            {
                Console.WriteLine(tie.InnerException);
                SendMessage(client, new { exception = "An Error Has Occured" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                SendMessage(client, new { exception = "An Error Has Occured" });
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Sends an exception message to the client.
        /// </summary>
        /// <param name="client">The client to send to.</param>
        /// <param name="message">The message to send. (Optional)</param>
        private static void SendMessage(TcpClient client, object message = null)
        {
            var messageString       = JObject.FromObject(message);

            var serializedMessage   = JsonConvert.SerializeObject(messageString);

            var messageByteArray    = Encoding.ASCII.GetBytes(serializedMessage);

            try
            {
                if (client.GetStream().CanWrite)
                {
                    client.GetStream().Write(messageByteArray, 0, messageByteArray.Length);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// returns a JObject as data from the client stream.
        /// </summary>
        /// <param name="client">The tcp client</param>
        /// <returns>The JObject</returns>
        private static JObject getJsonObjectFromStream(TcpClient client)
        {
            var message = new byte[1024];

            var bytesRead = client.GetStream().Read(message, 0, 1024);

            string myObject = Encoding.ASCII.GetString(message);
            Object deserializedProduct = JsonConvert.DeserializeObject(myObject);
            return JObject.FromObject(deserializedProduct);
        }

        /// <summary>
        /// Tries to execute the action given by the client.
        /// </summary>
        /// <param name="jsonObject">The Object received from the client.</param>
        private static void tryExecuteAction(TcpClient client, JObject jsonObject)
        {
            var actionToken = jsonObject["action"];

            // In case the json object does not have property 'action'
            if ((actionToken == null) ||
               (actionToken.Type == JTokenType.String && String.IsNullOrWhiteSpace(actionToken.ToString())) ||
               (actionToken.Type != JTokenType.String))
            {
                throw new ArgumentException("Error: No action specified.");
            }

            var action = (string)actionToken;

            Console.WriteLine("Trying to execute action: {0}", action);

            var method = typeof(CLImpl).GetMethod(action, BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException("Error: No known action called.");
            }

            method.Invoke(null, new object[] { client, jsonObject });
        }

        #region GameWindow
        private static void Bet(TcpClient client, JObject jsonObject)
        {
            var gameIdToken = jsonObject["gameId"];
            var playerIndexToken = jsonObject["playerIndex"];
            var coinsToken = jsonObject["coins"];

            if (((gameIdToken == null) || (gameIdToken.Type != JTokenType.Integer)) ||
                ((playerIndexToken == null) || (playerIndexToken.Type != JTokenType.Integer)) ||
                ((coinsToken == null) || (coinsToken.Type != JTokenType.Integer)))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Raise."));
            }

            var gameId = (int)gameIdToken;
            var playerIndex = (int)playerIndexToken;
            var coins = (int)coinsToken;

            Console.WriteLine("Bet. parameters are: gameId: {0}, playerIndex: {1}, coins: {2}", gameId, playerIndex, coins);

            sl.Bet(gameId, playerIndex, coins);
            var betResponse = new { response = true };

            SendMessage(client, betResponse);
            return;
        }
        private static void AddMessage(TcpClient client, JObject jsonObject)
        {
            var gameIdToken = jsonObject["gameId"];
            var playerIndexToken = jsonObject["playerIndex"];
            var messageTextToken = jsonObject["messageText"];

            if (((gameIdToken == null) || (gameIdToken.Type != JTokenType.Integer)) ||
                ((playerIndexToken == null) || (playerIndexToken.Type != JTokenType.Integer)) ||
                ((messageTextToken == null) || (messageTextToken.Type != JTokenType.String)))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Add Message."));
            }

            var gameId = (int)gameIdToken;
            var playerIndex = (int)playerIndexToken;
            var messageText = (string)messageTextToken;

            Console.WriteLine("Message added. parameters are: gameId: {0}, playerIndex: {1}, messageText: {2}", gameId, playerIndex, messageText);

            sl.AddMessage(gameId, playerIndex, messageText);
            var messageTextResponse = new { response = true };

            SendMessage(client, messageTextResponse);
            return;
        }
        private static void Fold(TcpClient client, JObject jsonObject)
        {
            var gameIdToken = jsonObject["gameId"];
            var playerIndexToken = jsonObject["playerIndex"];

            if (((gameIdToken == null) || (gameIdToken.Type != JTokenType.Integer)) ||
                ((playerIndexToken == null) || (playerIndexToken.Type != JTokenType.Integer)))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Fold."));
            }

            var gameId = (int)gameIdToken;
            var playerIndex = (int)playerIndexToken;

            Console.WriteLine("Fold. parameters are: gameId: {0}, playerIndex: {1}", gameId, playerIndex);

            sl.Fold(gameId, playerIndex);
            
            SendMessage(client, new { response = true });
            return;
        }
        #endregion

        private static void Login(TcpClient client, JObject jsonObject)
        {
            var usernameToken = jsonObject["username"];
            var passwordToken = jsonObject["password"];

            if ((usernameToken == null) || (usernameToken.Type != JTokenType.String) || 
                (String.IsNullOrWhiteSpace(usernameToken.ToString())) ||

                (passwordToken == null) || (passwordToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(passwordToken.ToString())))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Login."));
            }
            
            var loginResponse = sl.Login((string)usernameToken, (string)passwordToken);

            SendMessage(client, loginResponse);
            return;
        }

        private static void CreateGame(TcpClient client, JObject jsonObject) {
            var gameCreatorIdToken = jsonObject["gameCreatorId"];
            var gamePolicyToken = jsonObject["gamePolicy"];
            var buyInPolicyToken = jsonObject["buyInPolicy"];
            var startingChipsToken = jsonObject["startingChips"];
            var minimalBetToken = jsonObject["minimalBet"];
            var minimalPlayersToken = jsonObject["minimalPlayers"];
            var maximalPlayersToken = jsonObject["maximalPlayers"];
            var spectateAllowedToken = jsonObject["spectateAllowed"];

            if ((gameCreatorIdToken == null) || (gameCreatorIdToken.Type != JTokenType.Integer) ||
                (gamePolicyToken == null) || (gamePolicyToken.Type != JTokenType.Integer))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Create Game."));
            }

            var createGameResponse = sl.createGame(
                (int)gameCreatorIdToken, 
                (int)gamePolicyToken, 
                (int?)buyInPolicyToken, 
                (int?)startingChipsToken, 
                (int?)minimalBetToken, 
                (int?)minimalPlayersToken, 
                (int?)maximalPlayersToken, 
                (bool?)spectateAllowedToken);

            SendMessage(client, createGameResponse);
            return;
        }

        private static void getGame(TcpClient client, JObject jsonObject) {
            var gameIdToken = jsonObject["gameId"];

            if (gameIdToken == null || gameIdToken.Type != JTokenType.Integer)
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Get Game"));
            }

            var getGameResponse = sl.getGameById((int)gameIdToken);

            SendMessage(client, getGameResponse);
            return;
        }

        private static void Register(TcpClient client, JObject jsonObject)
        {
            var usernameToken   = jsonObject["username"];
            var passwordToken   = jsonObject["password"];
            var emailToken      = jsonObject["email"];
            var userImageToken  = jsonObject["userImage"];
            if ((usernameToken == null) || (usernameToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(usernameToken.ToString())) ||

                (passwordToken == null) || (passwordToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(passwordToken.ToString())) ||

                (emailToken == null) || (emailToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(emailToken.ToString())) ||

                (userImageToken == null) || (userImageToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(userImageToken.ToString())))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Register"));
            }

            var registerResponse = sl.Register(
                (string)usernameToken, 
                (string)passwordToken, 
                (string)emailToken, 
                (string)userImageToken);

            SendMessage(client, registerResponse);
            return;
        }

        private static void Logout(TcpClient client, JObject jsonObject)
        {
            var userIdToken = jsonObject["userId"];

            if (userIdToken == null || userIdToken.Type != JTokenType.Integer)
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Logout"));
            }

            var logoutResponse = sl.Logout((int)userIdToken);

            SendMessage(client, logoutResponse);
            return;
        }

        private static void JoinActiveGame(TcpClient client, JObject jsonObject)
        {
            var gameIdToken = jsonObject["gameId"];
            var userIdToken = jsonObject["userId"];

            if ((gameIdToken == null) || (gameIdToken.Type != JTokenType.Integer) ||
                (userIdToken == null) || (userIdToken.Type != JTokenType.Integer))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Join Active Game"));
            }

            var joinActiveGameResponse = sl.joinActiveGame((int)userIdToken, (int)gameIdToken);

            SendMessage(client, joinActiveGameResponse);
            return;
        }

        private static void SpectateActiveGame(TcpClient client, JObject jsonObject)
        {
            var gameIdToken = jsonObject["gameId"];
            var userIdToken = jsonObject["userId"];

            if ((gameIdToken == null) || (gameIdToken.Type != JTokenType.Integer) ||
                (userIdToken == null) || (userIdToken.Type != JTokenType.Integer))
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Spectate Active Game"));
            }

            var spectateActiveGameResponse = sl.spectateActiveGame((int)userIdToken, (int)gameIdToken);

            SendMessage(client, spectateActiveGameResponse);
            return;
        }

        private static void FindAllActiveAvailableGames(TcpClient client, JObject jsonObject)
        {
            var findAllActiveAvailableGamesResponse = sl.findAllActiveAvailableGames();

            SendMessage(client, findAllActiveAvailableGamesResponse);
            return;
        }

        //TODO:: Obsolete because game preferences is now decorator. Not finished.
        private static void FilterActiveGamesByGamePreferences(TcpClient client, JObject jsonObject)
        {
            var gamePolicy = jsonObject.Value<int?>("gamePolicy");
            var buyInPolicy = jsonObject.Value<int?>("buyInPolicy");
            var startingChips = jsonObject.Value<int?>("startingChips");
            var minimalBet = jsonObject.Value<int?>("minimalBet");
            var minimalPlayers = jsonObject.Value<int?>("minimalPlayers");
            var maximalPlayers = jsonObject.Value<int?>("maximalPlayers");
            var spectateAllowed = jsonObject.Value<bool?>("spectateAllowed");
        }

        private static void FilterActiveGamesByPotSize(TcpClient client, JObject jsonObject)
        {
            var potSizeToken = jsonObject["potSize"];

            if ((potSizeToken == null) || (potSizeToken.Type != JTokenType.Integer))
            {
                throw new TargetInvocationException(
                    new ArgumentException("Error: Parameters Mismatch at Filter Active Games By Pot Size"));
            }

            var filterActiveGamesByPotSizeResponse = sl.filterActiveGamesByPotSize((int?)potSizeToken);

            SendMessage(client, filterActiveGamesByPotSizeResponse);
            return;
        }

        private static void FilterActiveGamesByPlayerName(TcpClient client, JObject jsonObject)
        {
            var playerNameToken = jsonObject["playerName"];

            if ((playerNameToken == null) || (playerNameToken.Type != JTokenType.String) ||
                (String.IsNullOrWhiteSpace(playerNameToken.ToString())))
            {
                throw new TargetInvocationException(
                    new ArgumentException("Error: Parameters Mismatch at Filter Active Games By Player Name"));
            }

            var filterActiveGamesByPotSizeResponse = sl.filterActiveGamesByPlayerName((string)playerNameToken);

            SendMessage(client, filterActiveGamesByPotSizeResponse);
            return;
        }

        private static void EditUserProfile(TcpClient client, JObject jsonObject)
        {
            var userIdToken     = jsonObject["userId"];
            var nameToken       = jsonObject["name"];
            var passwordToken   = jsonObject["password"];
            var emailToken      = jsonObject["email"];
            var avatarToken     = jsonObject["avatar"];

            if (userIdToken == null || userIdToken.Type != JTokenType.Integer)
            {
                throw new TargetInvocationException(new ArgumentException("Error: Parameters Mismatch at Edit User Profile"));
            }

            var editUserProfileResponse = sl.editUserProfile(
                (int)userIdToken,
                (string)nameToken,
                (string)passwordToken,
                (string)emailToken, 
                (string)avatarToken);

            SendMessage(client, editUserProfileResponse);
            return;
        }

        static void Main()
        {
            TcpListener listener = null;
            try
            {
                var address = IPAddress.Parse("127.0.0.1");
                var port    = 2345;
                listener    = new TcpListener(address, port);

                listener.Start();

                Console.WriteLine(
                    String.Format("Server has been initialized at IP: {0} PORT: {1}", 
                    address.ToString(), 
                    port));

                while (true)
                {
                    Console.WriteLine("Waiting for new connection.");

                    TcpClient client = listener.AcceptTcpClient();

                    Console.WriteLine("Accepted new client");

                    Thread t = new Thread(ProcessClientRequests);
                    t.Start(client);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
        }
    }
}
