using NtlStudio.TreasureHunters.Model;

namespace JsonObjects
{
    [System.Serializable]
    public class NewGameJson
    {
        public string id;
    }

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
    public class TreasurePositionJson
    {
        public int x;
        public int y;
    }

    [System.Serializable]
    public class PlayerSessionIdJson
    {
        public string sessionid;
    }

    [System.Serializable]
    public class GameStateJson
    {
        public string state;
    }

    [System.Serializable]  
    public class CurrentPlayerJson
    {
        public int index;
        public string name;
        public string gamestate;
    }

    [System.Serializable]
    public class PlayerActionStateJson
    {
        public Position position;
        public ActionDirection direction;
        public ActionType type;
        public FieldCell cell;
    }

    [System.Serializable]
    public class PlayerMoveStatesJson
    {
        public string player;
        public PlayerActionStateJson[] actionstates;
    }

    [System.Serializable]
    public class PlayersMoveStatesJson
    {
        public PlayerMoveStatesJson[] players;
    }

    [System.Serializable]
    public class PlayerInfoJson
    {
        public int x;
        public int y;
        public bool isalive;
        public bool isplayerturn;
        public int[] visiblearea;
        public GameState gamestate;
        public string currentplayername;
    }

    [System.Serializable]
    public class PlayerActionResult
    {
        public bool hastreasure;
        public string state; // Game State
    }

    [System.Serializable]
    public class DataJson<T>
    {
        public T data;
        public bool successful;
    }

    public class PlayerSessionIdDataJson : DataJson<PlayerSessionIdJson> { }
    public class TreasurePositionDataJson : DataJson<TreasurePositionJson> { }
    public class CurrentPlayerDataJson : DataJson<CurrentPlayerJson> { }
    public class PlayersMoveHistoryDataJson : DataJson<PlayersMoveStatesJson> { }
    public class NewGameDataJson : DataJson<NewGameJson> { }
    public class DeleteGameDataJson : DataJson<string> { }
    public class GameDataJson : DataJson<GameJson> { }
    public class GameStateDataJson : DataJson<GameStateJson> { }
    public class GamesDataJson : DataJson<GamesJson> { }
    public class PlayerActionResultDataJson : DataJson<PlayerActionResult> { }
    public class PlayerInfoDataJson : DataJson<PlayerInfoJson> { }
    public class WinnerNameDataJson : DataJson<string> { }
}
