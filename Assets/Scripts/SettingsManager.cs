using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsCanvas;
    public AudioMixer audioMixer;
    public Animator pauseMenuAnimator;
    public List<GameObject> tutorialPages;
    public TextMeshProUGUI tutorialPageNum;
    public Dropdown pageDropDown;
    public Button btnDifficulty;

    private void Start()
    {
        //settingsCanvas.SetActive(false);
        PopulateList();
    }

    private void Update()
    {
        if (FindObjectOfType<BattleManagerV2>().isInBattle == true)
        {
            btnDifficulty.interactable = false;
        }
        else
        {
            btnDifficulty.interactable = true;
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }

    private float unalteretdTime;

    #region OpenClose Menus
    public void OpenSettings()
    {
        pauseMenuAnimator.SetBool("openPauseMenu", true);
        FindObjectOfType<Player>().currentMoveSpeed = 0;
    }

    public void CloseSettings()
    {
        pauseMenuAnimator.SetBool("openPauseMenu", false);
        FindObjectOfType<Player>().currentMoveSpeed = FindObjectOfType<Player>().startingMoveSpeed;
    }

    public void OpenTutorial()
    {
        pauseMenuAnimator.SetTrigger("openTutorialMenu");
    }

    public void CloseTutorial()
    {
        pauseMenuAnimator.SetTrigger("closeTutorialMenu");
    }

    public void OpenDifficultyMenu()
    {
        pauseMenuAnimator.SetTrigger("openDifficultyMenu");
    }

    public void CloseDifficultyMeny()
    {
        pauseMenuAnimator.SetTrigger("closeDifficultyMenu");
    }
    #endregion

    private void PopulateList()
    {
        List<string> names = new List<string>();
        foreach (GameObject item in tutorialPages)
        {
            names.Add(item.name);
        }
        pageDropDown.AddOptions(names);
    }
    
    public void Dropdown_IndexChanged(int i)
    {
        foreach (GameObject item in tutorialPages)
        {
            item.SetActive(false);
        }
        tutorialPages[i].SetActive(true);
    }
}
