using System;
using System.Collections;
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
   }
   #endregion

   public static Action onCounterEnd;

   [SerializeField] private TextMeshProUGUI timerText;


   private static int lvlDuration;
   private CancellationTokenSource _ct = new();
   private UniTask _counterTask = default;


   public static int Duration
   {
      set => lvlDuration = value;
   }


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
            onCounterEnd?.Invoke();
            break;
         }
      }
   }
}
