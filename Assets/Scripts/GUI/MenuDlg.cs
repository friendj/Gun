using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuDlg : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelOptionMenu;

    public Slider[] volumeSliders;
    public Toggle fullScreenToogle;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    int activeScreenResIndex;

    private void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screenResolution", 0);

        bool isFullScreen = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;
        SetFullScreen(isFullScreen);
        fullScreenToogle.isOn = isFullScreen;

        volumeSliders[0].value = Game.Instance.AudioManager.masterVolumePercent;
        volumeSliders[1].value = Game.Instance.AudioManager.musicVolumePercent;
        volumeSliders[2].value = Game.Instance.AudioManager.sfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        SetScreenResolution(activeScreenResIndex);
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene("Game1");
    }

    public void OnClickOption()
    {
        panelMainMenu.SetActive(false);
        panelOptionMenu.SetActive(true);
    }

    public void OnClickBack()
    {
        panelMainMenu.SetActive(true);
        panelOptionMenu.SetActive(false);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screenResolution", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullScreen;
        }

        if (isFullScreen)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution maxResolution = resolutions[resolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        Game.Instance.AudioManager.SetVolume(AudioManager.AudioChannel.Master, value);
    }

    public void SetMusicVolume(float value)
    {
        Game.Instance.AudioManager.SetVolume(AudioManager.AudioChannel.Music, value);
    }

    public void SetSfxVolume(float value)
    {
        Game.Instance.AudioManager.SetVolume(AudioManager.AudioChannel.Sfx, value);
    }
}
