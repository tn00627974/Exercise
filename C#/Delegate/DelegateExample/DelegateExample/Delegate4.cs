using DelegateExample6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DelegateExample
{
    // using System;
    // IDisposable 是 .NET 提供的介面，用來釋放非受控資源
    //public interface IDisposable
    //{
    //    void Dispose();
    //}

    public class GameManager
    {
        public event Action OnGameStart;  // 加上 event 關鍵字，讓其他類別可以訂閱
        public event Action OnGameEnd;
        public int playerSocre;

        public void StartGame()
        {
            Console.WriteLine("Game Start");
            OnGameStart?.Invoke();
        }

        public void EndGame()
        {
            Console.WriteLine("Game End");
            OnGameEnd?.Invoke();
        }
    }

    public class ResoureceSystem : IDisposable
    {
        private GameManager _gameManager;
        public ResoureceSystem(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnGameStart += LoadResources;
            _gameManager.OnGameEnd += UnloadResources;

            //_gameManager.OnGameStart?.Invoke(); // 若是 Action 在任何地方能呼叫，比較不好，改成 event。
        }

        // 解構子 Destructor : 當物件被垃圾回收時會自動呼叫
        //~ResoureceSystem()
        //{
        //    _gameManager.OnGameStart -= LoadResources;
        //    _gameManager.OnGameEnd -= UnloadResources;
        //}

        public void Dispose()
        {
            _gameManager.OnGameStart -= LoadResources;
            _gameManager.OnGameEnd -= UnloadResources;
        }

        public void LoadResources() => Console.WriteLine("Loading Resources");
        public void UnloadResources() => Console.WriteLine("Unloading Resources");
    }

    public class ScoreSystem : IDisposable
    {
        private GameManager _gameManager;
        public ScoreSystem(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnGameStart += LoadResources;
            _gameManager.OnGameEnd += UnloadResources;

            //_gameManager.OnGameStart?.Invoke(); // 若是 Action 在任何地方能呼叫，比較不好，改成 event。
        }

        // 解構子 Destructor : 當物件被垃圾回收時會自動呼叫
        //~ScoreSystem()
        //{
        //    _gameManager.OnGameStart -= LoadResources;
        //    _gameManager.OnGameEnd -= UnloadResources;
        //}
        public void Dispose()
        {
            _gameManager.OnGameStart -= LoadResources;
            _gameManager.OnGameEnd -= UnloadResources;
        }

        public void LoadResources() => Console.WriteLine("Loading Scores");
        public void UnloadResources() => Console.WriteLine("Unloading Scores");
    }

    public class Delegate4
    {
        private GameManager _gameManager;
        private ResoureceSystem _resoureceSystem;
        private ScoreSystem _scoreSystem;


        public void Main()
        {
            _gameManager = new GameManager();
            _resoureceSystem = new ResoureceSystem(_gameManager);
            _scoreSystem = new ScoreSystem(_gameManager);

            // 透過 using 來確保 Dispose 被呼叫，實作資源釋放
            using (var rs = new ResoureceSystem(_gameManager))
            using (var ss = new ScoreSystem(_gameManager))

            _gameManager.StartGame();
            _gameManager.EndGame();
        }
    }
   
}
    