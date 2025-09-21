using DelegateExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static DelegateExample.Delegate5;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DelegateExample6
{
    /*
     * 📌 題目描述

        請你實作一個簡易的 GameManager，他會觸發兩種事件：

        遊戲開始事件 (OnGameStart)

        玩家得分事件 (OnPlayerScored)

        同時建立兩個系統來「訂閱」這些事件：

        ScoreSystem：監聽玩家得分事件，並印出「玩家目前得分」。

        UIManager：監聽遊戲開始事件，並印出「遊戲開始！UI 已更新」。

        延續任務：加入玩家生命值 (HP) 系統
     */

    public class GameManager
    {
        public event Action OnGameStart; // 加上 event 關鍵字，讓其他類別可以訂閱
        public event Action OnGameEnd;
        public event Action<int> OnPlayScrored;
        public event Action<int> OnPlayerDamaged;

        public void StartGame()
        {
            Console.Write("遊戲開始! ");
            //if (OnGameStart != null ) OnGameStart.Invoke();
            OnGameStart?.Invoke();
        }

        public void EndGame()
        {
            Console.Write("遊戲結束! ");
            //if (OnGameStart != null ) OnGameStart.Invoke();
            OnGameEnd?.Invoke();
        }

        public void PlayerScored(int point)
        {
            Console.WriteLine($"得分 : {point}");
            //if (OnPlayScrored != null ) OnPlayScrored.Invoke(point);
            OnPlayScrored?.Invoke(point); 
        }
        public void PlayerDamaged(int damage)
        {
            Console.WriteLine($"玩家受傷 {damage}");
            OnPlayerDamaged?.Invoke(damage);
        }
    }

    public class ScoreSystem : IDisposable
    {
        private GameManager _gameManager;
        private int _score;
        public ScoreSystem(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnPlayScrored += UpdateScore;               
            _score = 0;
        }
        public void UpdateScore(int point)
        {
            _score += point;
            Console.WriteLine($"玩家目前得分 : {_score}");
        }

        public void Dispose()
        {
            _gameManager.OnPlayScrored -= UpdateScore;
            Console.WriteLine("ScoreSystem 已釋放");
        }
    }

    public class UIManger : IDisposable
    {
        private GameManager _gameManager;
        private int _score;
        public UIManger(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnGameStart += UpdateUI;
        }

        public void UpdateUI()
        {
            Console.WriteLine("UI 已更新");
        }

        public void Dispose()
        {
            _gameManager.OnGameStart -= UpdateUI;
            Console.WriteLine("UIManger 已釋放");
        }
    }

    public class HealthSystem : IDisposable
    {
        private GameManager _gameManager;
        private int _hp;
        public HealthSystem(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnPlayerDamaged += UpdateHealth;
            _hp = 100;
        }
        public void UpdateHealth(int damage)
        {
            _hp -= damage;
            if (_hp < 0) { _hp = 0; }
            Console.WriteLine($"玩家受傷，剩餘血量: {_hp}");
            if (_hp <= 0) 
            {
                _gameManager.EndGame();
                Console.WriteLine($"玩家死亡！遊戲結束！"); 
            }
            
        }
        public void Dispose()
        {
            _gameManager.OnPlayerDamaged -= UpdateHealth;
            Console.WriteLine("HealthSystem 已釋放");
        }
    }


    public class Delegate6
    {
        GameManager gm = new GameManager();
        public void Main()
        {
            using (var ss = new ScoreSystem(gm))          
            using (var ui = new UIManger(gm))
            using (var hs = new HealthSystem(gm))
            {
                gm.StartGame();
                gm.PlayerScored(10);
                gm.PlayerScored(20);
                gm.PlayerDamaged(90);
                gm.PlayerDamaged(40);
                gm.PlayerDamaged(50);
                gm.EndGame();
            } // 這裡會自動呼叫 ss.Dispose() 和 ui.Dispose()
        }
    }

}
    