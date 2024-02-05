using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    //定义木头的旋转速度
    public float rotateSpeed = 90;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //让木头围绕世界坐标系旋转
        this.transform.Rotate(Vector3.up*rotateSpeed*Time.deltaTime,Space.World);  
    }
}
