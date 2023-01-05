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

        Game.OnPerformAction += async _ => await UpdateMovesHistoryAsync();

        Game.OnEndMove += async () => await UpdatePlayerDetailsAsync(GameClientState.WaitingForTurn);

        Game.OnGameFinished += () =>
        {
            Server.GetWinnerAsync(Game.GameId, (winnerName) => { Game.WinnerName = winnerName; });
        };
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
            var currentPlayer = await Server.GetCurrentPlayerAsync(Game.GameId);

            if (currentPlayer.name == Game.PlayerName)
            {
                Game.State = currentPlayer.gamestate == "Finished" ? GameClientState.Finished : GameClientState.YourTurn;
                break;
            }

            Game.CurrentPlayerName = currentPlayer.name;
            Debug.Log($"Waiting for your turn. Current player is {currentPlayer.name}.");
            await Task.Delay(2000);
        }
    }

    private async Task UpdateMovesHistoryAsync()
    {
        Game.PlayersMovesHistory = await Server.GetMovesHistoryAsync(Game.GameId);
    }
}
