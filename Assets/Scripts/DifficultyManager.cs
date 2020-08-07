using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public Dropdown pageDifficultyDropDown;
    public Dropdown pageTypeOfPlayDropDown;
    public Dropdown pageTypeOfAiDropDown;
    
    public enum DifficultyState{Easy, Medium, Hard};
    public DifficultyState difficultyState;
    
    public enum TypeOfPlay { SinglePlayer, Multiplayer };
    public TypeOfPlay typeOfPlay;
    
    public enum TypeOfAi {NormalAi, AdvancedAi};
    public TypeOfAi typeOfAi;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            this.difficultyState = FindObjectOfType<DifficultyManager>().difficultyState;
            this.typeOfPlay = FindObjectOfType<DifficultyManager>().typeOfPlay;
            Destroy(FindObjectOfType<DifficultyManager>().gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PopulateDifficultyList();
        PopulateTypeofPlayList();
        PopulateTypeOfAiList();
    }

    public int DifficultyThreatCheck()
    {
        switch (difficultyState)
        {
            case DifficultyState.Easy:
                return -1;
            case DifficultyState.Medium:
                return 0;
            case DifficultyState.Hard:
                return 2;
            default:
                break;
        }
        return 0;
    }
    
    public int DifficultyHealthNumCheck(int maxHealth)
    {
        switch (difficultyState)
        {
            case DifficultyState.Easy:
                return Mathf.RoundToInt((float)0.75 * maxHealth);
            case DifficultyState.Medium:
                return Mathf.RoundToInt((float)0.50 * maxHealth);
            case DifficultyState.Hard:
                return Mathf.RoundToInt((float)0.25 * maxHealth);
            default:
                break;
        }

        return 0;
    }

    #region For Use In Difficulty DropDowns ONLY
    private void PopulateDifficultyList()
    {
        List<string> names = new List<string>();
        names.Add("Easy");
        names.Add("Meduim");
        names.Add("Hard");
        pageDifficultyDropDown.AddOptions(names);
        switch (difficultyState)
        {
            case DifficultyState.Easy:
                pageDifficultyDropDown.value = 0;
                break;
            case DifficultyState.Medium:
                pageDifficultyDropDown.value = 1;
                break;
            case DifficultyState.Hard:
                pageDifficultyDropDown.value = 2;
                break;
            default:
                break;
        }
    }
    
    public void Dropdown_DifficultyIndexChanged(int i)
    {
        switch (i)
        {
            case 0:
                difficultyState = DifficultyState.Easy;
                break;
            case 1:
                difficultyState = DifficultyState.Medium;
                break;
            case 2:
                difficultyState = DifficultyState.Hard;
                break;
            default:
                break;
        }
    }
    #endregion

    #region For Use In TypeOfPlay DropDowns ONLY
    private void PopulateTypeofPlayList()
    {
        List<string> names = new List<string>();
        names.Add("Single Player");
        names.Add("Multiplayer");
        pageTypeOfPlayDropDown.AddOptions(names);
        switch (typeOfPlay)
        {
            case TypeOfPlay.SinglePlayer:
                pageTypeOfPlayDropDown.value = 0;
                break;
            case TypeOfPlay.Multiplayer:
                pageTypeOfPlayDropDown.value = 1;
                break;
        }
    }

    public void DropDown_TypeOfPlayIndexChanged(int i)
    {
        switch (i)
        {
            case 0:
                typeOfPlay = TypeOfPlay.SinglePlayer;
                break;
            case 1:
                typeOfPlay = TypeOfPlay.Multiplayer;
                break;
        }
    }
    #endregion

    #region For Use In TypeOfAI DropDowns ONLY
    public void PopulateTypeOfAiList()
    {
        List<string> names = new List<string>();
        names.Add("Normal Ai");
        names.Add("Advanced Ai");
        pageTypeOfAiDropDown.AddOptions(names);
        switch (typeOfAi)
        {
            case TypeOfAi.NormalAi:
                pageTypeOfPlayDropDown.value = 0;
                break;
            case TypeOfAi.AdvancedAi:
                pageTypeOfPlayDropDown.value = 1;
                break;
        }
    }

    public void DropDown_TypeOfAiIndexChange(int i)
    {
        switch (i)
        {
            case 0:
                typeOfAi = TypeOfAi.NormalAi;
                break;
            case 1:
                typeOfAi = TypeOfAi.AdvancedAi;
                break;
        }
    }
    #endregion
}
