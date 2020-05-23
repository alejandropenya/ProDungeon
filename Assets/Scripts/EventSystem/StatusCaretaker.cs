namespace Utils.EventSystem
{
    public class StatusCaretaker
    {
        private GameStatus _gameStatus;

        public GameStatus GameStatus
        {
            get => _gameStatus;
            set => _gameStatus = value;
        }
    }
}