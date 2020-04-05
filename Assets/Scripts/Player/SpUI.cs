using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpUI : MonoBehaviour
{
    private float maxSP;
    //設置一個有矩形的位置大小等訊息之物件
    public RectTransform SPHealthBar, SPHurtBar, thisBar;
    //耐力刷新量
    [Header("耐力刷新量")]
    [SerializeField]
    private int SPSpeed = 15;
    //根據spspeed減少的血量

    private Vector2 SPBar;
    //比較慢的漸近sp
    private Vector2 SPSlowBar;
    //開始血量sp
    private Vector2 SPiniBar;
    //0
    private Vector2 SPzero;
    //是否可以sp
    private bool StartSP;
    //玩家
    public Player playerSP;

    public bool IsRunning = false;
    void Start()
    {
        maxSP = playerSP.PlayerSP;
        SPHealthBar.sizeDelta = new Vector2(playerSP.PlayerSP, SPHealthBar.sizeDelta.y);
        SPHurtBar.sizeDelta = new Vector2(playerSP.PlayerSP, SPHurtBar.sizeDelta.y);
        thisBar.sizeDelta = new Vector2(playerSP.PlayerSP, thisBar.sizeDelta.y);
        SPiniBar = new Vector2(maxSP, SPHealthBar.sizeDelta.y);
        SPSlowBar = new Vector2(Time.deltaTime * SPSpeed, 0);
        SPzero = new Vector2(0, SPHealthBar.sizeDelta.y);
        SPBar = new Vector2(SPSpeed, 0);
    }
    void Update()
    {
        //如果黃條>橘條
        if (SPHurtBar.sizeDelta.x > SPHealthBar.sizeDelta.x)
        {
            //慢慢地漸進跟上
            SPHurtBar.sizeDelta -= SPSlowBar * 2;
        }
        //如果黃條<=橘條
        else if (SPHurtBar.sizeDelta.x <= SPHealthBar.sizeDelta.x)
        {
            //兩個相等
            SPHurtBar.sizeDelta = SPHealthBar.sizeDelta;
            //玩家sp等於橘條
            playerSP.PlayerSP = SPHealthBar.sizeDelta.x;
            
            //開始sp   
            StartSP = true;
        }
        //如果sp>0
        if (SPHealthBar.sizeDelta.x >= 0)
        {
            //如果當前sp<最大sp且可以回sp
            if (SPHealthBar.sizeDelta.x < maxSP && StartSP && playerSP.OnAction == false && !IsRunning)
            {
                //慢慢地回
                SPHealthBar.sizeDelta += SPSlowBar * 1.5f;
                playerSP.PlayerSP = SPHealthBar.sizeDelta.x;
            }
            //如果當前sp>最大sp
            else if (SPHealthBar.sizeDelta.x > maxSP)
            {
                //回到原始狀態
                SPHealthBar.sizeDelta = SPiniBar;
                //不能回
                StartSP = false;
            }
        }
        //目前sp<=0
        else if (SPHealthBar.sizeDelta.x < 0)
        {
            //sp=0
            SPHealthBar.sizeDelta = SPzero;
            playerSP.PlayerSP = 0;
            
        }
    }
    /// <summary>
    /// 扣sp
    /// </summary>
    public void CoSp(float co)
    {
        SPHealthBar.sizeDelta -= new Vector2(co,0);
        
    }
}