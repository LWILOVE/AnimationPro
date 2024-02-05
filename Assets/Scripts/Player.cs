using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Player : MonoBehaviour
{

    private Animator anim;
    //获取动画状态机中变量的编码
    private int speedID = Animator.StringToHash("Speed");
    private int isRunID = Animator.StringToHash("IsRun");
    private int HorizontalID = Animator.StringToHash("Horizontal");
    private int colliderID = Animator.StringToHash("Collider");
    private int speedRotateID = Animator.StringToHash("SpeedRotate");
    private int speedZID = Animator.StringToHash("Speed Z");
    private int sliderID = Animator.StringToHash("Slider");
    private int isHoldLogID = Animator.StringToHash("IsHoldLog");

    private int vaultID = Animator.StringToHash("Vault");


    //获取角色控制器
    private CharacterController characterController;
    //位置匹配
    private Vector3 matchTarget = Vector3.zero;

    public GameObject unityLog = null;

    public Transform rightHand;
    public Transform leftHand;
    public PlayableDirector director;
    public PlayableDirector AtStart;
    public 
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        characterController = this.GetComponent<CharacterController>();
        director = GameObject.Find("Timeline").GetComponent<PlayableDirector>();
        //unityLog = GameObject.FindGameObjectWithTag("PlayerLog").gameObject;
        //unityLog = transform.Find("PlayerLog").gameObject;
        AtStart = GameObject.Find("Timeline2").GetComponent<PlayableDirector>();
        AtStart.Play();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(speedZID,Input.GetAxis("Vertical")*4.1f);
        anim.SetFloat(speedRotateID,Input.GetAxis("Horizontal")*126 );

        ProcessVault();
        ProcessSlider();
        //等效于if
        characterController.enabled = anim.GetFloat(colliderID) < 0.5f;
        //anim.SetFloat(speedID,Input.GetAxis("Vertical")*3);

        //anim.SetFloat(HorizontalID, Input.GetAxis("Horizontal")) ;
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    anim.SetBool(isRunID, true);
        //}
        //if (Input.GetKeyUp(KeyCode.LeftShift))
        //{
        //    anim.SetBool(isRunID, false);
        //}
    }
    //跳跃动画
    private void ProcessVault()
    {
        //判定是否要跳    
        bool isVault = false;
        //第二部分：获取当前第一层的状态的名字是否是Locomotion
        if (anim.GetFloat(speedZID) > 3 && anim.GetCurrentAnimatorStateInfo(0).IsName("LocalMotion"))
        {
            //接收碰撞信息
            RaycastHit hit;
            //进行射线检测
            if (Physics.Raycast(transform.position + Vector3.up * 0.3f, transform.forward, out hit, 3.5f))
            {
                if (hit.collider.tag == "Obstacle")
                {
                    if (hit.distance > 3 && hit.distance < 3.5f)
                    {
                        //获取检测位的位置
                        Vector3 point = hit.point;
                        //碰撞器y轴位置+碰撞器自身大小
                        point.y = hit.collider.transform.position.y + hit.collider.bounds.size.y + 0.12f;
                        matchTarget = point;
                        isVault = true;
                        director.Play();
                    }
                }
            }
        }
        anim.SetBool(vaultID, isVault);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Vault") && anim.IsInTransition(0) == false)
        {
            //参数1：被匹配的目标点   参数2：被匹配的物的旋转度  参数3：要匹配的部位
            //参数4：参数3和参数1匹配的权重值（参数1：位置权重比（1是完全匹配，0.5是半匹配），参数2：旋转权重值）
            //参数5：要匹配的动画的开始匹配时间百分比
            //参数6：要匹配的动画的完成匹配时间百分比
            anim.MatchTarget(matchTarget, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), 0.35f, 0.40f);
        }
    }
    //实现滑行
    private void ProcessSlider()
    {
        bool isSlider = false;
        if (anim.GetFloat(speedZID) > 3 && anim.GetCurrentAnimatorStateInfo(0).IsName("LocalMotion"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1.8f, transform.forward, out hit, 4f))
            {
                if (hit.collider.tag == "Obstacle")
                {
                    if (hit.distance > 3)
                    {
                        Vector3 point = hit.point;
                        point.y = 0;
                        matchTarget = point + transform.forward * 2;
                        isSlider = true;
                        director.Play();
                    }
                }
            }
        }
        anim.SetBool(sliderID,isSlider);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Slider") && anim.IsInTransition(0) == false)
        {
            anim.MatchTarget(matchTarget,Quaternion.identity,AvatarTarget.Root,new MatchTargetWeightMask(new Vector3(1,0,1),0),0.38f,0.7f);
        }
    }
    //扛起木头
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Log")
        {
            //销毁木头
            Destroy(other.gameObject);
            CarryWood();
        }
        //if (other.tag == "Playable")
        //{
        //    //播放跳跃凝视动画
        //    //director.Play();
        //}
    }
    //扛木头
    private void CarryWood()
    {
        unityLog.SetActive(true);
        anim.SetBool(isHoldLogID,true); 
    }
    //IK动画
    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1)
        {
            int weight = anim.GetBool(isHoldLogID)?1:0;
            //说明当前是被Hold Log这一层调用
            //进行IK位置匹配
            //参数1：要匹配的骨骼  
            //参数2：骨骼要要到达的点
            anim.SetIKPosition(AvatarIKGoal.RightHand,rightHand.position);
            //进行IK旋转匹配
            anim.SetIKRotation(AvatarIKGoal.RightHand,rightHand.rotation);
            //设置IK匹配的权重值    
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand,weight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand,weight);

            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            //进行IK旋转匹配
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            //设置IK匹配的权重值    
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        }
    }

}
