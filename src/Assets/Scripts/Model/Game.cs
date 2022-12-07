using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunters
{
    public class Game
    {
        public Board CurrentBoard => CurrentPlayer.Board;
        public Player CurrentPlayer => Players[_currentPlayer];
        public int PlayersCount => GameSettings.PlayersCount;

        public readonly List<Player> Players = new();
        private int _currentPlayer = 0;

        public delegate void GameEvent();

        public event GameEvent OnStartTurn;
        public event GameEvent OnEndTurn;
        public event GameEvent OnPlayerClicked;


        public void EndTurn()
        {
            OnEndTurn?.Invoke();
        }

        public void StartNextTurn()
        {
            _currentPlayer++;

            if (_currentPlayer >= GameSettings.PlayersCount)
                _currentPlayer = 0;

            OnStartTurn?.Invoke();
        }

        public void PlayerClicked()
        {
            OnPlayerClicked?.Invoke();
        }

        public void Init()
        {
            {
                var externalPlayer =
                    new NtlStudio.TreasureHunters.Model.Player("Player 1",
                        new NtlStudio.TreasureHunters.Model.Position());
                
                Debug.Log("The external player with name " + externalPlayer.Name + " has been created");
                
                var board = new Board(Levels.Level01);
                var player = new Player()
                {
                    Name = "Player 1",
                    Color = UnityEngine.Color.blue,
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                Players.Add(player);
            }

            {
                var board = new Board(Levels.Level01);
                var player = new Player()
                {
                    Name = "Player 2",
                    Color = UnityEngine.Color.yellow,
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                Players.Add(player);
            }

            Debug.Log("Game initialized successfully");
        }
    }
}