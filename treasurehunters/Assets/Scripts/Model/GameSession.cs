using System.Threading.Tasks;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();
    private static ServerConnection Server => ServerConnection.Instance();

    void Start()
    {
        Game.OnJoined += async () =>
        {
            await UpdatePlayerDetailsAsync(GameClientState.WaitingForStart);
            await UpdateTreasurePositionAsync_Debug();
        };

        Game.OnWaitingForStart += async () => await WaitForGameStartAsync();

        Game.OnWaitingForTurn += async () => await WaitForTurnAsync();

        // When it is again your turn, the client needs to pull updated visibility area
        Game.OnYourTurn += async () =>
        {
            await UpdatePlayerDetailsAsync(GameClientState.YourTurn);
            await UpdateMovesHistoryAsync();
        };

        Game.OnPerformActionServer += async _ =>
        {
            await UpdatePlayerDetailsAsync(Game.State);
            await UpdateMovesHistoryAsync();
        };

        Game.OnEndMove += async () => await UpdatePlayerDetailsAsync(GameClientState.WaitingForTurn);

        Game.OnGameFinished += async () => { Game.WinnerName = await Server.GetWinnerAsync(Game.GameId); };
    }

    async Task UpdatePlayerDetailsAsync(GameClientState nextState)
    {
        var player = await Server.GetPlayerInfoAsync(Game.GameId, Game.PlayerName);

        Debug.Log($"Updating player position to ({player.x}, {player.y})");
        Game.PlayerPosition = new TreasureHunters.Position(player.x, player.y);
        Game.SetVisibleArea(player.visiblearea);

        if (Game.State != GameClientState.Finished)
            Game.State = nextState;
    }

    async Task UpdateTreasurePositionAsync_Debug()
    {
        var treasurePosition = await Server.GetTreasurePositionAsync(Game.GameId);
        Game.TreasurePosition_Debug = new TreasureHunters.Position(treasurePosition.x, treasurePosition.y);
    }
    private readonly WaitForSeconds _waitFor2Seconds = new WaitForSeconds(2);

    async Task WaitForGameStartAsync()
    {
        while (true)
        {
            var gameState = await Server.GetGameStateAsync(Game.GameId);

            if (gameState.state != GameState.NotStarted.ToString())
            {
                Game.PlayersCount = gameState.playerscount;
                Game.State = GameClientState.WaitingForTurn;
                break;
            }

            Debug.Log($"Waiting for game to start. Current state is {gameState.state}.");
            await Task.Delay(2000);
        }
    }

    async Task WaitForTurnAsync()
    {
        while (true)
        {
            var playerInfo = await Server.GetPlayerInfoAsync(Game.GameId, Game.PlayerName);

            Game.SetVisibleArea(playerInfo.visiblearea);
            Game.CurrentPlayerName = playerInfo.currentplayername;

            if (!playerInfo.isalive)
            {
                Game.State = GameClientState.PlayerDied;
            }
            else if (playerInfo.gamestate == GameState.Finished)
            {
                Game.State = GameClientState.Finished;
                break;
            }
            else if (playerInfo.isplayerturn)
            {
                Game.State = GameClientState.YourTurn;
                break;
            }

            Debug.Log($"Waiting for your turn. Current player is {playerInfo.currentplayername}");
            await Task.Delay(2000);
        }
    }

    private async Task UpdateMovesHistoryAsync()
    {
        Game.PlayersMovesHistory = await Server.GetMovesHistoryAsync(Game.GameId);
    }
}
