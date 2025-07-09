using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LVLButtonBehaviour : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RawImage bk;
    public Texture2D inactiveTexture2D;

    public void Init(string lvlIndex, bool reached)
    {
        text.text = lvlIndex;
        SetStatus(reached);
    }

    private void SetStatus(bool reached)
    {
        if (reached)
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                LVLIndexer.currentLvlIndex = int.Parse(text.text) - 1;
                SceneLoader.instance.LoadLvlScene();
                FireBase.LogCustomEvent("lvl_button_clicked", new System.Collections.Generic.Dictionary<string, object>
                {
                    { "level_index", int.Parse(text.text) }
                });
            });
        }
        else
        {
            text.color = Color.white;
            bk.texture = inactiveTexture2D;
        }
    }

}
