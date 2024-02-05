using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    private Transform player;
    private Vector3 offset;
    private float smoothing = 3;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //TransformDirection:将坐标由局部坐标转化为世界坐标
        Vector3 targetPosition = player.position + player.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position,targetPosition,smoothing*Time.deltaTime);
        transform.LookAt(player.position);
    }
}
