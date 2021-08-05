using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    [CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy Wave")]
    public class Wave : ScriptableObject
    {
        //Lista che contiene gli indici del tipo di nemico da spawnare 
        [SerializeField] List<int> numbersToChooseFrom = new List<int>(new int[] { 0, 1, 2 });
        //Array che contiene il numero massimo di nemici permessi per ogni tipo di nemico
        public int[] maxEnemyTypes = new int[3];
        public float timeBetweenSpawns = 1f;

        //Seconds before ending the wave
        public float maxTime = 180f;
        public int MaxEnemies
        {
            get
            {
                int count = 0;
                for(int i = 0; i < maxEnemyTypes.Length; i++)
                    count += maxEnemyTypes[i];
                
                return count;
            }
        }
         
        public void InitializeEnemyTypesIndexList()
        {
            numbersToChooseFrom.Clear();
            for (int i = 0; i < maxEnemyTypes.Length; i++)
                numbersToChooseFrom.Add(i);
        }
        public int SelectEnemyType()
        {
            return numbersToChooseFrom.Count > 0? numbersToChooseFrom[Random.Range(0, numbersToChooseFrom.Count)] : -1;           
        }

        public void DiscardEnemyType(int enemyTypeIndexToRemove)
        {
            if (numbersToChooseFrom.Contains(enemyTypeIndexToRemove))
                numbersToChooseFrom.Remove(enemyTypeIndexToRemove);
            else
                Debug.Log("La lista degli indici dei tipi di nemici non contiene " + enemyTypeIndexToRemove);
        }
    }
}