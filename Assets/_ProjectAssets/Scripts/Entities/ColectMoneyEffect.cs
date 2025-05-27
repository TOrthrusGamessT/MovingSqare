using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Febucci.UI;
using TMPro;
using UnityEngine;


public class ColectMoneyEffect : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color[] textColors;
    public string[] textOptions;
    private TypewriterByWord _typewriterByWord;
    private TextMeshPro _tmp;


    private void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        _typewriterByWord = GetComponent<TypewriterByWord>();
    }

    public async void ShowText(int text)
    {
        _tmp.color = textColors[UnityEngine.Random.Range(0, textColors.Length)];
        _tmp.text = textOptions[UnityEngine.Random.Range(0, textOptions.Length)];
        _tmp.text = $"X{text}";
        _typewriterByWord.StartShowingText();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _typewriterByWord.StartDisappearingText();
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        Destroy(gameObject);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
