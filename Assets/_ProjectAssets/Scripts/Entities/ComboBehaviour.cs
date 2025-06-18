using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboBehaviour : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI comboTXT;

   [Header("Fill Effect")]
   [SerializeField] private RectTransform fillImage;
   public Color fillColorActive;
   public Color fillColorInactive;

   private RawImage _fillRawImage;

   private RectTransform _rectTransform;


   private int comboAmount;
   private int leanTweenID;
   private float remainingTime;
   private float timeBetweenSpawningCoins;
   private Tween fillTween;
   private Coroutine surviveModeCoroutine;

   private void Awake()
   {
      _rectTransform = GetComponent<RectTransform>();
      _fillRawImage = fillImage.GetComponent<RawImage>();
   }


   private void OnEnable()
   {
      CoinsBehaviour.onCoinDestroy += ComboStatus;
      Spawner.onSpawnManagerSetCoins += SetTimeBetweenSpawningCoins;
   }

   private void OnDisable()
   {
      CoinsBehaviour.onCoinDestroy -= ComboStatus;
      Spawner.onSpawnManagerSetCoins -= SetTimeBetweenSpawningCoins;
   }

   private void Start()
   {
      UpdateComboText();
   }

   private void SetTimeBetweenSpawningCoins(float time)
   {
      timeBetweenSpawningCoins = time;
   }


   private void ComboStatus(bool touchedByPlayer)
   {
      if (touchedByPlayer)
      {
         UpdateComboText();
         FillAnimation();
      }
      else
      {
         LoseCombo();
         ResetFillAnimation();
      }
   }

   private void UpdateComboText()
   {
      // BackgroundParticlesManager.instance.InitAnimation();
      comboAmount++;
      comboTXT.text = $"X{comboAmount}";

      CoinsBehaviour.amount = comboAmount;
      ScaleAnimation();

   }

   private void LoseCombo()
   {
      comboAmount = 1;
      comboTXT.text = $"X{comboAmount}";
      CoinsBehaviour.amount = comboAmount;
      ScaleAnimation();
   }

   private void ScaleAnimation()
   {
      LeanTween.scale(_rectTransform, Vector3.zero, 0.1f).setEasePunch()
         .setOnComplete(() =>
         {
            LeanTween.scale(_rectTransform, new Vector3(1f, 1f, 1f), 1f)
               .setEaseInElastic();
         });
   }

   private void FillAnimation()
   {
      ResetFillAnimation();
      _fillRawImage.color = fillColorActive;
      //TODO find a formula to calculate this 7.26
      fillTween = fillImage.DOLocalMoveY(-408f, 7.26f).SetEase(Ease.Linear);
   }

   private void ResetFillAnimation()
   {
      if (fillTween != null && fillTween.IsActive())
      {
         remainingTime = fillTween.Duration() - fillTween.Elapsed();
         fillTween.Kill();
      }

      LeanTween.cancel(leanTweenID);
      Vector3 initialPosition = fillImage.localPosition;
      initialPosition.y = 0f;
      fillImage.localPosition = initialPosition;
      _fillRawImage.color = fillColorInactive;
   }

}
