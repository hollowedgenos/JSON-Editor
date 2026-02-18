using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identification")]
    public string enemyID;
    public string enemyName;
    public Stats stats;

    [Header("Prefab Reference")]
    public GameObject enemyPrefab;

    [System.Serializable]
    public class Stats
    {
        [Header("Core Stats")]
        public int hp;
        public int defense;
        public int speed;

        [Header("Rewards")]
        public int gold_amount;
        public int xp_amount;
    }

}
