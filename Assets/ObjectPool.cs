using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    // ������ ������Ʈ�� ��β����־��� ��, �������� ���� ��Ȱ�� ����� ���ο� ������Ʈ�� �����ؼ� �����ֱ� ����.
    [SerializeField]
    private GameObject poolingObjectPrefab;
    
    // ť�� �̿��ؼ� ������ ������Ʈ�� ������� ���� , �ϳ��� ������Ʈ�� �ߺ����� �������� ����
    Queue<Bullet> poolingObjectQueue = new Queue<Bullet>();

    // ���� �ڵ忡���� ���ټ��� ���� �̱��� �������� ����
    private void Awake()
    {
        Instance = this;

        Initialize(50);
    }
    // CreateNewObject() �Լ����� ������� ���ο� ������Ʈ�� ť�� �־��ش�. 
    // ���� �������� ������ �����Ͽ��� �÷��� ���߿� �߻��� �����ϸ� ���� �ƴ� �ε����� ���� �� �ִ�.
    private void Initialize(int initCount)
    {
        for(int i=0;i<initCount;i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
    }
    // ���������� ���� ���ο� ������Ʈ�� ���� �� ��Ȱ��ȭ �ؼ� ��ȯ. 
    private Bullet CreateNewObject()
    {
        var newObj = Instantiate(poolingObjectPrefab).GetComponent<Bullet>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    // ��û�� �ڿ��� ���� ������Ʈ�� �����ִ� ������ �Ѵ�. ť�� ������ ������Ʈ�� ���ٸ� 
    // CreateNewObject()�� ȣ���ؼ� ���ο� ������Ʈ�� �����ؼ� �����ش�.
    public static Bullet GetObject()
    {
        if(Instance.poolingObjectQueue.Count>0)
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
    // ������ ������Ʈ�� �����޴� �Լ�.
    public static void ReturnObject(Bullet obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }
    
}
