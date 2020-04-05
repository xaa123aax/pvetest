using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;
    public EnemyAI enemyai;

    public CameraMove CamScript;
    
    /// <summary>
    /// 如果武器碰到其他東西
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //有tag是target
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyai.GetHit(damage);
            CamScript.ShakeCamera();
            
        }
    }
}
