using UnityEngine;

namespace ZombieAttack
{
    [CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy Wave")]
    public class Wave : ScriptableObject
    {
        public int maxEnemies;
        public float timeBetweenSpawns = 1f; 
    }
}