using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    
    public bool musicOn, soundOn;
    public TextMeshProUGUI money;

    
    void Start()
    {
        int cash =PlayerPrefs.GetInt("Money");
        if(cash<1000){
            money.text =cash.ToString();
        }else{
            cash = cash/1000;
            money.text = cash +"k";
        }
        AudioSetup();
    }
    
    public void MusicToogle(GameObject parent)
    {
        if (musicOn)
        {
            musicOn = false;
            this.GetComponent<AudioSource>().Stop();
        }
        else
        {
            musicOn = true;
            this.GetComponent<AudioSource>().Play();
        }
        
        GameObject onIcon = parent.transform.GetChild(0).gameObject;
        GameObject offIcon = parent.transform.GetChild(1).gameObject;
        
        onIcon.SetActive(musicOn);
        offIcon.SetActive(!musicOn);
    }
    public void SoundToogle(GameObject parent)
    {
        soundOn = !soundOn;
    
        GameObject onIcon = parent.transform.GetChild(0).gameObject;
        GameObject offIcon = parent.transform.GetChild(1).gameObject;
    
        onIcon.SetActive(soundOn);
        offIcon.SetActive(!soundOn);
    }
    public void SaveData()
    {
        if (musicOn)
        {
            PlayerPrefs.SetInt("music", 1);
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
        }
        if (soundOn)
        {
            PlayerPrefs.SetInt("sound", 1);
        }
        else
        {
            PlayerPrefs.SetInt("sound", 0);
        }
    }
    public void AudioSetup()
    {
        // Music setup
        if (PlayerPrefs.HasKey("music"))
        {
            if (PlayerPrefs.GetInt("music") == 1)
            {
                musicOn = true;
                this.GetComponent<AudioSource>().Play();
            }
            else
            {
                musicOn = false;
            }
        }
        else
        {
            musicOn = true;
            this.GetComponent<AudioSource>().Play();
        }
        
        // Update music icons
        GameObject musicOnIcon = GameObject.Find("Music").transform.GetChild(0).gameObject;
        GameObject musicOffIcon = GameObject.Find("Music").transform.GetChild(1).gameObject;
        musicOnIcon.SetActive(musicOn);
        musicOffIcon.SetActive(!musicOn);
        
        // Sound setup
        if (PlayerPrefs.HasKey("sound"))
        {
            if (PlayerPrefs.GetInt("sound") == 1)
            {
                soundOn = true;
            }
            else
            {
                soundOn = false;
            }
        }
        else
        {
            soundOn = true;
        }
        
        // Update sound icons
        GameObject soundOnIcon = GameObject.Find("Sound").transform.GetChild(0).gameObject;
        GameObject soundOffIcon = GameObject.Find("Sound").transform.GetChild(1).gameObject;
        soundOnIcon.SetActive(soundOn);
        soundOffIcon.SetActive(!soundOn);
    }
}
