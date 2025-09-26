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
    /*  觀察者模式 (Observer Pattern)
     *   定義一個一對多的關係，讓多個物件監聽某個主體物件的狀態變化，並自動更新相關物件的行為。
     *   觀察者模式是一種物件導向的設計模式，其核心思想是：物件之間的關係由觀察者和被觀察者所組成，被觀察者在狀態變化時，會通知所有觀察者，觀察者再根據自己的需求來更新自己的行為。
     *   觀察者模式的主要角色：
     *   1. Subject (主體物件)：被觀察者，也就是被通知的物件。
     *   2. Observer (觀察者)：監聽主體物件的物件。
     *   3. ConcreteSubject (具象主體物件)：實作主體物件的具象類別。
     *   4. ConcreteObserver (具象觀察者)：實作觀察者的具象類別。
     *   5. Client (客戶端)：使用主體物件的類別。
     *   觀察者模式的優點：
     *   1. 降低耦合度：主體物件和觀察者之間的耦合度降低，可讓主體物件和觀察者之間的通信簡化。
     *   2. 容易增加和刪除觀察者：增加或刪除觀察者只需要修改主體物件的狀態，而不需要修改觀察者的程式碼。
     *   3. 容易封裝狀態：主體物件的狀態變化時，觀察者可以根據自己的需求來更新自己的行為，而不用知道主體物件的實際狀態。
     *   觀察者模式的缺點：
     *   1. 觀察者太多時，效率低下：當觀察者太多時，主體物件的狀態變化會通知所有觀察者，造成效率的降低。
     *   2. 觀察者太多時，通知過多：當觀察者太多時，主體物件的狀態變化會通知所有觀察者，造成通知過多的問題。
     *   3. 觀察者沒有對應的主體物件：當觀察者沒有對應的主體物件時，無法監聽到主體物件的狀態變化。
     *   4. 觀察者的通知順序不固定：當主體物件的狀態變化時，觀察者的通知順序不固定，造成觀察者的行為不一致。
     * 📌 題目描述

        請你實作一個簡易的 GameManager，他會觸發兩種事件：

        遊戲開始事件 (OnGameStart)

        玩家得分事件 (OnPlayerScored)

        同時建立兩個系統來「訂閱」這些事件：

        ScoreSystem：監聽玩家得分事件，並印出「玩家目前得分」。

        UIManager：監聽遊戲開始事件，並印出「遊戲開始！UI 已更新」。

        🎯延續任務：加入玩家生命值 (HP) 系統
        🎯延續任務：新增 玩家等級系統 (LevelSystem)
            功能需求 : 
            玩家累積得分後可以升級。
            每累積 50 分 → 升 1 級
            初始等級為 1
            當玩家升級時，打印：
     */

    public class GameManager
    {
        public event Action OnGameStart; // 加上 event 關鍵字，讓其他類別可以訂閱
        public event Action OnGameEnd;
        public event Action<int> OnPlayScrored;
        public event Action<int> OnPlayerDamaged;
        public event Action<int> OnPlayerLevel;

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
        public int _score;
        public event Action<int> OnScoreChanged;
        public ScoreSystem(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnPlayScrored += UpdateScore;               
        }
        public void UpdateScore(int point)
        {
            _score += point;
            Console.WriteLine($"玩家目前得分 : {_score}");
            OnScoreChanged?.Invoke(_score); 
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


    public class LevelSystem : IDisposable
    {
        private ScoreSystem _scoreSystem;
        private int _level;
        public int level => _level; // 新增屬性，取得等級
        public Action<int> OnLevelChanged;
        public LevelSystem(ScoreSystem scoreSystem)
        {
            _scoreSystem = scoreSystem;
            _scoreSystem.OnScoreChanged += GetLevelScored;
            _level = 1;
        }
        public void GetLevelScored(int score)
        {
            if (score >= 50)
            {
                _level++;
                Console.WriteLine($"玩家升級，等級 {_level}");
                OnLevelChanged?.Invoke(_level);
            }
        }
        //取得等級
        public void PlayerLevel()
        {
            Console.WriteLine($"玩家等級 {level}");
        }
        public void Dispose()
        {
            //_scoreSystem.OnScoreChanged -= GetLevelScored;
            Console.WriteLine("LevelSystem 已釋放");
        }
    }


    public class Delegate6
    {
        GameManager gm = new GameManager();
        public void Main()
        {
            using (var ui = new UIManger(gm))
            using (var ss = new ScoreSystem(gm))
            using (var hs = new HealthSystem(gm))
            using (var ls = new LevelSystem(ss))
            {
                ls.OnLevelChanged += (level) => Console.WriteLine($"玩家等級: {level}");

                gm.StartGame();
                gm.PlayerScored(30);
                gm.PlayerScored(20);
                gm.PlayerScored(50);
                gm.PlayerDamaged(90);
                gm.PlayerDamaged(40);
                gm.PlayerDamaged(50);
                gm.EndGame();
            } // 這裡會自動呼叫 ss.Dispose() 和 ui.Dispose()
        }
    }

}
    