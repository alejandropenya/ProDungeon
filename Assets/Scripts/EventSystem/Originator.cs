namespace Utils.EventSystem
{
    public class Originator
    {
        private string statusName;
        
        public Originator(string newStatusName, string newGameStatus)
        {
            statusName = newStatusName;
        }

        public void SetGameStatus(GameStatus newGameStatus)
        {
            statusName = newGameStatus.GetName();
        }

        public GameStatus CreateGameStatus()
        {
            return new GameStatus(statusName);
        }

        public void SetStatusName(string newName)
        {
            statusName = newName;
        }

        public string GetStatusName()
        {
            return statusName;
        }
    }
}