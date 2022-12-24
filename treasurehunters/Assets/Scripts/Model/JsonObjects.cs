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
    class PlayersJson
    {
        public string sessionid;
    }

    [System.Serializable]
    class GameStateJson
    {
        public string state;
    }

    [System.Serializable]
    class CurrentPlayerJson
    {
        public int index;
        public string name;
    }

    [System.Serializable]
    class PlayerInfoJson
    {
        public int x;
        public int y;
        public int[] visiblearea;
    }

    [System.Serializable]
    class DataJson<T>
    {
        public T data;
        public bool successful;
    }
}
