using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> creditsPages;
    public Dropdown pageDropDown;
    public Animator mainMenuAnim;

    private void Start()
    {
        PopulateList();
        FindObjectOfType<AudioManager>().Play("Doggo_Theme");
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
        FindObjectOfType<AudioManager>().StopAll();
    }

    public void LoadPcgGame()
    {
        SceneManager.LoadScene("Scenes/ProceduralGenScene");
        FindObjectOfType<AudioManager>().StopAll();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        mainMenuAnim.SetTrigger("openCredits1");
    }

    public void CloseCredits()
    {
        mainMenuAnim.SetTrigger("closeCredits");
    }

    public void OpenDifficulty()
    {
        mainMenuAnim.SetTrigger("openISettings1");    
    }

    public void CloseDifficulty()
    {
        mainMenuAnim.SetTrigger("closeSettings");
    }
    
    private void PopulateList()
    {
        List<string> names = new List<string>();
        foreach (GameObject item in creditsPages)
        {
            names.Add(item.name);
        }
        pageDropDown.AddOptions(names);
    }
    
    public void Dropdown_IndexChanged(int i)
    {
        foreach (GameObject item in creditsPages)
        {
            item.SetActive(false);
        }
        creditsPages[i].SetActive(true);
        Debug.Log(creditsPages[i]);
    }

    public void OpenURL(String url)
    {
        Application.OpenURL(url);
    }
}
