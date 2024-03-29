﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    [HideInInspector] public DialogueManager.WhoIsTalking whoIsTalking;
    [TextArea(3, 10)]
    public string[] sentences;
}
