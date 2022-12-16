namespace JsonObjects
{
    [System.Serializable]
    public class GameJson
    {
        public string id;
        public string state;
        public int playerscount;
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
}
