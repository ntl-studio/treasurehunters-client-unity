using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;

namespace JsonObjects
{
    [System.Serializable]
    public class GameJson
    {
        public string id;
        public string state;
        public int playerscount;
        public string[] players;
    }

    [System.Serializable]
    public class GamesJson
    {
        public GameJson[] games;
    }

    [System.Serializable]
    class GameDataJson
    {
        public GameJson data;
    }

    [System.Serializable]
    class GamesDataJson
    {
        public GamesJson data;
    }

    [System.Serializable]
    class PlayersJson
    {
        public string sessionid;
    }

    [System.Serializable]
    class PlayersDataJson
    {
        public PlayersJson data;
        public bool successful;
    }

    [System.Serializable]
    class GameStateJson
    {
        public string state;
    }

    [System.Serializable]
    class GameStateDataJson
    {
        public GameStateJson data;
    }

    [System.Serializable]
    class CurrentPlayerJson
    {
        public int index;
        public string name;
    }

    [System.Serializable]
    class CurrentPlayerDataJson
    {
        public CurrentPlayerJson data;
    }

    [System.Serializable]
    class VisibleAreaJson
    {
        public int[] visiblearea;
    }

    [System.Serializable]
    class VisibleAreaDataJson 
    {
        public VisibleAreaJson data;
    }

    [System.Serializable]
    class PlayerPositionJson
    {
        public int x;
        public int y;
    }

    [System.Serializable]
    class PlayerPositionDataJson
    {
        public PlayerPositionJson data;
    }
}
