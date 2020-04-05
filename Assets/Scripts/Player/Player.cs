using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject MoveDir;
    [Header("移動速度")]
    [SerializeField]
    private float Speed = 2;
    [Header("目標")]
    public GameObject Target;
    [Header("目前鎖定距離倍率")]
    [SerializeField]
    private float dis;
    [Header("鎖定距離標準")]
    [SerializeField]
    private float DistanceOffset;
    [Header("開麥拉")]
    public GameObject Cam;
    public CameraController CamScript;
    [Header("動畫")]
    public Animator animator;
    [Header("有沒有做動作")]
    public bool OnAction = false;
    [Header("打擊判定")]
    public Collider Sword;
    public Weapon SwordScript;
    [Header("輕攻擊COMBO")]
    [SerializeField]
    private bool CheckCombo;
    [Header("重攻擊COMBO")]
    [SerializeField]
    private bool CheckHeavyCombo;
    [Header("血量")]
    public float PlayerHP;
    [Header("耐力條")]
    public float PlayerSP;
    [Header("SPUI")]
    public SpUI spui;
    public HpUI hpui;
    [Header("受擊音效")]
    public AudioSource SoundEffect;

    public SystemTime TimeController;
    public bool Playing = false;

    private int ComboNumber = 0;
    private int ComboCurrent;

    private float RunSpeed;
    private float AdimSpeed = 1;
    private bool AdimMode = false;
    private EnemyAI enemyAI;
    void Start()
    {
        SoundEffect = GetComponent<AudioSource>();
        enemyAI = GameObject.Find("Enemy").GetComponent<EnemyAI>();
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            AdimSpeed = 10;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            AdimSpeed = 1;
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            AdimMode = true;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AdimMode = false;
        }
        if (Playing)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                GetHit();
            }
            if (transform.position.y < 0)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            //如果沒鎖定
            if (Target == null)
            {
                //如果按下L
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CamScript.DetectEnemy();
                    Target = CamScript.SubTarget;
                    //變成鎖定
                }
                //如果有水平垂直輸入
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {

                    //有在走路


                    //判斷有沒有閃避
                    //Dodge();

                    //如果沒有其他動作
                    if (!OnAction)
                    {
                        Vector2 RunDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                        RunSpeed = Mathf.Clamp(RunDir.magnitude, 0, 1);
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            spui.IsRunning = true;
                            if (PlayerSP > 0)
                            {
                                animator.SetFloat("Speed", RunSpeed);
                                PlayerSP -= 10 * Time.deltaTime;
                                spui.CoSp(10 * Time.deltaTime);
                            }
                            else
                            {
                                animator.SetFloat("Speed", 0);
                                RunSpeed = 0;
                            }
                        }
                        else
                        {
                            animator.SetFloat("Speed", 0);
                            RunSpeed = 0;
                            spui.IsRunning = false;
                        }

                        animator.SetBool("isWalking", true);
                        Dodge();
                        //鎖定Y軸
                        Cam.transform.eulerAngles = new Vector3(Cam.transform.eulerAngles.x, Cam.transform.eulerAngles.y, 0);
                        //抓取四個象限得方向
                        float MoveX = Input.GetAxis("Vertical") * Cam.transform.forward.x + Input.GetAxis("Horizontal") * Cam.transform.right.x;
                        float MoveZ = Input.GetAxis("Vertical") * Cam.transform.forward.z + Input.GetAxis("Horizontal") * Cam.transform.right.z;
                        Vector3 Dir = new Vector3(MoveX, 0, MoveZ);
                        MoveDir.transform.position = Dir + transform.position;
                        transform.LookAt(MoveDir.transform.position);
                        transform.position = Vector3.Lerp(transform.position, MoveDir.transform.position, (Speed + RunSpeed * 2) * AdimSpeed * Time.deltaTime);
                        Debug.Log(Speed + RunSpeed);
                    }
                }
                else
                {
                    //沒在走
                    animator.SetBool("isWalking", false);
                    animator.SetFloat("Speed", 0);
                    RunSpeed = 0;
                    spui.IsRunning = false;
                }
            }
            //鎖定
            else if (Target != null)
            {
                if(spui.IsRunning == true)
                {
                    animator.SetFloat("Speed", 0);
                    RunSpeed = 0;
                    spui.IsRunning = false;
                }
                //隨時跟新目標距離，計算跟原本的差距
                dis = DistanceOffset / Vector3.Distance(Target.transform.position, transform.position);
                //如果按下L
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    CamScript.inViewTarget.Clear();
                    CamScript.SpottedEnemies = null;
                    CamScript.SubTarget = null;
                    Target = null;
                    //沒有走路
                    animator.SetBool("IsWalkingTarget", false);
                    //鎖定無效
                }
                //如果有水平或垂直輸入
                else if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {


                    //Dodge();
                    //如果沒其他動作
                    if (!OnAction)
                    {
                        //有走路
                        animator.SetBool("isWalking", true);
                        animator.SetBool("IsWalkingTarget", true);
                        //抓到他的X輸出方向
                        animator.SetFloat("Walkx", Input.GetAxis("Horizontal"));
                        animator.SetFloat("Walky", Input.GetAxis("Vertical"));
                        Dodge();
                        //面向Target方向
                        transform.LookAt(new Vector3(Target.transform.position.x, 0, Target.transform.position.z));
                        //視角隨水平輸入改變
                        transform.RotateAround(Target.transform.position, Vector3.down, Input.GetAxis("Horizontal") * Speed * AdimSpeed * 15 * Time.deltaTime * dis);
                        //移動
                        transform.Translate(0, 0, Input.GetAxis("Vertical") * Speed * AdimSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    //沒在走路
                    animator.SetBool("isWalking", false);
                    animator.SetBool("IsWalkingTarget", false);

                }
            }

            //按下左鍵且沒其他動作且sp>=15
            if (Input.GetMouseButtonDown(0) && !OnAction && PlayerSP >= 15)
            {
                animator.SetBool("Action", true);
                SwordScript.damage = 10;
                //結束確認是否有combo
                //CheckCombo = false;
                //sp減少15

                //正在動作中
                OnAction = true;
                //輕攻擊
                //animator.SetBool("LightAttack",true);
                ComboNumber++;
                animator.SetInteger("AttackCombo", ComboNumber);
                //減少耐力條

            }
            //按下右鍵且沒其他動作且sp>=25
            else if (Input.GetMouseButtonDown(1) && !OnAction && PlayerSP >= 25)
            {
                animator.SetBool("Action", true);
                SwordScript.damage = 25;
                //結束確認是否有combo
                //正在動作中
                OnAction = true;
                //重攻擊
                ComboNumber--;
                animator.SetInteger("AttackCombo", ComboNumber);
            }
            //如果正在查combo
            if (CheckCombo)
            {
                if (ComboNumber > 0)
                {
                    if (Input.GetMouseButtonDown(0) && PlayerSP >= 15)
                    {
                        //沒有combo無效
                        ComboNumber++;
                        animator.SetInteger("AttackCombo", ComboNumber);
                        //檢查結束
                        CheckCombo = false;
                    }
                }
                else if (ComboNumber < 0)
                {
                    if (Input.GetMouseButtonDown(1) && PlayerSP >= 25)
                    {
                        //沒有combo無效
                        ComboNumber--;
                        animator.SetInteger("AttackCombo", ComboNumber);
                        //檢查結束
                        CheckCombo = false;
                    }
                }
                //如果有新的左鍵輸入且sp>=60

                /*if (Input.GetMouseButtonDown(0) && PlayerSP >= 15)
                {
                    //sp-15
                    PlayerSP -= 15;
                    //沒有combo無效
                    animator.SetBool("NoCombo",false);
                    //檢查結束
                    CheckCombo = false;
                    //耐力條減少
                    spui.CoSp();
                }*/
            }
            //如果正在查HeavyCombo
            /*if (CheckHeavyCombo)
            {
                //如果有新的右鍵輸入且sp >= 15
                if (Input.GetMouseButtonDown(1) && PlayerSP >= 15)
                {
                    //sp-15
                    PlayerSP -= 15;
                    //沒有combo無效
                    animator.SetBool("NoCombo", false);
                    //檢查結束
                    CheckHeavyCombo = false;
                    //耐力條減少
                    spui.CoSp();
                }
            }*/
        }
    }
    /// <summary>
    /// 閃避
    /// </summary>
    private void Dodge()
    {
        //如果有空白鍵輸入且沒有其他動作且SP>=60
        if (Input.GetKeyDown(KeyCode.Space) && !OnAction && PlayerSP >= 15)
        {
            //sp-15
            PlayerSP -= 15;
            //減少SP
            spui.CoSp(15);
            //執行一次閃避
            if (Target == null)
            {
                animator.SetFloat("y", 1);
            }
            else
            {
                //角色方向轉向攝影機方向
                transform.rotation = Quaternion.Euler(new Vector3(0,Cam.transform.rotation.eulerAngles.y,0));
                //抓取水平及垂直輸入去決定動作的方向
                animator.SetFloat("x", Input.GetAxis("Horizontal"));
                animator.SetFloat("y", Input.GetAxis("Vertical"));
            }
            OnAction = true;
            animator.SetBool("Dodge",true);
            //animator.SetBool("Action", true);
        }
    }
    /// <summary>
    /// 沒有走路之外的動作
    /// </summary>
    public void notOnAction()
    {
        animator.SetBool("Dodge", false);
        animator.SetInteger("AttackCombo", 0);
        animator.SetBool("Action", false);
        OnAction = false;
        CheckCombo = false;
        ComboNumber = 0;
        Debug.Log("NoAction");
    }
    /// <summary>
    /// 輕攻擊連擊判定開始
    /// </summary>
    public void CheckComboStart()
    {
        //輕攻擊關閉
        animator.SetBool("LightAttack", false);
        //開始檢查是否有下一個連擊
        CheckCombo = true;
       

    }
    /// <summary>
    /// 重攻擊連擊判定開始
    /// </summary>
    public void CheckHeavyComboStart()
    {
        //重攻擊關閉
        animator.SetBool("HeavyAttack", false);
        //開始檢查是否有下一個連擊
        CheckHeavyCombo = true;

    }
    /// <summary>
    /// 重攻擊判定結束
    /// </summary>
    public void CheckHeavyComboEnd()
    {
        //如果確認沒被關閉
        if(CheckHeavyCombo == true)
        {
            //關閉他
            CheckHeavyCombo = false;
            //沒錯 沒有連擊
            animator.SetBool("NoCombo", true);
            //現在沒有動作喔
            OnAction = false;
        }
    }
    /// <summary>
    /// 輕攻擊判定結束
    /// </summary>
    public void CheckComboEnd()
    {
        //如果確認沒被關閉
        if (CheckCombo == true)
        {
            //關閉他
            CheckCombo = false;
            //沒錯 沒有連擊
            animator.SetBool("NoCombo", true);
            //現在沒有動作喔
            OnAction = false;
        }
    }

    public void ComboCheckStart()
    {
        if (ComboNumber > 0)
        {
            PlayerSP -= 15;
            spui.CoSp(15);
        }
        else if(ComboNumber < 0)
        {
            PlayerSP -= 25;
            spui.CoSp(25);
        }
        ComboCurrent = ComboNumber;
        CheckCombo = true;
    }
    public void ComboCheckEnd()
    {
        CheckCombo = false;
        if(ComboNumber == ComboCurrent)
        {
            ComboNumber = 0;
            animator.SetInteger("AttackCombo", ComboNumber);
        }
    }

    /// <summary>
    /// 攻擊都結束了
    /// </summary>
    public void AllEnd()
    {
        //沒錯 沒有連擊
        animator.SetBool("NoCombo", true);
        //將兩種攻擊狀態無效
        animator.SetBool("LightAttack", false);
        animator.SetBool("HeavyAttack", false);
    }
    /// <summary>
    /// 武器開啟Collider
    /// </summary>
    public void WeaponTrue()
    {
        //讓武器回到實體狀態
        Sword.enabled = true;
    }
    /// <summary>
    /// 武器關閉Collider
    /// </summary>
    public void WeaponFalse()
    {
        //讓武器回到實體狀態
        Sword.enabled = false;
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public void Dead()
    {
        //目前有執行動作
        OnAction = true;
        //玩家死亡
        animator.SetBool("IsDead", true);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        enemyAI.Player=null;
    }
    /// <summary>
    /// 受傷
    /// </summary>
    public void GetHit()
    {
        if (!AdimMode)
        {
            hpui.cohp();
            PlayerHP -= 10;
            animator.SetBool("IsHurt", true);
            OnAction = true;
            ComboNumber = 0;
            SoundEffect.Play();
            animator.SetInteger("AttackCombo", ComboNumber);
            WeaponFalse();
            CheckCombo = false;
            TimeController.LostFrame();
            if (PlayerHP <= 0)
            {
                Dead();
            }
        }
        //玩家受傷
    }
    /// <summary>
    /// 受傷結束
    /// </summary>
    public void Hurt()
    {
        //關閉
        animator.SetBool("IsHurt", false);
    }
    public void Revive()
    {
        animator.SetTrigger("Revive");
        animator.SetBool("IsDead", false);
        OnAction = false;
    }
}