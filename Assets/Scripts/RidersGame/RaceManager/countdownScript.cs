using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class countdownScript : MonoBehaviour
{
    public GameObject CountdownCanvas;
    [SerializeField] TextMeshProUGUI countdownText;
    GameObject startObject;
    float countdownTime = 5;

    void Update()
    {
        if (startObject == null)
        {
            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                int seconds = Mathf.FloorToInt(Mathf.Max(1, countdownTime - 1) % 60);
                countdownText.text = string.Format("{0}", seconds);
                if (countdownTime <= 1)
                {
                    countdownText.text = string.Format("GO!");
                    if (countdownTime <= 0)
                    {
                        CountdownCanvas.SetActive(false);
                    }
                }
                
            }
        }
    }
}
