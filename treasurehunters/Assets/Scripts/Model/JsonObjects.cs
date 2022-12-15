namespace JsonObjects
{
    [System.Serializable]
    class GameJson
    {
        public string id;
        public string state;
    }

    [System.Serializable]
    class GamesJson
    {
        public GameJson[] games;
    }

    [System.Serializable]
    class DataJson
    {
        public GamesJson data;
    }
}
