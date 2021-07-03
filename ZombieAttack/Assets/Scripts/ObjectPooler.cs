//Source: https://www.raywenderlich.com/847-object-pooling-in-unity#toc-anchor-002

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sistema di Object pooling per gli oggetti da lanciare
/// </summary>

namespace ZombieAttack
{
    //Classe per il singolo oggetto copiabile
    [System.Serializable]
    public class ObjectPoolItem
    {
        //Il vero e proprio oggetto da rendere copiabile
        public GameObject objectToPool;
        //Quantità iniziale da instanziare
        public int amountToPool;
        //Flag per rendere la lista espandibile a runtime
        public bool shouldExpand = true;
    }

    //Classe contenente una istanza pubblica e statica per creare più liste di oggetti identici
    public class ObjectPooler : MonoBehaviour
    {
        //Lista di oggetti da copiare
        public List<ObjectPoolItem> itemsToPool;

        //Lista dell'oggetto e delle sue copie
        public List<GameObject> pooledObjects;

        //Istanza per accedere agli oggetti del pooler
        public static ObjectPooler SharedInstance;

        private void Awake() => SharedInstance = this;

        //Si crea una lista di oggetti copia per ogni oggetto da poolizzare
        void Start()
        {
            pooledObjects = new List<GameObject>();
            foreach (ObjectPoolItem item in itemsToPool)
            {
                for (int i = 0; i < item.amountToPool; i++)
                {
                    GameObject obj = Instantiate(item.objectToPool, Vector3.zero, Quaternion.identity);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                }
            }
        }

        //Ritorna il primo oggetto copia utilizzabile, eventualmente se il flag è attivo espande la rispettiva lista ed instanzia una nuova copia
        public GameObject GetPooledObject(string tag)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) return pooledObjects[i];
            }

            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.objectToPool.tag == tag)
                {
                    if (item.shouldExpand)
                    {
                        GameObject obj = Instantiate(item.objectToPool, Vector3.zero, Quaternion.identity);
                        obj.SetActive(false);
                        pooledObjects.Add(obj);
                        return obj;
                    }
                }
            }
            return null;
        }
    }
}