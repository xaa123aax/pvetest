using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject Menu;
    public GameObject btn;
    public Text ReStartText;
    public Player player;
    public EnemyAI ai;
    public AudioSource BGM;
    private bool re;
    // Start is called before the first frame update
    void Start()
    {
        re = false;
        btn.SetActive(false);
    }
    void Update()
    {
        if(player.PlayerHP <= 0 && !re)
        {
            Setting();
            ReStartText.text = "猶豫，便會敗北";
        }
        else if (ai.Hp <= 0 && !re)
        {
            Setting();
            ReStartText.text = "勝利只是一時的";
        }
    }
    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }
    private void Setting()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        btn.SetActive(true);
        re = true;
    }
    public void GameStart()
    {
        player.Playing = true;
        Menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        BGM.Play();
    }
}
