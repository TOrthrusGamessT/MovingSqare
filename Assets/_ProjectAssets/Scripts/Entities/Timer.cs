using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;


public class Timer : MonoBehaviour
{
   #region Singleton  
   public static Timer instance;

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else if (instance != this)
      {
         Destroy(gameObject);
         return;
      }

      if (isSurviveMode)
      {
         StartSurviveModeTimer().Forget();
      }
   }
   #endregion

   [SerializeField] private bool isSurviveMode = false;
   public static Action onCounterEnd;

   [SerializeField] private TextMeshProUGUI timerText;


   private int lvlDuration;
   private CancellationTokenSource _ct = new();
   private UniTask _counterTask = default;
   private int elapsedSeconds = 0;
   private bool isTimerPaused = false;


   public static int Duration
   {
      set => instance.lvlDuration = value;
   }

   public static bool IsSurviveMode => instance != null && instance.isSurviveMode;
   public static int ElapsedSeconds => instance != null ? instance.elapsedSeconds : 0;


   private void OnEnable()
   {
      PlayerLife.onPlayerDie += PauseTimer;
   }

   private void OnDisable()
   {
      PlayerLife.onPlayerDie -= PauseTimer;
      _ct?.Cancel();
      _ct?.Dispose();
   }

   public async void StartCounter()
   {
      if (isTimerPaused) return;
      
      await AnimateTimerUp(lvlDuration);
      if (!isTimerPaused) // Check again after animation
      {
         _counterTask = Counter();
         _counterTask.Forget();
      }
   }

   private async UniTask AnimateTimerUp(float totalSeconds)
   {
      int current = 0;
      await DOTween.To(() => current, x =>
      {
         current = Mathf.RoundToInt(x);
         UpdateUITimer(current);
      }, totalSeconds, 1)
      .SetEase(Ease.Linear).AsyncWaitForCompletion();
   }

   private void UpdateUITimer(int totalSeconds)
   {
      if (totalSeconds >= 60)
      {
         int minutes = totalSeconds / 60;
         int seconds = totalSeconds % 60;
         timerText.text = $"{minutes}:{seconds:D2}";
      }
      else
      {
         timerText.text = $"{totalSeconds}";
      }
   }

   private void PauseTimer()
   {
      isTimerPaused = true;
      try
      {
         _ct?.Cancel();
      }
      catch (ObjectDisposedException)
      {
         // Token already disposed, ignore
      }
      
      _ct?.Dispose();
      _ct = new CancellationTokenSource();
      _counterTask = default;
   }

   private void ResumeTimer()
   {
      if (isTimerPaused && (_counterTask.Status != UniTaskStatus.Pending))
      {
         isTimerPaused = false;
         _counterTask = Counter();
         _counterTask.Forget();
      }
   }

   public void ResumeTimerPublic()
   {
      ResumeTimer();
   }


   private async UniTask Counter()
   {
      try
      {
         while (!isTimerPaused && lvlDuration > 0)
         {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _ct.Token);

            if (isTimerPaused) break; // Double-check after delay

            lvlDuration--;
            UpdateUITimer(lvlDuration);
            
            if (lvlDuration <= 0)
            {
               FireBase.LogCustomEvent("lvl_completed", new System.Collections.Generic.Dictionary<string, object>
               {
                  { "level_index", LVLIndexer.currentLvlIndex + 1 },
                  { "total_time_seconds", ElapsedSeconds }
               });
               onCounterEnd?.Invoke();
               break;
            }
         }
      }
      catch (OperationCanceledException)
      {
         // Timer was cancelled/paused, this is expected behavior
         Debug.Log("Timer was paused/cancelled");
      }
   }

   private async UniTask StartSurviveModeTimer()
   {
      elapsedSeconds = 0;
      try
      {
         while (true)
         {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
            elapsedSeconds++;
            UpdateUITimer(elapsedSeconds);
         }
      }
      catch (OperationCanceledException)
      {
         FireBase.LogCustomEvent("survive_mode_timer_stopped", new System.Collections.Generic.Dictionary<string, object>
         {
            { "elapsed_seconds", elapsedSeconds }
         });
      }
   }
}
