using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    //����ľͷ����ת�ٶ�
    public float rotateSpeed = 90;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //��ľͷΧ����������ϵ��ת
        this.transform.Rotate(Vector3.up*rotateSpeed*Time.deltaTime,Space.World);  
    }
}
