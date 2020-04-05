using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject CameraOffset;
    public GameObject CameraZoom;
    public float ShakeMaxRange;

    private RaycastHit CamColliderDetect;


    public float ShakeTime;

    //public Animation Shake;
    void FixedUpdate()
    {
        Debug.DrawRay(CameraZoom.transform.position, this.transform.position - CameraZoom.transform.position * 1);
        if (Physics.Raycast(CameraZoom.transform.position, this.transform.position - CameraZoom.transform.position, out CamColliderDetect, Vector3.Distance(CameraOffset.transform.position, CameraZoom.transform.position), LayerMask.GetMask("Obstacles")))
        {
            Debug.Log("有碰到東西");
            float distanceOffect = CamColliderDetect.distance / Vector3.Distance(CameraZoom.transform.position, CameraOffset.transform.position);
            if (distanceOffect < 1)
            {
                this.transform.position = CamColliderDetect.point;
                //this.transform.position = Vector3.Lerp(this.transform.position, CameraZoom.transform.position, distanceOffect / 2);
            }
        }
        else
        {
            this.transform.position = CameraOffset.transform.position;
        }
    }
    public void ShakeCamera()
    {
        StartCoroutine(RandomShake(ShakeTime));
    }
    IEnumerator RandomShake(float Timer)
    {
        Vector3 Temp = transform.position;
        while (Timer > 0)
        {
            Vector3 RanPos = Vector3.forward * Random.Range(-ShakeMaxRange, ShakeMaxRange) + Vector3.right * Random.Range(-ShakeMaxRange, ShakeMaxRange) + Vector3.up * Random.Range(-ShakeMaxRange, ShakeMaxRange);
            this.transform.position += RanPos;
            Timer -= Time.deltaTime * 2;
            yield return new WaitForSeconds(Time.deltaTime * 2);
        }
        this.transform.position = Temp;
        yield return null;
    }
}
