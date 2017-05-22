﻿using Backend.User;

namespace SL
{
	public interface SLInterface
	{
        object spectateActiveGame(int userId, int gameID);
        object joinActiveGame(int userId, int gameID);
        object leaveGame(SystemUser user, int gameID);

        object editUserProfile(int userId, string name, string password, string email, string avatar, int amount);

        object findAllActiveAvailableGames();
        object filterActiveGamesByPlayerName(string name);
        object filterActiveGamesByPotSize(int? size);
        object filterActiveGamesByGamePreferences(object pref);
        object filterActiveGamesByGamePreferences(string gamePolicy, int? gamePolicyLimit, int? buyInPolicy, int? startingChipsAmount, int? MinimalBet, int? minPlayers, int? maxPlayers, bool? isSpectatingAllowed, bool? isLeague, int minRank, int maxRank);
        object filterActiveGamesByGamePreferences(string gamePolicy, int? gamePolicyLimit, int? buyInPolicy, int? startingChipsAmount, int? MinimalBet, int? minPlayers, int? maxPlayers, bool? isSpectatingAllowed, bool? isLeague);
        //List<object> filterActiveGamesByGamePreferences(GamePreferences pref);
        //List<object> filterActiveGamesByGamePreferences(GameTypePolicy gamePolicy, int buyInPolicy, int startingChipsAmount, int MinimalBet, int minPlayers, int maxPlayers, bool? isSpectatingAllowed);
        object getAllGames();

        object createGame(int gameCreatorId, object pref);
        object createGame(int gameCreator, string gamePolicy, int? gamePolicyLimit, int? buyInPolicy, int? startingChipsAmount, int? MinimalBet, int? minPlayers, int? maxPlayers, bool? isSpectatingAllowed, bool? isLeague);

        object Login(string user, string password);
		object Register(string user, string password, string email, string userImage);
		object Logout(int userId);

        object getUserByName(string name);
        object getUserById(int userId);
        object getGameById(int gameId);
        //void replayGame(int gameId);
        //ReturnMessage addLeague(int minRank, int maxRank, string name);
        //ReturnMessage removeLeague(League league);
        //      League getLeagueByName(string name);
        //      League getLeagueById(Guid leagueId);
        //ReturnMessage setLeagueCriteria(int minRank, int maxRank, string leagueName, Guid leagueId, int userId);

        #region game
        object Bet(int gameId, int playerUserId, int coins);
        object AddMessage(int gameId, int playerIndex, string messageText);
        object Fold(int gameId, int playerIndex);
        object Check(int gameId, int playerIndex);
        object GetGameState(int gameId);
        object ChoosePlayerSeat(int gameId, int playerIndex);
        object GetPlayer(int gameId, int playerIndex);
        object GetPlayerCards(int gameId, int playerIndex);
        object GetShowOff(int gameId);
        #endregion
        }
}
