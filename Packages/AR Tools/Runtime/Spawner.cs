using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edwon.UnityTools 
{
    public class Spawner : MonoBehaviour
    {
        public bool debugLog;
        public bool spawnOnAwake = false;
        [Header("set spawn point here")]
        public Transform spawnPoint;
        [Header("or find by name")]
        public string spawnPointName;
        [Header("if null can be passed in by event")]
        public GameObject prefabToSpawn;
        [Header("prefab storage is how SpawnDuplicate() finds the same prefab as this")]
        public PrefabStorage prefabStorageToSearch;

        void Awake()
        {
            if (spawnPoint == null)
                spawnPoint = GameObject.Find(spawnPointName).transform;
            
            if (spawnOnAwake)
                SpawnSet();
        }

        public GameObject SpawnGiven(GameObject toSpawn)
        {
            if (debugLog)
                Debug.Log("SpawnGiven: " + toSpawn.name);
            return Spawn(toSpawn);
        }

        public void SpawnSet()
        {
            if (debugLog)
                Debug.Log("SpawnSet: " + prefabToSpawn.name);
            Spawn(prefabToSpawn);
        }

        public void SpawnDuplicate()
        {
            string prefabName = Utils.RemoveParenthesisAndInside(gameObject.name);
            GameObject prefabFromStorage = prefabStorageToSearch.GetPrefab(prefabName);
            if (prefabFromStorage != null)
            {
                if (debugLog)
                    Debug.Log("SpawnDuplicate: " + prefabFromStorage.name);
                Spawn(prefabFromStorage);        
            }
        }

        public void SpawnAndHoldDuplicate()
        {
            string prefabName = Utils.RemoveParenthesisAndInside(gameObject.name);
            GameObject prefabFromStorage = prefabStorageToSearch.GetPrefab(prefabName);
            if (prefabFromStorage != null)
            {
                GameObject spawned = Spawn(prefabFromStorage);
                IHoldable toHold = spawned.GetComponent<IHoldable>();
                Holder holder = spawnPoint.GetComponent<Holder>();
                holder.Hold(toHold);
                if (debugLog)
                    Debug.Log("SpawnAndHoldDuplicate: " + prefabFromStorage.name);
            }
            else
            {
                Debug.LogWarning("couldn't find prefab in storage with name " + prefabName);
            }
        }

        public void SpawnAndHoldDuplicateAfter(float delay)
        {
            Debug.Log("reimplement this");
            // Sequence s = DOTween.Sequence();
            // s.SetDelay(delay);
            // s.AppendCallback(()=> SpawnAndHoldDuplicate());
        }

        GameObject Spawn(GameObject toSpawn)
        {
            GameObject spawned = GameObject.Instantiate(toSpawn, spawnPoint.position, spawnPoint.rotation);
            string name = toSpawn.name;
            string nameStripped = Utils.RemoveParenthesisAndInside(name);
            int totalObjectsWithSameName = 1 + Utils.HowManyOtherGameObjectsWithSameName(name, true);
            spawned.name = nameStripped + " (" + totalObjectsWithSameName + ")";
            return spawned;
        }
    }
}