using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public Animator notificationAnim;
    public Text txtNotification;
    public string startAnimKeyword;
    
    //public Queue<string> notifciationsQueue = n;

    public void StartNotification(String inputText)
    {
        txtNotification.text = inputText;
        notificationAnim.SetTrigger(startAnimKeyword);
    }

    public void EndNotification()
    {
        notificationAnim.SetTrigger("endNotif");
    }

    /*public void CheckIfQueueEmpty()
    {
        if (notifciationsQueue.Count > 0)
        {
            txtNotification.text = notifciationsQueue.Dequeue();
            notificationAnim.SetTrigger(startAnimKeyword);
        }
    }*/
}
