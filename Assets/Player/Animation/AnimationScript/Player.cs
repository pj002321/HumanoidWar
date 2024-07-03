using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    #region Variables

    public Animator anim;
    public CharacterController controller;
    public GameObject fireStartPosition;
    private GameObject rifleObject;
    public GameObject bulletPrefab;
    public Transform headPosition;
    private AudioSource audiosource;
    public AudioClip audioClip;
    private Slider slider;
    public Image crossHair;

    private float rotationSpeed = 30f;
    public float moveSpeed = 3.5f;
    private float bulletSpeed = 4000;
    private float BulletActiveTerm = 0.0f;

    private int hashSlash = Animator.StringToHash("Slash");
    private int hashShot = Animator.StringToHash("Shoot");
    private int hashRoll = Animator.StringToHash("Roll");
    private int hashJump = Animator.StringToHash("Jump");
    private int hashJumpReady = Animator.StringToHash("JumpReady");
    private int hashPunch = Animator.StringToHash("Punch");
    private int hashRunState = Animator.StringToHash("RunState");
    private int hashDie = Animator.StringToHash("IsAlive");

    public Vector3 cameraOffset = new Vector3(0.8f, 0.5f, -3.0f);
    private bool isJumping = false;
    private bool isSlashJumping = false;

    float weightValue;
    private Quaternion initialRifleRotation;
    private Quaternion originCameraPosition;
  
    public int numberOfBullet = 40;
    private IObjectPool<Bullet> pool;
    RaycastHit hit;
    private int hp;
    public int maxHp = 100;
    #endregion Variables
    #region UnityMethods
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audiosource= GetComponent<AudioSource>();
        rifleObject = GameObject.Find("Rifle_1");
        weightValue = 0.5f;
        hp = maxHp;
        slider=GetComponentInChildren<Slider>();
        initialRifleRotation = rifleObject.transform.localRotation;
        originCameraPosition = Camera.main.transform.localRotation;
    }

    void Update()
    {
        if (!IsAlive)
        {
            anim.SetBool(hashDie, false);
            Invoke("LoadLobbyScene", 3f);
        }
        else
        {

            float mouseGetX = Input.GetAxis("Mouse X");

            float mouseGetY = Input.GetAxis("Mouse Y");
            float moveGetX = Input.GetAxis("Horizontal") * weightValue;
            float moveGetZ = Input.GetAxis("Vertical") * weightValue;
            transform.Rotate(Vector3.up, mouseGetX * rotationSpeed * Time.deltaTime);
            slider.value = hp * 0.01f;
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
                weightValue = 0.8f;
            }
            Vector3 moveDirection = new Vector3(moveGetX, 0, moveGetZ);
            moveDirection = transform.TransformDirection(moveDirection);
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);

            anim.SetFloat("Movez", moveGetZ);
            anim.SetFloat("Movex", moveGetX);
            BulletActiveTerm += Time.deltaTime * 5.0f;
            RifleRotationUpdate(mouseGetY);
            PlayerAnimationByState();
       
        }
    }
    #endregion UnityMethods
    #region CustomMethods
 
    void LoadLobbyScene()
    {
        SceneManager.LoadScene(0);
    }
    void BulletAction()
    {
        anim.SetTrigger("ShootTrigger");
        audiosource.clip = audioClip;
        audiosource.Play();

        var bullet = ObjectPool.GetObject();
        bullet.transform.position = fireStartPosition.transform.position;

        Ray ray =  Camera.main.ScreenPointToRay(new Vector2((Screen.width / 2), Screen.height / 2));

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = Vector3.zero;
        bulletRigidbody.angularVelocity = Vector3.zero;
        bulletRigidbody.AddForce(ray.direction * bulletSpeed, ForceMode.Force);

        StartCoroutine(ReleaseBulletAfterTime(bullet, 0.25f));
    }


    private IEnumerator ReleaseBulletAfterTime(Bullet bullet, float time)
    {
        yield return new WaitForSeconds(time);
      
        bullet.ReleaseToPool(); 
        
    }
    float rifleRotationX = 0;
    void RifleRotationUpdate(float mouseY)
    {
        rifleRotationX -= mouseY * rotationSpeed * Time.deltaTime;
        rifleRotationX = Mathf.Clamp(rifleRotationX, -10, 10);
        rifleObject.transform.localRotation = initialRifleRotation * Quaternion.Euler(rifleRotationX, 0, 0);

        crossHair.transform.position = new Vector2(Screen.width / 2, Screen.height / 2 - (rifleRotationX/4));
        Camera.main.transform.localRotation = originCameraPosition * Quaternion.Euler((rifleRotationX / 4), 0, 0);
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
            weightValue += (weightValue > 30.0f ? -0.3f : 0.3f) * Time.deltaTime;
            anim.SetBool(hashRunState, true);
        }
        else
        {
            anim.SetBool(hashRunState, false);
            weightValue = 0.5f;
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
    bool IsAlive => hp > 0;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("EnemyBullet"))
        {
            hp -= 1;
        }
    }
    #endregion CustomMethods
}
