using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Hi;
    [Header("範圍內目標")]
    public Collider [] SpottedEnemies;
    [Header("範圍內看到的目標")]
    public List<GameObject> inViewTarget = new List<GameObject>();
    [Header("Player")]
    public GameObject Player;
    [Header("Enemy")]
    public GameObject SubTarget;
    [Header("視窗x軸移動速度")]
    [SerializeField]
    private float xSpeed = 30;
    [Header("視窗y軸移動速度")]
    [SerializeField]
    private float ySpeed = 10;
    [Header("相機縮放基準")]
    [SerializeField]
    private float ZoomDistance = 40;
    [Header("滾輪速度")]
    [SerializeField]
    private float disSpeed = 300;
    [Header("最小縮放")]
    [SerializeField]
    private float minZoom = 30;
    [Header("最大縮放")]
    [SerializeField]
    private float maxZoom = 50;
    [Header("最大仰角")]
    [SerializeField]
    private float maxY = 30;
    [Header("視野距離")]
    [SerializeField]
    private float EyeViewDistance = 10;
    [Header("視野角度")]
    [SerializeField]
    private float viewAngle = 120;
    [Header("目前x軸")]
    [SerializeField]
    private float x = 90;
    [Header("目前y軸")]
    [SerializeField]
    private float y = 0;
    //攝影機最終旋轉位置
    private Quaternion rotationEuler;
    //攝影機位置
    private Vector3 cameraPosition;
    public Vector3 EnemyPosition;
    private Camera Cam;

    public float Radius;
    public float SmoothSpeed;
    private Vector3 SmoothPosition = Vector3.zero;
    void Start()
    {
        Cam = Camera.main;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.transform.position, EyeViewDistance);
        //Gizmos.DrawWireSphere();
    }
 
    //最後一個執行的Update
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position + Vector3.up, EnemyPosition - transform.position, Color.red);
        //位置=角色位置
        //transform.position = Player.transform.position;
        if (CheckMargin())
        {
            SmoothPosition = Vector3.Lerp(this.transform.position, Player.transform.position, SmoothSpeed * Time.deltaTime);
            this.transform.position = SmoothPosition;
        }
        
        //如果沒有目標
        if (SubTarget == null)
        {
            //攝影機移動跟隨滑鼠的X位移
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        }
        else
        {
            //等於目標跟玩家的方向 
            Vector3 EnemyVector = SubTarget.transform.position - Player.transform.position;
            //用兩個方向求得其中間角度
            //Vector3.up=(0,1,0)
            x = Vector3.SignedAngle(Vector3.forward, EnemyVector, Vector3.up);
        }
        //攝影機移動跟隨滑鼠的X位移
        y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        //如果左右的位移大於一定值拉回360度內
        x = x > 360 ? x -= 360 : x < 0 ? x += 360 : x;
        y = y > maxY ? y = maxY : y < -maxY ? y = -maxY : y;
        //攝影機的縮放依造滾輪得輸入做距離增減
        ZoomDistance -= Input.GetAxis("Mouse ScrollWheel") * disSpeed * Time.deltaTime;
        //攝影機的縮放限定在minZoom, maxZoom之間
        ZoomDistance = Mathf.Clamp(ZoomDistance, minZoom, maxZoom);
        Cam.fieldOfView = ZoomDistance;
        //旋轉UP
        rotationEuler = Quaternion.Euler(y, x, 0);
        //決定攝影機的位置
        transform.rotation = rotationEuler;
    }
    /// <summary>
    /// 偵測敵人
    /// </summary>
    public void DetectEnemy()
    {
        int j = 0;
        SpottedEnemies = Physics.OverlapSphere(transform.position, EyeViewDistance, LayerMask.GetMask("Enemy"));
       
        for (int i = 0; i < SpottedEnemies.Length; i++)
        {
            EnemyPosition = SpottedEnemies[i].transform.position;
            Debug.Log(SpottedEnemies[0] + "Outer");
            
            if (Vector3.Angle(transform.forward, EnemyPosition - transform.position) <= viewAngle / 2)
            {
                RaycastHit info = new RaycastHit();
                int layermask = LayerMask.GetMask("Enemy");
                Physics.Raycast(transform.position + Vector3.up, EnemyPosition - transform.position, out info, EyeViewDistance, layermask);
                Debug.Log(info.collider + "inner");
                if (info.collider == SpottedEnemies[i])
                {
                    inViewTarget.Add(SpottedEnemies[i].gameObject);
                    j++;
                }
            }
        }
        if(inViewTarget.Count != 0)
        {
            //複數目標
            if(inViewTarget.Count > 1)
            {
                for (int i = 0; i < inViewTarget.Count; i++)
                {
                    if (Vector3.Distance(transform.position, inViewTarget[i].transform.position) <= Vector3.Distance(transform.position, inViewTarget[0].transform.position))
                    {
                        inViewTarget[0] = inViewTarget[i];
                    }
                }
            }
            SubTarget = inViewTarget[0];
        }
    }

    ///<summary>
    ///平滑偵測
    ///</summary>
    bool CheckMargin()
    {
        return Vector3.Distance(this.transform.position, Player.transform.position) > Radius;
    }
    /*
    float SpeedChange(float BasicSpeed)
    {
        return BasicSpeed * Mathf.Sqrt(Vector3.Distance(this.transform.position, Player.transform.position));
    }
    */
}