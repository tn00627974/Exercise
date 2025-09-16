using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DelegateExample
{
    internal class GameManager 
    {
        private event Action _OnGameStart;  // 加上 event 關鍵字，讓其他類別可以訂閱
        private event Action _OnGameEnd; 

        public void StartGame() 
        {
            Console.WriteLine("Game Start");
            _OnGameStart?.Invoke();
        }

        public void EndGame()
        {
            Console.WriteLine("Game End");
            _OnGameEnd?.Invoke();
        }

        public class ResoureceSystem
        {
            private GameManager _gameManager;
            public ResoureceSystem(GameManager gameManager)
            {
                _gameManager = gameManager;
                _gameManager._OnGameStart += LoadResources;
                _gameManager._OnGameEnd += UnloadResources;

                //_gameManager._OnGameStart?.Invoke(); // 若是 Action 在任何地方能呼叫，比較不好。
            }

            public void LoadResources() => Console.WriteLine("Loading Resources");
            public void UnloadResources() => Console.WriteLine("Unloading Resources");
        }

        public class ScoreSystem
        {
            private GameManager _gameManager;
            public ScoreSystem(GameManager gameManager)
            {
                _gameManager = gameManager;
                _gameManager._OnGameStart += LoadResources;
                _gameManager._OnGameEnd += UnloadResources;
            }

            public void LoadResources() => Console.WriteLine("Loading Scores");
            public void UnloadResources() => Console.WriteLine("Unloading Scores");
 
        }



        public void Main()
        {
            GameManager newGame = new GameManager();
            ResoureceSystem resoureceSystem = new ResoureceSystem(newGame);
            ScoreSystem scoreSystem = new ScoreSystem(newGame);

            newGame.StartGame();
            newGame.EndGame();

        }


    }
    
}
