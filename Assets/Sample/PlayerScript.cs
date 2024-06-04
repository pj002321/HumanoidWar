using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{




    public CharacterController controller;
    private new Rigidbody rigidbody;
    public GameObject MyCamera;
    private GameObject HeadPosition;
    public GameObject BulletObject;
    public GameObject LockBulletObject;
    public GameObject ExplosionPreb;
    public GameObject ExplosionUAPreb;
    GameObject EnermyObject;


    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public Vector3 cameraOffset;
    Vector3 cameraPosition = new Vector3(0, 0, 0);
    private GameObject ShootStartPosition;
    private GameObject ExplosObjects;
    private float InitRotationY = 20f;
    private float bulletSpeed = 20000;
    private float LockingbulletSpeed = 50;
    private float ParticleForce = 5;
    private float m_BulletActiveTerm = 0.0f;
    private bool GuidBulletCollision = false;
    private float m_EnermyHP = 100;
    //Animator
    private int ExplosCount = 100;
    private float LifeTime = 5.0f;
    private float LifeLKTime = 2.0f;
    private float LifeUATime = 0.5f;
    private GameObject bullet;
    private GameObject guidedBullet; // 유도 중인 총알 저장 변수
    private float guidedBulletForce = 1000f; // 총알 유도 힘
    //RayCast
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        // CharacterController 
        controller = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();

        MyCamera = GameObject.Find("Camera");
        HeadPosition = GameObject.Find("SK_Soldier_Head_LOD1");
        transform.Rotate(Vector3.up, InitRotationY);

        ShootStartPosition = GameObject.Find("ShootPosition");


    }

    // Update is called once per frame
    void Update()
    {
        float mouseGetX = Input.GetAxis("Mouse X");
        bool PickingSwitch = false;
        if (Input.GetKey(KeyCode.Space))
        {
            PickingSwitch = true;
        }

        if (PickingSwitch == false)
            transform.Rotate(Vector3.up, mouseGetX * rotationSpeed * Time.deltaTime);

        float moveGetX = Input.GetAxis("Horizontal");
        float moveGetZ = Input.GetAxis("Vertical");


       
        Vector3 moveDirection = new Vector3(moveGetX, 0f, moveGetZ);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;

 
        moveDirection.y = 0f;


        controller.Move(moveDirection * Time.deltaTime);

        if (!controller.isGrounded)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }

        CameraUpdate();


        bool PickingTargetSwitch = false;
        m_BulletActiveTerm += Time.deltaTime * 3.0f;
        if (Input.GetMouseButton(0) && m_BulletActiveTerm > 1.0)
        {

            BulletAction();
            m_BulletActiveTerm = 0.0f;
        }
        if (Input.GetMouseButtonDown(1))
        {
            // 마우스 클릭 지점의 스크린 좌표를 얻음
            Vector3 mousePosition = Input.mousePosition;

            // 스크린 좌표를 월드 좌표로 변환
            Ray ray = MyCamera.GetComponent<Camera>().ScreenPointToRay(mousePosition);

            // 레이캐스트 수행하여 충돌하는 객체 검출
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌하는 객체가 선택된 경우
                if (hit.collider.CompareTag("EnermyTerret1"))
                {

                    GameObject target = hit.collider.gameObject;
                    GuidingBullet(target);
                }
            }
        }


        Attacked();

    }

    void CameraUpdate()
    {
        Vector3 CameraOffsetStartPosition = HeadPosition.transform.position;

        // 카메라를 플레이어 머리 위에 위치
        cameraOffset = new Vector3(-0.3f, 0.1f, -1.2f);
        MyCamera.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up) * Quaternion.Euler(0f, -20f, 0f); ;
        cameraPosition = CameraOffsetStartPosition + transform.up * cameraOffset.y + transform.right * cameraOffset.x + transform.forward * cameraOffset.z;
        MyCamera.transform.position = cameraPosition;

    }

    void BulletAction()
    {
        Vector3 Startposition = ShootStartPosition.transform.position;
        // 총알 생성

        GameObject bullet = Instantiate(BulletObject, Startposition + MyCamera.transform.forward, Quaternion.identity);
        // 총알 방향 설정
        bullet.transform.forward = MyCamera.transform.forward;
        bullet.transform.Rotate(new Vector3(1, 0, 0), 90.0f);

        // 총알 움직임 설정
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(MyCamera.transform.forward * bulletSpeed, ForceMode.Force);
        // lifetime 초 뒤에 사라짐
        Destroy(bullet, 2.0f);


    }

    void Attacked()
    {
        bool isColliding = false;

        if (Physics.Raycast(transform.position, MyCamera.transform.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("EnermyTerret1"))
            {
                isColliding = true;
            }
        }

        m_BulletActiveTerm += Time.deltaTime * 3.0f;

        if (Input.GetMouseButton(0) && m_BulletActiveTerm > 1.0)
        {

            if (isColliding == true)
            {
                UnderattackExplosionAction();
                m_EnermyHP -= 10.0f;
            }
            BulletAction();
            m_BulletActiveTerm = 0.0f;
        }
      
        if (m_EnermyHP <= 0.0f)
        {
            Destroy(hit.collider.gameObject);
            ExplosionAction();
            m_EnermyHP = 100.0f;
        }


    }
    void ExplosionAction()
    {

        for (int i = 0; i < ExplosCount; i++)
        {
            Vector3 StartPos = hit.collider.gameObject.transform.position;
            ExplosObjects = Instantiate(ExplosionPreb, new Vector3(StartPos.x, StartPos.y, StartPos.z), Quaternion.identity);
            ExplosObjects.transform.Rotate(10, 10, 10);
            Rigidbody Exrigidby = ExplosObjects.GetComponent<Rigidbody>();
            // 랜덤한 방향으로 폭발
            Vector3 RandomDir = Random.insideUnitSphere * ParticleForce;
            Exrigidby.AddForce(RandomDir * 10, ForceMode.Impulse);
            // Lifetime 초 뒤에 사라짐
            Destroy(ExplosObjects, LifeTime);
        }

    }
    void LokingVerExplosionAction(Vector3 Position)
    {

        for (int i = 0; i < ExplosCount; i++)
        {
            Vector3 StartPos = Position;
            ExplosObjects = Instantiate(ExplosionPreb, new Vector3(StartPos.x, StartPos.y, StartPos.z), Quaternion.identity);
            ExplosObjects.transform.Rotate(10, 10, 10);
            Rigidbody Exrigidby = ExplosObjects.GetComponent<Rigidbody>();
            // 랜덤한 방향으로 폭발
            Vector3 RandomDir = Random.insideUnitSphere * ParticleForce;
            Exrigidby.AddForce(RandomDir * 5, ForceMode.Impulse);
            // Lifetime 초 뒤에 사라짐
            Destroy(ExplosObjects, LifeLKTime);
        }

    }
    void UnderattackExplosionAction()
    {

        for (int i = 0; i < 30; i++)
        {
            Vector3 StartPos = hit.collider.gameObject.transform.position;
            GameObject ExplosObjects = Instantiate(ExplosionUAPreb, new Vector3(StartPos.x, StartPos.y + 1.0f, StartPos.z), Quaternion.identity);
            ExplosObjects.transform.Rotate(10, 10, 10);
            Rigidbody Exrigidby = ExplosObjects.GetComponent<Rigidbody>();
            // 랜덤한 방향으로 폭발
            Vector3 RandomDir = Random.insideUnitSphere * ParticleForce;
            Exrigidby.AddForce(RandomDir * 2, ForceMode.Impulse);
            // Lifetime 초 뒤에 사라짐
            Destroy(ExplosObjects, LifeUATime);
        }

    }

    void GuidingBullet(GameObject Target)
    {
        if (guidedBullet == null)
        {

            Vector3 startPosition = ShootStartPosition.transform.position;
            bullet = Instantiate(LockBulletObject, startPosition + transform.forward, Quaternion.identity);
            Vector3 targetDirection = Target.transform.position - bullet.transform.position;
            //bullet.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
            bullet.transform.rotation = Quaternion.LookRotation(targetDirection);


            float bulletSpeed = LockingbulletSpeed;
            float curvature = 40.0f; // 곡률 양을 제어
            float elapsedTime = 0.0f;
            float maxTime = 30.5f; //곡선 궤적의 지속 시간을 제어
            float maxLifetime = 10.0f; // 총알의 최대 수명을 설정


            guidedBullet = bullet;

            // 시간 경과에 따라 총알의 위치를 ​​업데이트하도록 코루틴 설정
            StartCoroutine(UpdateBulletPosition(bullet, targetDirection, bulletSpeed, curvature, elapsedTime, maxTime, maxLifetime));
        }
    }

    IEnumerator UpdateBulletPosition(GameObject bullet, Vector3 targetDirection, float bulletSpeed, float curvature, float elapsedTime, float maxTime, float maxLifetime)
    {
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        while (elapsedTime < maxTime)
        {
            // 경과 시간을 기준으로 현재 곡선 계수 계산
            float curveFactor = Mathf.Clamp01(elapsedTime / maxTime);
            curveFactor = Mathf.Pow(curveFactor, curvature);

            // 곡선 궤적을 따라 총알의 위치를 ​​업데이트합니다.
            Vector3 updatedPosition = bullet.transform.position + bullet.transform.forward * bulletSpeed * Time.deltaTime;
            Vector3 correctionDirection = targetDirection.normalized - bullet.transform.forward;
            updatedPosition += curveFactor * correctionDirection * bulletSpeed * Time.deltaTime;

            // 레이캐스트를 수행하여 총알의 경로를 따라 충돌을 확인합니다.
            RaycastHit hit;
            if (Physics.Raycast(bullet.transform.position, updatedPosition - bullet.transform.position, out hit, Vector3.Distance(updatedPosition, bullet.transform.position)))
            {
                GuidBulletCollision = true;
                LokingVerExplosionAction(bullet.transform.position); // Destroy 전에 호출
                Destroy(bullet);
                guidedBullet = null;
                yield break;
            }

            bulletRigidbody.MovePosition(updatedPosition);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Destroy the bullet after the remaining lifetime
        LokingVerExplosionAction(bullet.transform.position);
        Destroy(bullet);
        guidedBullet = null; // Reset the guided bullet
    }
}
