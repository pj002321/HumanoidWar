using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliatPool : MonoBehaviour
{
    public static GoliatPool Instance;

    // 꺼내준 오브젝트를 모두꺼내주었을 때, 돌려받지 못할 상활이 생기면 새로운 오브젝트를 생성해서 꺼내주기 위함.
    [SerializeField]
    private GameObject poolingObjectPrefab;

    // 큐를 이용해서 생성된 오브젝트를 순서대로 관리 , 하나의 오브젝트를 중복으로 빌려줌을 방지
    Queue<EnemyBullet> poolingObjectQueue = new Queue<EnemyBullet>();

    // 여러 코드에서의 접근성을 위해 싱글톤 패턴으로 구현
    private void Awake()
    {
        Instance = this;

        Initialize(50);
    }
    // CreateNewObject() 함수에서 만들어진 새로운 오브젝트를 큐에 넣어준다. 
    // 게임 시작전에 갯수를 조절하여서 플레이 도중에 발생할 과부하를 렉이 아닌 로딩으로 돌릴 수 있다.
    private void Initialize(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
    }
    // 프리펩으로 부터 새로운 오브젝트를 만든 뒤 비활성화 해서 반환. 
    private EnemyBullet CreateNewObject()
    {
        var newObj = Instantiate(poolingObjectPrefab).GetComponent<EnemyBullet>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    // 요청한 자에게 게임 오브젝트를 꺼내주는 역할을 한다. 큐에 빌려줄 오브젝트가 없다면 
    // CreateNewObject()를 호출해서 새로운 오브젝트를 생성해서 빌려준다.
    public static EnemyBullet GetObject()
    {
        if (Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }
    // 빌려준 오브젝트를 돌려받는 함수.
    public static void ReturnObject(EnemyBullet obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }

}
