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
      instance = FindObjectOfType<Timer>();

      if (instance == null)
         instance = this;

      if (isSurviveMode)
      {
         StartSurviveModeTimer().Forget();
      }
   }
   #endregion

   [SerializeField] private bool isSurviveMode = false;
   public static Action onCounterEnd;

   [SerializeField] private TextMeshProUGUI timerText;


   private static int lvlDuration;
   private CancellationTokenSource _ct = new();
   private UniTask _counterTask = default;
   private int elapsedSeconds = 0;


   public static int Duration
   {
      set => lvlDuration = value;
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
   }

   public async void StartCounter()
   {
      await AnimateTimerUp(lvlDuration);
      _counterTask = Counter();
      _counterTask.Forget();
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
      _ct.Cancel();
      _ct.Dispose();
      _ct = new();
      _counterTask = default;
   }

   private void ResumeTimer()
   {
      if (_counterTask.Status != UniTaskStatus.Pending)
      {
         _counterTask = Counter();
         _counterTask.Forget();
      }
   }


   private async UniTask Counter()
   {
      while (true)
      {
         await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _ct.Token);

         lvlDuration--;
         UpdateUITimer(lvlDuration);
         if (lvlDuration == 0)
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
