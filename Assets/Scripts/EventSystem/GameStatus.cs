using System.IO;
using UnityEngine;

namespace Utils.EventSystem
{
    public class GameStatus
    {
        private string name;
        private string gameStatus;

        public GameStatus()
        {
            name = "";
            gameStatus = "";
        }
        
        public GameStatus(string newName)
        {
            name = newName;
        }

        public void SetStatus()
        {
            Directory.Delete(string.Join(Path.DirectorySeparatorChar.ToString(), Application.persistentDataPath,
                Path.DirectorySeparatorChar.ToString(), "EventFiles" , Path.DirectorySeparatorChar.ToString()));
            
            CustomEvent.currentEvents.ForEach(cusEve =>
                    {
                        if (cusEve is NumericalEvent numericalEvent)
                        {
                            gameStatus += $"{numericalEvent.name}: {numericalEvent.VisibleCount},/n";
                            FileUtils.SaveScriptable(numericalEvent, numericalEvent.name, "EventFiles");
                        }                            
                    }
                );
        }

        public string GetStatusString()
        {
            return gameStatus;
        }

        public void SetName(string newName)
        {
            name = newName;
        }
        
        public string GetName()
        {
            return name;
        }
    }
}