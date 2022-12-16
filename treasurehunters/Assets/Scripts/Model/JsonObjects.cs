namespace JsonObjects
{
    [System.Serializable]
    public class GameJson
    {
        public string id;
        public string state;
    }

    [System.Serializable]
    public class GamesJson
    {
        public GameJson[] games;
    }

    [System.Serializable]
    class GamesDataJson
    {
        public GamesJson data;
    }

    [System.Serializable]
    class PlayersJson
    {
        public string[] name;
    }

    [System.Serializable]
    class PlayersDataJson
    {
        public PlayersJson data;
        public bool successful;
    }
}
