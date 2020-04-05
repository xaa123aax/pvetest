using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    //最大血量
    private float maxHealth;
    //設置一個有矩形的位置大小等訊息之物件
    public RectTransform HealthBar, HurtBar, thisBar;
    //血量刷新量
    [Header("血量刷新量")]
    [SerializeField]
    private int HpSpeed = 10;
    //根據hpspeed減少的血量
    private Vector2 HpBar;
    //比較慢的漸近血量
    private Vector2 SlowBar;
    //開始血量
    private Vector2 iniBar;
    //0
    private Vector2 zero;
    public EnemyAI enemyAI;
    void Start()
    {
        maxHealth = enemyAI.maxHp;
        HealthBar.sizeDelta = new Vector2(enemyAI.maxHp, HealthBar.sizeDelta.y);
        HurtBar.sizeDelta = new Vector2(enemyAI.maxHp, HurtBar.sizeDelta.y);
        thisBar.sizeDelta = new Vector2(enemyAI.maxHp, thisBar.sizeDelta.y);
        iniBar = new Vector2(maxHealth, HealthBar.sizeDelta.y);
        SlowBar = new Vector2(Time.deltaTime * HpSpeed, 0);
        zero = new Vector2(0, HealthBar.sizeDelta.y);
        HpBar = new Vector2(HpSpeed, 0);
    }
    void Update()
    {
        //如果紅條>綠條
        if (HurtBar.sizeDelta.x > HealthBar.sizeDelta.x)
        {
            //慢慢地漸進跟上
            HurtBar.sizeDelta -= SlowBar * 2;
        }
        //如果綠條<=紅條
        else if (HurtBar.sizeDelta.x <= HealthBar.sizeDelta.x)
        {
            //兩個相等
            HurtBar.sizeDelta = HealthBar.sizeDelta;
        }
    }
    public void Enemycohp(int damage)
    {
        //扣除一次傷害的size
        HealthBar.sizeDelta -= new Vector2(damage,0);
    }
}
