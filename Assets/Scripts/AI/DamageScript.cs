    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    ParticleSystem Par;
    Player player;

    public CameraMove CamScript;
    
    private void Start()
    {
        Par = GetComponent<ParticleSystem>();
        player = GameObject.Find("ybot").GetComponent<Player>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Par.Play();
            print("打到玩家了");
            player.GetHit();
            CamScript.ShakeCamera();
            
        }
    }
}
