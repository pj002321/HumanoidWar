using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    public Animator anim;
    public CharacterController controller;
    private GameObject MyCamera;
    private GameObject FireStartPosition;
    private GameObject FireStartLook;
    public GameObject BulletObject;
    private GameObject HeadPosition;
    private float rotationSpeed = 30f;
    public float moveSpeed = 2.5f;
    private float bulletSpeed = 20000;
    private float BulletActiveTerm = 0.0f;
    
    private int hashSlash = Animator.StringToHash("Slash");
    private int hashShot = Animator.StringToHash("Shoot");
    private int hashRoll = Animator.StringToHash("Roll");
    private int hashJump = Animator.StringToHash("Jump");
    private int hashJumpReady = Animator.StringToHash("JumpReady");
    private int hashPunch = Animator.StringToHash("Punch");
    private int hashRunState = Animator.StringToHash("RunState");

    private bool isJumping = false;
    private bool isSlashJumping = false;
    float WeightValue;

    RaycastHit hit;
    #endregion Variables
    #region UnityMethods
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        MyCamera = GameObject.Find("Camera");
        FireStartPosition = GameObject.Find("FirePosition");
        FireStartLook = GameObject.Find("Rifle_1");
        HeadPosition = GameObject.Find("mixamorig:Head");
        WeightValue = 0.5f;
    }

    void Update()
    {
        float mouseGetX = Input.GetAxis("Mouse X");
        float mouseGetY = Input.GetAxis("Mouse Y");
        float moveGetX = Input.GetAxis("Horizontal") * WeightValue;
        float moveGetZ = Input.GetAxis("Vertical") * WeightValue;
        transform.Rotate(Vector3.up, mouseGetX * rotationSpeed * Time.deltaTime);
        float YPosition = transform.position.y;


        anim.SetFloat("Height", transform.position.y);
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2.0f))
        {

            if (hit.distance < 0.1f)
            {

                isJumping = false;
                isSlashJumping = false;
            }
        }
        if (moveGetX == 0.0f && moveGetZ == 0.0f)
        {
            anim.SetBool("Idle", true);
            WeightValue = 0.8f;
        }
        Vector3 moveDirection = new Vector3(moveGetX, 0, moveGetZ);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime * moveSpeed);

        anim.SetFloat("Movez", moveGetZ);
        anim.SetFloat("Movex", moveGetX);
        BulletActiveTerm += Time.deltaTime * 5.0f;
        PlayerAnimationByState();
        CameraUpdate();

    }
    #endregion UnityMethods
    #region CustomMethods
    void CameraUpdate()
    {
        Vector3 CameraOffsetStartPosition = HeadPosition.transform.position;
        Vector3 cameraOffset = new Vector3(1.4f, 0.0f, -2.5f);
        MyCamera.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up)/* * Quaternion.Euler(0f, -20f, 0f);*/ ;
        Vector3 cameraPosition = CameraOffsetStartPosition + transform.up * cameraOffset.y + transform.right * cameraOffset.x + transform.forward * cameraOffset.z;
        MyCamera.transform.position = cameraPosition;
    }

    void BulletAction()
    {
        Vector3 Startposition = FireStartPosition.transform.position;
        GameObject bullet = Instantiate(BulletObject, Startposition + MyCamera.transform.forward, Quaternion.identity);
        bullet.transform.forward = MyCamera.transform.forward;
        bullet.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(FireStartLook.transform.forward * bulletSpeed, ForceMode.Force);
        Destroy(bullet, 2.0f);
    }

    void PlayerAnimationByState()
    {
        bool isShooting = Input.GetMouseButton(0) && BulletActiveTerm > 1.0f;
        if (isShooting)
        {
            BulletAction();
            BulletActiveTerm = 0.0f;
        }
        anim.SetBool(hashShot, isShooting);
        anim.SetBool(hashRoll, Input.GetKeyDown(KeyCode.Q));
        BulletActiveTerm = Input.GetKeyDown(KeyCode.Q) ? -1.0f : BulletActiveTerm;

        anim.SetBool(hashSlash, Input.GetMouseButton(1));
        anim.SetBool(hashJump, Input.GetKeyDown(KeyCode.Space));


        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyUp(KeyCode.G))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                moveSpeed = 1.1f;
                anim.SetBool(hashJumpReady, true);
            }
            else
            {
                SlashJump();
            }
        }
        anim.SetBool(hashPunch, Input.GetMouseButton(1));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            WeightValue += (WeightValue > 30.0f ? -0.3f : 0.3f) * Time.deltaTime;
            anim.SetBool(hashRunState, true);
        }
        else
        {
            WeightValue = 0.8f;
            anim.SetBool(hashRunState, false);
        }
    }

    void SlashJump()
    {
        if (!isSlashJumping)
        {
            isSlashJumping = true;
            controller.Move(Vector3.up * 10.0f); 
            anim.SetBool(hashJumpReady, false);
        }
    }
    #endregion CustomMethods
}
