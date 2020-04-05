using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemTime : MonoBehaviour
{
    public float KeepTime;
    private bool CountOn = false;
    private bool IsAdim = false;
    private float Temp;
    private float CountTime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("Oh My Adim!!");
            IsAdim = true;
        }

        if (Input.GetKey(KeyCode.N))
        {
            if(IsAdim)
                ITimeStop();
        }
        if (Input.GetKey(KeyCode.M))
        {
            if(IsAdim)
                ITimeStart();
        }
        if (CountOn)
        {
            KeepTime -= CountTime;
            if (KeepTime < 0)
            {
                ITimeStart();
                CountOn = false;
                KeepTime = Temp;
            }
        }
    }
    public void ITimeStop()
    {
        Debug.Log("Time Stop");
        Time.timeScale = 0;
    }
    public void ITimeStart()
    {
        Debug.Log("Time GoGo");
        Time.timeScale = 1;
    }
    public void LostFrame()
    {
        CountTime = Time.deltaTime;
        Temp = KeepTime;
        CountOn = true;
        ITimeStop();
    }
}
