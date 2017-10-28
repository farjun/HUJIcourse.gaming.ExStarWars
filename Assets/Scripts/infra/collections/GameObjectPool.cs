using UnityEngine;
using System.Collections.Generic;

namespace Infra.Collections {
public class GameObjectPool : MonoBehaviour {

    public GameObject pooledObjectPrefab;
    public int poolSize = 10;
    [Tooltip("Set to 0 to not allow auto expanding the pool.")]
    public int increaseBy = 0;

    private int lastReturned = 0;
    public List<GameObject> pool;

    public GameObject this[int index] {
        get {
            return pool[index];
        }
    }

    protected void Awake() {
        if (pool.Count < poolSize) {
            Add(pooledObjectPrefab, poolSize - pool.Count);
        }
    }

    public void Add(GameObject pooledObjectPrefab, int count = 1) {
        var isActive = pooledObjectPrefab.activeSelf;
        pooledObjectPrefab.SetActive(false);

        for (int i = 0; i < count; i++) {
            GameObject obj = Instantiate(pooledObjectPrefab);

            obj.transform.SetParent(transform, false);
            obj.name = pooledObjectPrefab.name + i;
            pool.Add(obj);
        }

        pooledObjectPrefab.SetActive(isActive);
    }

    public void ReturnAll() {
        if (pool == null) return;

        foreach (GameObject go in pool) {
            IPoolable comp = go.GetComponent<IPoolable>();
            if (comp != null) {
                comp.ReturnSelf();
            }
            go.SetActive(false);
        }
    }

    public bool HasActiveObjects() {
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].activeInHierarchy) {
                return true;
            }
        }
        return false;
    }

    public T Borrow<T>(params object[] activateParams) where T : IPoolable {
        GameObject go = BorrowGameObject();
        if (go == null) return default(T);
        return ActivateComponent<T>(go, activateParams);
    }

    private T ActivateComponent<T>(GameObject go, params object[] activateParams) where T : IPoolable {
        T comp = go.GetComponent<T>();
        go.SetActive(true);
        comp.Activate(activateParams);
        return comp;
    }

    private GameObject BorrowGameObject() {
        for (int i = lastReturned; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                lastReturned = i;
                return pool[i];
            }
        }
        for (int i = 0; i < lastReturned; i++) {
            if (!pool[i].activeInHierarchy) {
                lastReturned = i;
                return pool[i];
            }
        }
        // All objects are active.
        if (increaseBy > 0) {
            lastReturned = pool.Count;
            Add(pooledObjectPrefab, increaseBy);
            return pool[lastReturned];
        }
        return null;
    }
}
}
